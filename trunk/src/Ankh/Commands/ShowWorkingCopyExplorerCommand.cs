using System;
using System.Text;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the Working Copy Explorer window.
    /// </summary>
    [VSNetCommand(AnkhCommand.ShowWorkingCopyExplorer,
		"ShowWorkingCopyExplorer",
         Text = "&Working Copy Explorer",
         Tooltip = "Show the Working Copy Explorer window.",
         Bitmap = ResourceBitmaps.WorkExplorer ),
         VSNetControl( "Tools.AnkhSVN", Position = 5 )]
    public class ShowWorkingCopyExplorerCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            e.Context.Package.ShowToolWindow(AnkhToolWindow.WorkingCopyExplorer);
        }

        #endregion
    }
}