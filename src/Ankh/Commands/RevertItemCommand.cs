// $Id$
using System;
using NSvn;
using NSvn.Core;
using NSvn.Common;
using EnvDTE;
using Ankh.UI;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for RevertItem.
    /// </summary>

    [VSNetCommand("RevertItem", Text = "Revert", Tooltip = "Reverts selected item",
         Bitmap = ResourceBitmaps.Revert),
    VSNetControl( "Item.Ankh", Position = 1 ),
    VSNetControl( "Project.Ankh", Position = 1 ),
    VSNetControl( "Folder.Ankh", Position = 1 ),
    VSNetControl( "Solution.Ankh", Position = 1)]
    internal class RevertItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            ModifiedVisitor m = new ModifiedVisitor();
            context.SolutionExplorer.VisitSelectedItems( m, true );
            
            if ( m.Modified )
                return vsCommandStatus.vsCommandStatusEnabled |
                    vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusSupported;
        }

        public override void Execute(Ankh.AnkhContext context)
        {
            RevertVisitor v = new RevertVisitor();
            context.SolutionExplorer.VisitSelectedItems( v, true );
                
            v.Revert( context );
            context.SolutionExplorer.RefreshSelectionParents();
               
        }
        #endregion
        
        /// <summary>
        /// A visitor reverts visited item in the Working copy.
        /// </summary>
        private class RevertVisitor : LocalResourceVisitorBase
        { 
            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
            {
                if ( resource.Status.TextStatus != StatusKind.Normal ||
                    (resource.Status.PropertyStatus != StatusKind.Normal && 
                    resource.Status.PropertyStatus != StatusKind.None ) )
                    this.revertables.Add( resource );
            }

            public void Revert(Ankh.AnkhContext context)
            {
                // no revertables?
                if ( this.revertables.Count < 1 )
                    return;

                // make the user confirm that he really wants to revert.
                StringBuilder builder = new StringBuilder();
                foreach( WorkingCopyResource r in this.revertables )
                    builder.Append( NSvn.Utils.GetWorkingCopyRootedPath( r.Path ) + 
                        Environment.NewLine );
                string msg = "Do you really want to revert the following item(s)?" + 
                    Environment.NewLine + Environment.NewLine + builder.ToString();

                if( MessageBox.Show( msg, "Revert", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information ) == 
                    DialogResult.Yes )
                {
                    context.OutputPane.StartActionText("Reverting");
                    // do the actual revert
                    foreach( WorkingCopyResource r in this.revertables )
                    {
                        try
                        {
                            r.Revert( true );
                        }
                        catch( NotVersionControlledException )
                        {
                            // empty
                        }
                    }
                    context.OutputPane.EndActionText();
                }
            }

            private ArrayList revertables = new ArrayList();
        }
    }
}



