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
using Ankh.Scc;
using Ankh.Selection;
using Ankh.UI.SccManagement;
using System.Windows.Forms;

namespace Ankh.Commands
{
    [SvnCommand(AnkhCommand.ProjectCommit)]
    [SvnCommand(AnkhCommand.SolutionCommit)]
    class SolutionCommitCommand : CommandBase
    {
        string logMessage;
        string issueId;
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SolutionExists || e.State.SolutionBuilding || e.State.Debugging || e.State.SolutionOpening)
            {
                e.Enabled = false;
                return;
            }
            e.Enabled = !EnumTools.IsEmpty(GetChanges(e));
        }

        public override void OnExecute(CommandEventArgs e)
        {
            using (ProjectCommitDialog dlg = new ProjectCommitDialog())
            {
                dlg.Context = e.Context;
                dlg.PreserveWindowPlacement = true;
                dlg.LoadChanges(GetChanges(e));

                dlg.LogMessageText = logMessage ?? "";
                dlg.IssueNumberText = issueId ?? "";

                DialogResult dr = dlg.ShowDialog(e.Context);

                logMessage = dlg.LogMessageText;
                issueId = dlg.IssueNumberText;

                if (dr == DialogResult.OK)
                {
                    PendingChangeCommitArgs pca = new PendingChangeCommitArgs();
                    pca.StoreMessageOnError = true;
                    // TODO: Commit it!
                    List<PendingChange> toCommit = new List<PendingChange>(dlg.GetSelection());
                    dlg.FillArgs(pca);

                    if (e.GetService<IPendingChangeHandler>().Commit(toCommit, pca))
                    {
                        logMessage = issueId = null;
                    }
                }
            }
        }

        sealed class ProjectListFilter
        {
            readonly HybridCollection<string> files = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            readonly HybridCollection<string> folders = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            readonly IProjectFileMapper _mapper;

            public ProjectListFilter(IAnkhServiceProvider context, IEnumerable<SccProject> projects)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                if (projects == null)
                    throw new ArgumentNullException("projects");

                _mapper = context.GetService<IProjectFileMapper>();
                List<SccProject> projectList = new List<SccProject>(projects);

                files.AddRange(_mapper.GetAllFilesOf(projectList));

                foreach (SccProject p in projectList)
                {
                    ISvnProjectInfo pi = _mapper.GetProjectInfo(p);

                    if (pi == null)
                        continue; // Ignore solution and non scc projects

                    string dir = pi.ProjectDirectory;

                    if (!string.IsNullOrEmpty(dir) && !folders.Contains(dir))
                        folders.Add(dir);
                }
            }

            public bool ShowChange(PendingChange pc)
            {
                if (files.Contains(pc.FullPath))
                    return true;
                if (!_mapper.ContainsPath(pc.FullPath))
                {
                    foreach (string f in folders)
                    {
                        if (pc.IsBelowPath(f))
                        {
                            // Path is not contained in any other project but below one of the project roots
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        static IEnumerable<PendingChange> GetChanges(BaseCommandEventArgs e)
        {
            IPendingChangesManager pcm = e.GetService<IPendingChangesManager>();
            if (e.Command == AnkhCommand.SolutionCommit)
            {
                foreach (PendingChange pc in pcm.GetAll())
                {
                    yield return pc;
                }
            }
            else
            {
                ProjectListFilter plf = new ProjectListFilter(e.Context, e.Selection.GetSelectedProjects(false));

                foreach (PendingChange pc in pcm.GetAll())
                {
                    if (plf.ShowChange(pc))
                        yield return pc;
                }
            }
        }
    }
}
