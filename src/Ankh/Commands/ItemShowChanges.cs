// $Id$
//
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
                SvnItem sel = e.Selection.ActiveDocumentItem;

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
            List<SvnItem> selectedFiles = new List<SvnItem>();

            if (e.Command == AnkhCommand.DocumentShowChanges)
            {
                SvnItem item = e.Selection.ActiveDocumentItem;

                if(item == null)
                    return;
                selectedFiles.Add(item);
            }
            else
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    if (!item.IsVersioned || (item.Status.LocalNodeStatus == SvnStatus.Added && !item.Status.IsCopied))
                        continue;

                    if ( e.Command == AnkhCommand.ItemCompareBase
                         || e.Command == AnkhCommand.ItemShowChanges)
                    {
                        if (!(item.IsModified || item.IsDocumentDirty)
                            || !item.IsLocalDiffAvailable // exclude if local diff is not available
                            )
                            continue;
                    }

                    if (e.Command == AnkhCommand.DiffLocalItem
                        && !NotDeletedFilter(item))
                    {
                        continue;
                    }

                    selectedFiles.Add(item);
                }

            SvnRevisionRange revRange = null;
            switch (e.Command)
            {
                case AnkhCommand.DiffLocalItem:
                    break; // revRange null -> show selector
                case AnkhCommand.ItemCompareBase:
                case AnkhCommand.ItemShowChanges:
                case AnkhCommand.DocumentShowChanges:
                    revRange = new SvnRevisionRange(SvnRevision.Base, SvnRevision.Working);
                    break;
                case AnkhCommand.ItemCompareCommitted:
                    revRange = new SvnRevisionRange(SvnRevision.Committed, SvnRevision.Working);
                    break;
                case AnkhCommand.ItemCompareLatest:
                    revRange = new SvnRevisionRange(SvnRevision.Head, SvnRevision.Working);
                    break;
                case AnkhCommand.ItemComparePrevious:
                    revRange = new SvnRevisionRange(SvnRevision.Previous, SvnRevision.Working);
                    break;
            }

            if (e.PromptUser || selectedFiles.Count > 1 || revRange == null)
            {
                SvnRevision start = revRange == null ? SvnRevision.Base : revRange.StartRevision;
                SvnRevision end = revRange == null ? SvnRevision.Working : revRange.EndRevision;

                // should we show the path selector?
                if (e.PromptUser || !Shift)
                {
                    using (CommonFileSelectorDialog dlg = new CommonFileSelectorDialog())
                    {
                        dlg.Text = CommandStrings.CompareFilesTitle;
                        dlg.Items = selectedFiles;
                        dlg.RevisionStart = start;
                        dlg.RevisionEnd = end;

                        if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                            return;

                        selectedFiles.Clear();
                        selectedFiles.AddRange(dlg.GetCheckedItems());
                        start = dlg.RevisionStart;
                        end = dlg.RevisionEnd;
                    }
                }

                revRange = new SvnRevisionRange(start, end);
            }

            if (revRange.EndRevision.RevisionType == SvnRevisionType.Working ||
                revRange.StartRevision.RevisionType == SvnRevisionType.Working)
            {
                // Save only the files needed

                IAnkhOpenDocumentTracker tracker = e.GetService<IAnkhOpenDocumentTracker>();
                if (tracker != null)
                    tracker.SaveDocuments(SvnItem.GetPaths(selectedFiles));
            }

            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();
            foreach (SvnItem item in selectedFiles)
            {
                AnkhDiffArgs da = new AnkhDiffArgs();

                if ((item.Status.IsCopied || item.IsReplaced) &&
                    (!revRange.StartRevision.RequiresWorkingCopy || !revRange.EndRevision.RequiresWorkingCopy))
                {
                    // The file is copied, use its origins history instead of that of the new file
                    SvnUriTarget copiedFrom = diff.GetCopyOrigin(item);

                    // TODO: Maybe handle Previous/Committed as history

                    if (copiedFrom != null && !revRange.StartRevision.RequiresWorkingCopy)
                    {
                        if (null == (da.BaseFile = diff.GetTempFile(copiedFrom, revRange.StartRevision, true)))
                            return; // Canceled
                        da.BaseTitle = diff.GetTitle(copiedFrom, revRange.StartRevision);
                    }

                    if (copiedFrom != null && !revRange.EndRevision.RequiresWorkingCopy)
                    {
                        if (null == (da.MineFile = diff.GetTempFile(copiedFrom, revRange.EndRevision, true)))
                            return; // Canceled
                        da.MineTitle = diff.GetTitle(copiedFrom, revRange.EndRevision);
                    }
                }

                if (da.BaseFile == null)
                {
                    if (null == (da.BaseFile = (revRange.StartRevision == SvnRevision.Working) ? item.FullPath :
                        diff.GetTempFile(item, revRange.StartRevision, true)))
                    {
                        return; // Canceled
                    }

                    da.BaseTitle = diff.GetTitle(item, revRange.StartRevision);
                }

                if (da.MineFile == null)
                {
                    if (null == (da.MineFile = (revRange.EndRevision == SvnRevision.Working) ? item.FullPath :
                        diff.GetTempFile(item, revRange.EndRevision, true)))
                    {
                        return; // Canceled
                    }

                    da.MineTitle = diff.GetTitle(item, revRange.EndRevision);
                }

                if (!String.Equals(da.MineFile, item.FullPath, StringComparison.OrdinalIgnoreCase))
                    da.ReadOnly = true;

                diff.RunDiff(da);
            }
        }

        static bool NotDeletedFilter(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return !item.IsDeleteScheduled && item.Exists;
        }
    }
}
