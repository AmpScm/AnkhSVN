using Ankh.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.RepositoryExplorer.Commands
{
    [Command(AnkhCommand.RepositoryExplorerComboBox, AlwaysAvailable=true)]
    [Command(AnkhCommand.RepositoryExplorerComboBoxFill, AlwaysAvailable = true)]
    sealed class RepositoryExplorerLocation : ICommandHandler
    {
        RepositoryExplorerControl _ctrl;
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (_ctrl == null)
                _ctrl = e.GetService<RepositoryExplorerControl>();

            if (_ctrl == null)
            {
                e.Enabled = e.Visible = false;
                return;
            }
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (e.Command == AnkhCommand.RepositoryExplorerComboBoxFill)
            {
                Uri uri = _ctrl.SelectedUri;

                if (uri != null)
                    e.Result = new string[] { uri.ToString() };
            }
            else if (e.Argument != null)
            {
            }
            else
            {
                Uri uri = _ctrl.SelectedUri;

                if (uri != null)
                    e.Result = uri.ToString();
            }
        }
    }
}
