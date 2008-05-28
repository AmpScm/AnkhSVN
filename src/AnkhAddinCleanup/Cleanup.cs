using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using Microsoft.Office.Core;

namespace AnkhAddinCleanup
{
    public class Cleanup
    {
        public void Clean(DTE dte)
        {
            foreach (CommandBar bar in ((CommandBars)dte.CommandBars))
            {
                DeleteControls(bar.Controls);
            }
            foreach (Command command in dte.Commands)
            {
                if (command.Name != null && command.Name.StartsWith("Ankh"))
                {
                    // TODO: Check command GUID
                    command.Delete();
                }
            }
        }

        private void DeleteControls(CommandBarControls controls)
        {
            foreach (CommandBarControl control in controls)
            {
                if (control.accChildCount > 0 && control.Type == MsoControlType.msoControlPopup)
                {
                    CommandBarPopup popup = control as CommandBarPopup;
                    if (popup != null)
                        DeleteControls(popup.Controls);
                }

                if (control.Caption.StartsWith("Ankh"))
                {
                    // TODO: Check something else than Caption, maybe control.Creator ?
                    // false to remove permanently
                    control.Delete(false);
                }
            }
        }
    }
}
