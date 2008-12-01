using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using Ankh.Scc;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Ankh.VS;
using Ankh.Commands;
using System.Windows.Forms;
using Ankh.UI;

namespace Ankh.PendingChanges
{
    /// <summary>
    /// 
    /// </summary>
    [GlobalService(typeof(IPendingChangeHandler))]
    partial class PendingChangeHandler : AnkhService, IPendingChangeHandler
    {
        public PendingChangeHandler(IAnkhServiceProvider context)
            : base(context)
        {

        }

        public bool Commit(IEnumerable<PendingChange> changes, PendingChangeCommitArgs args)
        {
            // Ok, to make a commit happen we have to take 'a few' steps
            ILastChangeInfo ci = GetService<ILastChangeInfo>();

            if (ci != null)
                ci.SetLastChange(null, null);

            bool storeMessage = args.StoreMessageOnError;
            using (PendingCommitState state = new PendingCommitState(Context, changes))
                try
                {
                    state.KeepLocks = args.KeepLocks;
                    state.KeepChangeLists = args.KeepChangeLists;
                    state.LogMessage = args.LogMessage;

                    if (!PreCommit_VerifySingleRoot(state)) // Verify single root 'first'
                        return false;

                    if (!PreCommit_VerifyLogMessage(state))
                        return false;

                    if (!PreCommit_SaveDirty(state))
                        return false;

                    if (!PreCommit_AddNewFiles(state))
                        return false;

                    if (!PreCommit_DeleteMissingFiles(state))
                        return false;

                    state.FlushState();

                    if (!PreCommit_AddNeededParents(state))
                        return false;

                    if (!PreCommit_VerifySingleRoot(state)) // Verify single root 'again'
                        return false;
                    // if(!PreCommit_....())
                    //  return;

                    bool ok = false;
                    using (DocumentLock dl = GetService<IAnkhOpenDocumentTracker>().LockDocuments(state.CommitPaths, DocumentLockType.NoReload))
                    {
                        dl.MonitorChanges(); // Monitor files that are changed by keyword expansion

                        if (Commit_CommitToRepository(state))
                        {
                            storeMessage = true;
                            ok = true;
                        }

                        dl.ReloadModified();
                    }

                    return ok;
                }
                finally
                {
                    if (storeMessage)
                    {
                        if (!string.IsNullOrEmpty(state.LogMessage))
                        {
                            IAnkhConfigurationService config = GetService<IAnkhConfigurationService>();

                            if (config != null)
                            {
                                config.GetRecentLogMessages().Add(state.LogMessage);
                            }
                        }
                    }
                }
        }

        private bool PreCommit_VerifySingleRoot(PendingCommitState state)
        {
            SvnWorkingCopy wc = null;
            foreach (PendingChange pc in state.Changes)
            {
                SvnItem item = pc.Item;

                if (item.IsVersioned)
                {
                    SvnWorkingCopy w = item.WorkingCopy;

                    if(wc == null)
                        wc = w;
                    else if (w != null && w != wc)
                    {
                        state.MessageBox.Show("You can only commit from one working copy at a time", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);

                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Verifies if the log message is valid for the current policy
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private bool PreCommit_VerifyLogMessage(PendingCommitState state)
        {
            return true; // Logmessage always ok for now
        }

        /// <summary>
        /// Save all documents in the selection
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private bool PreCommit_SaveDirty(PendingCommitState state)
        {
            IAnkhOpenDocumentTracker tracker = state.GetService<IAnkhOpenDocumentTracker>();

            if (!tracker.SaveDocuments(state.CommitPaths))
            {
                state.MessageBox.Show("Failed to save all selected documents", "AnkhSvn", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds all files which are marked as to be added to subversion
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private bool PreCommit_AddNewFiles(PendingCommitState state)
        {
            foreach (PendingChange pc in state.Changes)
            {
                if (pc.Change != null && pc.Change.State == PendingChangeKind.New)
                {
                    SvnItem item = pc.Item;

                    // HACK: figure out why PendingChangeKind.New is still true
                    if (item.IsVersioned)
                        continue; // No need to add

                    SvnAddArgs a = new SvnAddArgs();
                    a.AddParents = true;
                    a.Depth = SvnDepth.Empty;
                    a.ThrowOnError = false;

                    if (!state.Client.Add(pc.FullPath, a))
                    {
                        if (state.MessageBox.Show(a.LastException.Message, "AnkhSvn", MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Error) == DialogResult.Cancel)
                            return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Adds all new parents of files to add to subversion
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private bool PreCommit_AddNeededParents(PendingCommitState state)
        {
            foreach (string path in new List<string>(state.CommitPaths))
            {
                SvnItem item = state.Cache[path];

                if (item.Status.LocalContentStatus == SvnStatus.Added)
                {
                    SvnItem parent = item.Parent;

                    while (parent != null && (parent.Status.LocalContentStatus == SvnStatus.Added))
                    {
                        if (!state.CommitPaths.Contains(parent.FullPath))
                            state.CommitPaths.Add(parent.FullPath);

                        parent = parent.Parent;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Fixes up missing files by deleting them
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private bool PreCommit_DeleteMissingFiles(PendingCommitState state)
        {
            foreach (string path in new List<string>(state.CommitPaths))
            {
                SvnItem item = state.Cache[path];

                if (item.Status.LocalContentStatus == SvnStatus.Missing && item.Status.NodeKind == SvnNodeKind.File)
                {
                    SvnDeleteArgs da = new SvnDeleteArgs();
                    da.KeepLocal = true;
                    da.ThrowOnError = false;

                    if (!state.Client.Delete(path, da))
                    {
                        state.MessageBox.Show(da.LastException.Message, "AnkhSvn", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            return true;
        }

        delegate void DoAsync(PendingCommitState ps, DoAsync closer);

        /// <summary>
        /// Finalizes the action by committing to the repository
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private bool Commit_CommitToRepository(PendingCommitState state)
        {
            bool ok = false;
            SvnCommitResult rslt = null;

            SvnDepth depth = state.CalculateCommitDepth();

            if (depth == SvnDepth.Unknown)
                return false;

            ProgressRunnerResult r = state.GetService<IProgressRunner>().RunModal("Committing",
                delegate(object sender, ProgressWorkerArgs e)
                {
                    SvnCommitArgs ca = new SvnCommitArgs();
                    ca.Depth = depth;
                    ca.KeepLocks = state.KeepLocks;
                    ca.KeepChangeLists = state.KeepChangeLists;
                    ca.LogMessage = state.LogMessage;
                    ok = e.Client.Commit(
                        state.CommitPaths,
                        ca, out rslt);
                });

            if (rslt != null)
            {
                ILastChangeInfo ci = GetService<ILastChangeInfo>();

                if (ci != null)
                    ci.SetLastChange("Committed:", rslt.Revision.ToString());
            }

            return ok;
        }        
    }
}
