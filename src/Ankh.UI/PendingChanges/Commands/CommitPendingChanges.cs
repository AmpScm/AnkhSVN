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
using System.Text;
using Ankh.Commands;
using Ankh.UI.PendingChanges.Commits;

namespace Ankh.UI.PendingChanges.Commands
{
    [SvnCommand(AnkhCommand.CommitPendingChanges)]
    [SvnCommand(AnkhCommand.CommitPendingChangesKeepingLocks)]
    [SvnCommand(AnkhCommand.PendingChangesApplyWorkingCopy)]
    class CommitPendingChanges : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.State.SolutionBuilding || e.State.Debugging || e.State.SolutionOpening)
            {
                e.Enabled = false;
                return;
            }

            PendingCommitsPage commitPage = e.GetService<PendingCommitsPage>();
            PendingIssuesPage issuesPage = e.GetService<PendingIssuesPage>();
            if (commitPage == null)
            {
                e.Enabled = false;
                return;
            }

            switch (e.Command)
            {
                case AnkhCommand.CommitPendingChanges:
                case AnkhCommand.CommitPendingChangesKeepingLocks:
                    e.Enabled = true
                        // check if commit page or issues page is visible
                        && (false
                             || commitPage.Visible
                             || (issuesPage != null && issuesPage.Visible)
                             )
                         // make sure commit page can commit
                         && commitPage.CanCommit(e.Command == AnkhCommand.CommitPendingChangesKeepingLocks)
                         ;
                    break;
                case AnkhCommand.PendingChangesApplyWorkingCopy:
                    e.Enabled = commitPage.Visible && commitPage.CanApplyToWorkingCopy();
                    break;
            }
        }

        public void OnExecute(CommandEventArgs e)
        {
            PendingCommitsPage page = e.Context.GetService<PendingCommitsPage>();

            if (page != null)
                switch (e.Command)
                {
                    case AnkhCommand.CommitPendingChanges:
                    case AnkhCommand.CommitPendingChangesKeepingLocks:
                        page.DoCommit(e.Command == AnkhCommand.CommitPendingChangesKeepingLocks);
                        break;
                    case AnkhCommand.PendingChangesApplyWorkingCopy:
                        page.ApplyToWorkingCopy();
                        break;
                }
        }
    }
}
