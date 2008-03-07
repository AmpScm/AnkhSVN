// $Id$
using System;
using System.Collections;
using System.Windows.Forms;
using AnkhSvn.Ids;
using SharpSvn;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to revert current item to last updated revision.
    /// </summary>
    [VSNetCommand(AnkhCommand.RevertItem,
		"RevertItem",
         Text = "&Revert...",
         Tooltip = "Revert this item to last updated revision.",
         Bitmap = ResourceBitmaps.Revert),
         VSNetItemControl( VSNetControlAttribute.AnkhSubMenu, Position = 4 )]
    public class RevertItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if ( e.Context.Selection.GetSelectionResources( true, 
                new ResourceFilterCallback( SvnItem.ModifiedFilter ) ).Count == 0 )
            {
                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            SaveAllDirtyDocuments( context );

            // get the modified resources
            IList resources = context.Selection.GetSelectionResources( true,
                new ResourceFilterCallback( SvnItem.ModifiedFilter ) );

            SvnDepth depth = SvnDepth.Empty;
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
                depth = info.Depth;
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
                SvnRevertArgs args = new SvnRevertArgs();
                args.Depth = depth;
                context.Client.Revert(paths, args);
            }
            catch //( NotVersionControlledException )
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