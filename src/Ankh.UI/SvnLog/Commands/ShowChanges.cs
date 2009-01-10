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
using Ankh.Commands;
using SharpSvn;
using Ankh.VS;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Ids;
using Ankh.Scc.UI;
using Ankh.Scc;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogShowChanges, AlwaysAvailable = true)]
    class ShowChanges : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            ILogControl logWindow = e.Selection.ActiveDialogOrFrameControl as ILogControl;

            if (logWindow == null || logWindow.Origins == null)
            {
                e.Enabled = false;
                return;
            }

            if (UpdateForChangedFiles(e))
                return;

            UpdateForRevChanges(logWindow, e);
        }

        void UpdateForRevChanges(ILogControl logWindow, CommandUpdateEventArgs e)
        {
            SvnOrigin first = null;
            foreach (SvnOrigin origin in logWindow.Origins)
            {
                if (first != null)
                {
                    // We can't diff multiple items
                    e.Enabled = false;
                    return;
                }
                first = origin;
            }

            if (first == null)
            {
                e.Enabled = false;
                return;
            }

            SvnPathTarget pt = first.Target as SvnPathTarget;

            if (pt != null)
            {
                if (e.GetService<IFileStatusCache>()[pt.FullPath].IsDirectory)
                {
                    // We can't diff directories at this time
                    e.Enabled = false;
                    return;
                }
            }

            // Note: We can't have a local directory, but we can have a remote one.
        }

        bool UpdateForChangedFiles(CommandUpdateEventArgs e)
        {
            ISvnLogChangedPathItem change = null;
            foreach (ISvnLogChangedPathItem c in e.Selection.GetSelection<ISvnLogChangedPathItem>())
            {
                if (change != null)
                {
                    e.Enabled = false;
                    break;
                }
                change = c;
            }
            if (change != null)
            {
                // Skip all the files we cannot diff
                switch (change.Action)
                {
                    case SvnChangeAction.Add:
                        if (change.CopyFromRevision >= 0)
                            break; // We can retrieve this file using CopyFromPath
                        e.Enabled = false;
                        break;
                    case SvnChangeAction.Delete:
                        e.Enabled = false;
                        break;
                }
                return true;
            }

            return false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            ILogControl logWindow = e.Selection.ActiveDialogOrFrameControl as ILogControl;

            if (PerformRevisionChanges(logWindow, e))
                return;

            PerformFileChanges(e);
        }

        bool PerformRevisionChanges(ILogControl log, CommandEventArgs e)
        {
            long min = long.MaxValue;
            long max = long.MinValue;

            bool touched = false;

            HybridCollection<string> changedPaths = new HybridCollection<string>();
            foreach (Ankh.Scc.ISvnLogItem item in e.Selection.GetSelection<Ankh.Scc.ISvnLogItem>())
            {
                min = Math.Min(min, item.Revision);
                max = Math.Max(max, item.Revision);
                touched = true;
            }

            if (touched)
            {
                ExecuteDiff(e, log.Origins, new SvnRevisionRange(min - 1, max));
                return true;
            }

            return false;
        }

        void PerformFileChanges(CommandEventArgs e)
        {
            ISvnLogChangedPathItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogChangedPathItem>());

            if (item != null)
            {
                switch (item.Action)
                {
                    case SvnChangeAction.Delete:
                        return;
                    case SvnChangeAction.Add:
                    case SvnChangeAction.Replace:
                        if (item.CopyFromRevision < 0)
                            return;
                        break;
                }

                ExecuteDiff(e, new SvnOrigin[] { item.Origin }, new SvnRevisionRange(item.Revision - 1, item.Revision));
            }
        }

        void ExecuteDiff(CommandEventArgs e, ICollection<SvnOrigin> targets, SvnRevisionRange range)
        {
            if (targets.Count != 1)
                return;

            SvnTarget diffTarget = EnumTools.GetSingle(targets).Target;

            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();
            AnkhDiffArgs da = new AnkhDiffArgs();

            string[] files = diff.GetTempFiles(diffTarget, range.StartRevision, range.EndRevision, true);

            if (files == null)
                return;

            da.BaseFile = files[0];
            da.MineFile = files[1];
            da.BaseTitle = diff.GetTitle(diffTarget, range.StartRevision);
            da.MineTitle = diff.GetTitle(diffTarget, range.EndRevision);
            diff.RunDiff(da);
        }
    }

    public static class LogHelper
    {
        public static IEnumerable<SvnItem> IntersectWorkingCopyItemsWithChangedPaths(IEnumerable<SvnItem> workingCopyItems, IEnumerable<string> changedPaths)
        {
            foreach (SvnItem i in workingCopyItems)
            {
                foreach (string s in changedPaths)
                {
                    if (i.Status.Uri.ToString().EndsWith(s))
                    {
                        yield return i;
                        break;
                    }
                }
            }
        }
    }
}
