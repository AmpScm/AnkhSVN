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
using SharpSvn;

using Ankh.UI;
using Ankh.Scc;
using System.Windows.Forms;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that updates an item.
    /// </summary>
    [Command(AnkhCommand.UpdateItemSpecific)]
    [Command(AnkhCommand.UpdateItemLatest)]
    [Command(AnkhCommand.UpdateItemLatestRecursive)]
    class UpdateItem : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool hasDirectory = false;
            bool hasFile = false;
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
            SvnDepth depth;
            List<string> files = new List<string>();
            if (e.Command == AnkhCommand.UpdateItemSpecific)
            {
                IUIShell uiShell = e.GetService<IUIShell>();

                PathSelectorInfo info = new PathSelectorInfo("Select Items to Update",
                    e.Selection.GetSelectedSvnItems(true));

                info.CheckedFilter += delegate(SvnItem item) { return item.IsVersioned; };
                info.VisibleFilter += delegate(SvnItem item) { return item.IsVersioned; };
                info.EnableRecursive = true;
                info.RevisionStart = SvnRevision.Head;
                info.Depth = SvnDepth.Infinity;

                PathSelectorResult result = !Shift ? uiShell.ShowPathSelector(info) : info.DefaultResult;

                if (!result.Succeeded)
                    return;

                updateTo = result.RevisionStart;
                depth = result.Depth;
                List<SvnItem> dirs = new List<SvnItem>();

                foreach (SvnItem item in result.Selection)
                {
                    if (!item.IsVersioned)
                        continue;

                    if (item.IsDirectory)
                    {
                        if (result.Depth < SvnDepth.Infinity)
                        {
                            AnkhMessageBox mb = new AnkhMessageBox(e.Context);

                            DialogResult dr = mb.Show(CommandStrings.CantUpdateDirectoriesNonRecursive, "", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning);

                            if (dr != DialogResult.Yes)
                                return;

                            depth = SvnDepth.Infinity;
                        }
                    }

                    bool found = false;
                    foreach (SvnItem dir in dirs)
                    {
                        if (item.IsBelowPath(dir))
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
            else
            {
                updateTo = SvnRevision.Head;
                depth = SvnDepth.Infinity;
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
                                                             ua.Depth = depth;
                                                             ua.Revision = updateTo;
                                                             e.GetService<IConflictHandler>().
                                                                 RegisterConflictHandler(ua, ee.Synchronizer);
                                                             ee.Client.Update(files, ua, out ur);
                                                         });
            }
        }
    }
}
