// $Id$
using System;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that displays the log window.
    /// </summary>
    [VSNetCommand("ShowCommitDialog", Text = "Show the commit log dialog", 
         Tooltip = "Show the commit log dialog",
         Bitmap = ResourceBitmaps.Default),
    VSNetControl( "Tools.AnkhSVN", Position = 1 )]
    public class ShowCommitDialogCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            context.UIShell.ToggleCommitDialog( true );
        }
    }
}
