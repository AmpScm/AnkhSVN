using System;
using System.Collections;

using System.Text;
using SharpSvn;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to lock the selected item.
    /// </summary>
    [Command(AnkhCommand.Lock)]
    public class LockCommand : CommandBase
    {
        #region Implementation of ICommand


        public override void OnExecute(CommandEventArgs e)
        {
            /*IContext context = e.Context.GetService<IContext>();

            IList resources = context.Selection.GetSelectionResources(true, 
                new ResourceFilterCallback( SvnItem.NotLockedAndLockableFilter ) );

            this.info = new LockDialogInfo( resources, resources );
            
            // is Shift down?
            if ( !CommandBase.Shift )
            {
                this.info = context.UIShell.ShowLockDialog( this.info );
                if( this.info == null)
                    return;
            }

            this.lockFailedFiles = new ArrayList();
            context.Client.Notify += new EventHandler<SvnNotifyEventArgs>(OnClientNotification);
            try
            {
                context.UIShell.RunWithProgressDialog( new SimpleProgressWorker(
                    new SimpleProgressWorkerCallback( this.ProgressCallback ) ), "Locking files" );
                foreach ( SvnItem item in info.CheckedItems )
                    item.MarkDirty();
            }
            finally
            {
                context.Client.Notify -= new EventHandler<SvnNotifyEventArgs>( OnClientNotification );
            }

            if ( this.lockFailedFiles.Count > 0 )
            {
                this.ShowLockFailedMessage( context );
            }*/
        }

        #endregion

        private void ShowLockFailedMessage( IContext context )
        {
            StringBuilder sb = new StringBuilder();
            bool onlyOne = this.lockFailedFiles.Count == 1;

            sb.AppendFormat("The following file{0} {1} out of date and could not be locked:", 
                onlyOne ? "" : "s", 
                onlyOne ? "was" : "were");
            sb.Append( Environment.NewLine );
            sb.Append( Environment.NewLine );

            foreach ( string path in this.lockFailedFiles )
            {
                sb.Append( path );
                sb.Append( Environment.NewLine );
            }

            context.UIShell.ShowMessageBox( sb.ToString(), "Lock failed", System.Windows.Forms.MessageBoxButtons.OK );
        }

        void OnClientNotification( object sender, SvnNotifyEventArgs args )
        {
            if ( args.Action == SvnNotifyAction.LockFailedLock )
            {
                this.lockFailedFiles.Add( args.Path );
            }
        }

        private void ProgressCallback(IContext context)
        {
            string[] paths = SvnItem.GetPaths(info.CheckedItems);
            SvnLockArgs args = new SvnLockArgs();
            args.StealLock = info.StealLocks;
            args.Comment = info.Message;
            using (SvnClient client = context.ClientPool.GetClient())
            {
                client.Lock(paths, args);
            }
        }

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (!item.IsLocked && item.IsVersioned && item.IsFile)
                    return;
            }
            e.Enabled = false;
        }
        
        private LockDialogInfo info;
        private ArrayList lockFailedFiles;
    }
}