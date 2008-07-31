using System;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;
using Ankh.UI;
using Ankh.UI.WorkingCopyExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add a new root to the Working Copy Explorer.
    /// </summary>
    [Command(AnkhCommand.WorkingCopyBrowse, ArgumentDefinition="d")]
    public class AddWorkingCopyExplorerRootCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IUIShell shell = e.GetService<IUIShell>();
            string info;

            if (e.Argument is string)
            {
                // Allow opening from
                info = (string)e.Argument;
            }
            else
                info = shell.ShowAddWorkingCopyExplorerRootDialog();

            if (!string.IsNullOrEmpty(info))
            {
                WorkingCopyExplorerControl ctrl = e.Selection.ActiveDialogOrFrameControl as WorkingCopyExplorerControl;

                if (ctrl == null)
                {
                    IAnkhPackage pkg = e.GetService<IAnkhPackage>();
                    pkg.ShowToolWindow(AnkhToolWindow.WorkingCopyExplorer);
                }

                ctrl = e.Selection.ActiveDialogOrFrameControl as WorkingCopyExplorerControl;

                if (ctrl != null)
                    ctrl.BrowsePath(info);
            }
        }
    }
}
