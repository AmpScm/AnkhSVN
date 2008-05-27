using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogIncludeMergedRevisions)]
    class IncludeMergedRevisions : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            LogToolControl logControl = e.Context.GetService<LogToolControl>();
            e.Latched = logControl != null && logControl.IncludeMerged;
        }

        public void OnExecute(CommandEventArgs e)
        {
            LogToolControl logControl = e.Context.GetService<LogToolControl>();
            logControl.IncludeMerged = !logControl.IncludeMerged;
            logControl.Restart();
        }
    }
}
