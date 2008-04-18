using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.PcSortPath)]
    [Command(AnkhCommand.PcSortProject)]
    [Command(AnkhCommand.PcSortChange)]
    [Command(AnkhCommand.PcSortChangeList)]
    [Command(AnkhCommand.PcSortFullPath)]
    [Command(AnkhCommand.PcSortLocked)]
    [Command(AnkhCommand.PcSortModified)]
    [Command(AnkhCommand.PcSortName)]
    [Command(AnkhCommand.PcSortRepository)]
    [Command(AnkhCommand.PcSortType)]
    [Command(AnkhCommand.PcSortAscending)]
    [Command(AnkhCommand.PcSortDescending)]
    public class PendingCommitSortCommands : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            PendingCommitsPage pcc = e.GetService<PendingCommitsPage>();

            if (pcc == null)
                e.Enabled = false;
            else
                pcc.UpdateSort(e);
        }

        public void OnExecute(CommandEventArgs e)
        {
            PendingCommitsPage pcc = e.GetService<PendingCommitsPage>();

            if (pcc != null)
                pcc.SetSort(e.Command);
        }

        #endregion
    }
}
