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

using Ankh.Commands;

using System.Windows.Forms;
using Ankh.Scc;
using Ankh.VS;
using Ankh.Selection;

namespace Ankh.UI.MergeWizard.Commands
{
    [Command(AnkhCommand.ItemMerge)]
    [Command(AnkhCommand.ProjectMerge)]
    [Command(AnkhCommand.SolutionMerge)]
    class Merge : ICommandHandler
    {
        /// <see cref="Ankh.Commands.ICommandHandler.OnUpdate" />
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            IFileStatusCache statusCache;
            int n = 0;
            switch (e.Command)
            {
                case AnkhCommand.ItemMerge:
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (!item.IsVersioned)
                        {
                            e.Enabled = false;
                            return;
                        }

                        n++;

                        if (n > 1)
                            break;
                    }
                    break;
                case AnkhCommand.ProjectMerge:
                    statusCache = e.GetService<IFileStatusCache>();
                    IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();
                    foreach (SvnProject project in e.Selection.GetSelectedProjects(false))
                    {
                        ISvnProjectInfo projInfo = pfm.GetProjectInfo(project);
                        if (projInfo == null || string.IsNullOrEmpty(projInfo.ProjectDirectory))
                        {
                            e.Enabled = false;
                            return;
                        }
                        SvnItem projectDir = statusCache[projInfo.ProjectDirectory];
                        if (!projectDir.IsVersioned)
                        {
                            e.Enabled = false;
                            return;
                        }

                        n++;

                        if (n > 1)
                            break;
                    }
                    break;
                case AnkhCommand.SolutionMerge:
                    statusCache = e.GetService<IFileStatusCache>();
                    IAnkhSolutionSettings solutionSettings = e.GetService<IAnkhSolutionSettings>();
                    if (solutionSettings == null || string.IsNullOrEmpty(solutionSettings.ProjectRoot))
                    {
                        e.Enabled = false;
                        return;
                    }
                    SvnItem solutionItem = statusCache[solutionSettings.ProjectRoot];
                    if (solutionItem.IsVersioned)
                        n = 1;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (n != 1)
                e.Enabled = false;
        }

        /// <see cref="Ankh.Commands.ICommandHandler.OnExecute" />
        public void OnExecute(CommandEventArgs e)
        {
            List<SvnItem> svnItems = new List<SvnItem>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();

            switch (e.Command)
            {
                case AnkhCommand.ItemMerge:
                    // TODO: Check for solution and/or project selection to use the folder instead of the file
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        svnItems.Add(item);
                    }
                    break;
                case AnkhCommand.ProjectMerge:
                    foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
                    {
                        IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();

                        ISvnProjectInfo info = pfm.GetProjectInfo(p);
                        if (info != null && info.ProjectDirectory != null)
                        {
                            svnItems.Add(cache[info.ProjectDirectory]);
                        }
                    }
                    break;
                case AnkhCommand.SolutionMerge:
                    svnItems.Add(cache[e.GetService<IAnkhSolutionSettings>().ProjectRoot]);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            IEnumerable<string> selectedFiles = e.Selection.GetSelectedFiles(true);
            IAnkhOpenDocumentTracker tracker = e.GetService<IAnkhOpenDocumentTracker>();

            using (DocumentLock lck = tracker.LockDocuments(selectedFiles, DocumentLockType.ReadOnly))
            using (lck.MonitorChangesForReload())
            using (MergeWizardDialog dialog = new MergeWizardDialog(e.Context, new MergeUtils(e.Context), svnItems[0]))
            {
                DialogResult result = dialog.ShowDialog(e.Context);
                //result = uiService.ShowDialog(dialog);

                if (result == DialogResult.OK)
                {
                    using (MergeResultsDialog mrd = new MergeResultsDialog())
                    {
                        mrd.MergeActions = dialog.Wizard.MergeActions;
                        mrd.ResolvedMergeConflicts = dialog.Wizard.ResolvedMergeConflicts;

                        mrd.ShowDialog(e.Context);
                    }
                }

            }
        }
    }
}
