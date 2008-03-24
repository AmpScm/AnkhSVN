using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using AnkhSvn.Ids;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.PendingChangesViewLogMessage)]
    public class ViewLogMessage : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            PendingCommitsPage page = e.Context.GetService<PendingCommitsPage>();

            if(page == null || !page.Visible)
            {
                e.Enabled = false;
                return;
            }

            e.Latched = page.LogMessageVisible;
        }

        public void OnExecute(CommandEventArgs e)
        {
            PendingCommitsPage page = e.Context.GetService<PendingCommitsPage>();

            if (page != null)
                page.LogMessageVisible = !page.LogMessageVisible;
        }
    }
}
