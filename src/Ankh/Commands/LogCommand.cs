// Copyright 2005-2009 The AnkhSVN Project
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

using System.Diagnostics;
using SharpSvn;
using System.Collections.Generic;
using Ankh.UI;
using Ankh.UI.SvnLog;
using Ankh.Selection;
using Ankh.VS;
using Ankh.Scc;
using Ankh.Scc.UI;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the change log for the selected item.
    /// </summary>
    [SvnCommand(AnkhCommand.Log)]
    [SvnCommand(AnkhCommand.DocumentHistory)]
    [SvnCommand(AnkhCommand.ProjectHistory)]
    [SvnCommand(AnkhCommand.SolutionHistory)]
    [SvnCommand(AnkhCommand.ReposExplorerLog, AlwaysAvailable = true)]
    [SvnCommand(AnkhCommand.AnnotateShowLog, AlwaysAvailable = true)]
    sealed class LogCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            int i;

            switch (e.Command)
            {
                case AnkhCommand.ProjectHistory:
                    SccProject p = EnumTools.GetFirst(e.Selection.GetSelectedProjects(false));
                    if (p == null)
                        break;

                    ISccProjectInfo pi = e.GetService<IProjectFileMapper>().GetProjectInfo(p);

                    if (pi == null || string.IsNullOrEmpty(pi.ProjectDirectory))
                        break; // No project location

                    if (e.GetService<ISvnStatusCache>()[pi.ProjectDirectory].HasCopyableHistory)
                        return; // Ok, we have history!                                           

                    break; // No history

                case AnkhCommand.SolutionHistory:
                    IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

                    if (ss == null || string.IsNullOrEmpty(ss.ProjectRoot))
                        break;

                    if (e.GetService<ISvnStatusCache>()[ss.ProjectRoot].HasCopyableHistory)
                        return; // Ok, we have history!

                    break; // No history
                case AnkhCommand.DocumentHistory:
                    SvnItem docitem = e.Selection.ActiveDocumentSvnItem;
                    if (docitem != null && docitem.HasCopyableHistory)
                        return;
                    break; // No history
                case AnkhCommand.Log:
                    int itemCount = 0;
                    int needsRemoteCount = 0;
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (!item.IsVersioned)
                        {
                            e.Enabled = false;
                            return;
                        }

                        if (item.IsReplaced || item.IsAdded)
                        {
                            if (item.HasCopyableHistory)
                                needsRemoteCount++;
                            else
                            {
                                e.Enabled = false;
                                return;
                            }
                        }
                        itemCount++;
                    }
                    if (itemCount == 0 || (needsRemoteCount != 0 && itemCount > 1))
                    {
                        e.Enabled = false;
                        return;
                    }
                    if (needsRemoteCount >= 1)
                    {
                        // One remote log
                        Debug.Assert(needsRemoteCount == 1);
                        return;
                    }
                    
                    // Local log only
                    return;
                case AnkhCommand.ReposExplorerLog:
                    i = 0;
                    foreach (ISvnRepositoryItem item in e.Selection.GetSelection<ISvnRepositoryItem>())
                    {
                        if (item == null || item.Origin == null)
                            continue;
                        i++;
                        break;
                    }
                    if (i >= 1)
                        return;
                    break;
                case AnkhCommand.AnnotateShowLog:
                    IAnnotateSection section = EnumTools.GetSingle(e.Selection.GetSelection<IAnnotateSection>());
                    if (section != null && section.Revision >= 0)
                        return;
                    break;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            List<SvnOrigin> selected = new List<SvnOrigin>();
            ISvnStatusCache cache = e.GetService<ISvnStatusCache>();

            switch (e.Command)
            {
                case AnkhCommand.Log:
                    IAnkhDiffHandler diffHandler = e.GetService<IAnkhDiffHandler>();
                    List<SvnOrigin> items = new List<SvnOrigin>();
                    foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
                    {
                        Debug.Assert(i.IsVersioned);

                        if (i.IsReplaced || i.IsAdded)
                        {
                            if (!i.HasCopyableHistory)
                                continue;

                            items.Add(new SvnOrigin(diffHandler.GetCopyOrigin(i), i.WorkingCopy.RepositoryRoot));
                            continue;
                        }

                        items.Add(new SvnOrigin(i));
                    }
                    PerformLog(e.Context, items, null, null);
                    break;
                case AnkhCommand.SolutionHistory:
                    IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

                    PerformLog(e.Context, new SvnOrigin[] { new SvnOrigin(cache[settings.ProjectRoot]) }, null, null);
                    break;
                case AnkhCommand.ProjectHistory:
                    IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
                    foreach (SccProject p in e.Selection.GetSelectedProjects(false))
                    {
                        ISccProjectInfo info = mapper.GetProjectInfo(p);

                        if (info != null)
                            selected.Add(new SvnOrigin(cache[info.ProjectDirectory]));
                    }

                    PerformLog(e.Context, selected, null, null);
                    break;
                case AnkhCommand.DocumentHistory:
                    SvnItem docItem = e.Selection.ActiveDocumentSvnItem;
                    Debug.Assert(docItem != null);

                    PerformLog(e.Context, new SvnOrigin[] { new SvnOrigin(docItem) }, null, null);
                    break;
                case AnkhCommand.ReposExplorerLog:
                    foreach (ISvnRepositoryItem i in e.Selection.GetSelection<ISvnRepositoryItem>())
                    {
                        if (i != null && i.Origin != null)
                            selected.Add(i.Origin);
                    }

                    if (selected.Count > 0)
                        PerformLog(e.Context, selected, null, null);
                    break;
                case AnkhCommand.AnnotateShowLog:
                    IAnnotateSection section = EnumTools.GetSingle(e.Selection.GetSelection<IAnnotateSection>());

                    if (section == null)
                        return;

                    PerformLog(e.Context, new SvnOrigin[] { section.Origin }, section.Revision, null);

                    break;
            }
        }

        static void PerformLog(IAnkhServiceProvider context, ICollection<SvnOrigin> targets, SvnRevision start, SvnRevision end)
        {
            IAnkhPackage package = context.GetService<IAnkhPackage>();

            package.ShowToolWindow(AnkhToolWindow.Log);

            LogToolWindowControl logToolControl = context.GetService<ISelectionContext>().ActiveFrameControl as LogToolWindowControl;
            if (logToolControl != null)
                logToolControl.StartLog(targets, start, end);
        }
    }
}
