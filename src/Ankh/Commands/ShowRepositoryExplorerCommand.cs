// $Id: MergeItemCommand.cs 594 2003-06-01 23:56:34Z Arild $
using System;
using EnvDTE;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
	/// <summary>
    /// Command to show the Repository Explorer window.
	/// </summary>
    [VSNetCommand(AnkhCommand.ShowRepositoryExplorer,
		"ShowRepositoryExplorer",
         Text = "&Repository Explorer",
         Tooltip = "Show the Repository Explorer window.",
         Bitmap = ResourceBitmaps.ReposExplorer),
    VSNetControl( "Tools.AnkhSVN", Position = 4 )]
	public class ShowRepositoryExplorerCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            e.Context.Package.ShowToolWindow(AnkhToolWindow.RepositoryExplorer);
        }

        #endregion
    }
}