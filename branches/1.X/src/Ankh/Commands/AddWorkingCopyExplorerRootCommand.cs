using System;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add a new root to the Working Copy Explorer.
    /// </summary>
    [VSNetCommand( "AddWorkingCopyExplorerRoot",
         Text = "A&dd New Root...",
         Tooltip = "Add a new root to the Working Copy Explorer.",
         Bitmap = ResourceBitmaps.AddFolder ),
         VSNetControl( "WorkingCopyExplorer", Position = 1 )]
    public class AddWorkingCopyExplorerRootCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus( IContext context )
        {
            return Enabled;
        }

        public override void Execute( IContext context, string parameters )
        {
            string newRoot = context.UIShell.ShowAddWorkingCopyExplorerRootDialog();
            if ( newRoot != null )
            {
                context.WorkingCopyExplorer.AddRoot( newRoot );
            }
        }

        #endregion
    }
}
