using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.UI.PendingChanges.Commits;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.CommitPendingChanges)]
    [Command(AnkhCommand.CommitPendingChangesKeepingLocks)]
    class CommitPendingChanges : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            PendingCommitsPage page = e.Context.GetService<PendingCommitsPage>();

            if (page == null || !page.Visible)
                e.Enabled = false;
            else
                e.Enabled = page.CanCommit(e.Command == AnkhCommand.CommitPendingChangesKeepingLocks);
        }

        public void OnExecute(CommandEventArgs e)
        {
            PendingCommitsPage page = e.Context.GetService<PendingCommitsPage>();

            if (page != null)
                page.DoCommit(e.Command == AnkhCommand.CommitPendingChangesKeepingLocks);
        }
    }
}
