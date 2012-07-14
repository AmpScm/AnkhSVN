using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.SvnLogComboBox, AlwaysAvailable = true)]
    [Command(AnkhCommand.SvnLogComboBoxFill, AlwaysAvailable = true)]
    sealed class LogComboBox : ICommandHandler
    {
        LogToolWindowControl _ctrl;

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (_ctrl == null)
                _ctrl = e.GetService<LogToolWindowControl>();

            //if (_ctrl == null)
            {
                e.Enabled = e.Visible = false;
            }
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (e.Command == AnkhCommand.SvnLogComboBoxFill)
            {
                e.Result = new string[] { "1 item" };
                return;
            }
            
        }
    }
}
