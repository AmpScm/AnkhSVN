using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.UI;

namespace Ankh.Scc.Commands
{
    [Command(AnkhCommand.NotifyWcToNew, AlwaysAvailable=true)]
    class UnsupportedWcVersion : ICommandHandler
    {

        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        bool _skip;
        public void OnExecute(CommandEventArgs e)
        {
            if(_skip) // Only show this message once!
                return;

            _skip = true;
            AnkhMessageBox mb = new AnkhMessageBox(e.Context);
            mb.Show(string.Format(Resources.UnsupportedWorkingCopyFound, e.Argument));
        }
    }
}
