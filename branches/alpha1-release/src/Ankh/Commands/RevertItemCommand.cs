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
using Ankh.Solution;

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
                    return vsCommandStatus.vsCommandStatusEnabled;
            }

            public override void Execute(Ankh.AnkhContext context)
            {
                RevertVisitor v = new RevertVisitor();
                context.SolutionExplorer.VisitSelectedNodes( v );
                
                v.Revert( context );
                context.SolutionExplorer.RefreshSelection();
               
            }
        #endregion
        
            /// <summary>
            /// A visitor reverts visited item in the Working copy.
            /// </summary>
            private class RevertVisitor : LocalResourceVisitorBase, INodeVisitor
            { 
                public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
                {                    
                    this.revertables.Add( resource );
                }


                /// <summary>
                /// Revert selected items.
                /// </summary>
                /// <param name="context"></param>
                public void Revert(Ankh.AnkhContext context)
                {
                    // make the user confirm that he really wants to revert.
                    StringBuilder builder = new StringBuilder();
                    foreach( WorkingCopyResource r in this.revertables )
                        builder.Append( NSvn.Utils.GetWorkingCopyRootedPath( r.Path ) + 
                            Environment.NewLine );
                    string msg = "Do you really want to revert the following item(s)?" + 
                        Environment.NewLine + Environment.NewLine + builder.ToString();

                    if( MessageBox.Show( msg, "Revert", MessageBoxButtons.YesNo ) == 
                        DialogResult.Yes )
                    {
                        context.OutputPane.StartActionText("Reverting");
                        // do the actual revert
                        foreach( WorkingCopyResource r in this.revertables )
                            r.Revert( true );
                         context.OutputPane.EndActionText();
                    }
                }

                private ArrayList revertables = new ArrayList();

                public void VisitProject(Ankh.Solution.ProjectNode node)
                {
                    node.VisitResources( this, false );                
                }

                public void VisitProjectItem(Ankh.Solution.ProjectItemNode node)
                {
                    node.VisitResources( this, true );                
                }

                public void VisitSolutionNode(Ankh.Solution.SolutionNode node)
                {
                    node.VisitResources( this, false );
                }
            }
    }
}



