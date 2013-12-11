using System;
using System.Collections.Generic;

using Ankh.Commands;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.Scc;
using Ankh.VS;
using Ankh.Selection;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;


namespace Ankh.UI.SvnLog.Commands
{
    /// <summary>
    /// Command to open an identified issue in Log Message view, or on a Log entry
    /// </summary>
    [SvnCommand(AnkhCommand.LogOpenIssue)]
    sealed class LogOpenIssue : ICommandHandler
    {
        IAnkhIssueService _issueService;

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            ISvnLogItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());

            if (item == null || EnumTools.IsEmpty(item.Issues))
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (_issueService == null)
                _issueService = e.Context.GetService<IAnkhIssueService>();

            if (_issueService == null)
                return;


            ISvnLogItem selectedLog = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());
            if (selectedLog == null)
                return;

            string issueid = null;
            IEnumerable<TextMarker> issues = selectedLog.Issues;
            if (!EnumTools.IsEmpty<TextMarker>(issues))
            {
                using (Ankh.UI.IssueTracker.IssueSelector dlg = new Ankh.UI.IssueTracker.IssueSelector())
                {
                    dlg.Context = e.Context;
                    dlg.LoadIssues(issues);
                    if (!dlg.IsSingleIssue(out issueid))
                    {
                        if (dlg.ShowDialog(e.Context) == System.Windows.Forms.DialogResult.OK)
                        {
                            issueid = dlg.SelectedIssue;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(issueid)) { _issueService.OpenIssue(issueid); }
        }
    }
}
