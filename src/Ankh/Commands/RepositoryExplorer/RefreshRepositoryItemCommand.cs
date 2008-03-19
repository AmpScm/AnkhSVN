// $Id$
using System;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;

namespace Ankh.Commands
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
            IContext context = e.Context.GetService<IContext>();
            // we only want directories
            if (context.RepositoryExplorer.SelectedNode == null ||
                !context.RepositoryExplorer.SelectedNode.IsDirectory)
            {
                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            context.RepositoryExplorer.Refresh( context.RepositoryExplorer.SelectedNode );
        }

        #endregion
    }
}