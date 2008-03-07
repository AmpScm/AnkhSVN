// $Id$
using System;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to remove a URL from the Repository Explorer.
    /// </summary>
    [VSNetCommand(AnkhCommand.RemoveRepositoryRoot,
		"RemoveRepositoryRootCommand",
         Text = "&Remove Repository URL",
         Tooltip = "Remove a URL from the Repository Explorer.",
         Bitmap = ResourceBitmaps.RemoveURL )]
         [VSNetControl( "ReposExplorer", Position = 1 )]
    public class RemoveRepositoryRootCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.Context.RepositoryExplorer.IsRootNode(e.Context.RepositoryExplorer.SelectedNode))
            {
                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            context.RepositoryExplorer.RemoveRoot( context.RepositoryExplorer.SelectedNode );
        }

        #endregion
    }
}