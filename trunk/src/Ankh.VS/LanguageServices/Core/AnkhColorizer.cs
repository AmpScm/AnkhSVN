using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Ankh.VS.LanguageServices.Core
{
    public class AnkhColorizer : AnkhService, IVsColorizer, IVsColorizer2
    {
        IVsTextLines _lines;

        [CLSCompliant(false)]
        public AnkhColorizer(AnkhLanguage language, IVsTextLines lines)
            : base(language)
        {
            if (lines == null)
                throw new ArgumentNullException("lines");

            _lines = lines;
        }

        protected AnkhLanguage Language
        {
            get { return (AnkhLanguage)Context; }
        }

        public void Close()
        {
            IVsTextLines lines = _lines;
            _lines = null;

            if (lines != null && Marshal.IsComObject(lines))
                try
                {
                    Marshal.ReleaseComObject(lines);
                }
                catch { }
        }

        #region IVsColorizer Members

        void IVsColorizer.CloseColorizer()
        {
            Close();
        }

        ///
        int IVsColorizer.ColorizeLine(int iLine, int iLength, IntPtr pszText, int iState, uint[] pAttributes)
        {
            string text = Marshal.PtrToStringUni(pszText, iLength);

            int endState;
            ColorizeLine(text, iLine, iState, pAttributes, out endState);
            return endState; // End state
        }

        [CLSCompliant(false)]
        protected virtual void ColorizeLine(string line, int lineNr, int startState, uint[] attrs, out int endState)
        {
            endState = startState;
        }

        /// <summary>
        /// Allows retrieving a specific line from the text buffer
        /// </summary>
        /// <param name="lineNr"></param>
        /// <returns></returns>
        protected string GetLine(int lineNr)
        {
            if (lineNr < 0)
                throw new ArgumentOutOfRangeException("lineNr");
            else if (_lines == null)
                return null;

            int lastLine, lastIndex;
            if (!ErrorHandler.Succeeded(_lines.GetLastLineIndex(out lastLine, out lastIndex)))
                return null;

            if (lineNr > lastLine)
                return null;

            LINEDATA[] data = new LINEDATA[1];

            if (!ErrorHandler.Succeeded(_lines.GetLineData(lineNr, data, null)))
                return null;

            return Marshal.PtrToStringUni(data[0].pszText, data[0].iLength);
        }

        int IVsColorizer.GetStartState(out int startState)
        {
            startState = StartState;
            return VSConstants.S_OK;
        }

        protected virtual int StartState
        {
            get { return 0; }
        }

        int IVsColorizer.GetStateAtEndOfLine(int iLine, int iLength, IntPtr pszText, int iState)
        {
            string text = Marshal.PtrToStringUni(pszText, iLength);

            int endState;

            GetStateAtEndOfLine(text, iLine, iState, out endState);

            return endState;
        }

        protected virtual void GetStateAtEndOfLine(string line, int lineNr, int startState, out int endState)
        {
            uint[] attrs = new uint[line.Length];
            ColorizeLine(line, lineNr, startState, attrs, out endState);
        }

        int IVsColorizer.GetStateMaintenanceFlag(out int pfFlag)
        {
            pfFlag = Language.NeedsPerLineState ? 1 : 0;
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsColorizer2 Members

        public int BeginColorization()
        {
            return VSConstants.S_OK;
        }

        public int EndColorization()
        {
            return VSConstants.S_OK;
        }

        #endregion

        public enum TokenColor
        {
            Text = 0,
            Keyword = 1,
            Comment = 2,
            Identifier = 3,
            String = 4,
            Number = 5,
        }
    }
}
