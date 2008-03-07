// $Id$
using System;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to refresh the current item in the Repository Explorer.
    /// </summary>
    [VSNetCommand(AnkhCommand.RefreshRepositoryItem,
		"RefreshRepositoryItem",
         Text = "Refres&h",
         Tooltip = "Refresh this item.",
         Bitmap = ResourceBitmaps.Refresh ),
         VSNetControl( "ReposExplorer", Position = 1 )]
    public class RefreshRepositoryItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            // we only want directories
            if (e.Context.RepositoryExplorer.SelectedNode == null ||
                !e.Context.RepositoryExplorer.SelectedNode.IsDirectory)
            {
                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            context.RepositoryExplorer.Refresh( context.RepositoryExplorer.SelectedNode );
        }

        #endregion
    }
}