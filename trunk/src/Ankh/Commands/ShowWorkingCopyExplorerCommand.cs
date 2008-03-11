using System;
using System.Text;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the Working Copy Explorer window.
    /// </summary>
    [Command(AnkhCommand.ShowWorkingCopyExplorer)]
    public class ShowWorkingCopyExplorerCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            context.Package.ShowToolWindow(AnkhToolWindow.WorkingCopyExplorer);
        }

        #endregion
    }
}