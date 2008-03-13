using System;
using System.Collections;
using SharpSvn;
using AnkhSvn.Ids;
using Ankh.VS;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to unlock the selected items.
    /// </summary>
    [Command(AnkhCommand.Unlock)] 
	public class UnlockCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsLocked)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            /*IList resources = context.Selection.GetSelectionResources(true, 
                new ResourceFilterCallback( SvnItem.LockedFilter ) );

            this.info = new PathSelectorInfo( "Unlock files", resources, resources );

            if ( !CommandBase.Shift )
            {
                this.info = context.UIShell.ShowPathSelector( this.info );

                if ( this.info == null )
                    return;
            }

            context.UIShell.RunWithProgressDialog( new SimpleProgressWorker(
                new SimpleProgressWorkerCallback( this.DoUnlock )), "Unlocking files" );
            
            foreach( SvnItem item in this.info.CheckedItems )
                item.MarkDirty();*/
        }

        #endregion

        private void DoUnlock(AnkhWorkerArgs e)
        {
            string[] paths = SvnItem.GetPaths(this.info.CheckedItems);
            SvnUnlockArgs args = new SvnUnlockArgs();
            args.BreakLock = false;
            e.Client.Unlock(paths, args);
        }

        private PathSelectorInfo info;
	}
}