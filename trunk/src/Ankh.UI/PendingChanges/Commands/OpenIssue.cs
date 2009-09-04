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
    [Command(AnkhCommand.PcLogEditorOpenIssue)]
    [Command(AnkhCommand.LogOpenIssue)]
    public class OpenIssue : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.PcLogEditorOpenIssue)
            {
                LogMessageEditor lme = e.Selection.GetActiveControl<LogMessageEditor>();

                if (lme == null)
                    e.Enabled = false;
            }
            else if (e.Command == AnkhCommand.LogOpenIssue)
            {
                ISvnLogItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());
                string issue;

                if (item == null)
                    e.Enabled = false;
                else if (null == (issue = EnumTools.GetSingle(item.Issues)))
                    e.Enabled = false;
            }
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (e.Command == AnkhCommand.LogOpenIssue)
            {
                ISvnLogItem selectedLog = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());
                if (selectedLog == null)
                    return;

                string issue = EnumTools.GetSingle(selectedLog.Issues);

                IAnkhIssueService iService = e.Context.GetService<IAnkhIssueService>();
                if (!string.IsNullOrEmpty(issue) && iService != null)
                    iService.OpenIssue(issue);
            }
            // PcLogEditorOpenIssue execution is handled via LogViewFilter in LogMessageEditor
        }

        #endregion
    }
}
