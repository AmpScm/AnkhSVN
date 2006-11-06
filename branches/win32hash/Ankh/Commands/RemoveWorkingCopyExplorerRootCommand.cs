using System;
using System.Text;
using System.Collections;

namespace Ankh.Commands
{
    [VSNetCommand( "RemoveWorkingCopyExplorerRoot",
         Text = "Remove root",
         Tooltip = "Remove this root.",
         Bitmap = ResourceBitmaps.Default ),
    VSNetControl( "WorkingCopyExplorer", Position = 1 )]
    public class RemoveWorkingCopyExplorerRootCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus( IContext context )
        {
            return context.WorkingCopyExplorer.IsRootSelected ? Enabled : EnvDTE.vsCommandStatus.vsCommandStatusInvisible;
        }

        public override void Execute( IContext context, string parameters )
        {
            context.WorkingCopyExplorer.RemoveSelectedRoot();
        }
    }
}
