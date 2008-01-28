using System;
using System.Text;

namespace Ankh.Commands
{
    [VSNetCommand( "AddWorkingCopyExplorerRoot",
         Text = "Add new root...",
         Tooltip = "Add new working copy explorer root.",
         Bitmap = ResourceBitmaps.AddFolder ),
    VSNetControl( "WorkingCopyExplorer", Position = 1 )]
    class AddWorkingCopyExplorerRootCommand : CommandBase
    {
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
    }
}
