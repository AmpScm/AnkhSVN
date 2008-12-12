using System;
using System.Text;
using System.Collections;
using Ankh.Ids;
using Ankh.UI.WorkingCopyExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to remove current root from the Working Copy Explorer.
    /// </summary>
    [Command(AnkhCommand.RemoveWorkingCopyExplorerRoot)]
    class RemoveWorkingCopyExplorerRootCommand : CommandBase
    {
        

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            WorkingCopyExplorerControl ctrl = e.Selection.ActiveDialogOrFrameControl as WorkingCopyExplorerControl;

            if (ctrl == null)
                e.Enabled = false;
            else
                e.Enabled = ctrl.IsWcRootSelected();
        }

        public override void OnExecute(CommandEventArgs e)
        {
            WorkingCopyExplorerControl ctrl = e.Selection.ActiveDialogOrFrameControl as WorkingCopyExplorerControl;

            if (ctrl != null)
                ctrl.RemoveRoot();            
        }
    }
}
