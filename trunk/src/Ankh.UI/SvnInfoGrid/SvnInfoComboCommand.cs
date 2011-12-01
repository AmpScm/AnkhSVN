using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;

namespace Ankh.UI.SvnInfoGrid
{
    [Command(AnkhCommand.SvnInfoComboBox, AlwaysAvailable=true)]
    [Command(AnkhCommand.SvnInfoComboBoxFill, AlwaysAvailable = true)]
    class SvnInfoComboCommand : ICommandHandler
    {
        public virtual void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public virtual void OnExecute(CommandEventArgs e)
        {
            if (e.Command == AnkhCommand.SvnInfoComboBoxFill)
            {
                e.Result = new string[] { "First", "Other" };
                return;
            }

            if (e.Argument != null)
            {
                string value = e.Argument as string;

                if (value != null)
                {
                    // Selection changed
                }
                else
                {
                    // Keyboard filter
                }
            }
            else
                e.Result = "First";
        }
    }
}
