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

namespace Ankh.UI.PendingChanges
{
    /// <summary>
    /// 
    /// </summary>
    partial class PendingCommitsPage
    {
        // TODO: This code should probably be moved to some kind of reusable service
        class PendingCommitState : AnkhService, IDisposable
        {
            SvnClient _client;
            bool _keepLocks;
            bool _keepChangeLists;
            HybridCollection<PendingChange> _changes = new HybridCollection<PendingChange>();
            HybridCollection<string> _commitPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            string _logMessage;

            public PendingCommitState(IAnkhServiceProvider context, IEnumerable<PendingChange> changes)
                : base(context)
            {
                if (changes == null)
                    throw new ArgumentNullException("changes");

                _changes.UniqueAddRange(changes);

                foreach (PendingChange pc in _changes)
                {
                    if (!_commitPaths.Contains(pc.FullPath))
                        _commitPaths.Add(pc.FullPath);
                }
            }

            public SvnClient Client
            {
                get 
                {
                    if (_client == null)
                        _client = GetService<ISvnClientPool>().GetClient();
                    
                    return _client; 
                }
            }

            public HybridCollection<PendingChange> Changes
            {
                get { return _changes; }
            }

            public HybridCollection<string> CommitPaths
            {
                get { return _commitPaths; }
            }

            public string LogMessage
            {
                get { return _logMessage; }
                set { _logMessage = value; }
            }

            public bool KeepLocks
            {
                get { return _keepLocks; }
                set { _keepLocks = value; }
            }

            public bool KeepChangeLists
            {
                get { return _keepChangeLists; }
                set { _keepChangeLists = value; }
            }

            [DebuggerStepThrough]
            public new T GetService<T>()
                where T : class
            {
                return base.GetService<T>();
            }

            [DebuggerStepThrough]
            public new T GetService<T>(Type serviceType)
                where T : class
            {
                return base.GetService<T>(serviceType);
            }

            AnkhMessageBox _mb;
            public AnkhMessageBox MessageBox
            {
                get { return _mb ?? (_mb = new AnkhMessageBox(this)); }
            }

            IFileStatusCache _cache;
            public IFileStatusCache Cache
            {
                get { return _cache ?? (_cache = GetService<IFileStatusCache>()); }
            }

            #region IDisposable Members

            public void Dispose()
            {
                FlushState();                
            }

            #endregion

            public SvnDepth CalculateCommitDepth()
            {
                SvnDepth depth = SvnDepth.Infinity;
                bool requireInfinity = false;

                foreach (string path in CommitPaths)
                {
                    SvnItem item = Cache[path];

                    if (item.IsDirectory)
                    {
                        if (item.IsDeleteScheduled)
                        {
                            // Infinity = OK
                            requireInfinity = true;
                        }
                        else
                            depth = SvnDepth.Empty;
                    }
                }

                if (requireInfinity && depth != SvnDepth.Infinity)
                {
                    // Houston we have a problem.
                    // - Directory deletes require depth infinity
                    // - There is another directory commit

                    // Let's see if committing with depth infinity would go wrong
                    bool hasOther = false;
                    using (SvnClient cl = GetService<ISvnClientPool>().GetNoUIClient())
                    {
                        bool cancel = false;
                        SvnStatusArgs sa = new SvnStatusArgs();
                        sa.ThrowOnError = false;
                        sa.ThrowOnCancel = false;
                        sa.RetrieveIgnoredEntries = false;
                        sa.Depth = SvnDepth.Infinity;
                        sa.Cancel += delegate(object sender, SvnCancelEventArgs ee) { if(cancel) ee.Cancel = true; };
                        
                        foreach (string path in CommitPaths)
                        {
                            SvnItem item = Cache[path];

                            if (!item.IsDirectory || item.IsDeleteScheduled)
                                continue; // Only check not to be deleted directories

                            if (!cl.Status(path, sa,
                                delegate(object sender, SvnStatusEventArgs ee)
                                {
                                    if (!CommitPaths.Contains(ee.FullPath))
                                    {
                                        hasOther = true;
                                        cancel = true; // Cancel via the cancel hook
                                    }
                                }))
                            {
                                hasOther = true;
                            }

                            if (hasOther)
                                break;
                        }
                    }

                    if (!hasOther)
                    {
                        // Ok; it is safe to commit with depth infinity; all items that would be committed
                        // with infinity would have been committed anyway

                        depth = SvnDepth.Infinity;
                    }
                }

                // Returns SvnDepth.Infinity if there are directories scheduled for commit 
                // and all directories scheduled for commit are to be deleted
                //
                // Returns SvnDepth.Empty in all other cases
                return depth;
            }

            internal void FlushState()
            {
                // This method assumes giving back the SvnClient instance flushes the state to the FileState cache
                if (_client != null)
                {
                    IDisposable cl = _client;
                    _client = null;
                    cl.Dispose();
                }
            }
        }

        internal void DoCommit(bool keepingLocks)
        {
            // Ok, to make a commit happen we have to take 'a few' steps
            ILastChangeInfo ci = UISite.GetService<ILastChangeInfo>();

            if (ci != null)
                ci.SetLastChange(null, null);


            List<PendingChange> changes = new List<PendingChange>();

            foreach (PendingCommitItem pci in _listItems.Values)
            {
                if (pci.Checked)
                {
                    changes.Add(pci.PendingChange);
                }
            }
            
            using (PendingCommitState state = new PendingCommitState(UISite, changes))
            {
                state.KeepLocks = keepingLocks;
                state.LogMessage = logMessageEditor.Text;

                if (!PreCommit_VerifySingleRoot(state)) // Verify single root 'first'
                    return;

                if (!PreCommit_VerifyLogMessage(state))
                    return;

                if (!PreCommit_SaveDirty(state))
                    return;

                if (!PreCommit_AddNewFiles(state))
                    return;

                if (!PreCommit_DeleteMissingFiles(state))
                    return;

                state.FlushState();

                if (!PreCommit_AddNeededParents(state))
                    return;

                if (!PreCommit_VerifySingleRoot(state)) // Verify single root 'again'
                    return;
                // if(!PreCommit_....())
                //  return;

                using (DocumentLock dl = UISite.GetService<IAnkhOpenDocumentTracker>().LockDocuments(state.CommitPaths, DocumentLockType.NoReload))
                {
                    dl.MonitorChanges(); // Monitor files that are changed by keyword expansion

                    if (Commit_CommitToRepository(state))
                    {
                        // TODO: Store the logmessage!

                        logMessageEditor.Text = ""; // Clear the existing logmessage when the commit succeeded                        
                    }

                    dl.ReloadModified();
                }
            }
        }

        private bool PreCommit_VerifySingleRoot(PendingCommitState state)
        {
            string guid = null;
            foreach (PendingChange pc in state.Changes)
            {
                SvnItem item = pc.Item;

                if (item.IsVersioned)
                {
                    string id = item.Status.RepositoryId;

                    if (guid == null)
                        id = guid;
                    else if (guid != id)
                    {
                        state.MessageBox.Show("You can only commit to one repository at a time", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);

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

            ProgressRunnerResult r = state.GetService<IProgressRunner>().Run("Committing...",
                delegate(object sender, ProgressWorkerArgs e)
                {
                    SvnCommitArgs ca = new SvnCommitArgs();
                    ca.Depth = state.CalculateCommitDepth();
                    ca.KeepLocks = state.KeepLocks;
                    ca.KeepChangeLists = state.KeepChangeLists;
                    ca.LogMessage = state.LogMessage;
                    ok = e.Client.Commit(
                        state.CommitPaths,
                        ca, out rslt);
                });

            if (rslt != null && UISite != null)
            {
                ILastChangeInfo ci = UISite.GetService<ILastChangeInfo>();

                if (ci != null)
                    ci.SetLastChange("Committed:", rslt.Revision.ToString());
            }

            return ok;
        }

        internal bool CanCommit(bool keepingLocks)
        {
            if (_listItems.Count == 0)
                return false;

            foreach (PendingCommitItem pci in _listItems.Values)
            {
                if (!pci.Checked)
                    continue;

                if (!keepingLocks || pci.PendingChange.Item.IsLocked)
                    return true;
            }

            return false;
        }
    }
}
