using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogShowChangedPaths)]
    class ShowChangedPaths : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            LogToolControl logControl = e.Context.GetService<LogToolControl>();
            e.Latched = logControl != null ? logControl.ChangedPathsVisible : false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            LogToolControl logControl = e.Context.GetService<LogToolControl>();
            logControl.ChangedPathsVisible = !logControl.ChangedPathsVisible;
        }
    }
}
