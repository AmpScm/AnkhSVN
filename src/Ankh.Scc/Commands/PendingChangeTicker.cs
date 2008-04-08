using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using AnkhSvn.Ids;

namespace Ankh.Scc.Commands
{
    [Command(AnkhCommand.TickRefreshPendingTasks)]
    sealed class PendingChangeTicker : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            // NOOP
        }

        public void OnExecute(CommandEventArgs e)
        {
            PendingChangeManager pm = e.GetService<PendingChangeManager>(typeof(IPendingChangesManager));

            if (pm != null)
                pm.OnTickRefresh();
        }

    }
}
