using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.SccManagement;
using Ankh.Ids;

namespace Ankh.Commands
{
    [Command(AnkhCommand.FileSccChangeSourceControl)]
    sealed class ChangeSourceControlCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SccProviderActive || !e.State.SolutionExists)
            {
                e.Visible = e.Enabled = false;
                return;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            using (ChangeSourceControl csc = new ChangeSourceControl())
            {
                csc.Context = e.Context;

                csc.ShowDialog(e.Context.DialogOwner);
            }
        }
    }
}
