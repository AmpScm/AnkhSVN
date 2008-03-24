// $Id$
using System;
using AnkhSvn.Ids;

namespace Ankh.Commands
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
            IContext context = e.Context.GetService<IContext>();

            if (!context.RepositoryExplorer.IsRootNode(context.RepositoryExplorer.SelectedNode))
            {
                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            context.RepositoryExplorer.RemoveRoot( context.RepositoryExplorer.SelectedNode );
        }

        #endregion
    }
}