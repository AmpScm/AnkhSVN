// $Id$
using System;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to refresh the current item in the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.RefreshRepositoryItem)]
    public class RefreshRepositoryItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IExplorersShell shell = e.GetService<IExplorersShell>();
            
            // we only want directories
            if (shell.RepositoryExplorerService.SelectedNode == null ||
                !shell.RepositoryExplorerService.SelectedNode.IsDirectory)
            {
                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IExplorersShell shell = e.GetService<IExplorersShell>();

            shell.RepositoryExplorerService.Refresh( shell.RepositoryExplorerService.SelectedNode );
        }

        #endregion
    }
}