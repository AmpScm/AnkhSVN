using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;

namespace Ankh.UI.Commands
{
    [Command(AnkhCommand.ForceUIShow, AlwaysAvailable=true)]
    sealed class UIEditCommand : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            e.GetService<CommandMapper>().EnableCustomizeMode();
            e.Enabled = e.Visible = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
