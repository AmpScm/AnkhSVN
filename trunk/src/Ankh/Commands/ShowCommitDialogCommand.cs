// $Id$
using System;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to show the commit log dialog.
    /// </summary>
    [Command(AnkhCommand.ShowPendingChanges)]
    public class ShowCommitDialogCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            context.Package.ShowToolWindow(AnkhToolWindow.PendingChanges);
        }
    }
}
