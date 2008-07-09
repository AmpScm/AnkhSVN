using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.Diagnostics;
using System.Threading;
using Ankh.ContextServices;

namespace Ankh
{
    public sealed class NotificationHandler
    {
        IAnkhServiceProvider _context;
        IAnkhOperationLogger _logger;

        public NotificationHandler(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;

            // TODO: Localize
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
        }

        public void Hook(SvnClient client, bool hook)
        {
            if (client == null)
                throw new ArgumentNullException("hook");

            client.Notify += new EventHandler<SvnNotifyEventArgs>(OnNotification);
        }

        void OnNotification(object sender, SvnNotifyEventArgs e)
        {
            SynchronizationContext context = SynchronizationContext.Current;

            if (context != null)
            {
                e.Detach();
                context.Post(
                    delegate(object state)
                    {
                        OnNotify(e);
                    }, null);
            }
            else
                OnNotify(e);
        }

        IAnkhOperationLogger Logger
        {
            get { return _logger ?? (_logger = _context.GetService<IAnkhOperationLogger>()); }
        }

        void OnNotify(SvnNotifyEventArgs e)
        {
            if (Logger == null)
                return;

            string actionValue;
            
            if ( actionStatus.TryGetValue(e.Action, out actionValue) && actionValue != null)
            {
                string nodeKind = "";
                if ( e.NodeKind == SvnNodeKind.File )
                    nodeKind = " file";
                else if (e.NodeKind == SvnNodeKind.Directory)
                    nodeKind = " directory";

                Logger.WriteLine(string.Format("{0}{1}: {2}", actionValue, nodeKind, e.Path ));
            }

			if (e.Action == SvnNotifyAction.CommitSendData)
                Logger.Write(".");

            if (e.Action == SvnNotifyAction.UpdateCompleted)
                Logger.WriteLine(string.Format("{0}Updated {1} to revision {2}.", Environment.NewLine, e.Path, e.Revision));
        }

        readonly Dictionary<SvnNotifyAction, string> actionStatus = new Dictionary<SvnNotifyAction, string>();
    }
}
