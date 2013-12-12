// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SharpSvn;
using Ankh.Scc;
using Ankh.VS;
using Ankh.UI.SccManagement;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.Configuration;

namespace Ankh.Services.PendingChanges
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

        IAnkhIssueService _issueService;
        IAnkhIssueService IssueService
        {
            get { return _issueService ?? (_issueService = GetService<IAnkhIssueService>()); }
        }

        IProjectCommitSettings _commitSettings;
        IProjectCommitSettings CommitSettings
        {
            get { return _commitSettings ?? (_commitSettings = GetService<IProjectCommitSettings>()); }
        }

        IAnkhConfigurationService _config;
        IAnkhConfigurationService Config
        {
            get { return _config ?? (_config = GetService<IAnkhConfigurationService>()); }
        }

        public bool ApplyChanges(IEnumerable<PendingChange> changes, PendingChangeApplyArgs args)
        {
            using (PendingCommitState state = new PendingCommitState(Context, changes))
            {
                if (!PreCommit_SaveDirty(state))
                    return false;

                if (!PreCommit_AddNewFiles(state))
                    return false;

                if (!PreCommit_HandleMissingFiles(state))
                    return false;

                state.FlushState();

                if (!PreCommit_AddNeededParents(state))
                    return false;

                return true;
            }
        }

        public bool CreatePatch(IEnumerable<PendingChange> changes, PendingChangeCreatePatchArgs args)
        {
            using (PendingCommitState state = new PendingCommitState(Context, changes))
            {
                if (!PreCommit_VerifySingleRoot(state)) // Verify single root 'first'
                    return false;

                if (!PreCommit_SaveDirty(state))
                    return false;

                if (args.AddUnversionedFiles)
                {
                    if (!PreCommit_AddNewFiles(state))
                        return false;

                    if (!PreCommit_HandleMissingFiles(state))
                        return false;
                }
                state.FlushState();

                if (!PreCommit_AddNeededParents(state))
                    return false;

                if (!PreCommit_VerifySingleRoot(state)) // Verify single root 'again'
                    return false;
            }

            string relativeToPath = args.RelativeToPath;
            string relativeToPathP = relativeToPath.EndsWith("\\") ? relativeToPath : (relativeToPath + "\\");
            string fileName = args.FileName;
            SvnRevisionRange revRange = new SvnRevisionRange(SvnRevision.Base, SvnRevision.Working);

            SvnDiffArgs a = new SvnDiffArgs();
            a.IgnoreAncestry = true;
            a.NoDeleted = false;
            a.Depth = SvnDepth.Empty;

            using (MemoryStream stream = new MemoryStream())
            {
                GetService<IProgressRunner>().RunModal(PccStrings.DiffTitle,
                    delegate(object sender, ProgressWorkerArgs e)
                    {
                        foreach (PendingChange pc in changes)
                        {
                            SvnItem item = pc.SvnItem;
                            SvnWorkingCopy wc;
                            if (!string.IsNullOrEmpty(relativeToPath)
                                && item.FullPath.StartsWith(relativeToPathP, StringComparison.OrdinalIgnoreCase))
                                a.RelativeToPath = relativeToPath;
                            else if ((wc = item.WorkingCopy) != null)
                                a.RelativeToPath = wc.FullPath;
                            else
                                a.RelativeToPath = null;

                            e.Client.Diff(item.FullPath, revRange, a, stream);
                        }

                        stream.Flush();
                        stream.Position = 0;
                    });
                using (StreamReader sr = new StreamReader(stream))
                {
                    string line;

                    // Parse to lines to resolve EOL issues
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        while (null != (line = sr.ReadLine()))
                            sw.WriteLine(line);
                    }
                }
            }
            return true;
        }

        public IEnumerable<PendingCommitState> GetCommitRoots(IEnumerable<PendingChange> changes)
        {
            List<SvnWorkingCopy> wcs = new List<SvnWorkingCopy>();
            List<List<PendingChange>> pcs = new List<List<PendingChange>>();

            foreach (PendingChange pc in changes)
            {
                SvnItem item = pc.SvnItem;
                SvnWorkingCopy wc = item.WorkingCopy;

                if (wc != null)
                {
                    int n = wcs.IndexOf(wc);

                    List<PendingChange> wcChanges;
                    if (n < 0)
                    {
                        wcs.Add(wc);
                        pcs.Add(wcChanges = new List<PendingChange>());
                    }
                    else
                        wcChanges = pcs[n];

                    wcChanges.Add(pc);
                }
            }

            if (wcs.Count <= 1)
            {
                yield return new PendingCommitState(Context, changes);
                yield break;
            }

            using (MultiWorkingCopyCommit dlg = new MultiWorkingCopyCommit())
            {
                dlg.SetInfo(wcs, pcs);

                if (dlg.ShowDialog(Context) != DialogResult.OK || dlg.ChangeGroups.Count == 0)
                {
                    yield return null;
                    yield break;
                }

                foreach (List<PendingChange> chg in dlg.ChangeGroups)
                {
                    yield return new PendingCommitState(Context, chg);
                }
            }
        }


        public bool Commit(IEnumerable<PendingChange> changes, PendingChangeCommitArgs args)
        {
            // Ok, to make a commit happen we have to take 'a few' steps
            ILastChangeInfo ci = GetService<ILastChangeInfo>();

            if (ci != null)
                ci.SetLastChange(null, null);

            bool storeMessage = args.StoreMessageOnError;

            foreach (PendingCommitState state in GetCommitRoots(changes))
            {
                if (state == null)
                    return false;

                try
                {
                    state.KeepLocks = args.KeepLocks;
                    state.KeepChangeLists = args.KeepChangeLists;
                    state.LogMessage = args.LogMessage;
                    state.IssueText = args.IssueText;

                    if (!PreCommit_VerifySingleRoot(state)) // Verify single root 'first'
                        return false;

                    // Verify this before verifying log message
                    // so that issue tracker integration has precedence 
                    if (!PreCommit_VerifyIssueTracker(state))
                        return false;

                    if (!PreCommit_VerifyLogMessage(state))
                        return false;

                    if (!PreCommit_VerifyNoConflicts(state))
                        return false;

                    if (!PreCommit_SaveDirty(state))
                        return false;

                    if (!PreCommit_AddNewFiles(state))
                        return false;

                    if (!PreCommit_HandleMissingFiles(state))
                        return false;

                    state.FlushState();

                    if (!PreCommit_AddNeededParents(state))
                        return false;

                    if (!PreCommit_VerifySingleRoot(state)) // Verify single root 'again'
                        return false;

                    if (!PreCommit_VerifyTargetsVersioned(state))
                        return false;
                    // if(!PreCommit_....())
                    //  return;


                    bool ok = false;
                    using (DocumentLock dl = GetService<IAnkhOpenDocumentTracker>().LockDocuments(state.CommitPaths, DocumentLockType.NoReload))
                    using (dl.MonitorChangesForReload()) // Monitor files that are changed by keyword expansion
                    {
                        if (Commit_CommitToRepository(state))
                        {
                            storeMessage = true;
                            ok = true;
                        }
                    }

                    if (!ok)
                        return false;
                }
                finally
                {
                    string msg = state.LogMessage;

                    state.Dispose();

                    if (storeMessage && msg != null && msg.Trim().Length > 0)
                    {
                        Config.GetRecentLogMessages().Add(msg);
                    }
                }
            }

            return true;
        }

        private bool PreCommit_VerifyNoConflicts(PendingCommitState state)
        {
            foreach (PendingChange pc in state.Changes)
            {
                SvnItem item = pc.SvnItem;

                if (item.IsConflicted)
                {
                    state.MessageBox.Show(PccStrings.OneOrMoreItemsConflicted,
                        "",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return false;
                }
            }

            return true;
        }

        private bool PreCommit_VerifySingleRoot(PendingCommitState state)
        {
            SvnWorkingCopy wc = null;
            foreach (PendingChange pc in state.Changes)
            {
                SvnItem item = pc.SvnItem;

                if (item.IsVersioned || item.IsVersionable)
                {
                    SvnWorkingCopy w = item.WorkingCopy;

                    if (wc == null)
                        wc = w;
                    else if (w != null && w != wc && w.RepositoryRoot != wc.RepositoryRoot)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(PccStrings.CommitSingleWc);
                        sb.AppendFormat(PccStrings.WorkingCopyXFromRepositoryY, wc.FullPath, wc.RepositoryRoot);
                        sb.AppendLine();
                        sb.AppendFormat(PccStrings.WorkingCopyXFromRepositoryY, w.FullPath, wc.RepositoryRoot);
                        sb.AppendLine();

                        state.MessageBox.Show(sb.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        return false;
                    }
                }
            }
            return true;
        }

        private bool PreCommit_VerifyIssueTracker(PendingCommitState state)
        {
            IssueRepository iRepo = IssueService.CurrentIssueRepository;
            if (iRepo != null)
            {
                List<Uri> uris = new List<Uri>();
                foreach (PendingChange pc in state.Changes)
                {
                    uris.Add(pc.Uri);
                }
                PreCommitArgs args = new PreCommitArgs(uris.ToArray(), 1);
                args.CommitMessage = state.LogMessage;
                args.IssueText = state.IssueText;

                iRepo.PreCommit(args);
                if (args.Cancel)
                    return false;

                state.LogMessage = args.CommitMessage;
                state.IssueText = null;

                if (args.SkipIssueVerify)
                    state.SkipIssueVerify = true;

                foreach (KeyValuePair<string, string> kv in args.CustomProperties)
                    state.CustomProperties[kv.Key] = kv.Value;
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
            if (state.LogMessage == null)
                return true; // Skip checks

            // And after checking whether the message is valid: Normalize the message the way the CLI would
            // * No whitespace at the end of lines
            // * Always a newline at the end

            StringBuilder sb = new StringBuilder();
            foreach (string line in state.LogMessage.Replace("\r", "").Split('\n'))
            {
                sb.AppendLine(line.TrimEnd());
            }

            string msg = sb.ToString();

            // no need to check for issue id if issue tracker integration already did it.
            if (CommitSettings.WarnIfNoIssue && !state.SkipIssueVerify)
            {
                bool haveIssue = false;
                // Use the project commit settings class to add an issue number (if available)

                if (CommitSettings.ShowIssueBox && !string.IsNullOrEmpty(state.IssueText))
                    haveIssue = true;

                IEnumerable<TextMarker> markers;
                if (!haveIssue
                    && !string.IsNullOrEmpty(state.LogMessage)
                    && IssueService.TryGetIssues(state.LogMessage, out markers)
                    && !EnumTools.IsEmpty(markers))
                {
                    haveIssue = true;
                }

                if (!haveIssue &&
                    state.MessageBox.Show(PccStrings.NoIssueNumber, "",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return false;
                }
            }
            msg = CommitSettings.BuildLogMessage(msg, state.IssueText);

            // And make sure the log message ends with a single newline
            state.LogMessage = msg.TrimEnd() + Environment.NewLine;

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
                state.MessageBox.Show(PccStrings.FailedToSaveBeforeCommit, "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
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
            Queue<string> toAdd = new Queue<string>();
            foreach (PendingChange pc in state.Changes)
            {
                if (pc.Change != null
                    && (pc.Change.State == PendingChangeKind.New
                        || pc.Change.State == PendingChangeKind.DeletedNew))
                {
                    SvnItem item = pc.SvnItem;

                    // HACK: figure out why PendingChangeKind.New is still true
                    if (item.IsVersioned && !item.IsDeleteScheduled)
                        continue; // No need to add

                    toAdd.Enqueue(item.FullPath);
                }
            }
            while (toAdd.Count > 0)
            {
                SvnException error = null;

                state.GetService<IProgressRunner>().RunModal(PccStrings.AddingTitle,
                    delegate(object sender, ProgressWorkerArgs e)
                    {
                        SvnAddArgs aa = new SvnAddArgs();
                        aa.AddParents = true;
                        aa.Depth = SvnDepth.Empty;
                        aa.ThrowOnError = false;

                        while (toAdd.Count > 0)
                        {
                            if (!e.Client.Add(toAdd.Dequeue(), aa))
                            {
                                error = aa.LastException;
                                break;
                            }
                        }
                    });

                if (error != null)
                {
                    if (error.SvnErrorCode == SvnErrorCode.SVN_ERR_WC_UNSUPPORTED_FORMAT)
                    {
                        state.MessageBox.Show(error.Message + Environment.NewLine + Environment.NewLine
                            + PccStrings.YouCanDownloadAnkh, "", MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return false;
                    }
                    else if (state.MessageBox.Show(error.Message, "", MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Error) != DialogResult.OK)
                        return false;
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

                if (item.IsNewAddition)
                {
                    SvnItem parent = item.Parent;
                    SvnWorkingCopy wc = item.WorkingCopy;

                    if (wc == null)
                    {
                        // This should be impossible. A node can't be added and not in a WC
                        item.MarkDirty();
                        continue;
                    }

                    string wcPath = wc.FullPath;

                    while (parent != null &&
                           !string.Equals(parent.FullPath, wcPath, StringComparison.OrdinalIgnoreCase)
                           && parent.IsNewAddition)
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
        /// Fixes up missing files by fixing their casing or deleting them
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private bool PreCommit_HandleMissingFiles(PendingCommitState state)
        {
            foreach (string path in new List<string>(state.CommitPaths))
            {
                SvnItem item = state.Cache[path];

                if (item.Status.LocalNodeStatus != SvnStatus.Missing)
                    continue;

                if (item.IsCasingConflicted)
                {
                    string correctCasing = GetSvnCasing(item);
                    string actualCasing = SvnTools.GetTruePath(item.FullPath);

                    if (correctCasing == null || actualCasing == null || !string.Equals(correctCasing, actualCasing, StringComparison.OrdinalIgnoreCase))
                        continue; // Nothing to fix here :(

                    string correctFile = Path.GetFileName(correctCasing);
                    string actualFile = Path.GetFileName(actualCasing);

                    if (correctFile == actualFile)
                        continue; // Casing issue is not in the file; can't fix :(

                    IAnkhOpenDocumentTracker odt = GetService<IAnkhOpenDocumentTracker>();
                    using (odt.LockDocument(correctCasing, DocumentLockType.NoReload))
                    using (odt.LockDocument(actualCasing, DocumentLockType.NoReload))
                    {
                        try
                        {
                            File.Move(actualCasing, correctCasing);

                            // Fix the name in the commit list
                            state.CommitPaths[state.CommitPaths.IndexOf(path)] = actualCasing;
                        }
                        catch
                        { }
                        finally
                        {
                            item.MarkDirty();
                            GetService<IFileStatusMonitor>().ScheduleGlyphUpdate(item.FullPath);
                        }
                    }
                }
                else if (!item.Exists)
                {
                    SvnDeleteArgs da = new SvnDeleteArgs();
                    da.KeepLocal = true;
                    da.ThrowOnError = false;

                    if (!state.Client.Delete(path, da))
                    {
                        state.MessageBox.Show(da.LastException.Message, "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool PreCommit_VerifyTargetsVersioned(PendingCommitState state)
        {
            Queue<string> toAdd = new Queue<string>();
            foreach (PendingChange pc in state.Changes)
            {
                if (!pc.SvnItem.IsVersioned)
                {
                    state.MessageBox.Show(string.Format(PccStrings.CantCommitUnversionedTargetX, pc.FullPath), "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        static string GetSvnCasing(SvnItem item)
        {
            string name = null;
            // Find the correct casing
            using (SvnWorkingCopyClient wcc = new SvnWorkingCopyClient())
            {
                SvnWorkingCopyEntriesArgs ea = new SvnWorkingCopyEntriesArgs();
                ea.ThrowOnCancel = false;
                ea.ThrowOnError = false;

                wcc.ListEntries(item.Directory, ea,
                    delegate(object sender, SvnWorkingCopyEntryEventArgs e)
                    {
                        if (string.Equals(e.FullPath, item.FullPath, StringComparison.OrdinalIgnoreCase))
                        {
                            name = e.FullPath;
                        }
                    });
            }

            return name;
        }

        /// <summary>
        /// Finalizes the action by committing to the repository
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        private bool Commit_CommitToRepository(PendingCommitState state)
        {
            bool ok = false;
            SvnCommitResult rslt = null;

            bool enableHooks = Config.Instance.EnableTortoiseSvnHooks;

            bool outOfDateError = false;
            bool otherError = false;

            StringBuilder outOfDateMessage = null;
            state.GetService<IProgressRunner>().RunModal(PccStrings.CommitTitle,
                delegate(object sender, ProgressWorkerArgs e)
                {
                    string itemPath = null;
                    SvnCommitArgs ca = new SvnCommitArgs();
                    ca.Depth = SvnDepth.Empty;
                    ca.KeepLocks = state.KeepLocks;
                    ca.KeepChangeLists = state.KeepChangeLists;
                    ca.LogMessage = state.LogMessage;

                    foreach (KeyValuePair<string, string> kv in state.CustomProperties)
                        ca.LogProperties.Add(kv.Key, kv.Value);

                    ca.AddExpectedError(SvnErrorCode.SVN_ERR_WC_NOT_UP_TO_DATE);
                    ca.AddExpectedError(SvnErrorCode.SVN_ERR_CLIENT_FORBIDDEN_BY_SERVER);
                    ca.AddExpectedError(SvnErrorCode.SVN_ERR_CLIENT_NO_LOCK_TOKEN);
                    ca.AddExpectedError(SvnErrorCode.SVN_ERR_IO_INCONSISTENT_EOL);
                    ca.AddExpectedError(SvnErrorCode.SVN_ERR_FS_TXN_OUT_OF_DATE);
                    ca.AddExpectedError(SvnErrorCode.SVN_ERR_RA_OUT_OF_DATE);
                    ca.AddExpectedError(SvnErrorCode.SVN_ERR_WC_FOUND_CONFLICT);
                    ca.Notify += delegate(object notifySender, SvnNotifyEventArgs notifyE)
                                {
                                    switch (notifyE.Action)
                                    {
                                        case SvnNotifyAction.FailedOutOfDate:
                                            if (notifyE.Error != null)
                                                ca.AddExpectedError(notifyE.Error.SvnErrorCode); // Don't throw an exception for this error
                                            outOfDateError = true;
                                            itemPath = itemPath ?? notifyE.FullPath;
                                            break;
                                        case SvnNotifyAction.FailedConflict:
                                        case SvnNotifyAction.FailedMissing:
                                        case SvnNotifyAction.FailedNoParent:
                                        case SvnNotifyAction.FailedLocked:
                                        case SvnNotifyAction.FailedForbiddenByServer:
                                            if (notifyE.Error != null)
                                                ca.AddExpectedError(notifyE.Error.SvnErrorCode); // Don't throw an exception for this error
                                            otherError = true;
                                            itemPath = itemPath ?? notifyE.FullPath;
                                            break;
                                    }
                                };
                    ca.RunTortoiseHooks = enableHooks;

                    ok = e.Client.Commit(
                        state.CommitPaths,
                        ca, out rslt);

                    if (!ok && ca.LastException != null)
                    {
                        if (!outOfDateError && !otherError)
                            outOfDateError = true; // Remaining errors are handled as exception

                        outOfDateMessage = new StringBuilder();
                        Exception ex = ca.LastException;

                        while (ex != null)
                        {
                            outOfDateMessage.AppendLine(ex.Message);
                            ex = ex.InnerException;
                        }

                        if (!string.IsNullOrEmpty(itemPath))
                        {
                            outOfDateMessage.AppendLine();
                            outOfDateMessage.AppendFormat(PccStrings.WhileCommittingX, itemPath);
                        }
                    }
                });

            if (outOfDateMessage != null)
            {
                state.MessageBox.Show(outOfDateMessage.ToString(),
                                      outOfDateError ? PccStrings.OutOfDateCaption : PccStrings.CommitFailedCaption,
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (rslt != null)
            {
                ILastChangeInfo ci = GetService<ILastChangeInfo>();

                if (ci != null)
                    ci.SetLastChange(PccStrings.CommittedPrefix, rslt.Revision.ToString());

                if (!string.IsNullOrEmpty(rslt.PostCommitError))
                    state.MessageBox.Show(rslt.PostCommitError, PccStrings.PostCommitError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                PostCommit_IssueTracker(state, rslt);
            }
            return ok;
        }

        private void PostCommit_IssueTracker(PendingCommitState state, SvnCommitResult result)
        {
            IssueRepository iRepo = IssueService.CurrentIssueRepository;
            if (iRepo == null)
                return;

            List<Uri> uris = new List<Uri>();
            foreach (PendingChange pc in state.Changes)
            {
                uris.Add(pc.Uri);
            }

            PostCommitArgs pca = new PostCommitArgs(uris.ToArray(), result.Revision, state.LogMessage);
            try
            {
                iRepo.PostCommit(pca);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }
    }
}
