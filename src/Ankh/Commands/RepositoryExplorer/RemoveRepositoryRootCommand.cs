// $Id$
using System;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to remove a URL from the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.RemoveRepositoryRoot)]
    public class RemoveRepositoryRootCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IExplorersShell shell = e.GetService<IExplorersShell>();
            IContext context = e.Context.GetService<IContext>();

            if (!shell.RepositoryExplorerService.IsRootNode(shell.RepositoryExplorerService.SelectedNode))
            {
                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IExplorersShell shell = e.GetService<IExplorersShell>();

            shell.RepositoryExplorerService.RemoveRoot(shell.RepositoryExplorerService.SelectedNode);
        }

        #endregion
    }
}