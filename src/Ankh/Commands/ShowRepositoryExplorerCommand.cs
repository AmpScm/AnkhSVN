// $Id: MergeItemCommand.cs 594 2003-06-01 23:56:34Z Arild $
using System;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
    /// Command to show the Repository Explorer window.
	/// </summary>
    [VSNetCommand("ShowRepositoryExplorer",
         Text = "&Repository Explorer",
         Tooltip = "Show the Repository Explorer window.",
         Bitmap = ResourceBitmaps.ReposExplorer),
    VSNetControl( "Tools.AnkhSVN", Position = 4 )]
	public class ShowRepositoryExplorerCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.IContext context)
        {
            return Enabled;
        }

        public override void Execute(Ankh.IContext context, string parameters)
        {
            context.UIShell.ShowRepositoryExplorer( true );
        }

        #endregion
    }
}