// $Id: MergeItemCommand.cs 594 2003-06-01 23:56:34Z Arild $
using System;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
	/// Displays the Repository Explorer tool window.
	/// </summary>
    [VSNetCommand("ShowRepositoryExplorer", Text = "Repository Explorer", Tooltip = "Show the repository explorer window",
         Bitmap = ResourceBitmaps.ReposExplorer),
    VSNetControl( "MenuBar.View", Position = 1 ),
    VSNetControl( "MenuBar.Tools.AnkhSVN", Position = 1 )]
	internal class ShowRepositoryExplorerCommand : CommandBase
	{
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            return Enabled;
        }
		
        public override void Execute(Ankh.AnkhContext context, string parameters)
        {
            context.RepositoryExplorerWindow.Visible = true;
        }        
   
	}
}
