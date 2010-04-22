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


namespace Ankh.UI.PendingChanges.Commands
{
    /// <summary>
    /// Command to open an identified issue in Log Message view, or on a Log entry
    /// </summary>
    [Command(AnkhCommand.PcLogEditorOpenIssue)]
    [Command(AnkhCommand.LogOpenIssue)]
    public class OpenIssue : ICommandHandler
    {
        IAnkhIssueService _issueService;

        static string GetLine(IVsTextLines lines, int lineNr)
        {
            if (lineNr < 0)
                throw new ArgumentOutOfRangeException("lineNr");
            else if (lines == null)
                return null;

            int lastLine, lastIndex;
            if (!ErrorHandler.Succeeded(lines.GetLastLineIndex(out lastLine, out lastIndex)))
                return null;

            if (lineNr > lastLine)
                return null;

            LINEDATA[] data = new LINEDATA[1];

            if (!ErrorHandler.Succeeded(lines.GetLineData(lineNr, data, null)))
                return null;

            return Marshal.PtrToStringUni(data[0].pszText, data[0].iLength);
        }

        #region ICommandHandler Members

        bool TryGetIssue(BaseCommandEventArgs e, out IssueMarker value)
        {
            value = null;
            IVsTextView tv = ((ISelectionContextEx)e.Selection).ActiveFrameTextView;

            if (tv == null)
            {
                LogMessageEditor editor = e.Selection.GetActiveControl<LogMessageEditor>();

                if (editor == null)
                    return false;

                tv = editor.TextView;
            }

            int x, y;
            if (!ErrorHandler.Succeeded(tv.GetCaretPos(out y, out x)))
                return false;

            IVsTextLines lines;
            if (!ErrorHandler.Succeeded(tv.GetBuffer(out lines)))
                return false;

            string text, pre = null, post = null;

            text = GetLine(lines, y);

            if (string.IsNullOrEmpty(text))
                return false;

            string combined = null;
            int start = 0;
            if (y > 0)
            {
                pre = GetLine(lines, y - 1);
                combined = pre + '\n';
                start = combined.Length;
            }

            combined += text;

            post = GetLine(lines, y + 1);

            if (!string.IsNullOrEmpty(post))
                combined += '\n' + post;
            if ((y > 0) && !ErrorHandler.Succeeded(tv.GetTextStream(y - 1, 0, y, 0, out pre)))
                return false;

            if (!ErrorHandler.Succeeded(tv.GetTextStream(y + 1, 0, y + 2, 0, out post)))
                post = null;

            
            if (!string.IsNullOrEmpty(pre))
            {
                combined = pre.TrimEnd('\r', '\n') + '\n';
                start = combined.Length;
            }

            combined += text;

            if (!string.IsNullOrEmpty(post))
            {
                post = post.TrimEnd('\r', '\n');
                combined += '\n' + post;
            }

            if (_issueService == null)
                _issueService = e.GetService<IAnkhIssueService>();

            IEnumerable<IssueMarker> markers;

            int posToCheck = x + start;

            if (!_issueService.TryGetIssues(combined, out markers))
                return false;

            foreach (IssueMarker im in markers)
            {
                if (im.Index > posToCheck)
                    break;

                if (im.Index + im.Length >= posToCheck)
                {
                    value = im;
                    return true;
                }
            }

            return false;
        }

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            switch (e.Command)
            {
                case AnkhCommand.PcLogEditorOpenIssue:
                    {
                        IssueMarker im;
                        if (!TryGetIssue(e, out im))
                            e.Enabled = false;
                    }
                    break;
                case AnkhCommand.LogOpenIssue:
                    {
                        ISvnLogItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());
                        if (item == null)
                            e.Enabled = false;
                        else if (EnumTools.IsEmpty(item.Issues))
                            e.Enabled = false;
                    }
                    break;
            }
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (_issueService == null)
                _issueService = e.Context.GetService<IAnkhIssueService>();

            if (_issueService == null)
                return;

            switch (e.Command)
            {
                case AnkhCommand.LogOpenIssue:
                    {
                        ISvnLogItem selectedLog = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());
                        if (selectedLog == null)
                            return;

                        string issueid = null;
                        IEnumerable<IssueMarker> issues = selectedLog.Issues;
                        if (!EnumTools.IsEmpty<IssueMarker>(issues))
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

                    break;
                case AnkhCommand.PcLogEditorOpenIssue:
                    IssueMarker marker;

                    if (TryGetIssue(e, out marker))
                    {
                        _issueService.OpenIssue(marker.Value);
                    }
                    break;
            }
        }

        #endregion
    }
}
