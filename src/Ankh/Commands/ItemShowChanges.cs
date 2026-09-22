// Copyright 2003-2009 The AnkhSVN Project
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
using System.Windows.Forms;

using SharpSvn;

using Ankh.Scc;
using Ankh.Scc.UI;
using Ankh.UI.PathSelector;

namespace Ankh.Commands
{
    /// <summary>
    /// Shows differences compared to local text base.
    /// </summary>
    [SvnCommand(AnkhCommand.DiffLocalItem)]
    [SvnCommand(AnkhCommand.ItemCompareBase)]
    [SvnCommand(AnkhCommand.ItemCompareCommitted)]
    [SvnCommand(AnkhCommand.ItemCompareLatest)]
    [SvnCommand(AnkhCommand.ItemComparePrevious)]
    [SvnCommand(AnkhCommand.ItemCompareSpecific)]
    [SvnCommand(AnkhCommand.ItemShowChanges)]
    [SvnCommand(AnkhCommand.DocumentShowChanges)]
    public sealed class DiffLocalItem : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.DocumentShowChanges)
            {
                SvnItem sel = e.Selection.ActiveDocumentSvnItem;

                if (sel == null || sel.IsDirectory ||!sel.IsLocalDiffAvailable)
                    e.Enabled = false;

                return;
            }

            bool noConflictDiff = e.Command == AnkhCommand.ItemShowChanges;

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (!item.IsFile
                    && (e.Command != AnkhCommand.DiffLocalItem || item.NodeKind != SvnNodeKind.File))
                {
                    e.Enabled = false;
                    return;
                }
                if (item.IsVersioned && (item.Status.LocalNodeStatus != SvnStatus.Added || item.Status.IsCopied))
                {
                    if (e.Command == AnkhCommand.ItemCompareBase 
                        || e.Command == AnkhCommand.ItemShowChanges
                        )
                    {
                        if (!item.IsLocalDiffAvailable)
                        {
                            // skip if local diff is not available
                            // single-select -> don't show 'Show Changes" option
                            // multi-select -> show the option, exclude these items during execution
                            continue;
                        }
                    }

                    if (noConflictDiff && item.IsConflicted)
                    {
                        // Use advanced diff to get a diff, or 'Edit Conflict' to resolve it
                        continue;
                    }

                    return;
                }
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            List<SvnItem> selectedFiles = GetSelectedFiles(e);
            if (selectedFiles == null)
                return;

            SvnRevisionRange revisionRange;
            if (!TryGetRevisionRange(e, selectedFiles, out revisionRange))
                return;

            SaveWorkingDocumentsIfNeeded(e, selectedFiles, revisionRange);

            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();
            foreach (SvnItem item in selectedFiles)
            {
                if (!TryRunDiff(item, revisionRange, diff))
                    return;
            }
        }

        private List<SvnItem> GetSelectedFiles(CommandEventArgs e)
        {
            List<SvnItem> selectedFiles = new List<SvnItem>();

            if (e.Command == AnkhCommand.DocumentShowChanges)
            {
                SvnItem item = e.Selection.ActiveDocumentSvnItem;
                if (item == null)
                    return null;

                selectedFiles.Add(item);
                return selectedFiles;
            }

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (DiffLocalItemLogic.ShouldInclude(
                        e.Command,
                        DiffLocalItemSelectionInfo.From(item)))
                {
                    selectedFiles.Add(item);
                }
            }

            return selectedFiles;
        }

        private bool TryGetRevisionRange(
            CommandEventArgs e,
            IList<SvnItem> selectedFiles,
            out SvnRevisionRange revisionRange)
        {
            revisionRange =
                DiffLocalItemLogic.GetDefaultRevisionRange(e.Command);

            if (!e.PromptUser
                && selectedFiles.Count <= 1
                && revisionRange != null)
            {
                return true;
            }

            SvnRevision start = revisionRange == null
                ? SvnRevision.Base
                : revisionRange.StartRevision;
            SvnRevision end = revisionRange == null
                ? SvnRevision.Working
                : revisionRange.EndRevision;

            if (e.PromptUser || !Shift)
            {
                using (CommonFileSelectorDialog dialog =
                    new CommonFileSelectorDialog())
                {
                    dialog.Text = CommandStrings.CompareFilesTitle;
                    dialog.Items = selectedFiles;
                    dialog.RevisionStart = start;
                    dialog.RevisionEnd = end;

                    if (dialog.ShowDialog(e.Context) != DialogResult.OK)
                        return false;

                    selectedFiles.Clear();
                    foreach (SvnItem item in dialog.GetCheckedItems())
                        selectedFiles.Add(item);

                    start = dialog.RevisionStart;
                    end = dialog.RevisionEnd;
                }
            }

            revisionRange = new SvnRevisionRange(start, end);
            return true;
        }

        private static void SaveWorkingDocumentsIfNeeded(
            CommandEventArgs e,
            IList<SvnItem> selectedFiles,
            SvnRevisionRange revisionRange)
        {
            if (!DiffLocalItemLogic.RequiresDocumentSave(revisionRange))
                return;

            IAnkhOpenDocumentTracker tracker =
                e.GetService<IAnkhOpenDocumentTracker>();

            if (tracker != null)
                tracker.SaveDocuments(SvnItem.GetPaths(selectedFiles));
        }

        private static bool TryRunDiff(
            SvnItem item,
            SvnRevisionRange revisionRange,
            IAnkhDiffHandler diff)
        {
            AnkhDiffArgs args;
            if (!TryCreateDiffArgs(item, revisionRange, diff, out args))
                return false;

            diff.RunDiff(args);
            return true;
        }

        private static bool TryCreateDiffArgs(
            SvnItem item,
            SvnRevisionRange revisionRange,
            IAnkhDiffHandler diff,
            out AnkhDiffArgs args)
        {
            args = new AnkhDiffArgs();

            if (DiffLocalItemLogic.ShouldUseCopyOrigin(
                    item.Status.IsCopied,
                    item.IsReplaced,
                    revisionRange))
            {
                SvnUriTarget copiedFrom = diff.GetCopyOrigin(item);

                string baseFile;
                string baseTitle;
                if (!TryGetCopyOriginSide(
                        diff,
                        copiedFrom,
                        revisionRange.StartRevision,
                        out baseFile,
                        out baseTitle))
                {
                    return false;
                }

                args.BaseFile = baseFile;
                args.BaseTitle = baseTitle;

                string mineFile;
                string mineTitle;
                if (!TryGetCopyOriginSide(
                        diff,
                        copiedFrom,
                        revisionRange.EndRevision,
                        out mineFile,
                        out mineTitle))
                {
                    return false;
                }

                args.MineFile = mineFile;
                args.MineTitle = mineTitle;
            }

            if (args.BaseFile == null)
            {
                string baseFile;
                string baseTitle;
                if (!TryGetItemSide(
                        diff,
                        item,
                        revisionRange.StartRevision,
                        out baseFile,
                        out baseTitle))
                {
                    return false;
                }

                args.BaseFile = baseFile;
                args.BaseTitle = baseTitle;
            }

            if (args.MineFile == null)
            {
                string mineFile;
                string mineTitle;
                if (!TryGetItemSide(
                        diff,
                        item,
                        revisionRange.EndRevision,
                        out mineFile,
                        out mineTitle))
                {
                    return false;
                }

                args.MineFile = mineFile;
                args.MineTitle = mineTitle;
            }

            if (!String.Equals(
                    args.MineFile,
                    item.FullPath,
                    StringComparison.OrdinalIgnoreCase))
            {
                args.ReadOnly = true;
            }

            return true;
        }

        private static bool TryGetCopyOriginSide(
            IAnkhDiffHandler diff,
            SvnUriTarget copiedFrom,
            SvnRevision revision,
            out string file,
            out string title)
        {
            file = null;
            title = null;

            if (copiedFrom == null || revision.RequiresWorkingCopy)
                return true;

            file = diff.GetTempFile(copiedFrom, revision, true);
            if (file == null)
                return false;

            title = diff.GetTitle(copiedFrom, revision);
            return true;
        }

        private static bool TryGetItemSide(
            IAnkhDiffHandler diff,
            SvnItem item,
            SvnRevision revision,
            out string file,
            out string title)
        {
            file = revision == SvnRevision.Working
                ? item.FullPath
                : diff.GetTempFile(item, revision, true);

            if (file == null)
            {
                title = null;
                return false;
            }

            title = diff.GetTitle(item, revision);
            return true;
        }
    }
}
