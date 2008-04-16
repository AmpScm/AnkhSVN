using System;
using System.Text;
using System.Collections;
using AnkhSvn.Ids;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to remove current root from the Working Copy Explorer.
    /// </summary>
    [Command(AnkhCommand.RemoveWorkingCopyExplorerRoot)]
    public class RemoveWorkingCopyExplorerRootCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IExplorersShell shell = e.Context.GetService<IExplorersShell>();

            if(!shell.WorkingCopyExplorerService.IsRootSelected)
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IExplorersShell shell = e.Context.GetService<IExplorersShell>();

            shell.WorkingCopyExplorerService.RemoveSelectedRoot();
        }

        #endregion
    }
}