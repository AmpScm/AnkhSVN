using System;
using System.Text;
using System.Collections;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to remove current root from the Working Copy Explorer.
    /// </summary>
    [Command(AnkhCommand.RemoveWorkingCopyExplorerRoot)]
    public class RemoveWorkingCopyExplorerRootCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            if(!context.WorkingCopyExplorer.IsRootSelected)
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            context.WorkingCopyExplorer.RemoveSelectedRoot();
        }

        #endregion
    }
}