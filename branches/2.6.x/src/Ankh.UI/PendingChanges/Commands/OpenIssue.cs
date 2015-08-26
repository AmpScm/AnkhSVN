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
    [SvnCommand(AnkhCommand.PcLogEditorOpenIssue)]
    [SvnCommand(AnkhCommand.PcLogEditorOpenRevision)]
    sealed class OpenIssue : ICommandHandler
    {
        IAnkhIssueService _issueService;

        static string GetLine(IVsTextLines lines, int lineNr)
        {
            if (lineNr < 0)
                throw new ArgumentOutOfRangeException("lineNr");
            else if (lines == null)
                return null;

            int lastLine, lastIndex;
            if (!VSErr.Succeeded(lines.GetLastLineIndex(out lastLine, out lastIndex)))
                return null;

            if (lineNr > lastLine)
                return null;

            LINEDATA[] data = new LINEDATA[1];

            if (!VSErr.Succeeded(lines.GetLineData(lineNr, data, null)))
                return null;

            return Marshal.PtrToStringUni(data[0].pszText, data[0].iLength);
        }

        #region ICommandHandler Members

        bool TryGetMarker(BaseCommandEventArgs e, bool issue, out TextMarker value)
        {
            value = null;
            IVsTextView tv = ((ISelectionContextEx)e.Selection).ActiveFrameTextView;

            if (tv == null)
            {
                LogMessageEditor editor = e.Selection.GetActiveControl<LogMessageEditor>();

                if (editor == null)
                    return false;

                tv = ((IAnkhHasVsTextView)editor).TextView;
            }

            int x, y;
            if (!VSErr.Succeeded(tv.GetCaretPos(out y, out x)))
                return false;

            IVsTextLines lines;
            if (!VSErr.Succeeded(tv.GetBuffer(out lines)))
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

            if (_issueService == null)
                _issueService = e.GetService<IAnkhIssueService>();

            IEnumerable<TextMarker> markers;

            int posToCheck = x + start;

            if (issue)
            {
                if (!_issueService.TryGetIssues(combined, out markers))
                    return false;
            }
            else
            {
                if (!_issueService.TryGetRevisions(combined, out markers))
                    return false;
            }

            foreach (TextMarker im in markers)
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
            TextMarker im;
            if (!TryGetMarker(e, (e.Command == AnkhCommand.PcLogEditorOpenIssue), out im))
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (_issueService == null)
                _issueService = e.Context.GetService<IAnkhIssueService>();

            if (_issueService == null)
                return;

            TextMarker marker;

            switch (e.Command)
            {
                case AnkhCommand.PcLogEditorOpenIssue:
                    if (TryGetMarker(e, true, out marker))
                    {
                        _issueService.OpenIssue(marker.Value);
                    }
                    break;
                case AnkhCommand.PcLogEditorOpenRevision:
                    if (TryGetMarker(e, false, out marker))
                    {
                        _issueService.OpenRevision(marker.Value);
                    }
                    break;
            }
        }

        #endregion
    }
}
