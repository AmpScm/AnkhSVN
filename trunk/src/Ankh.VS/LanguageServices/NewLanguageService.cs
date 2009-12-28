using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Cursor = System.Windows.Forms.Cursor;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using LanguagePreferences = Microsoft.VisualStudio.Package.LanguagePreferences;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;

using Ankh.Selection;


namespace Ankh.VS.LanguageServices
{
    public abstract partial class AnkhLanguageService :
                                            AnkhService,
                                            IVsLanguageInfo,
        /* IVsProvideColorableItems, */
                                            IObjectWithAutomation
    {

        protected AnkhLanguageService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        AnkhCommandMenu _defaultContextMenu;
        public AnkhCommandMenu DefaultContextMenu
        {
            get { return _defaultContextMenu; }
            set { _defaultContextMenu = value; }
        }

        #region IVsLanguageInfo Members

        int IVsLanguageInfo.GetCodeWindowManager(IVsCodeWindow codeWindow, out IVsCodeWindowManager codeWindowManager)
        {
            codeWindowManager = CreateCodeWindowManager(codeWindow);

            if (codeWindowManager != null)
                return VSConstants.S_OK;
            else
                return VSConstants.E_NOTIMPL;
        }

        [CLSCompliant(false)]
        protected virtual AnkhCodeWindowManager CreateCodeWindowManager(IVsCodeWindow codeWindow)
        {
            return new AnkhCodeWindowManager(this, codeWindow);
        }

        int IVsLanguageInfo.GetColorizer(IVsTextLines lines, out IVsColorizer colorizer)
        {
            colorizer = CreateColorizer(lines);

            if (colorizer != null)
                return VSConstants.S_OK;
            else
                return VSConstants.E_NOTIMPL;
        }

        [CLSCompliant(false)]
        protected virtual AnkhColorizer CreateColorizer(IVsTextLines lines)
        {
            return new AnkhColorizer(this, lines);
        }

        int IVsLanguageInfo.GetFileExtensions(out string pbstrExtensions)
        {
            pbstrExtensions = FileExtensions ?? "";
            return VSConstants.S_OK;
        }

        protected virtual string FileExtensions
        {
            get { return ""; }
        }

        public int GetLanguageName(out string name)
        {
            name = Name;
            return VSConstants.S_OK;
        }

        public abstract string Name
        {
            get;
        }

        [CLSCompliant(false)]
        protected virtual LanguagePreferences CreatePreferences()
        {
            LanguagePreferences preferences;

            preferences = new LanguagePreferences(this, GetType().GUID, Name);
            preferences.Init();

            return preferences;
        }

        LanguagePreferences _preferences;
        [CLSCompliant(false)]
        public LanguagePreferences LanguagePreferences
        {
            get { return _preferences ?? (_preferences = CreatePreferences()); }
        }

        #endregion

        #region IObjectWithAutomation Members

        public object AutomationObject
        {
            get { return LanguagePreferences; }
        }

        #endregion

        [CLSCompliant(false)]
        protected internal virtual void OnNewView(AnkhCodeWindowManager codeWindowManager, IVsTextView view)
        {
            AnkhViewFilter filter = CreateFilter(codeWindowManager, view);

            if (filter != null)
            {
                IOleCommandTarget chained;
                view.AddCommandFilter(filter, out chained);

                filter.AddChained(chained);
            }
        }

        [CLSCompliant(false)]
        protected virtual AnkhViewFilter CreateFilter(AnkhCodeWindowManager codeWindowManager, IVsTextView view)
        {
            return new AnkhViewFilter(codeWindowManager, view);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool NeedsPerLineState
        {
            get { return false; }
        }
    }

    public class AnkhCodeWindowManager : AnkhService, IVsCodeWindowManager
    {
        IVsCodeWindow _window;

        [CLSCompliant(false)]
        public AnkhCodeWindowManager(AnkhLanguageService language, IVsCodeWindow window)
            : base(language)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            _window = window;
        }

        public AnkhLanguageService Language
        {
            get { return (AnkhLanguageService)Context; }
        }

        public void Close()
        {
            object window = _window;
            _window = null;
            if (window != null)
            {
                if (Marshal.IsComObject(window))
                    Marshal.ReleaseComObject(window);
            }
        }

        #region IVsCodeWindowManager Members

        public int AddAdornments()
        {
            IVsTextView textView;
            if (ErrorHandler.Succeeded(_window.GetPrimaryView(out textView)) && textView != null)
                OnNewView(textView);

            if (ErrorHandler.Succeeded(_window.GetSecondaryView(out textView)) && textView != null)
                OnNewView(textView);

            return VSConstants.S_OK;
        }

        public int RemoveAdornments()
        {
            return VSConstants.S_OK;
        }

        [CLSCompliant(false)]
        public virtual int OnNewView(IVsTextView view)
        {
            Language.OnNewView(this, view);

            return VSConstants.S_OK;
        }

        #endregion
    }

    public class AnkhColorizer : AnkhService, IVsColorizer, IVsColorizer2
    {
        IVsTextLines _lines;

        [CLSCompliant(false)]
        public AnkhColorizer(AnkhLanguageService language, IVsTextLines lines)
            : base(language)
        {
            if (lines == null)
                throw new ArgumentNullException("lines");

            _lines = lines;
        }

        protected AnkhLanguageService Language
        {
            get { return (AnkhLanguageService)Context; }
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

    public class AnkhViewFilter : AnkhService, IVsTextViewFilter, IOleCommandTarget
    {
        IOleCommandTarget _fallThrough;
        [CLSCompliant(false)]
        public AnkhViewFilter(AnkhCodeWindowManager codeWindowManager, IVsTextView textView)
            : base(codeWindowManager)
        {
        }

        protected AnkhCodeWindowManager CodeWindowManager
        {
            get { return (AnkhCodeWindowManager)Context; }
        }

        protected AnkhLanguageService Language
        {
            get { return CodeWindowManager.Language; }
        }

        #region IVsTextViewFilter Members

        [CLSCompliant(false)]
        public int GetDataTipText(TextSpan[] pSpan, out string pbstrText)
        {
            pbstrText = null;
            return VSConstants.E_NOTIMPL;
        }

        [CLSCompliant(false)]
        public int GetPairExtents(int iLine, int iIndex, TextSpan[] pSpan)
        {
            return VSConstants.E_NOTIMPL;
        }

        [CLSCompliant(false)]
        public int GetWordExtent(int iLine, int iIndex, uint dwFlags, TextSpan[] pSpan)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IOleCommandTarget Members

        int IOleCommandTarget.QueryStatus(ref Guid cmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            /* TODO: Handle things that *must* be handled on every language service; bubble everything else
             *       up the chain */
            Debug.Assert(cCmds == 1);

            int hr = QueryStatus(ref cmdGroup, ref prgCmds[0], pCmdText);

            if (hr != VSConstants.E_FAIL)
                return hr;

            if (_fallThrough != null)
                return _fallThrough.QueryStatus(ref cmdGroup, cCmds, prgCmds, pCmdText);
            else
                return VSConstants.E_FAIL; // delegate to next command target.
        }

        [CLSCompliant(false)]
        protected virtual int QueryStatus(ref Guid cmdGroup, ref OLECMD oleCmd, IntPtr pCmdText)
        {
            if (cmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)oleCmd.cmdID)
                {
                    case VSConstants.VSStd2KCmdID.OUTLN_START_AUTOHIDING:
                    case VSConstants.VSStd2KCmdID.OUTLN_STOP_HIDING_ALL:
                        return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;

                    default:
                        return VSConstants.E_FAIL; ; /* Unhandled */
                }
            }

            return VSConstants.E_FAIL;
        }

        [CLSCompliant(false)]
        public virtual int Exec(ref Guid cmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (cmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)nCmdID)
                {
                    case VSConstants.VSStd2KCmdID.SHOWCONTEXTMENU:
                        AnkhCommandMenu menu = Language.DefaultContextMenu;

                        if (menu != 0)
                            ShowContextMenu(AnkhId.CommandSetGuid, (int)menu, this);
                        else
                            ShowContextMenu(VsMenus.guidSHLMainMenu, VsMenus.IDM_VS_CTXT_CODEWIN, this);
                        return VSConstants.S_OK;
                }
            }
            if (_fallThrough != null)
                return _fallThrough.Exec(ref cmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            else
                return VSConstants.E_FAIL; // delegate to next command target.
        }

        [CLSCompliant(false)]
        public virtual void ShowContextMenu(Guid cmdGroup, int menuId, IOleCommandTarget target)
        {
            IVsUIShell uiShell = GetService<IVsUIShell>(typeof(SVsUIShell));

            if (uiShell != null)
            {
                Point pt = Cursor.Position;
                POINTS[] where = new POINTS[1];
                where[0].x = (short)pt.X;
                where[0].y = (short)pt.Y;

                uiShell.ShowContextMenu(0, ref cmdGroup, menuId, where, target);
            }
        }

        #endregion

        internal void AddChained(IOleCommandTarget chained)
        {
            _fallThrough = chained;
        }
    }
}
