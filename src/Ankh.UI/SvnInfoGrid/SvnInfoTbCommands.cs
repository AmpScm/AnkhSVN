using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.UI.SvnInfoGrid
{
    [Command(AnkhCommand.SvnInfoCategorized, AlwaysAvailable = true)]
    [Command(AnkhCommand.SvnInfoAlphabetical, AlwaysAvailable = true)]
    class SvnInfoTbCommands : ICommandHandler
    {
        SvnInfoGridControl _control;

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (_control == null)
            {
                _control = e.GetService<SvnInfoGridControl>();

                if (_control == null)
                {
                    e.Enabled = false;
                    return;
                }
            }

            PropertySort sort = _control.Grid.PropertySort;

            bool categorized = (PropertySort.NoSort != (sort & PropertySort.Categorized));

            if (e.Command == AnkhCommand.SvnInfoAlphabetical)
                e.Checked = !categorized;
            else
                e.Checked = categorized;
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (_control == null)
                return; // Can never happen as update already checked this

            if (e.Command == AnkhCommand.SvnInfoAlphabetical)
                _control.Grid.PropertySort = PropertySort.Alphabetical;
            else
                _control.Grid.PropertySort = PropertySort.CategorizedAlphabetical;

            // And tell VS that the state of other commands changed to
            // avoid an ugly delay in updating the other toolbar button
            IVsUIShell shell = e.GetService<IVsUIShell>();
            if (shell != null)
                shell.UpdateCommandUI(1); // Force an immediate update
        }
    }
}
