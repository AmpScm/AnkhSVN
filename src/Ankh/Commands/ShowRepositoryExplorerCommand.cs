// $Id: MergeItemCommand.cs 594 2003-06-01 23:56:34Z Arild $
using System;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
	/// <summary>
    /// Command to show the Repository Explorer window.
	/// </summary>
    [Command(AnkhCommand.ShowRepositoryExplorer)]
	public class ShowRepositoryExplorerCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            context.Package.ShowToolWindow(AnkhToolWindow.RepositoryExplorer);
        }

        #endregion
    }
}