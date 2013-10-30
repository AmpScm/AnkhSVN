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

using System.Collections.Generic;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Commands;
using Ankh.Configuration;
using Ankh.Scc;
using Ankh.Scc.UI;
using Ankh.VS;


namespace Ankh.UI.SvnLog.Commands
{
    [SvnCommand(AnkhCommand.LogChangeLogMessage, AlwaysAvailable = true)]
    class ChangeLogMessage : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>()) == null)
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            IAnkhSolutionSettings slnSettings = e.GetService<IAnkhSolutionSettings>();
            List<ISvnLogItem> logItems = new List<ISvnLogItem>(e.Selection.GetSelection<ISvnLogItem>());
            if (logItems.Count != 1)
                return;

            using (EditLogMessageDialog dialog = new EditLogMessageDialog())
            {
                dialog.Context = e.Context;
                dialog.LogMessage = logItems[0].LogMessage;

                if (dialog.ShowDialog(e.Context) == DialogResult.OK)
                {
                    if (dialog.LogMessage == logItems[0].LogMessage)
                        return; // No changes

                    IAnkhConfigurationService config = e.GetService<IAnkhConfigurationService>();

                    if (config != null)
                    {
                        if (dialog.LogMessage != null && dialog.LogMessage.Trim().Length > 0)
                            config.GetRecentLogMessages().Add(dialog.LogMessage);
                    }

                    using (SvnClient client = e.GetService<ISvnClientPool>().GetClient())
                    {
                        SvnSetRevisionPropertyArgs sa = new SvnSetRevisionPropertyArgs();
                        sa.AddExpectedError(SvnErrorCode.SVN_ERR_REPOS_DISABLED_FEATURE);
                        client.SetRevisionProperty(logItems[0].RepositoryRoot, logItems[0].Revision, SvnPropertyNames.SvnLog, dialog.LogMessage, sa);

                        if (sa.LastException != null &&
                            sa.LastException.SvnErrorCode == SvnErrorCode.SVN_ERR_REPOS_DISABLED_FEATURE)
                        {
                            AnkhMessageBox mb = new AnkhMessageBox(e.Context);

                            mb.Show(sa.LastException.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            return;
                        }
                    }

                    ILogControl logWindow = e.Selection.GetActiveControl<ILogControl>();

                    if (logWindow != null)
                    {
                        // TODO: Somehow repair scroll position/number of items loaded
                        logWindow.Restart();
                    }
                }
            }
        }
    }
}
