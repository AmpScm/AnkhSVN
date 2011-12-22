using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ankh.Commands;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.PendingChangesSpacer, AlwaysAvailable=true, HideWhenDisabled=false)]
    sealed class SpacerHider : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = false;
            if (PendingChangesToolControl.ShownVertically)
                e.Visible = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
