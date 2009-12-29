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
using System.Text;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Ankh.VS.LanguageServices.Core;

namespace Ankh.VS.LanguageServices.UnifiedDiff
{
    class UnifiedDiffDropDownBar : AnkhLanguageDropDownBar
    {
        public UnifiedDiffDropDownBar(UnifiedDiffLanguage language, AnkhCodeWindowManager manager)
            : base(language, manager)
        {
        }

        protected override int NumberOfCombos
        {
            get { return 1; }
        }

        readonly List<string> _indexes = new List<string>();
        readonly List<int> _lines = new List<int>();
        int _lastSelectedType = -1;
        public bool OnSynchronizeDropdowns(LanguageService languageService, Microsoft.VisualStudio.TextManager.Interop.IVsTextView textView, int line, int col, System.Collections.ArrayList dropDownTypes, System.Collections.ArrayList dropDownMembers, ref int selectedType, ref int selectedMember)
        {
            IVsTextLines lines;

            if (!ErrorHandler.Succeeded(textView.GetBuffer(out lines)))
                return false;

            int lastLine, linelen;
            ErrorHandler.ThrowOnFailure(lines.GetLastLineIndex(out lastLine, out linelen));

            bool changed = false;

            selectedType = -1;
            selectedMember = -1;

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
                dropDownTypes.Clear();
                for (int i = 0; i < _indexes.Count; i++)
                {
                    TextSpan ts;
                    ts.iStartLine = _lines[i];
                    ts.iStartIndex = 0;
                    ts.iEndLine = (i+1 < _lines.Count) ? _lines[i+1]-1 : lastLine;                    
                    ErrorHandler.ThrowOnFailure(lines.GetLengthOfLine(ts.iEndLine, out ts.iEndIndex));
                    ts.iEndIndex++;

                    dropDownTypes.Add(new DropDownMember(_indexes[i], ts, 1, DROPDOWNFONTATTR.FONTATTR_PLAIN));
                }
            }

            int j = 0;
            foreach (DropDownMember dm in dropDownTypes)
            {
                TextSpan ts = dm.Span;
                if ((line > ts.iStartLine || line == ts.iStartLine && col >= ts.iStartIndex) &&
                    (line < ts.iEndLine || line == ts.iEndLine && col < ts.iEndIndex))
                {
                    selectedType = j;
                }
                j++;
            }

            if (!changed && _lastSelectedType != selectedType)
                changed = true;

            _lastSelectedType = selectedType;
            
            return changed;
        }        
    }
}
