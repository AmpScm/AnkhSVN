using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.PcGroupPath)]
    [Command(AnkhCommand.PcGroupProject)]
    [Command(AnkhCommand.PcGroupChange)]
    [Command(AnkhCommand.PcGroupChangeList)]
    [Command(AnkhCommand.PcGroupFullPath)]
    [Command(AnkhCommand.PcGroupLocked)]
    [Command(AnkhCommand.PcGroupModified)]
    [Command(AnkhCommand.PcGroupName)]
    [Command(AnkhCommand.PcGroupRepository)]
    [Command(AnkhCommand.PcGroupType)]
    [Command(AnkhCommand.PcGroupAscending)]
    [Command(AnkhCommand.PcGroupDescending)]
    class PendingCommitGroupCommands : ICommandHandler
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
