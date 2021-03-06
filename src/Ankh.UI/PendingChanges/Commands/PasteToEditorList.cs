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
using Ankh.Scc;
using System.Windows.Forms;
using Ankh.UI.PendingChanges.Commits;
using Ankh.UI.Commands;

namespace Ankh.UI.PendingChanges.Commands
{
    [SvnCommand(AnkhCommand.PcLogEditorPasteFileList, HideWhenDisabled=false)]
    [SvnCommand(AnkhCommand.PcLogEditorPasteRecentLog, HideWhenDisabled=false)]
    class PasteToEditorList : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            LogMessageEditor lme = e.Selection.GetActiveControl<LogMessageEditor>();

            if (lme == null || lme.ReadOnly || lme.PendingChangeUI == null)
                e.Enabled = e.Visible = false;
            else if (e.Command == AnkhCommand.PcLogEditorPasteFileList && !lme.PendingChangeUI.HasCheckedItems)
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            LogMessageEditor lme = e.Selection.GetActiveControl<LogMessageEditor>();

            if (lme == null || lme.PendingChangeUI == null)
                return;

            switch (e.Command)
            {
                case AnkhCommand.PcLogEditorPasteFileList:
                    OnPasteList(e, lme);
                    break;
                case AnkhCommand.PcLogEditorPasteRecentLog:
                    OnPasteRecent(e, lme);
                    break;
            }
        }

        void OnPasteList(CommandEventArgs e, LogMessageEditor lme)
        {
            StringBuilder sb = new StringBuilder();
            foreach (PendingChange pci in lme.PendingChangeUI.CheckedItems)
            {
                sb.AppendFormat("* {0}", pci.RelativePath);
                sb.AppendLine();
            }

            lme.PasteText(sb.ToString());
        }

        void OnPasteRecent(CommandEventArgs e, LogMessageEditor lme)
        {
            using (RecentMessageDialog rmd = new RecentMessageDialog())
            {
                rmd.Context = e.Context;

                if (DialogResult.OK != rmd.ShowDialog(e.Context))
                    return;

                string text = rmd.SelectedText;
                if (!string.IsNullOrEmpty(text))
                    lme.PasteText(text);
            }
        }
    }
}
