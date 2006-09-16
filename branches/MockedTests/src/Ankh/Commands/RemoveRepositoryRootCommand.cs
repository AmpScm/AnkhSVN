// $Id$
using System;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for RemoveRepositoryRootCommand.
    /// </summary>
    [VSNetCommand("RemoveRepositoryRootCommand", Tooltip = "Remove an URL from the repository explorer",
         Text="Remove Repository URL", Bitmap = ResourceBitmaps.Default )]
    [VSNetControl( "ReposExplorer", Position = 1 )]
    public class RemoveRepositoryRootCommand : CommandBase
    {
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


    }
}
