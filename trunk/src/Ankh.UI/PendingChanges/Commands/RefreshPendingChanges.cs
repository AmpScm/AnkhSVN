using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.RefreshPendingChanges)]
    public class RefreshPendingChanges : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            PendingCommitsPage page = e.Context.GetService<PendingCommitsPage>();

            if(page == null || !page.Visible)
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            PendingCommitsPage page = e.Context.GetService<PendingCommitsPage>();

            if (page == null || (!page.Visible && (e.Command != AnkhCommand.TickRefreshPendingTasks)))
                return;

            page.RefreshList();
        }
    }
}
