using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogFetchAll)]
    class FetchAll : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }
        public void OnExecute(CommandEventArgs e)
        {
            LogToolWindowControl logControl = e.Context.GetService<LogToolWindowControl>();
            logControl.FetchAll();
        }
    }
}
