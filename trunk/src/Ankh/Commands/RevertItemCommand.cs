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
    [Command(AnkhCommand.RevertItem)]
    public class RevertItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsModified)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            SaveAllDirtyDocuments( context );

            /*// get the modified resources
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
            using (context.OutputPane.StartActionText("Reverting"))
            {
                SvnRevertArgs args = new SvnRevertArgs();
                args.Depth = depth;
                args.ThrowOnError = false;
                context.Client.Revert(paths, args);

                foreach (SvnItem item in resources)
                    item.MarkDirty();
            }*/
        }

        #endregion
    }
}