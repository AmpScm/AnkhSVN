// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using Ankh.VS.LanguageServices.Core;

namespace Ankh.VS.LanguageServices.UnifiedDiff
{
    class UnifiedDiffDropDownBar : AnkhLanguageDropDownBar, IVsTextLinesEvents
    {
        IVsTextLines _buffer;
        uint _linesCookie;

        public UnifiedDiffDropDownBar(UnifiedDiffLanguage language, AnkhCodeWindowManager manager)
            : base(language, manager)
        {
            IVsTextView view = EnumTools.GetFirst(manager.GetViews());
            
            if (ErrorHandler.Succeeded(view.GetBuffer(out _buffer)))
            {
                if (!TryHookConnectionPoint<IVsTextLinesEvents>(_buffer, this, out _linesCookie))
                    _linesCookie = 0;
            }
        }

        protected override void OnInitialize()
        {
            _shouldParse = true;
            base.OnInitialize();
        }

        protected override void OnClose()
        {
            try
            {
                if (_buffer != null)
                {
                    if (_linesCookie != 0)
                        ReleaseHook<IVsTextLinesEvents>(_buffer, _linesCookie);
                }
            }
            finally
            {
                _buffer = null;
                _linesCookie = 0;
                base.OnClose();
            }
        }

        protected override int NumberOfCombos
        {
            get { return 1; }
        }

        bool _shouldParse;
        protected override void OnIdle(AnkhIdleArgs e)
        {
            base.OnIdle(e);

            if (_shouldParse)
            {
                _shouldParse = false;

                if (Parse())
                {
                    ScheduleSynchronize();
                }
            }
        }

        readonly List<string> _indexes = new List<string>();
        readonly List<int> _lines = new List<int>();
        private bool Parse()
        {
            IVsTextLines lines = _buffer;

            int lastLine, linelen;
            ErrorHandler.ThrowOnFailure(lines.GetLastLineIndex(out lastLine, out linelen));

            bool changed = false;

            int n = 0;
            for (int i = 0; i < lastLine; i++)
            {
                ErrorHandler.ThrowOnFailure(lines.GetLengthOfLine(i, out linelen));
                if (linelen < 8)
                    continue; // No 'Index: ' line

                string start;
                ErrorHandler.ThrowOnFailure(lines.GetLineText(i, 0, i, 7, out start));
                if (!string.Equals(start, "Index: "))
                    continue;

                ErrorHandler.ThrowOnFailure(lines.GetLineText(i, 7, i, linelen, out start));

                start = start.Trim();

                if (n >= _indexes.Count || _indexes[n] != start || _lines[n] != i)
                {
                    changed = true;
                    if (n <= _indexes.Count)
                    {
                        _lines.RemoveRange(n, _indexes.Count - n);
                        _indexes.RemoveRange(n, _indexes.Count - n);
                    }

                    _indexes.Add(start);
                    _lines.Add(i);
                }                
                n++;
            }

            if (changed)
            {
                DropDownTypes.Clear();
                for (int i = 0; i < _indexes.Count; i++)
                {
                    TextSpan ts;
                    ts.iStartLine = _lines[i];
                    ts.iStartIndex = 0;
                    ts.iEndLine = (i+1 < _lines.Count) ? _lines[i+1]-1 : lastLine;                    
                    ErrorHandler.ThrowOnFailure(lines.GetLengthOfLine(ts.iEndLine, out ts.iEndIndex));
                    ts.iEndIndex++;

                    DropDownTypes.Add(new ComboMember(_indexes[i], 1, DROPDOWNFONTATTR.FONTATTR_PLAIN, ts));
                }
            }

            return changed;
        }

        #region IVsTextBufferDataEvents Members

        public void OnFileChanged(uint grfChange, uint dwFileAttrs)
        {
            _shouldParse = true;
        }

        public int OnLoadCompleted(int fReload)
        {
            _shouldParse = true;
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsTextLinesEvents Members

        public void OnChangeLineAttributes(int iFirstLine, int iLastLine)
        {
            _shouldParse = true;
        }

        public void OnChangeLineText(TextLineChange[] pTextLineChange, int fLast)
        {
            _shouldParse = true;
        }

        #endregion
    }
}
