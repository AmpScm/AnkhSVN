using System;
using System.Text;

namespace Ankh.Commands
{


    /// <summary>
    /// Displays the Working Copy Explorer tool window.
    /// </summary>
    [VSNetCommand( "ShowWorkingCopyExplorer", Text = "&Working Copy Explorer", Tooltip = "Show the WC Explorer window",
         Bitmap = ResourceBitmaps.WorkExplorer ),
    VSNetControl( "Tools.AnkhSVN", Position = 1 )]
    public class ShowWorkingCopyExplorerCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus( Ankh.IContext context )
        {
            return Enabled;
        }

        public override void Execute( Ankh.IContext context, string parameters )
        {
            context.UIShell.ShowWorkingCopyExplorer( true );
        }

    }
}
