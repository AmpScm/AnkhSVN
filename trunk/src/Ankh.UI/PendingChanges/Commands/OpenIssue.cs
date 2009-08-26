using Ankh.Commands;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.PcLogEditorOpenIssue, HideWhenDisabled = false)]
    public class OpenIssue : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            LogMessageEditor lme = e.Selection.GetActiveControl<LogMessageEditor>();
            IAnkhIssueService iService = e.GetService<IAnkhIssueService>();
            if (lme == null
                || iService == null
                || !(iService.CurrentIssueRepositorySettings is IssueRepository))
                e.Enabled = e.Visible = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            // execution is handled via LogViewFilter in LogMessageEditor
        }

        #endregion
    }
}
