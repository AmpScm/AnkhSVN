using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.PcColViewPath)]
    [Command(AnkhCommand.PcColViewProject)]
    [Command(AnkhCommand.PcColViewChange)]
    [Command(AnkhCommand.PcColViewChangeList)]
    [Command(AnkhCommand.PcColViewFullPath)]
    [Command(AnkhCommand.PcColViewLocked)]
    [Command(AnkhCommand.PcColViewModified)]
    [Command(AnkhCommand.PcColViewName)]
    [Command(AnkhCommand.PcColViewRepository)]
    [Command(AnkhCommand.PcColViewType)]
    class PendingCommitViewCommands : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
        }

        #endregion
    }
}
