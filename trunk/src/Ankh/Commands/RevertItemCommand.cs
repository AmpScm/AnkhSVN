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

    [VSNetCommand("RevertItem", Text = "&Revert...", Tooltip = "Reverts selected item",
         Bitmap = ResourceBitmaps.Revert),
    VSNetItemControl( "Ankh", Position = 1 )]
    public class RevertItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.IContext context)
        {   
            if ( context.Selection.GetSelectionResources( true, 
                new ResourceFilterCallback( SvnItem.ModifiedFilter ) ).Count > 0 )
            {
                return Enabled;
            }
            else
                return Disabled;
        }

        public override void Execute(Ankh.IContext context, string parameters)
        {
            this.SaveAllDirtyDocuments( context );

            // get the modified resources
            IList resources = context.Selection.GetSelectionResources( true,
                new ResourceFilterCallback( SvnItem.ModifiedFilter ) );

            Recurse recurse = Recurse.None;
            bool confirmed = false;
            // is Shift down?
            if ( !CommandBase.Shift )
            {
                PathSelectorInfo info = new PathSelectorInfo( "Select items to revert", 
                    resources, resources );
                info = context.UIShell.ShowPathSelector( info );
                if ( info == null )
                    return;
                confirmed = true;
                recurse = info.Recurse;
                resources = info.CheckedItems;                
            }

            string[] paths = SvnItem.GetPaths( resources );
            
            // ask for confirmation if the Shift dialog hasn't been used
            if ( !confirmed )
            {
                string msg = "Do you really want to revert these item(s)?" + 
                    Environment.NewLine + Environment.NewLine;            
                msg += string.Join( Environment.NewLine, paths );

                if( MessageBox.Show( context.HostWindow, msg, "Revert", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information ) != DialogResult.Yes )
                {
                    return;
                }
            }
               
            // perform the actual revert 
            context.OutputPane.StartActionText("Reverting");  
            context.ProjectFileWatcher.StartWatchingForChanges(); 
            try
            {
                context.Client.Revert( paths, recurse );
            }
            catch( NotVersionControlledException )
            {
                // empty
            }

            if ( !context.ReloadSolutionIfNecessary() )
            {
                foreach( SvnItem item in resources )
                    item.Refresh( context.Client );
            }

            context.OutputPane.EndActionText();
        }               
        #endregion       
    }
}



