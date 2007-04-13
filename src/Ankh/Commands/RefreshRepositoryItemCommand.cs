// $Id$
using System;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that refreshes an item in the repository explorer.
    /// </summary>
    [VSNetCommand("RefreshRepositoryItem", 
         Tooltip="Refresh this item", Text = "Refres&h",
         Bitmap = ResourceBitmaps.Refresh ),
    VSNetControl( "ReposExplorer", Position = 1 ) ]
    public class RefreshRepositoryItemCommand : CommandBase
    {
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
    }
}
