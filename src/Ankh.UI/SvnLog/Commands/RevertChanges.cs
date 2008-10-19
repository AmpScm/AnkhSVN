using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using Ankh.Commands;
using Ankh.Scc.UI;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogRevertThisRevisions)]
    [Command(AnkhCommand.LogRevertTo)]
    class RevertChanges : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            ILogControl logWindow = e.Selection.ActiveDialogOrFrameControl as ILogControl;
            if (logWindow == null || !logWindow.HasWorkingCopyItems)
            {
                e.Enabled = false;
                return;
            }

            int count = 0;
            foreach (ISvnLogItem item in e.Selection.GetSelection<ISvnLogItem>())
            {
                count++;

                if (count > 1)
                    break;
            }

            switch(e.Command)
            {
                case AnkhCommand.LogRevertTo:
                    if (count == 1)
                        return;
                    break;
                case AnkhCommand.LogRevertThisRevisions:
                    if (count > 0)
                        return;
                    break;
            }
            e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            ILogControl logWindow = e.Selection.ActiveDialogOrFrameControl as ILogControl;
            if (logWindow == null || !logWindow.HasWorkingCopyItems)
                return;

            IProgressRunner progressRunner = e.GetService<IProgressRunner>();
            IWorkingCopyOperations wcOperations = e.GetService<IWorkingCopyOperations>();

            List<SvnRevision> revisions = new List<SvnRevision>();
            foreach (ISvnLogItem item in e.Selection.GetSelection<ISvnLogItem>())
            {
                revisions.Add(item.Revision);
            }

            if (revisions.Count == 0)
                return;

            if (e.Command == AnkhCommand.LogRevertTo && revisions.Count > 1)
                return;

            IAnkhOpenDocumentTracker tracker = e.GetService<IAnkhOpenDocumentTracker>();

            if(logWindow.WorkingCopyItems != null && logWindow.WorkingCopyItems.Length > 0)
                tracker.SaveDocuments(SvnItem.GetPaths(logWindow.WorkingCopyItems));

            progressRunner.Run("Reverting",
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    using (DocumentLock dl = tracker.LockDocuments(e.Selection.GetSelectedFiles(true), DocumentLockType.NoReload))
                    {
                        dl.MonitorChanges();

                        foreach (SvnItem item in logWindow.WorkingCopyItems)
                        {
                            SvnTarget source = new SvnPathTarget(item.FullPath);
                            string target = item.FullPath;

                            foreach (SvnRevision rev in revisions)
                            {
                                ee.Client.Merge(target, source, new SvnRevisionRange(rev.Revision, rev.Revision-1));
                            }
                        }
                        dl.ReloadModified();
                    }
                });
        }
    }
}
