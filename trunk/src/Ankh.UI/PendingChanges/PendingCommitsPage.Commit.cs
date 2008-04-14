using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using Ankh.Scc;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Ankh.VS;

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

            public PendingCommitState(IAnkhServiceProvider context, SvnClient client, IEnumerable<PendingChange> changes)
                : base(context)
            {
                if (client == null)
                    throw new ArgumentNullException("client");
                else if (changes == null)
                    throw new ArgumentNullException("changes");

                _client = client;
                _changes.UniqueAddRange(changes);

                foreach (PendingChange pc in _changes)
                {
                    if (!_commitPaths.Contains(pc.FullPath))
                        _commitPaths.Add(pc.FullPath);
                }
            }

            public SvnClient Client
            {
                get { return _client; }
            }

            public HybridCollection<PendingChange> Changes
            {
                get { return _changes; }
            }

            public Collection<string> CommitPaths
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
                //throw new NotImplementedException();
            }

            #endregion
        }

        internal void DoCommit(bool keepingLocks)
        {
            // Ok, to make a commit happen we have to take 'a few' steps

            List<PendingChange> changes = new List<PendingChange>();

            foreach (PendingCommitItem pci in _listItems.Values)
            {
                if (pci.Checked)
                {
                    changes.Add(pci.PendingChange);
                }
            }

            using (SvnClient client = UISite.GetService<ISvnClientPool>().GetClient())
            using (PendingCommitState state = new PendingCommitState(UISite, client, changes))
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

                if (!PreCommit_AddNeededParents(state))
                    return;

                if (!PreCommit_VerifySingleRoot(state)) // Verify single root 'again'
                    return;
                // if(!PreCommit_....())
                //  return;

                Commit_CommitToRepository(state);
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
                if (pc.Change.State == PendingChangeKind.New)
                {
                    SvnItem item = pc.Item;

                    SvnAddArgs a = new SvnAddArgs();
                    a.AddParents = true;
                    a.Depth = SvnDepth.Empty;
                    a.ThrowOnError = false;

                    if (!state.Client.Add(pc.FullPath))
                    {
                        state.MessageBox.Show(a.LastException.Message, "AnkhSvn", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
            ProgressRunnerResult r = state.GetService<IProgressRunner>().Run("Committing...",
                delegate(object sender, ProgressWorkerArgs e)
                {
                    SvnCommitArgs ca = new SvnCommitArgs();
                    ca.Depth = SvnDepth.Empty;
                    ca.KeepLocks = state.KeepLocks;
                    ca.KeepChangeList = state.KeepChangeLists;
                    ca.LogMessage = state.LogMessage;
                    e.Client.Commit(
                        state.CommitPaths,
                        ca);
                });

            
            return true;
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
