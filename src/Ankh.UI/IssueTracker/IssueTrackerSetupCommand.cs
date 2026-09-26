// Copyright 2009 The AnkhSVN Project
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
using Ankh.Commands;
using Ankh.VS;
using Ankh.Scc;

namespace Ankh.UI.IssueTracker
{
    [SvnCommand(AnkhCommand.SolutionIssueTrackerSetup)]
    class IssueTrackerSetupCommand : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            SvnItem item = GetRoot(e);
            IAnkhIssueService service = e.GetService<IAnkhIssueService>();
            int connectorCount =
                service == null || service.Connectors == null
                    ? 0
                    : service.Connectors.Count;

            bool available = IssueTrackerAvailabilityLogic.CanConfigure(
                item != null && item.IsVersioned,
                connectorCount);

            // The command is registered as defaultInvisible. Explicitly drive
            // visibility as well as enablement so it appears when a connector
            // is genuinely available and stays out of the menu otherwise.
            e.Enabled = available;
            e.Visible = available;
        }

        public void OnExecute(CommandEventArgs e)
        {
            using (IssueTrackerConfigDialog dialog =
                new IssueTrackerConfigDialog(e.Context))
            {
                if (dialog.ShowDialog(e.Context)
                    == System.Windows.Forms.DialogResult.OK)
                {
                    IssueTrackerAssociationManager.Apply(
                        e.Context,
                        dialog.NewIssueRepository);
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the "project root"
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static SvnItem GetRoot(BaseCommandEventArgs e)
        {
            SvnItem item = null;
            switch (e.Command)
            {
                case AnkhCommand.SolutionIssueTrackerSetup:
                    IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();
                    if (ss == null)
                        return null;

                    string root = ss.ProjectRoot;

                    if (string.IsNullOrEmpty(root))
                        return null;

                    item = e.GetService<ISvnStatusCache>()[root];
                    break;
            }

            return item;
        }
    }
}
