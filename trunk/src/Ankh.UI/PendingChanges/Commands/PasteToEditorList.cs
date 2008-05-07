using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.PcLogEditorPasteFileList)]
    [Command(AnkhCommand.PcLogEditorPasteRecentLog)]
    class PasteToEditorList : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            PendingCommitsPage page = e.Context.GetService<PendingCommitsPage>();

            if(page == null || !page.Visible)
                e.Enabled = false;
            else
                page.OnUpdate(e);            
        }

        public void OnExecute(CommandEventArgs e)
        {
            PendingCommitsPage page = e.Context.GetService<PendingCommitsPage>();

            if(page != null)
                page.OnExecute(e);
        }
    }
}
