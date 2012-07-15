using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;

namespace Ankh.UI.WorkingCopyExplorer.Commands
{
    [Command(AnkhCommand.WCExplorerComboBox, AlwaysAvailable=true)]
    [Command(AnkhCommand.WCExplorerComboBoxFill, AlwaysAvailable = true)]
    sealed class WCExplorerLocation : ICommandHandler
    {
        WorkingCopyExplorerControl _ctrl;
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (_ctrl == null)
                _ctrl = e.GetService<WorkingCopyExplorerControl>();

            if (_ctrl == null)
            {
                e.Enabled = e.Visible = false;
                return;
            }
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (e.Command == AnkhCommand.WCExplorerComboBoxFill)
            {
                string path = _ctrl.SelectedDirectory;

                if (path != null)
                    e.Result = new string[] { path };
            }
            else if (e.Argument != null)
            {
            }
            else
            {
                e.Result = _ctrl.SelectedDirectory;
            }
        }
    }
}
