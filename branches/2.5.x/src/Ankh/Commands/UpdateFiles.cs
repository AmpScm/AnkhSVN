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

using System.Collections.Generic;
using System.Windows.Forms;
using SharpSvn;

using Ankh.Scc;
using Ankh.UI;
using Ankh.UI.PathSelector;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that updates an item.
    /// </summary>
    [SvnCommand(AnkhCommand.UpdateItemSpecific)]
    [SvnCommand(AnkhCommand.UpdateItemLatest)]
    [SvnCommand(AnkhCommand.UpdateItemLatestRecursive)]
    [SvnCommand(AnkhCommand.UpdateProjectFileSpecific)]
    sealed class UpdateFiles : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool hasDirectory = false;
            bool hasFile = false;

            if (e.State.SolutionBuilding || e.State.Debugging || e.State.SolutionOpening)
            {
                e.Enabled = false;
                return;
            }

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsDirectory)
                    hasDirectory = true;
                else if (item.IsFile)
                    hasFile = true;

                if (hasFile && hasDirectory)
                    break;
            }

            if (hasDirectory && !hasFile)
            {
                // User should use the recursive folder update
                e.Enabled = false;
                return;
            }

            if (e.Command != AnkhCommand.UpdateProjectFileSpecific)
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
                {
                    if (item.IsVersioned)
                        return;
                }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            SvnRevision updateTo;
            List<string> files = new List<string>();

            if (e.Command == AnkhCommand.UpdateItemSpecific
                || e.Command == AnkhCommand.UpdateProjectFileSpecific)
            {
                updateTo = SvnRevision.Head;
                List<SvnItem> items = new List<SvnItem>();

                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
                {
                    if (item.IsFile && item.IsVersioned)
                        items.Add(item);
                }

                using (CommonFileSelectorDialog dlg = new CommonFileSelectorDialog())
                {
                    dlg.Text = CommandStrings.UpdateFilesTitle;
                    dlg.Items = items;
                    dlg.RevisionStart = updateTo;

                    if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    files.AddRange(SvnItem.GetPaths(dlg.GetCheckedItems()));
                    updateTo = dlg.RevisionStart;
                }
            }
            else
            {
                updateTo = SvnRevision.Head;
                List<SvnItem> dirs = new List<SvnItem>();

                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
                {
                    if (!item.IsVersioned)
                        continue;

                    bool found = false;
                    foreach (SvnItem p in dirs)
                    {
                        if (item.IsBelowPath(p) && p.WorkingCopy == item.WorkingCopy)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                        continue;

                    files.Add(item.FullPath);

                    if (item.IsDirectory)
                        dirs.Add(item);
                }
            }

            IAnkhOpenDocumentTracker tracker = e.GetService<IAnkhOpenDocumentTracker>();
            tracker.SaveDocuments(e.Selection.GetSelectedFiles(true));
            using (DocumentLock lck = tracker.LockDocuments(files, DocumentLockType.NoReload))
            using (lck.MonitorChangesForReload())
            {
                SvnUpdateResult ur;
                ProgressRunnerArgs pa = new ProgressRunnerArgs();
                pa.CreateLog = true;

                e.GetService<IProgressRunner>().RunModal(CommandStrings.UpdatingTitle, pa,
                                                         delegate(object sender, ProgressWorkerArgs ee)
                                                         {
                                                             SvnUpdateArgs ua = new SvnUpdateArgs();
                                                             ua.Depth = SvnDepth.Infinity;
                                                             ua.Revision = updateTo;
                                                             e.GetService<IConflictHandler>().
                                                                 RegisterConflictHandler(ua, ee.Synchronizer);
                                                             ee.Client.Update(files, ua, out ur);
                                                         });
            }
        }
    }
}
