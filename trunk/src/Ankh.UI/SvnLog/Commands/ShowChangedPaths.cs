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
            LogToolWindowControl logControl = e.Context.GetService<LogToolWindowControl>();
            e.Latched = logControl != null ? logControl.ChangedPathsVisible : false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            LogToolWindowControl logControl = e.Context.GetService<LogToolWindowControl>();
            logControl.ChangedPathsVisible = !logControl.ChangedPathsVisible;
        }
    }
}
