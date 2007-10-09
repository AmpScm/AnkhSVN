// $Id$
using System;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to remove a URL from the Repository Explorer.
    /// </summary>
    [VSNetCommand("RemoveRepositoryRootCommand",
         Text = "&Remove Repository URL",
         Tooltip = "Remove a URL from the Repository Explorer.",
         Bitmap = ResourceBitmaps.RemoveURL )]
         [VSNetControl( "ReposExplorer", Position = 1 )]
    public class RemoveRepositoryRootCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            if ( context.RepositoryExplorer.IsRootNode( context.RepositoryExplorer.SelectedNode ) )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            context.RepositoryExplorer.RemoveRoot( context.RepositoryExplorer.SelectedNode );
        }

        #endregion
    }
}