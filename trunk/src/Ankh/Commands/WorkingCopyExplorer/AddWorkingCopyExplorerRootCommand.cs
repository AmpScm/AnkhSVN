using System;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add a new root to the Working Copy Explorer.
    /// </summary>
    [Command(AnkhCommand.AddWorkingCopyExplorerRoot)]
    public class AddWorkingCopyExplorerRootCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            string newRoot = context.UIShell.ShowAddWorkingCopyExplorerRootDialog();
            if ( newRoot != null )
            {
                context.WorkingCopyExplorer.AddRoot( newRoot );
            }
        }
    }
}
