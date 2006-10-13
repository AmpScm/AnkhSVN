using System;
using System.Collections;
using NSvn.Core;
using System.Text;

namespace Ankh.Commands
{
    /// <summary>
    /// A command class to support the svn lock command
    /// </summary>
    [VSNetCommand("Lock", Text = "Lock...", Tooltip = "Locks the selected item",
         Bitmap = ResourceBitmaps.Lock),
    VSNetItemControl( "Ankh", Position = 1 )]    
    public class LockCommand : CommandBase
	{
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

        private void ShowLockFailedMessage( IContext context )
        {
            StringBuilder sb = new StringBuilder();
            bool onlyOne = this.lockFailedFiles.Count == 1;

            sb.AppendFormat("The following file{0} {1} out of date and could not be locked:", 
                onlyOne ? "" : "s", 
                onlyOne ? "was" : "were");
            sb.AppendLine();
            sb.AppendLine();

            foreach ( string path in this.lockFailedFiles )
            {
                sb.AppendLine( path );
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
