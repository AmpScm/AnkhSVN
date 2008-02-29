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

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // we only want directories
            if ( context.RepositoryExplorer.SelectedNode != null &&
                context.RepositoryExplorer.SelectedNode.IsDirectory )
            {
                return Enabled;
            }
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            context.RepositoryExplorer.Refresh( context.RepositoryExplorer.SelectedNode );
        }

        #endregion
    }
}