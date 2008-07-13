using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;

namespace Ankh.Scc.Commands
{
    [Command(AnkhCommand.ActivateSccProvider, AlwaysAvailable=true)]
    sealed class ActivateSccProvider : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
        }
    }
}
