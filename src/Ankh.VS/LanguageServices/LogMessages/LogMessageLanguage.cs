// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TextManager.Interop;
using Ankh.VS.LanguageServices.Core;

namespace Ankh.VS.LanguageServices.LogMessages
{
    /// <summary>
    /// Implements a simple VS Languageservice to implement syntaxcoloring on our LogMessages
    /// </summary>
    [Guid(AnkhId.LogMessageLanguageServiceId), ComVisible(true), CLSCompliant(false)]
    [GlobalService(typeof(LogMessageLanguage), PublicService = true)]
    public partial class LogMessageLanguage : AnkhLanguage
    {
        public const string ServiceName = AnkhId.LogMessageServiceName;

        public LogMessageLanguage(IAnkhServiceProvider context)
            : base(context)
        {
            DefaultContextMenu = AnkhCommandMenu.LogMessageEditorContextMenu;
        }

        protected override AnkhColorizer CreateColorizer(IVsTextLines textLines)
        {
            return new LogMessageColorizer(this, textLines);
        }

        public override bool NeedsPerLineState
        {
            get { return true; }
        }

        public override string Name
        {
            get { return ServiceName; }
        }
    }

    /*class LogmessageSource : AnkhSource
    {
        public LogmessageSource(LogMessageLanguageService service, IVsTextLines textLines, Colorizer colorizer)
            : base(service, textLines, colorizer)
        {
        }

        bool _initializedInfo;
        CommentInfo _commentInfo;
        public override CommentInfo GetCommentFormat()
        {
            if (!_initializedInfo)
            {
                _commentInfo.BlockStart = null;
                _commentInfo.BlockEnd = null;
                _commentInfo.LineStart = "#";
                _commentInfo.UseLineComments = true;

                _initializedInfo = true;
            }
            return _commentInfo;
        //}
    }*/

#if NOT
        /// <summary>
        /// Gets the data tip text.
        /// </summary>
        /// <param name="aspan">[in,out] The selection on input; on output the range to which the tooltip applies.</param>
        /// <param name="textValue">The text value.</param>
        /// <returns></returns>
        public override int GetDataTipText(TextSpan[] aspan, out string textValue)
        {
            if (aspan == null || aspan.Length != 1 || aspan[0].iEndLine != aspan[0].iStartLine)
                return base.GetDataTipText(aspan, out textValue);

            textValue = null;

            int lineNr = aspan[0].iStartLine;
            int iFrom = Math.Min(aspan[0].iStartIndex, aspan[0].iEndIndex);
            int iTo = Math.Max(aspan[0].iStartIndex, aspan[0].iEndIndex);

            string line = Source.GetLine(lineNr);

            if (line == null)
                return VSConstants.E_FAIL;

            while (iFrom > 0 && iFrom < line.Length)
            {
                if (!char.IsWhiteSpace(line, iFrom - 1) && "*?;".IndexOf(line[iFrom - 1]) < 0)
                    iFrom--;
                else
                    break;
            }

            while (iTo + 1 < line.Length)
            {
                if (!char.IsWhiteSpace(line, iTo) && "*?;".IndexOf(line[iTo]) < 0)
                    iTo++;
                else
                    break;
            }

            string text = iTo < line.Length ? line.Substring(iFrom, iTo - iFrom + 1) : null;

            if (string.IsNullOrEmpty(text))
                return VSConstants.E_FAIL;

            IPendingChangesManager mgr = _service.Context.GetService<IPendingChangesManager>();
            PendingChange change = null;
            if (mgr == null || !mgr.TryMatchFile(text, out change))
                return VSConstants.E_FAIL;

            aspan[0].iStartIndex = iFrom;
            aspan[0].iEndIndex = iTo;

            textValue = change.LogMessageToolTipText;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Handles open-request if the current token is an issue "identifier"
        /// </summary>
        /// <returns></returns>
        private bool HandleOpenIssue()
        {
            string issueId;
            if (GetIssueIdAtCurrentCaretPosition(false, out issueId))
            {
                IAnkhIssueService iService = _service.GetService<IAnkhIssueService>();

                if (iService != null)
                    iService.OpenIssue(issueId);
            }
            return true;
        }

        /// <summary>
        /// Gets the issue id at the current caret.
        /// </summary>
        /// <param name="select">true if the issue needs to be highlighted</param>
        /// <param name="issueId">filled with the identified issue id</param>
        /// <returns></returns>
        private bool GetIssueIdAtCurrentCaretPosition(bool select, out string issueId)
        {
            if (TextView != null && Source != null)
            {
                int cLine;
                int cCol;
                if (TextView.GetCaretPos(out cLine, out cCol) == VSConstants.S_OK)
                {
                    TokenInfo ti = Source.GetTokenInfo(cLine, cCol);
                    if (ti != null && ti.Type == TokenType.Identifier)
                    {
                        // extract issue id
                        if (TextView.GetTextStream(cLine, ti.StartIndex, cLine, ti.EndIndex + 1, out issueId) == VSConstants.S_OK
                            && select)
                        {
                            // highlight the issue
                            TextView.SetSelection(cLine, ti.StartIndex, cLine, ti.EndIndex + 1);
                        }
                        return true;
                    }
                }
            }
            issueId = null;
            return false;
        }
    }
#endif
}
