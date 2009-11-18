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

using Ankh.UI;
using System;
using SharpSvn;
using System.Collections.Generic;
using Ankh.Scc.UI;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Shows differences compared to local text base.
    /// </summary>
    [Command(AnkhCommand.DiffLocalItem)]
    [Command(AnkhCommand.ItemCompareBase)]
    [Command(AnkhCommand.ItemCompareCommitted)]
    [Command(AnkhCommand.ItemCompareHead)]
    [Command(AnkhCommand.ItemComparePrevious)]
    [Command(AnkhCommand.ItemCompareSpecific)]
    [Command(AnkhCommand.ItemShowChanges)]
    [Command(AnkhCommand.DocumentShowChanges)]
    public sealed class DiffLocalItem : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.DocumentShowChanges)
            {
                SvnItem sel = e.Selection.ActiveDocumentItem;

                if (sel == null || sel.IsDirectory || !sel.IsLocalDiffAvailable)
                    e.Enabled = false;

                return;
            }

            bool noConflictDiff = e.Command == AnkhCommand.ItemShowChanges;

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsDirectory)
                {
                    e.Enabled = false;
                    return;
                }
                if (item.IsVersioned && (item.Status.CombinedStatus != SvnStatus.Added || item.Status.IsCopied))
                {
                    if (e.Command == AnkhCommand.ItemCompareBase || e.Command == AnkhCommand.ItemShowChanges)
                    {
                        if (!item.IsLocalDiffAvailable)
                            continue;
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
                    if (!item.IsVersioned || (item.Status.CombinedStatus == SvnStatus.Added && !item.Status.IsCopied))
                        continue;

                    if (e.Command == AnkhCommand.ItemCompareBase || e.Command == AnkhCommand.ItemShowChanges)
                    {
                        if (!(item.IsModified || item.IsDocumentDirty))
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
                case AnkhCommand.ItemCompareHead:
                    revRange = new SvnRevisionRange(SvnRevision.Head, SvnRevision.Working);
                    break;
                case AnkhCommand.ItemComparePrevious:
                    revRange = new SvnRevisionRange(SvnRevision.Previous, SvnRevision.Working);
                    break;
            }

            if (e.PromptUser || selectedFiles.Count > 1 || revRange == null)
            {
                PathSelectorInfo info = new PathSelectorInfo("Select item for Diff", selectedFiles);
                info.SingleSelection = false;
                info.RevisionStart = revRange == null ? SvnRevision.Base : revRange.StartRevision;
                info.RevisionEnd = revRange == null ? SvnRevision.Working : revRange.EndRevision;

                info.EnableRecursive = false;
                info.Depth = SvnDepth.Infinity;

                PathSelectorResult result;
                // should we show the path selector?
                if (e.PromptUser || !Shift)
                {
                    IUIShell ui = e.GetService<IUIShell>();

                    result = ui.ShowPathSelector(info);
                    if (!result.Succeeded)
                        return;
                }
                else
                    result = info.DefaultResult;

                selectedFiles.Clear();
                selectedFiles.AddRange(result.Selection);
                revRange = new SvnRevisionRange(result.RevisionStart, result.RevisionEnd);
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
    }
}
