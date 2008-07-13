// $Id$
using System;
using Ankh.Ids;
using Ankh.UI;

namespace Ankh.Commands
{
    /// <summary>
    /// Command implementation of the show toolwindow commands
    /// </summary>
    [Command(AnkhCommand.ShowPendingChanges)]
    [Command(AnkhCommand.ShowWorkingCopyExplorer)]
    [Command(AnkhCommand.ShowRepositoryExplorer, AlwaysAvailable=true)]
    public class ShowToolWindows : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            switch (e.Command)
            {
                case AnkhCommand.ShowPendingChanges:
                    if (!e.State.SccProviderActive)
                        e.Visible = e.Enabled = false;
                    break;
            }
        }
        public override void OnExecute(CommandEventArgs e)
        {
            IAnkhPackage package = e.Context.GetService<IAnkhPackage>();

            AnkhToolWindow toolWindow;
            switch (e.Command)
            {
                case AnkhCommand.ShowPendingChanges:
                    toolWindow = AnkhToolWindow.PendingChanges;
                    break;
                case AnkhCommand.ShowWorkingCopyExplorer:
                    toolWindow = AnkhToolWindow.WorkingCopyExplorer;
                    break;
                case AnkhCommand.ShowRepositoryExplorer:
                    toolWindow = AnkhToolWindow.RepositoryExplorer;
                    break;
                default:
                    return;
            }

            package.ShowToolWindow(toolWindow);
        }
    }
}
