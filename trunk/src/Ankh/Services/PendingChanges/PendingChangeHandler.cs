// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using Ankh.UI;
using Ankh.Services.PendingChanges;

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
                            SvnItem item = pc.Item;
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

                    if (!PreCommit_HandleMissingFiles(state))
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

                    if (wc == null)
                        wc = w;
                    else if (w != null && w != wc)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(PccStrings.CommitSingleWc);
                        sb.AppendFormat(PccStrings.WorkingCopyX, wc.FullPath);
                        sb.AppendLine();
                        sb.AppendFormat(PccStrings.WorkingCopyX, w.FullPath);
                        sb.AppendLine();

                        state.MessageBox.Show(sb.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

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

            state.LogMessage = sb.ToString().TrimEnd() + Environment.NewLine;

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
                        if (state.MessageBox.Show(a.LastException.Message, "", MessageBoxButtons.OKCancel, System.Windows.Forms.MessageBoxIcon.Error) == DialogResult.Cancel)
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

                if (item.IsAdded || item.IsReplaced)
                {
                    SvnItem parent = item.Parent;

                    while (parent != null && (parent.IsAdded || parent.IsReplaced))
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

                if (item.Status.LocalContentStatus != SvnStatus.Missing)
                    continue;

                if (item.IsCasingConflicted)
                {
                    string correctCasing = GetSvnCasing(item);
                    string actualCasing = SvnTools.GetFullTruePath(item.FullPath);

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

            SvnDepth depth = state.CalculateCommitDepth();

            if (depth == SvnDepth.Unknown)
                return false;

            ProgressRunnerResult r = state.GetService<IProgressRunner>().RunModal(PccStrings.CommitTitle,
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
                    ci.SetLastChange(PccStrings.CommittedPrefix, rslt.Revision.ToString());
            }

            return ok;
        }
    }
}
