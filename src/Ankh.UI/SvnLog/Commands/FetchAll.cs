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
            LogToolControl logControl = e.Context.GetService<LogToolControl>();
        }
        public void OnExecute(CommandEventArgs e)
        {
            LogToolControl logControl = e.Context.GetService<LogToolControl>();
            logControl.FetchAll();
        }
    }
}
