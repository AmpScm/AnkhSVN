using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.Diagnostics;

namespace Ankh
{
    public sealed class NotificationHandler
    {
        private NotificationHandler(IContext ankhContext)
        {
            this.ankhContext = ankhContext;
            actionStatus[SvnNotifyAction.Add] = "Added";
            actionStatus[SvnNotifyAction.Copy] = "Copied";
            actionStatus[SvnNotifyAction.Delete] = "Deleted";
            actionStatus[SvnNotifyAction.Restore] = "Restored";
            actionStatus[SvnNotifyAction.Revert] = "Reverted";
            actionStatus[SvnNotifyAction.RevertFailed] = "Revert failed";
            actionStatus[SvnNotifyAction.Resolved] = "Resolved";
            actionStatus[SvnNotifyAction.Skip] = "Skipped";
            actionStatus[SvnNotifyAction.UpdateDelete] = "Deleted";
            actionStatus[SvnNotifyAction.UpdateAdd] = "Added";
            actionStatus[SvnNotifyAction.UpdateUpdate] = "Updated";
            actionStatus[SvnNotifyAction.UpdateCompleted] = null;
            actionStatus[SvnNotifyAction.UpdateExternal] = "Updated external";
            actionStatus[SvnNotifyAction.CommitModified] = "Modified";
            actionStatus[SvnNotifyAction.CommitAdded] = "Added";
            actionStatus[SvnNotifyAction.CommitDeleted] = "Deleted";
            actionStatus[SvnNotifyAction.CommitReplaced] = "Replaced";
            actionStatus[SvnNotifyAction.CommitSendData] = null;
            actionStatus[SvnNotifyAction.LockLocked] = "Locked";
            actionStatus[SvnNotifyAction.LockUnlocked] = "Unlocked";
            actionStatus[SvnNotifyAction.LockFailedLock] = "Failed lock";
            actionStatus[SvnNotifyAction.LockFailedUnlock] = "Failed unlock";

            ankhContext.Client.Notify += new EventHandler<SvnNotifyEventArgs>(OnNotification);
#if DEBUG
            ankhContext.Client.Cancel += new EventHandler<SvnCancelEventArgs>(OnCancel);
#endif
        }

        void OnCancel(object sender, SvnCancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Cancel called. Cancelled: " + e.Cancel,
                "Ankh");
        }

        void OnNotification(object sender, SvnNotifyEventArgs e)
        {
            if ( this.ankhContext.UIShell.SynchronizingObject.InvokeRequired )
            {
                Debug.WriteLine( "OnNotification: Invoking back to main GUI thread", 
                    "Ankh" );
                this.ankhContext.UIShell.SynchronizingObject.Invoke(
                    new EventHandler<SvnNotifyEventArgs>(this.OnNotification),
                    new object[]{sender, e} );
                return;
            }

            string actionValue;
            if ( actionStatus.TryGetValue(e.Action, out actionValue) && actionValue != null)
            {
                string nodeKind = "";
                if ( e.NodeKind == SvnNodeKind.File )
                    nodeKind = " file";
                else if (e.NodeKind == SvnNodeKind.Directory)
                    nodeKind = " directory";

                this.ankhContext.OutputPane.WriteLine( "{0}{1}: {2}",
                    actionValue,
                    nodeKind, 
                    e.Path );
            }

			if (e.Action == SvnNotifyAction.CommitSendData)
                this.ankhContext.OutputPane.Write( '.' );

            if (e.Action == SvnNotifyAction.UpdateCompleted)
                this.ankhContext.OutputPane.WriteLine( "{0}Updated {1} to revision {2}.", 
                    Environment.NewLine, 
                    e.Path, 
                    e.Revision);
        }

        public static NotificationHandler GetHandler(IContext ankhContext)
        {
            lock (_lock)
            {
                if (_instance != null)
                    return _instance;

                _instance = new NotificationHandler(ankhContext);
                return _instance;
            }
        }

        readonly IContext ankhContext;
        readonly Dictionary<SvnNotifyAction, string> actionStatus = new Dictionary<SvnNotifyAction, string>();
        static NotificationHandler _instance;
        static readonly object _lock = new object();
    }
}
