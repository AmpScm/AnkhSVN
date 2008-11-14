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
            if (!e.State.SolutionExists)
            {
                e.Enabled = false;
                return;
            }
            PendingChangesPage page = GetPage(e);

            if (page == null || !page.CanRefreshList)
                e.Enabled = false;
        }

        private PendingChangesPage GetPage(BaseCommandEventArgs e)
        {
            PendingChangesPage page = e.Context.GetService<PendingCommitsPage>();

            if (page != null && page.Visible)
                return page;

            page = e.Context.GetService<RecentChangesPage>();

            if (page != null && page.Visible)
                return page;

            return null;
        }

        public void OnExecute(CommandEventArgs e)
        {
            PendingChangesPage page = GetPage(e);

            if(page != null && page.CanRefreshList)
                page.RefreshList();
        }
    }
}
