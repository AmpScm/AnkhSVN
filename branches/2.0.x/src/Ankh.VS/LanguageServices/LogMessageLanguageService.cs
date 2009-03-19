// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Package;
using System.ComponentModel.Design;
using Ankh.Ids;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using Ankh.Scc;
using Ankh.VS;

namespace Ankh.VS.LanguageServices
{
    /// <summary>
    /// Implements a simple VS Languageservice to implement syntaxcoloring on our LogMessages
    /// </summary>
    [Guid(AnkhId.LogMessageLanguageServiceId), ComVisible(true), CLSCompliant(false)]
    [GlobalService(typeof(LogMessageLanguageService), PublicService = true)]
    public partial class LogMessageLanguageService : AnkhLanguageService, IAnkhServiceImplementation, IAnkhServiceProvider
    {
        public const string ServiceName = AnkhId.LogMessageServiceName;

        public LogMessageLanguageService(IAnkhServiceProvider context)
            :base(context)
        {
        }


        public override void UpdateLanguageContext(Microsoft.VisualStudio.TextManager.Interop.LanguageContextHint hint, Microsoft.VisualStudio.TextManager.Interop.IVsTextLines buffer, Microsoft.VisualStudio.TextManager.Interop.TextSpan[] ptsSelection, Microsoft.VisualStudio.Shell.Interop.IVsUserContext context)
        {
            base.UpdateLanguageContext(hint, buffer, ptsSelection, context);
        }

        public override ViewFilter CreateViewFilter(CodeWindowManager mgr, IVsTextView newView)
        {
            return new LogMessageViewFilter(this, mgr, newView);
        }

        LanguagePreferences _preferences;
        public override LanguagePreferences GetLanguagePreferences()
        {
            if (_preferences == null)
            {
                _preferences = new LanguagePreferences(this.Site, typeof(LogMessageLanguageService).GUID, ServiceName);
                _preferences.Init();
            }

            return _preferences;
        }

        CommentScanner _scanner;
        public override IScanner GetScanner(Microsoft.VisualStudio.TextManager.Interop.IVsTextLines buffer)
        {
            if (_scanner == null)
                _scanner = new CommentScanner(Context);
            return _scanner;
        }

        public override string Name
        {
            get { return ServiceName; }
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return null;
        }

        public override Source CreateSource(Microsoft.VisualStudio.TextManager.Interop.IVsTextLines buffer)
        {
            return new LogmessageSource(this, buffer, GetColorizer(buffer));
        }

        class CommentScanner : AnkhService, IScanner
        {
            int _offset;
            string _line;
            #region IScanner Members

            public CommentScanner(IAnkhServiceProvider context)
                : base(context)
            {
            }

            IProjectCommitSettings _projectCommitSettings;

            IProjectCommitSettings CommitSettings
            {
                get { return _projectCommitSettings ?? (_projectCommitSettings = GetService<IProjectCommitSettings>()); }
            }

            IEnumerable<IssueMarker> _issueIds;
            IEnumerator<IssueMarker> _mover;
            IssueMarker _nextIssue;

            /// <summary>
            /// Scans the token and provide info about it.
            /// </summary>
            /// <param name="tokenInfo">The token info.</param>
            /// <param name="state">The state.</param>
            /// <returns></returns>
            public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
            {
                if (string.IsNullOrEmpty(_line) || _offset >= _line.Length)
                    return false;

                int pState = state;
                state = 0;
                bool atStart = false;

                if (_offset == 0)
                {
                    atStart = true;
                    while (_offset < _line.Length)
                    {
                        if (char.IsWhiteSpace(_line, _offset))
                            _offset++;
                        else
                            break;
                    }
                }

                if (_offset < _line.Length)
                {
                    switch (_line[_offset])
                    {
                        case '#':
                            if(!atStart)
                                goto default;
                            if (tokenInfo != null)
                            {
                                tokenInfo.Color = TokenColor.Comment;
                                tokenInfo.StartIndex = _offset;
                                tokenInfo.EndIndex = _line.Length;
                                tokenInfo.Trigger = TokenTriggers.None;
                                tokenInfo.Type = TokenType.LineComment;
                            }
                            state = 1;
                            _offset = _line.Length;
                            return true;
                        default:
                            if (_issueIds == null)
                            {
                                _issueIds = CommitSettings.GetIssues(_line);
                                _mover = _issueIds.GetEnumerator();
                                if (_mover.MoveNext())
                                    _nextIssue = _mover.Current;
                                else
                                    _nextIssue = null;
                            }

                            while (_nextIssue != null && _mover.Current.Index < _offset)
                            {
                                if (_mover.MoveNext())
                                    _nextIssue = _mover.Current;
                                else
                                    _nextIssue = null;
                            }

                            if (_nextIssue != null)
                            {
                                if (_offset < _nextIssue.Index)
                                {
                                    // Text before the issue

                                    if (tokenInfo != null)
                                    {
                                        tokenInfo.Color = TokenColor.Text;
                                        tokenInfo.StartIndex = _offset;
                                        tokenInfo.EndIndex = _nextIssue.Index -1;
                                        tokenInfo.Trigger = TokenTriggers.None;
                                        tokenInfo.Type = TokenType.Text;
                                    }
                                    _offset = _nextIssue.Index;
                                    return true;
                                }
                                else if (_offset == _nextIssue.Index)
                                {
                                    if (tokenInfo != null)
                                    {
                                        tokenInfo.Color = TokenColor.Keyword;
                                        tokenInfo.StartIndex = _offset;
                                        tokenInfo.EndIndex = _offset + _nextIssue.Length-1;
                                        tokenInfo.Trigger = TokenTriggers.None;
                                        tokenInfo.Type = TokenType.Keyword;
                                    }

                                    state = 2;

                                    _offset += _nextIssue.Length;
                                    return true;
                                }
                            }

                            if (tokenInfo != null)
                            {
                                tokenInfo.Color = TokenColor.Text;
                                tokenInfo.StartIndex = _offset;
                                tokenInfo.EndIndex = _line.Length;
                                tokenInfo.Trigger = TokenTriggers.None;
                                tokenInfo.Type = TokenType.Text;
                            }
                            state = 0;
                            _offset = _line.Length;
                            return true;
                    }
                }
                return false;
            }

            public void SetSource(string source, int offset)
            {
                _line = source;
                _offset = offset;
                _issueIds = null;
                _nextIssue = null;
            }

            #endregion
        }
    }

    class LogmessageSource : Source
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
        }
    }

    class LogMessageViewFilter : AnkhViewFilter
    {
        readonly LogMessageLanguageService _service;
        public LogMessageViewFilter(LogMessageLanguageService service, CodeWindowManager mgr, IVsTextView view)
            : base(mgr, view)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _service = service;
        }

        public override void PrepareContextMenu(ref int menuId, ref Guid groupGuid, ref Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget target)
        {
            if (groupGuid == Microsoft.VisualStudio.Shell.VsMenus.guidSHLMainMenu && menuId == Microsoft.VisualStudio.Shell.VsMenus.IDM_VS_CTXT_CODEWIN)
            {
                groupGuid = AnkhId.CommandSetGuid;
                menuId = (int)AnkhCommandMenu.LogMessageEditorContextMenu;
            }
        }

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
    }
}
