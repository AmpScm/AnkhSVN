using System;
using AnkhSvn.Ids;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add a new root to the Working Copy Explorer.
    /// </summary>
    [Command(AnkhCommand.AddWorkingCopyExplorerRoot)]
    public class AddWorkingCopyExplorerRootCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IExplorersShell shell = e.Context.GetService<IExplorersShell>();

            string newRoot = shell.ShowAddWorkingCopyExplorerRootDialog();
            if ( newRoot != null )
            {
                shell.WorkingCopyExplorerService.AddRoot(newRoot);
            }
        }
    }
}
