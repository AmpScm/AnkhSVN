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
using System.Windows.Forms;

using SharpSvn;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Scc;
using Ankh.VS;
using Ankh.Scc.UI;


namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogChangeLogMessage, AlwaysAvailable = true)]
    class ChangeLogMessage : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            int count = 0;
            foreach (ISvnLogItem i in e.Selection.GetSelection<ISvnLogItem>())
            {
                count++;

                if (count > 1)
                    break;
            }
            if (count != 1)
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            ILogControl logWindow = (ILogControl)e.Selection.ActiveDialogOrFrameControl;
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

                    using (SvnClient client = e.GetService<ISvnClientPool>().GetClient())
                    {
                        client.SetRevisionProperty(new SvnUriTarget(logItems[0].RepositoryRoot, logItems[0].Revision), SvnPropertyNames.SvnLog, dialog.LogMessage);
                    }

                    logWindow.Restart();
                    // TODO: Somehow repair scroll position/number of items loaded
                }
            }
        }
    }
}
