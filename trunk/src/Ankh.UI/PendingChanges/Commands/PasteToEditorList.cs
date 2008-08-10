using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Scc;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.PcLogEditorPasteFileList)]
    [Command(AnkhCommand.PcLogEditorPasteRecentLog)]
    class PasteToEditorList : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            LogMessageEditor lme = e.Selection.GetActiveControl<LogMessageEditor>();

            if (lme == null || lme.PasteSource == null || !lme.PasteSource.HasPendingChanges)
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            LogMessageEditor lme = e.Selection.GetActiveControl<LogMessageEditor>();

            if (lme == null || lme.PasteSource == null)
                return;

            StringBuilder sb = new StringBuilder();
            foreach (PendingChange pci in lme.PasteSource.PendingChanges)
            {
                sb.AppendFormat("* {0}", pci.RelativePath);
                sb.AppendLine();
            }

            lme.PasteText(sb.ToString());
        }
    }
}
