// $Id$
using System;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the commit log dialog.
    /// </summary>
    [VSNetCommand(AnkhCommand.ShowCommitDialog,
		"ShowCommitDialog",
         Text = "Show Commit &Log Dialog", 
         Tooltip = "Show the commit log dialog.",
         Bitmap = ResourceBitmaps.ShowCommit),
         VSNetControl( "Tools.AnkhSVN", Position = 2 )]
    public class ShowCommitDialogCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            context.Package.ShowToolWindow(AnkhToolWindow.PendingChanges);
        }

        #endregion
    }
}
