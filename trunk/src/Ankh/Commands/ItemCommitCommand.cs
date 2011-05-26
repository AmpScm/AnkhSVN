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

using System.Windows.Forms;
using System.Collections.Generic;
using Ankh.Scc;
using Ankh.UI.SccManagement;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to commit selected items to the Subversion repository.
    /// </summary>
    [Command(AnkhCommand.CommitItem)]
    [Command(AnkhCommand.CommitProjectFile)]
    class ItemCommitCommand : CommandBase
    {
        string storedLogMessage;
        string storedIssueNumber;

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool recursive = (e.Command != AnkhCommand.CommitProjectFile);

            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(recursive))
            {
                if (i.IsVersioned)
                {
                    if (i.IsModified || i.IsDocumentDirty)
                        return; // There might be a new version of this file
                }
                else if (i.IsIgnored)
                    continue;
                else if (i.InSolution && i.IsVersionable)
                    return; // The file is 'to be added'
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            using (ProjectCommitDialog pcd = new ProjectCommitDialog())
            {
                pcd.Context = e.Context;
                pcd.LogMessageText = storedLogMessage;
                pcd.IssueNumberText = storedIssueNumber;

                pcd.PreserveWindowPlacement = true;

                pcd.LoadItems(e.Selection.GetSelectedSvnItems(true));

                DialogResult dr = pcd.ShowDialog(e.Context);

                storedLogMessage = pcd.LogMessageText;
                storedIssueNumber = pcd.IssueNumberText;

                if (dr != DialogResult.OK)
                    return;

                PendingChangeCommitArgs pca = new PendingChangeCommitArgs();
                pca.StoreMessageOnError = true;
                // TODO: Commit it!
                List<PendingChange> toCommit = new List<PendingChange>(pcd.GetSelection());
                pcd.FillArgs(pca);

                e.GetService<IPendingChangeHandler>().Commit(toCommit, pca);
            }

            // not in the finally, because we want to preserve the message for a 
            // non-successful commit
            storedLogMessage = null;
            storedIssueNumber = null;
        }
    }
}
