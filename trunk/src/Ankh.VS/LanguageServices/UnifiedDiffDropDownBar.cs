using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;

namespace Ankh.VS.LanguageServices
{
    class UnifiedDiffDropDownBar : TypeAndMemberDropdownBars
    {
        readonly IAnkhServiceProvider _context;
        public UnifiedDiffDropDownBar(UnifiedDiffLanguageService languageService)
            : base(languageService)
        {
            _context = languageService;
        }

        readonly List<string> _indexes = new List<string>();
        readonly List<int> _lines = new List<int>();
        int _lastSelectedType = -1;
        public override bool OnSynchronizeDropdowns(LanguageService languageService, Microsoft.VisualStudio.TextManager.Interop.IVsTextView textView, int line, int col, System.Collections.ArrayList dropDownTypes, System.Collections.ArrayList dropDownMembers, ref int selectedType, ref int selectedMember)
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

        public override int OnItemChosen(int combo, int entry)
        {
            return base.OnItemChosen(combo, entry);
        }

        public override int OnItemSelected(int combo, int index)
        {
            return base.OnItemSelected(combo, index);
        }

        public override int GetComboTipText(int combo, out string text)
        {
            return base.GetComboTipText(combo, out text);
        }

        public override int GetComboAttributes(int combo, out uint entries, out uint entryType, out IntPtr iList)
        {
            return base.GetComboAttributes(combo, out entries, out entryType, out iList);
        }

        public override int GetEntryText(int combo, int entry, out string text)
        {
            return base.GetEntryText(combo, entry, out text);
        }

        public override int GetEntryAttributes(int combo, int entry, out uint fontAttrs)
        {
            return base.GetEntryAttributes(combo, entry, out fontAttrs);
        }

        public override int GetEntryImage(int combo, int entry, out int imgIndex)
        {
            return base.GetEntryImage(combo, entry, out imgIndex);
        }
    }
}
