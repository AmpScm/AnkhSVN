using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Scc;
using System.Windows.Forms;
using Ankh.UI.PendingChanges.Commits;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.PcLogEditorPasteFileList)]
    [Command(AnkhCommand.PcLogEditorPasteRecentLog)]
    class PasteToEditorList : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            LogMessageEditor lme = e.Selection.GetActiveControl<LogMessageEditor>();

            if (lme == null || lme.PasteSource == null)
                e.Enabled = false;
            else if(e.Command == AnkhCommand.PcLogEditorPasteFileList && !lme.PasteSource.HasPendingChanges)
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            LogMessageEditor lme = e.Selection.GetActiveControl<LogMessageEditor>();

            if (lme == null || lme.PasteSource == null)
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
            foreach (PendingChange pci in lme.PasteSource.PendingChanges)
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

                lme.PasteText(rmd.SelectedText);
            }
        }
    }
}
