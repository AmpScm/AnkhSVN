using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI.SccManagement;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    [Command(AnkhCommand.FileSccChangeSourceControl)]
    sealed class ChangeSourceControlCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Selection.SolutionFilename))
                e.Enabled = e.Visible = false;
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
