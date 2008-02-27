using System;
using System.Collections;
using NSvn.Core;
using System.Text;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to lock the selected item.
    /// </summary>
    [VSNetCommand("Lock",
         Text = "Loc&k...",
         Tooltip = "Lock the selected item.",
         Bitmap = ResourceBitmaps.Lock),
         VSNetItemControl( VSNetControlAttribute.AnkhSubMenu, Position = 8 )]
    public class LockCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void Execute(IContext context, string parameters)
        {
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
            context.Client.Notification += new NotificationDelegate( OnClientNotification );
            try
            {
                context.UIShell.RunWithProgressDialog( new SimpleProgressWorker(
                    new SimpleProgressWorkerCallback( this.ProgressCallback ) ), "Locking files" );
                foreach ( SvnItem item in info.CheckedItems )
                    item.Refresh( context.Client );
            }
            finally
            {
                context.Client.Notification -= new NotificationDelegate( OnClientNotification );
            }

            if ( this.lockFailedFiles.Count > 0 )
            {
                this.ShowLockFailedMessage( context );
            }
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

        void OnClientNotification( object sender, NotificationEventArgs args )
        {
            if ( args.Action == NotifyAction.FailedLock )
            {
                this.lockFailedFiles.Add( args.Path );
            }
        }

        private void ProgressCallback( IContext context )
        {
            string[] paths = SvnItem.GetPaths( info.CheckedItems );
            
            context.Client.Lock( paths, this.info.Message, this.info.StealLocks );
        }

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            IList resources = context.Selection.GetSelectionResources( true,
                new ResourceFilterCallback(SvnItem.NotLockedAndLockableFilter) );
            return resources.Count > 0 ? Enabled : Disabled;
        }
        
        private LockDialogInfo info;
        private ArrayList lockFailedFiles;
    }
}