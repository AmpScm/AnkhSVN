using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogStrictNodeHistory)]
    class LogStrictNodeHistory:ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)

        {
            LogToolControl logControl = e.Context.GetService<LogToolControl>();
            e.Latched = logControl != null && logControl.StrictNodeHistory;
        }

        public void OnExecute(CommandEventArgs e)
        {
            LogToolControl logControl = e.Context.GetService<LogToolControl>();
            logControl.StrictNodeHistory = !logControl.StrictNodeHistory;
            logControl.Restart();
        }
    }
}
