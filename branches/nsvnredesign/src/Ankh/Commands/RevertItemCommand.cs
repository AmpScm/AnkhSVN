// $Id$
using System;

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
            if ( context.SolutionExplorer.GetSelectionResources( true, 
                new ResourceFilterCallback( CommandBase.ModifiedFilter ) ).Count > 0 )
            {
                return vsCommandStatus.vsCommandStatusSupported |
                    vsCommandStatus.vsCommandStatusEnabled;
            }
            else
                return vsCommandStatus.vsCommandStatusSupported;
        }

        public override void Execute(Ankh.AnkhContext context, string parameters)
        {
            // get the modified resources
            IList resources = context.SolutionExplorer.GetSelectionResources( true,
                new ResourceFilterCallback( CommandBase.ModifiedFilter ) );

            // does the user really want to revert these items?
            string[] paths = SvnItem.GetPaths( resources );
            string msg = "Do you really want to reverse these item(s)?" + 
                Environment.NewLine + Environment.NewLine;            
            msg += string.Join( Environment.NewLine, paths );

            if( MessageBox.Show( context.HostWindow, msg, "Revert", MessageBoxButtons.YesNo,
                MessageBoxIcon.Information ) == 
                DialogResult.Yes )
            {
                
                context.OutputPane.StartActionText("Reverting");               
                try
                {
                    context.Context.Revert( paths, false );
                }
                catch( NotVersionControlledException )
                {
                    // empty
                }
                foreach( SvnItem item in resources )
                    item.Refresh( context.Context );
                context.OutputPane.EndActionText();
            }               
        }
        #endregion       
    }
}



