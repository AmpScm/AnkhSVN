// $Id$
using System;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the commit log dialog.
    /// </summary>
    [VSNetCommand("ShowCommitDialog",
         Text = "Show Commit &Log Dialog", 
         Tooltip = "Show the commit log dialog.",
         Bitmap = ResourceBitmaps.ShowCommit),
         VSNetControl( "Tools.AnkhSVN", Position = 1 )]
    public class ShowCommitDialogCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            context.UIShell.ToggleCommitDialog( true );
        }

        #endregion
    }
}