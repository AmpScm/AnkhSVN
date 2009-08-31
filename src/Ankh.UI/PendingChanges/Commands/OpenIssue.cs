using Ankh.Commands;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.Scc;
using System.Collections.Generic;
using Ankh.VS;

namespace Ankh.UI.PendingChanges.Commands
{
    /// <summary>
    /// Command to open an identified issue in Log Message view, or on a Log entry
    /// </summary>
    [Command(AnkhCommand.PcLogEditorOpenIssue, HideWhenDisabled = false)]
    [Command(AnkhCommand.LogOpenIssue, HideWhenDisabled = false)]
    public class OpenIssue : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            IAnkhIssueService iService = e.GetService<IAnkhIssueService>();
            if (iService == null
                || !(iService.CurrentIssueRepositorySettings is IssueRepository)
                )
            {
                e.Enabled = e.Visible = false;
            }
            else if (e.Command == AnkhCommand.PcLogEditorOpenIssue)
            {
                LogMessageEditor lme = e.Selection.GetActiveControl<LogMessageEditor>();
                e.Enabled = e.Visible = lme != null;
            }
            else if (e.Command == AnkhCommand.LogOpenIssue)
            {
                ISvnLogItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());
                IEnumerable<IssueMarker> issues;
                if (item == null
                    || string.IsNullOrEmpty(item.LogMessage)
                    || !iService.TryGetIssues(item.LogMessage, out issues)
                    || EnumTools.IsEmpty<IssueMarker>(issues)
                )
                {
                    e.Enabled = false;
                }
            }
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (e.Command == AnkhCommand.LogOpenIssue)
            {
                ISvnLogItem selectedLog = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());
                if (selectedLog == null)
                    return;
                IAnkhIssueService iService = e.Context.GetService<IAnkhIssueService>();
                if (iService != null)
                {
                    IEnumerable<IssueMarker> issues;
                    if (iService.TryGetIssues(selectedLog.LogMessage, out issues))
                    {
                        // TODO show a dialog to let users select an issue in case of multiple issues
                        IssueMarker im = EnumTools.GetFirst<IssueMarker>(issues);
                        iService.OpenIssue(im.Value);
                    }
                }
            }
            // PcLogEditorOpenIssue execution is handled via LogViewFilter in LogMessageEditor
        }

        #endregion
    }
}
