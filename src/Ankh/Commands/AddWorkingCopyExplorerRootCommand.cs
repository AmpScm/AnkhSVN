using System;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add a new root to the Working Copy Explorer.
    /// </summary>
    [VSNetCommand(AnkhCommand.AddWorkingCopyExplorerRoot,
		"AddWorkingCopyExplorerRoot",
         Text = "A&dd New Root...",
         Tooltip = "Add a new root to the Working Copy Explorer.",
         Bitmap = ResourceBitmaps.AddFolder ),
         VSNetControl( "WorkingCopyExplorer", Position = 1 )]
    public class AddWorkingCopyExplorerRootCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            string newRoot = context.UIShell.ShowAddWorkingCopyExplorerRootDialog();
            if ( newRoot != null )
            {
                context.WorkingCopyExplorer.AddRoot( newRoot );
            }
        }
    }
}
