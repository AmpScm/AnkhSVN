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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

using Ankh.Scc.UI;
using Ankh.UI;

using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;

namespace Ankh.VS.TextEditor
{
    /// <summary>
    /// This class is used to implement CodeEditorUserControl
    /// </summary>
    /// <seealso cref="UserControl"/>
    partial class TheVSTextEditor : Control, IAnkhHasVsTextView, IVSTextEditorImplementation, IAnkhLegacyKeyMessageSuppressFilter
    {
        CodeEditorWindow _nativeWindow;
        IAnkhServiceProvider _context;
        Guid? _forceLanguageService;
        IGetWpfEditorInfo _wpfEditorInfo;
        IWpfEditorInfo _wpfInfo;

        public TheVSTextEditor()
        {
            BackColor = SystemColors.Window;
        }

        public TheVSTextEditor(IContainer container)
            : this()
        {
            container.Add(this);
        }

        [Localizable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [DefaultValue(null), Localizable(false)]
        public virtual Guid? ForceLanguageService
        {
            get { return _forceLanguageService; }
            set
            {
                _forceLanguageService = value;

                if (_nativeWindow != null && value.HasValue)
                    _nativeWindow.SetLanguageService(value.Value);
            }
        }

        private bool _disableWordwrap;
        [Localizable(false), DefaultValue(false)]
        public bool DisableWordWrap
        {
            get { return _disableWordwrap; }
            set
            {
                _disableWordwrap = value;
                if (_nativeWindow != null)
                {
                    _nativeWindow.SetWordWrapDisabling(value);
                }
            }
        }

        bool _showHorizontalScrollBar;

        [Localizable(false), DefaultValue(false)]
        public bool HideHorizontalScrollBar
        {
            get { return !_showHorizontalScrollBar; }
            set
            {
                _showHorizontalScrollBar = !value;
                if (_nativeWindow != null)
                {
                    _nativeWindow.ShowHorizontalScrollBar = !value;
                    UpdateSize();
                }
            }
        }

        bool _notInteractiveEditor;

        [Localizable(false), DefaultValue(true), DesignOnly(true)]
        public bool InteractiveEditor
        {
            get { return !_notInteractiveEditor; }
            set
            {
                _notInteractiveEditor = ! value;

                if (_nativeWindow != null)
                    _nativeWindow.InteractiveEditor = value;
            }
        }

        public Point ViewToBuffer(Point p)
        {
            if (_nativeWindow == null)
                return p;
            else
                return _nativeWindow.ViewToBuffer(p);
        }

        public Point BufferToView(Point p)
        {
            if (_nativeWindow == null)
                return p;
            else
                return _nativeWindow.BufferToView(p);
        }

        bool _enableSplitter;
        [Localizable(false), DefaultValue(false)]
        public bool EnableSplitter
        {
            get { return _enableSplitter; }
            set
            {
                _enableSplitter = value;
                if (_nativeWindow != null)
                {
                    _nativeWindow.EnableSplitter = value;
                }
            }
        }

        bool _enableDropDownBar;
        [Localizable(false), DefaultValue(false)]
        public bool EnableNavigationBar
        {
            get { return _enableDropDownBar; }
            set
            {
                _enableDropDownBar = value;
                if (_nativeWindow != null)
                {
                    _nativeWindow.EnableNavigationBar = value;
                }
            }
        }

        bool _readOnly;
        [Localizable(false), DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                if (_nativeWindow != null)
                    _nativeWindow.SetReadOnly(value);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LineHeight
        {
            get
            {
                if (DesignMode || _nativeWindow == null)
                    return 0; // Designer scenario

                if (_wpfInfo != null)
                    return _wpfInfo.GetLineHeight();

                return _nativeWindow.LineHeight;
            }
        }

        public void Clear(bool clearUndoBuffer)
        {
            if (_nativeWindow != null)
                _nativeWindow.Clear(clearUndoBuffer);
            else
                Text = "";
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (_nativeWindow != null)
            {
                _nativeWindow.Dispose();
                _nativeWindow = null;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (DesignMode)
                return;

            try
            {
                Control topParent = TopLevelControl;

                if (topParent == null)
                {
                    topParent = this;
                    Control parent;
                    while (null != (parent = topParent.Parent))
                    {
                        topParent = parent;
                    }
                }

                VSContainerForm ownerForm = topParent as VSContainerForm;
                if (ownerForm != null && ownerForm.Modal)
                {
                    InitializeForm(ownerForm);
                    return;
                }

                IAnkhToolWindowControl toolWindow = topParent as IAnkhToolWindowControl;

                if (toolWindow != null)
                {
                    InitializeToolWindow(toolWindow);
                    return;
                }

                VSEditorControl documentForm = topParent as VSEditorControl;
                if (documentForm != null)
                {
                    InitializeDocumentForm(documentForm);
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(_text))
                {
                    Text = _text;
                }
            }
        }

        void InitializeForm(VSContainerForm ownerForm)
        {
            if (_nativeWindow == null)
            {
                Init(ownerForm.Context, true, false);
            }

            IAnkhVSContainerForm cf = ownerForm;

            cf.AddCommandTarget(_nativeWindow);
            cf.AddWindowPane(_nativeWindow.WindowPane);
            cf.ContainerMode |= VSContainerMode.TranslateKeys | VSContainerMode.UseTextEditorScope;
        }

        bool _inToolWindow;
        void InitializeToolWindow(IAnkhToolWindowControl toolWindow)
        {
            _inToolWindow = true;
            Init(toolWindow.ToolWindowHost, false, false);
            toolWindow.ToolWindowHost.AddCommandTarget(_nativeWindow);
        }

        bool _inDocumentForm;
        void InitializeDocumentForm(VSEditorControl documentForm)
        {
            _inDocumentForm = true;
            Init(documentForm.Context, false, false);
            documentForm.AddCommandTarget(_nativeWindow);
        }

        /// <summary>
        /// Creation and initialization of <see cref="CodeEditorWindow"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="modal">if set to <c>true</c> [allow modal].</param>
        protected void Init(IAnkhServiceProvider context, bool modal, bool toolWindow)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _wpfEditorInfo = context.GetService<IGetWpfEditorInfo>();

            try
            {
                _nativeWindow = new CodeEditorWindow(_context, this);

                // Set init only values
                _nativeWindow.EnableSplitter = EnableSplitter;
                _nativeWindow.EnableNavigationBar = EnableNavigationBar;
                _nativeWindow.InteractiveEditor = InteractiveEditor;

                _nativeWindow.Init(modal, toolWindow, _readOnly, ForceLanguageService);

                _nativeWindow.ShowHorizontalScrollBar = !HideHorizontalScrollBar;
                _nativeWindow.SetWordWrapDisabling(DisableWordWrap);
                _nativeWindow.Size = ClientSize;
                _nativeWindow.SetReadOnly(_readOnly);
            }
            catch
            {
                _nativeWindow = null;
                throw;
            }

            if (_wpfEditorInfo != null)
                _wpfInfo = _nativeWindow.GetWpfInfo(_wpfEditorInfo);
        }

        void UpdateSize()
        {
            if (_nativeWindow != null)
            {
                if (!_inToolWindow && !_inDocumentForm)
                    _nativeWindow.Size = ClientSize;
                else
                {
                    // Delay until sizing of toolwindow completed
                    BeginInvoke((AnkhAction)
                        delegate
                        {
                            _nativeWindow.Size = ClientSize;
                        });
                }

            }
        }

        // Called from VSTextEditor.Select(directed, forward)
        void IVSTextEditorImplementation.Select(bool directed, bool forward)
        {
            Select(directed, forward);
        }

        public IOleCommandTarget EditorCommandTarget
        {
            get
            {
                if (_nativeWindow != null)
                    return _nativeWindow.EditorCommandTarget;
                else
                    return null;
            }
        }

        public Point EditorClientTopLeft
        {
            get
            {
                if (_nativeWindow == null)
                    return PointToScreen(new Point(0, 0));
                else if (_wpfInfo != null)
                    return PointToScreen(_wpfInfo.GetTopLeft());

                Point? p = _nativeWindow.EditorClientTopLeft;

                if (p.HasValue)
                    return p.Value;
                else
                    return PointToScreen(new Point(0, 0));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                _wpfInfo = null;

                if (disposing)
                {
                    if (_nativeWindow != null)
                    {
                        _nativeWindow.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Determines whether the specified key is a
        /// regular input key or a special key that requires preprocessing.        
        /// </summary>
        /// <param name="keyData">Specifies key codes and modifiers</param>
        /// <returns>Always returns true</returns>
        protected override bool IsInputKey(Keys keyData)
        {
            Keys key = (keyData & Keys.KeyCode);

            switch (key)
            {
                case Keys.Tab:
                case Keys.Return:
                    {
                        if (_readOnly)
                            return false; // Allow using tab for dialog navigation

                        if ((keyData & Keys.Modifiers & ~Keys.Shift) == Keys.Control)
                        {
                            Form f = TopLevelControl as Form;

                            if (f != null && f.Modal)
                                return false; // Ctrl+Return should be the default button
                        }
                    }
                    break;
                case Keys.Escape:
                    {
                        Form f = TopLevelControl as Form;

                        if (f != null && f.Modal)
                            return false; // Escape should close a dialog
                    }
                    break;
            }

            // Since we process each pressed keystroke, the return value is always true.
            return true;
        }

        protected override bool IsInputChar(char charCode)
        {
            return true;
        }

        /// <summary>
        /// Overrides OnGotFocus method to handle OnGotFocus event
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (_nativeWindow != null)
            {
                _nativeWindow.Focus();
            }
        }

        /// <summary>
        /// Overrides OnSizeChanged method to handle OnSizeChanged event
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (!DesignMode)
                UpdateSize();
        }

        string _text;

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("")]
        public override string Text
        {
            get
            {
                if (_nativeWindow != null)
                    return (_text = _nativeWindow.Text) ?? "";
                else
                    return _text ?? "";
            }
            set
            {
                _text = value;
                if (_nativeWindow != null)
                    _nativeWindow.Text = value;
            }
        }

        /// <summary>
        /// Pastes the specified text at the current location
        /// </summary>
        /// <param name="text">The text.</param>
        public void PasteText(string text)
        {
            _nativeWindow.PasteText(text);
        }

        /// <summary>
        /// Gets the find target.
        /// </summary>
        /// <value>The find target.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IVsFindTarget FindTarget
        {
            get
            {
                if (_nativeWindow != null)
                    return _nativeWindow.FindTarget;

                return null;
            }
        }

        public IVsTextView TextView
        {
            get { return _nativeWindow.TextView; }
        }

        public bool PreFilterKeyMessage(Keys keyData)
        {
            System.Diagnostics.Debug.Assert(_wpfEditorInfo == null); // No filtering necessary in VS2010+

            switch (keyData)
            {
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                case Keys.Left:
                case Keys.Back:
                case Keys.Home:
                case Keys.End:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Insert:
                case Keys.Delete:
                    return true;
                case Keys.Return:
                    if (ModifierKeys == Keys.Control && TopLevelControl is Form)
                        return false;
                    return true;
                default:
                    return false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            if (key == Keys.Tab && (keyData & Keys.Alt) == Keys.None)
            {
                keyData = keyData & ~Keys.Control;
            }
            else if (key == Keys.Return && (keyData & Keys.Modifiers) == Keys.Control)
            {
                keyData = keyData & ~Keys.Control;
            }
            bool b = base.ProcessCmdKey(ref msg, keyData);

            return b;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            if (key == Keys.Tab && (keyData & Keys.Alt) == Keys.None)
            {
                keyData = keyData & ~Keys.Control;
            }
            else if (key == Keys.Return && (keyData & Keys.Modifiers) == Keys.Control)
            {
                keyData = keyData & ~Keys.Control;
            }
            return base.ProcessDialogKey(keyData);
        }

        #region IVSTextEditorImplementation Members

        public void SetInitialText(string text)
        {
            Text = text;
        }

        public event EventHandler<VSTextEditorScrollEventArgs> HorizontalTextScroll
        {
            add { _nativeWindow.HorizontalTextScroll += value; }
            remove 
            {
                // Allow unhooking to succeed after disposing
                if (_nativeWindow != null)
                    _nativeWindow.HorizontalTextScroll -= value; 
            }
        }

        public event EventHandler<VSTextEditorScrollEventArgs> VerticalTextScroll
        {
            add { _nativeWindow.VerticalTextScroll += value; }
            remove 
            {
                // Allow unhooking to succeed after disposing
                if (_nativeWindow != null)
                    _nativeWindow.VerticalTextScroll -= value;
            }
        }

        public void LoadFile(string path, bool monitorChanges)
        {
            if (_nativeWindow == null)
                throw new InvalidOperationException();

            _nativeWindow.LoadFile(path);

            if (!monitorChanges)
                _nativeWindow.IgnoreFileChanges(true);
        }

        #endregion
    }

    /// <summary>
    /// This class is a low-level encapsulation of a text editor to provide some helper functions
    /// </summary>
    internal class CodeEditorWindow : AnkhService, IOleCommandTarget, IDisposable, IVsTextViewEvents
    {
        #region Fields

        readonly Control _container;
        readonly IOleServiceProvider _serviceProvider;
        IVsWindowPane _windowPane;

        /// <summary>
        /// Editor window handle
        /// </summary>
        IntPtr editorHwnd;
        IntPtr viewHwnd;

        /// <summary>
        /// The IOleCommandTarget interface enables objects and their containers to dispatch commands to each other.
        /// For example, an object's toolbars may contain buttons for commands such as Print, Print Preview, Save, New, and Zoom. 
        /// </summary>
        private IOleCommandTarget commandTarget;

        /// <summary>
        /// Reference to VsCodeWindow object
        /// </summary>
        IVsCodeWindow _codeWindow;

        IVsTextBuffer _textBuffer;
        IVsTextView _textView;
        Guid? _languageService;

        #endregion

        public CodeEditorWindow(IAnkhServiceProvider context, Control container)
            : base(context)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            _container = container;
            _serviceProvider = context.GetService<IOleServiceProvider>();
        }

        public string Text
        {
            get
            {
                if (_textBuffer == null)
                    return null;

                IVsTextLines lines = _textBuffer as IVsTextLines;

                if (lines == null)
                    return null;

                string text;
                int endLine, endIndex;
                Marshal.ThrowExceptionForHR(lines.GetLastLineIndex(out endLine, out endIndex));
                Marshal.ThrowExceptionForHR(lines.GetLineText(0, 0, endLine, endIndex, out text));
                return text;
            }
            set
            {
                if (_textBuffer == null)
                    return;

                if (string.IsNullOrEmpty(value))
                    value = "";

                IVsTextLines lines = _textBuffer as IVsTextLines;

                if (lines == null)
                    return;

                int endLine, endIndex;

                Marshal.ThrowExceptionForHR(lines.GetLastLineIndex(out endLine, out endIndex));

                IntPtr pText = Marshal.StringToCoTaskMemUni(value);
                try
                {
                    if (_ro)
                        InternalSetReadOnly(false);

                    Marshal.ThrowExceptionForHR(lines.ReloadLines(0, 0, endLine, endIndex, pText, value.Length, null));
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pText);

                    if (_ro)
                        InternalSetReadOnly(true);
                }

                _textView.SetCaretPos(0, 0); // Move cursor to 0,0
                _textView.SetScrollPosition(0, 0); // Scroll horizontally
                _textView.SetScrollPosition(1, 0); // Scroll vertically     
            }
        }

        /// <summary>
        /// Gets the window pane.
        /// </summary>
        /// <value>The window pane.</value>
        public IVsWindowPane WindowPane
        {
            get { return _windowPane; }
        }

        public bool PasteText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            IVsTextLines lines = _textBuffer as IVsTextLines;

            if (lines == null || _textView == null)
                return false;

            Point[] s = GetSelection();

            if (s == null)
            {
                Point p = GetCaretPosition();
                s = new Point[] { p, p };
            }

            IntPtr pText = Marshal.StringToCoTaskMemUni(text);
            try
            {
                return VSErr.Succeeded(lines.ReplaceLines(s[0].Y, s[0].X, s[1].Y, s[1].X, pText, text.Length, null));
            }
            finally
            {
                Marshal.FreeCoTaskMem(pText);
            }


        }

        Point[] GetSelection()
        {
            int y1, x1, y2, x2;
            if (!VSErr.Succeeded(_textView.GetSelection(out y1, out x1, out y2, out x2)))
                return null;

            if (y1 < y2)
                return new Point[] { new Point(x1, y1), new Point(x2, y2) };
            else if (y2 < y1)
                return new Point[] { new Point(x2, y2), new Point(x1, y1) };
            else
                return new Point[] { new Point(Math.Min(x1, x2), y1), new Point(Math.Max(x1, x2), y1) };
        }

        Point GetCaretPosition()
        {
            int y, x;

            if (!VSErr.Succeeded(_textView.GetCaretPos(out y, out x)))
                return new Point();
            else
                return new Point(x, y);
        }

        public IVsFindTarget FindTarget
        {
            get
            {
                IVsFindTarget ft = _textView as IVsFindTarget;

                if (ft != null)
                    return ft;

                ft = _textBuffer as IVsFindTarget;

                return ft;
            }
        }

        #region Properties

        /// <summary>
        /// Determines editor's window placement
        /// </summary>
        public Size Size
        {
            set
            {
                if (editorHwnd != IntPtr.Zero)
                {
                    // Around our editor is a VS Splitter window that is responsible for showing the scroll bars
                    // As we don't know a valid method to hide those; we just move them out of the visual area

                    int width = value.Width;
                    int height = value.Height;

                    if (!ShowHorizontalScrollBar)
                    {
                        IVsTextManager2 manager = GetService<IVsTextManager2>(typeof(SVsTextManager));

                        if (manager != null)
                        {
                            FRAMEPREFERENCES2[] items = new FRAMEPREFERENCES2[1];
                            LANGPREFERENCES2[] lp = null;

                            if (_languageService.HasValue)
                            {
                                lp = new LANGPREFERENCES2[1];
                                lp[0].guidLang = _languageService.Value;
                            }
                            else
                            {
                                Guid lid;
                                if (VSErr.Succeeded(_textBuffer.GetLanguageServiceID(out lid)))
                                {
                                    lp = new LANGPREFERENCES2[1];
                                    lp[0].guidLang = lid;
                                }
                            }

                            if (VSErr.Succeeded(manager.GetUserPreferences2(null, items, lp, null)))
                            {
                                // Only hide the horizontal scrollbar if one would have been visible 
                                if (items[0].fHorzScrollbar != 0 && (lp == null || lp[0].fWordWrap == 0))
                                {
                                    height += SystemInformation.HorizontalScrollBarHeight;
                                }
                            }
                        }
                    }

                    NativeMethods.SetWindowPos(editorHwnd, IntPtr.Zero, 0, 0,
                        width, height, 0x16); // 0x16 = SWP_NOACTIVATE | SWP_NOZORDER | SWP_NOMOVE
                }
            }
        }

        bool _showHorizontalScrollBar;
        public bool ShowHorizontalScrollBar
        {
            get { return _showHorizontalScrollBar; }
            set { _showHorizontalScrollBar = value; }
        }

        bool _enableSplitter;
        public bool EnableSplitter
        {
            get { return _enableSplitter; }
            set { _enableSplitter = value; }
        }

        bool _enableDropDownBar;
        public bool EnableNavigationBar
        {
            get { return _enableDropDownBar; }
            set { _enableDropDownBar = value; }
        }

        bool _interactiveEditor;
        public bool InteractiveEditor
        {
            get { return _interactiveEditor; }
            set { _interactiveEditor = value; }
        }

        #endregion

        #region Methods

        const uint VIF_ENABLEAUTONOMOUSFIND = 0x00010000;

        /// <summary>
        /// Used to create Code Window
        /// </summary>
        /// <param name="parentHandle">Parent window handle</param>
        /// <param name="place">The place.</param>
        /// <param name="modal">if set to <c>true</c> [allow modal].</param>
        /// <param name="toolWindow"></param>
        /// <param name="forceLanguageService"></param>
        /// <param name="codeWindow">Represents a multiple-document interface (MDI) child that contains one or more code views.</param>
        private void CreateCodeWindow(IntPtr parentHandle, Rectangle place, bool modal, bool toolWindow, bool readOnly, Guid? forceLanguageService, out IVsCodeWindow codeWindow)
        {
            codeWindow = CreateLocalInstance<IVsCodeWindow>(typeof(VsCodeWindowClass), _serviceProvider);

            // initialize code window
            INITVIEW[] initView = new INITVIEW[1];
            initView[0].fSelectionMargin = 0;
            initView[0].IndentStyle = vsIndentStyle.vsIndentStyleSmart;
            initView[0].fWidgetMargin = 0;
            initView[0].fVirtualSpace = 0;
            initView[0].fDragDropMove = 1;
            initView[0].fVisibleWhitespace = 0;

            uint initViewFlags =
                (uint)TextViewInitFlags.VIF_SET_WIDGET_MARGIN |
                (uint)TextViewInitFlags.VIF_SET_SELECTION_MARGIN |
                (uint)TextViewInitFlags2.VIF_SUPPRESSBORDER |
                //(uint)TextViewInitFlags2.VIF_SUPPRESS_STATUS_BAR_UPDATE |
                (uint)TextViewInitFlags2.VIF_SUPPRESSTRACKCHANGES;

            if (modal || toolWindow)
                initViewFlags |= (uint)TextViewInitFlags2.VIF_SUPPRESSTRACKGOBACK;

            //if (VSVersion.V2012OrLater)
            //    initViewFlags |= VIF_ENABLEAUTONOMOUSFIND;

            if (modal)
                initViewFlags |= (uint)TextViewInitFlags2.VIF_ACTIVEINMODALSTATE;

            if (readOnly)
                initViewFlags |= (uint)TextViewInitFlags2.VIF_READONLY;

            _codewindowbehaviorflags cwFlags = _codewindowbehaviorflags.CWB_DEFAULT;

            if (!EnableSplitter)
                cwFlags |= _codewindowbehaviorflags.CWB_DISABLESPLITTER;

            if (!EnableNavigationBar)
                cwFlags |= _codewindowbehaviorflags.CWB_DISABLEDROPDOWNBAR;

            IVsCodeWindowEx codeWindowEx = codeWindow as IVsCodeWindowEx;
            Marshal.ThrowExceptionForHR(codeWindowEx.Initialize((uint)cwFlags, 0, null, null, initViewFlags, initView));

            // set buffer
            _textBuffer = CreateLocalInstance<IVsTextBuffer>(typeof(VsTextBufferClass), _serviceProvider);
            _textBuffer.InitializeContent("", 0);

            if (forceLanguageService.HasValue)
            {
                Guid languageService = forceLanguageService.Value;

                SetLanguageServiceInternal(languageService);
            }

            if (InteractiveEditor)
            {
                Guid roles = Vs2010TextBufferUserDataGuid.VsTextViewRoles_guid;
                IVsUserData userData = _textBuffer as IVsUserData;

                // The documentation says these context apply to the text buffer.
                // Do it here just to be sure.
                if (userData != null)
                    userData.SetData(ref roles, "INTERACTIVE");

                // But VS2010 requires it on the code window, in VS2008 this
                // interface is not implemented here.
                userData = codeWindow as IVsUserData;

                if (userData != null)
                    userData.SetData(ref roles, "INTERACTIVE");
            }

            Marshal.ThrowExceptionForHR(codeWindow.SetBuffer((IVsTextLines)_textBuffer));

            // create pane window
            _windowPane = (IVsWindowPane)codeWindow;
            Marshal.ThrowExceptionForHR(_windowPane.SetSite(_serviceProvider));

            Marshal.ThrowExceptionForHR(_windowPane.CreatePaneWindow(parentHandle, place.X, place.Y, place.Width, place.Height, out editorHwnd));

            //set the inheritKeyBinding guid so that navigation keys work. The VS 2008 SDK does this from the language service. 
            // The VS2005 sdk doesn't
            IOleServiceProvider sp = codeWindow as IOleServiceProvider;
            if (sp != null)
            {
                ServiceProvider site = new ServiceProvider(sp);
                IVsWindowFrame frame = site.GetService(typeof(IVsWindowFrame).GUID) as IVsWindowFrame;
                if (frame != null)
                {
                    Guid CMDUIGUID_TextEditor = new Guid(0x8B382828, 0x6202, 0x11d1, 0x88, 0x70, 0x00, 0x00, 0xF8, 0x75, 0x79, 0xD2);
                    Marshal.ThrowExceptionForHR(frame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref CMDUIGUID_TextEditor));
                }
            }
        }

        public IVsTextView TextView
        {
            get
            {
                IVsTextView view;
                Marshal.ThrowExceptionForHR(_codeWindow.GetPrimaryView(out view));
                return view;
            }
        }

        /// <summary>
        /// Sets focus to Editor's Window
        /// </summary>
        public void Focus()
        {
            NativeMethods.SetFocus(editorHwnd);
            if (_textView != null)
                _textView.SendExplicitFocus();
        }

        /// <summary>
        /// Window initialization
        /// </summary>
        /// <param name="allowModal">if set to <c>true</c> [allow modal].</param>
        /// <param name="forceLanguageService"><c>null</c> or the language service to force.</param>
        public void Init(bool allowModal, bool toolWindow, bool readOnly, Guid? forceLanguageService)
        {
            //Create window
            CreateCodeWindow(_container.Handle, _container.ClientRectangle, allowModal, toolWindow, readOnly, forceLanguageService, out _codeWindow);
            commandTarget = _codeWindow as IOleCommandTarget;

            IVsTextView textView;
            Marshal.ThrowExceptionForHR(_codeWindow.GetPrimaryView(out textView));

            _languageService = forceLanguageService;
            _textView = textView;
            IntPtr h = textView.GetWindowHandle();

            if (h != IntPtr.Zero && NativeMethods.IsWindow(h))
                viewHwnd = h;

            NativeMethods.ShowWindow(editorHwnd, 4); // 4 = SW_SHOWNOACTIVATE

            HookEvents(true);
        }

        private IVsTextEditorPropertyContainer _editorPropertyContainer;
        IVsTextEditorPropertyContainer EditorPropertyContainer
        {
            get
            {
                if (_editorPropertyContainer != null)
                    return _editorPropertyContainer;

                IVsTextEditorPropertyCategoryContainer spPropCatContainer =
                    (IVsTextEditorPropertyCategoryContainer)_textView;

                if (spPropCatContainer != null)
                {
                    IVsTextEditorPropertyContainer spPropContainer;
                    Guid GUID_EditPropCategory_View_MasterSettings =
                        new Guid("{D1756E7C-B7FD-49a8-B48E-87B14A55655A}");
                    Marshal.ThrowExceptionForHR(spPropCatContainer.GetPropertyCategory(
                        ref GUID_EditPropCategory_View_MasterSettings,
                        out spPropContainer));

                    _editorPropertyContainer = spPropContainer;
                }

                return _editorPropertyContainer;
            }
        }

        bool _ro;
        internal void SetReadOnly(bool value)
        {
            if (_textBuffer != null && _ro != value)
            {
                _ro = value;

                InternalSetReadOnly(value);
            }
        }

        void InternalSetReadOnly(bool value)
        {
            uint state;
            if (VSErr.Succeeded(_textBuffer.GetStateFlags(out state)))
            {
                if (value)
                    state |= (uint)BUFFERSTATEFLAGS.BSF_USER_READONLY;
                else
                    state &= ~(uint)BUFFERSTATEFLAGS.BSF_USER_READONLY;

                _textBuffer.SetStateFlags(state);
            }
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Releases all resources, other than memory, used by the CodeEditorUserControl
        /// </summary>
        public void Dispose()
        {
            if (_codeWindow != null)
            {
                HookEvents(false);
                _codeWindow.Close();
                _codeWindow = null;
            }
        }

        #endregion

        //This implementation is a simple delegation to the implementation inside the text view 
        /// <summary>
        /// Executes a specified command or displays help for a command.
        /// </summary>
        /// <param name="pguidCmdGroup">Pointer to command group</param>
        /// <param name="nCmdID">Identifier of command to execute</param>
        /// <param name="nCmdexecopt">Options for executing the command</param>
        /// <param name="pvaIn">Pointer to input arguments</param>
        /// <param name="pvaOut">Pointer to command output</param>
        /// <returns></returns>
        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (HasFocus)
                return commandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            else
                return VSErr.OLECMDERR_E_NOTSUPPORTED;
        }


        /// <summary>
        /// Queries the object for the status of one or more commands generated by user interface events.
        /// </summary>
        /// <param name="pguidCmdGroup">Pointer to command group</param>
        /// <param name="cCmds">Number of commands in prgCmds array</param>
        /// <param name="prgCmds">Array of commands</param>
        /// <param name="pCmdText">Pointer to name or status of command</param>
        /// <returns></returns>
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (HasFocus)
                return commandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            else
                return VSErr.OLECMDERR_E_NOTSUPPORTED;
        }

        public IOleCommandTarget EditorCommandTarget
        {
            get { return commandTarget; }
        }

        public bool HasFocus
        {
            get
            {
                if (_container.ContainsFocus)
                {
                    IContainerControl p = _container.GetContainerControl();

                    if (p != null && p.ActiveControl != _container)
                        _container.Select(); // The editor has the focus: then it must be the active control

                    return true;
                }

                return false;
            }
        }

        internal void ReplaceContents(string pathToReplaceWith)
        {
            IVsTextBuffer tempBuffer = CreateLocalInstance<IVsTextBuffer>(typeof(VsTextBufferClass), _serviceProvider);
            IntPtr buffer = IntPtr.Zero;
            bool setReadOnly = false;
            try
            {
                IVsPersistDocData2 tempDocData = (IVsPersistDocData2)tempBuffer;
                tempDocData.LoadDocData(pathToReplaceWith);

                IVsTextStream tempStream = (IVsTextStream)tempBuffer;

                int size;
                Marshal.ThrowExceptionForHR(tempStream.GetSize(out size));

                buffer = Marshal.AllocCoTaskMem((size + 1) * sizeof(char));

                if (_ro)
                {
                    InternalSetReadOnly(false);
                    setReadOnly = true;
                }

                Marshal.ThrowExceptionForHR(tempStream.GetStream(0, size, buffer));

                IVsTextStream destStream = (IVsTextStream)_textBuffer;
                int oldDestSize;
                Marshal.ThrowExceptionForHR(destStream.GetSize(out oldDestSize));

                destStream.ReplaceStream(0, oldDestSize, buffer, size);
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(buffer);

                if (tempBuffer != null && Marshal.IsComObject(tempBuffer))
                    Marshal.ReleaseComObject(tempBuffer);

                if (setReadOnly)
                    InternalSetReadOnly(true);
            }
        }

        internal void LoadFile(string path)
        {
            IVsPersistDocData2 docData = (IVsPersistDocData2)_textBuffer;
            Marshal.ThrowExceptionForHR(docData.LoadDocData(path));
        }

        internal void Clear(bool clearUndoBuffer)
        {
            Text = "";

            if (clearUndoBuffer)
            {
                IOleUndoManager mgr;

                if (VSErr.Succeeded(_textBuffer.GetUndoManager(out mgr)) && mgr != null)
                {
                    mgr.DiscardFrom(null);
                }
            }
        }

        internal void IgnoreFileChanges(bool ignore)
        {
            IVsDocDataFileChangeControl dfc = (IVsDocDataFileChangeControl)_textBuffer;

            Marshal.ThrowExceptionForHR(dfc.IgnoreFileChanges(ignore ? 1 : 0));
        }

        public Point? EditorClientTopLeft
        {
            get
            {
                RECT rect;
                if (viewHwnd == IntPtr.Zero
                    || !NativeMethods.GetWindowRect(viewHwnd, out rect))
                {
                    return null;
                }

                return new Point(rect.left, rect.top);
            }
        }

        internal int LineHeight
        {
            get
            {
                int height;
                Marshal.ThrowExceptionForHR(_textView.GetLineHeight(out height));
                return height;
            }
        }

        void IVsTextViewEvents.OnChangeCaretLine(IVsTextView pView, int iNewLine, int iOldLine)
        {
        }


        /// <summary>
        /// Occurs when the text view scrolled
        /// </summary>
        void IVsTextViewEvents.OnChangeScrollInfo(IVsTextView pView, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
        {
            bool vertical = (iBar == 1);
            VSTextEditorScrollEventArgs ea = new VSTextEditorScrollEventArgs(iMinUnit, iMaxUnits, iVisibleUnits, iFirstVisibleUnit);

            if (vertical)
                OnVerticalTextScroll(ea);
            else
                OnHorizontalTextScroll(ea);
        }

        public event EventHandler<VSTextEditorScrollEventArgs> HorizontalTextScroll;

        private void OnHorizontalTextScroll(VSTextEditorScrollEventArgs ea)
        {
            if (HorizontalTextScroll != null)
                HorizontalTextScroll(this, ea);
        }

        public event EventHandler<VSTextEditorScrollEventArgs> VerticalTextScroll;
        private void OnVerticalTextScroll(VSTextEditorScrollEventArgs ea)
        {
            if (VerticalTextScroll != null)
                VerticalTextScroll(this, ea);
        }

        void IVsTextViewEvents.OnKillFocus(IVsTextView pView)
        {
        }

        void IVsTextViewEvents.OnSetBuffer(IVsTextView pView, IVsTextLines pBuffer)
        {
        }

        void IVsTextViewEvents.OnSetFocus(IVsTextView pView)
        {
        }

        uint _textEventsCookie;
        void HookEvents(bool hook)
        {
            if (hook && _textEventsCookie == 0)
                TryHookConnectionPoint<IVsTextViewEvents>(_textView, this, out _textEventsCookie);
            else if (!hook && _textEventsCookie != 0)
            {
                uint ck = _textEventsCookie;
                _textEventsCookie = 0;
                ReleaseHook<IVsTextViewEvents>(_textView, ck);
            }
        }

        internal void SetLanguageService(Guid languageService)
        {
            if (_textBuffer == null)
                throw new InvalidOperationException();

            SetLanguageServiceInternal(languageService);
        }

        private void SetLanguageServiceInternal(Guid languageService)
        {
            Marshal.ThrowExceptionForHR(_textBuffer.SetLanguageServiceID(ref languageService));
        }

        #region Native Methods
        static class NativeMethods
        {
            [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
            internal static extern IntPtr SetFocus(IntPtr hWnd);

            [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

            [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool IsWindow(IntPtr hWnd);
        }
        #endregion

        IVsTextLayer _topLayer;
        bool _searchedTop;

        IVsTextLayer TopLayer
        {
            get
            {
                if (_searchedTop)
                    return _topLayer;

                if (_textView == null)
                    return null;
                _searchedTop = true;

                IVsLayeredTextView layeredView = _textView as IVsLayeredTextView;

                if (layeredView != null)
                {
                    IVsTextLayer topLayer;
                    if (VSErr.Succeeded(layeredView.GetTopmostLayer(out topLayer)))
                        return _topLayer = topLayer;
                }

                return null;
            }
        }

        internal void SetWordWrapDisabling(bool disable)
        {
            if (EditorPropertyContainer == null)
                return;

            if (disable)
                EditorPropertyContainer.SetProperty(VSEDITPROPID.VSEDITPROPID_ViewLangOpt_WordWrap, false);
            else
                EditorPropertyContainer.RemoveProperty(VSEDITPROPID.VSEDITPROPID_ViewLangOpt_WordWrap);
        }

        internal Point ViewToBuffer(Point p)
        {
            IVsTextLayer topLayer = TopLayer;

            if (topLayer == null)
                return p;

            int bY, bX;
            if (VSErr.Succeeded(topLayer.LocalLineIndexToBase(p.Y, p.X, out bY, out bX)))
                return new Point(bX, bY);
            else
                return new Point(-1, -1); // Not represented in buffer
        }

        internal Point BufferToView(Point p)
        {
            IVsTextLayer topLayer = TopLayer;

            if (topLayer == null)
                return p;

            int bY, bX;
            if (VSErr.Succeeded(topLayer.BaseLineIndexToLocal(p.Y, p.X, out bY, out bX)))
                return new Point(bX, bY);
            else
                return new Point(-1, -1); // Not represented in view
        }

        internal IWpfEditorInfo GetWpfInfo(IGetWpfEditorInfo wpfEditorInfo)
        {
            return wpfEditorInfo.GetWpfInfo(_textView);
        }


        #region From the VS2010 SDK: VSConstants.cs

        /// <summary>
        /// These are IVsUserData properties that are supported by the TextBuffer (DocData) object
        /// of the Source Code (Text) Editor. The IVsUserData interface is retrieved by 
        /// QueryInterface on the IVsTextLines object of the Text Editor.
        /// </summary>
        public static class Vs2010TextBufferUserDataGuid
        {
            /// <summary>string: Moniker of document loaded in the buffer. It will be the full path of file if the document is a file.</summary>
            public const string VsBufferMoniker_string = "{978A8E17-4DF8-432A-9623-D530A26452BC}";
            /// <summary>string: Moniker of document loaded in the TextBuffer. It will be the full path of file if the document is a file.</summary>
            public static readonly Guid VsBufferMoniker_guid = new Guid(VsBufferMoniker_string);

            /// <summary>bool: true if buffer is a file on disk</summary>
            public const string VsBufferIsDiskFile_string = "{D9126592-1473-11D3-BEC6-0080C747D9A0}";
            /// <summary>bool: true if buffer is a file on disk</summary>
            public static readonly Guid VsBufferIsDiskFile_guid = new Guid(VsBufferIsDiskFile_string);

            /// <summary>uint: VS Text File Format (VSTFF) for buffer. codepage = bufferVSTFF & __VSTFF.VSTFF_CPMASK; vstffFlags = bufferVSTFF & __VSTFF.VSTFF_FLAGSMASK;</summary>
            public const string VsBufferEncodingVSTFF_string = "{16417F39-A6B7-4C90-89FA-770D2C60440B}";
            /// <summary>uint: VS Text File Format (VSTFF) for buffer. codepage = bufferVSTFF & __VSTFF.VSTFF_CPMASK; vstffFlags = bufferVSTFF & __VSTFF.VSTFF_FLAGSMASK;</summary>
            public static readonly Guid VsBufferEncodingVSTFF_guid = new Guid(VsBufferEncodingVSTFF_string);

            /// <summary>uint: This should only be used by editor factories that want to specify a codepage on loading from the openwith dialog. 
            /// This data is only for a set purpose.  You cannot get the value of this back.
            /// </summary>
            public const string VsBufferEncodingPromptOnLoad_string = "{99EC03F0-C843-4C09-BE74-CDCA5158D36C}";
            /// <summary>uint: This should only be used by editor factories that want to specify a codepage on loading from the openwith dialog. 
            /// This data is only for a set purpose.  You cannot get the value of this back.
            /// </summary>
            public static readonly Guid VsBufferEncodingPromptOnLoad_guid = new Guid(VsBufferEncodingPromptOnLoad_string);

            /// <summary>bool: If true and the current BufferEncoding is CHARFMT_MBCS, the buffer will runs it's HTML charset tag detection code to determine a codepage to load and save the file. The detected codepage overrides any codepage set in CHARFMT_MBCS.
            /// This is forced on in the buffer's IPersistFileFormat::LoadDocData when it sees an HTML type of file, according to the extension mapping in "$RootKey$\Languages\File Extensions".
            /// </summary>
            public const string VsBufferDetectCharSet_string = "{36358D1F-BF7E-11D1-B03A-00C04FB68006}";
            /// <summary>bool: If true and the current BufferEncoding is CHARFMT_MBCS, the buffer will runs it's HTML charset tag detection code to determine a codepage to load and save the file. The detected codepage overrides any codepage set in CHARFMT_MBCS.
            /// This is forced on in the buffer's IPersistFileFormat::LoadDocData when it sees an HTML type of file, according to the extension mapping in "$RootKey$\Languages\File Extensions".
            /// </summary>
            public static readonly Guid VsBufferDetectCharSet_guid = new Guid(VsBufferDetectCharSet_string);

            /// <summary>bool: (default = true) If true then a change to the buffer's moniker will cause the buffer to change the language service 
            /// based on the file extension of the moniker.
            /// </summary>
            public const string VsBufferDetectLangSID_string = "{17F375AC-C814-11D1-88AD-0000F87579D2}";
            /// <summary>bool: (default = true) If true then a change to the buffer's moniker will cause the buffer to change the language service 
            /// based on the file extension of the moniker.
            /// </summary>
            public static readonly Guid VsBufferDetectLangSID_guid = new Guid(VsBufferDetectLangSID_string);

            /// <summary>string: This property will be used to set the SEID_PropertyBrowserSID element of the selection for text views.  
            /// This is only used if you have a custom property browser. If this property is not set, the standard property browser 
            /// will be associated with the view.
            /// </summary>
            public const string PropertyBrowserSID_string = "{CE6DDBBA-8D13-11D1-8889-0000F87579D2}";
            /// <summary>string: This property will be used to set the SEID_PropertyBrowserSID element of the selection for text views.  
            /// This is only used if you have a custom property browser. If this property is not set, the standard property browser 
            /// will be associated with the view.
            /// </summary>
            public static readonly Guid PropertyBrowserSID_guid = new Guid(PropertyBrowserSID_string);

            /// <summary>string: This property provides a specific error message for when the buffer originates the BUFFER_E_READONLY error.
            /// Set this string to be the (localized) text you want displayed to the user.  Note that the buffer itself does not 
            /// put up UI, but only calls IVsUIShell::SetErrorInfo. The caller can decide whether to show the message to the user.
            /// </summary>
            public const string UserReadOnlyErrorString_string = "{A3BCFE56-CF1B-11D1-88B1-0000F87579D2}";
            /// <summary>string: This property provides a specific error message for when the buffer originates the BUFFER_E_READONLY error.
            /// Set this string to be the (localized) text you want displayed to the user.  Note that the buffer itself does not 
            /// put up UI, but only calls IVsUIShell::SetErrorInfo. The caller can decide whether to show the message to the user.
            /// </summary>
            public static readonly Guid UserReadOnlyErrorString_guid = new Guid(UserReadOnlyErrorString_string);

            /// <summary>object: This property is used to get access to the buffer's storage object.
            /// The returned pointer can be QI'd for IVsTextStorage and IVsPersistentTextImage.  
            /// This is a get-only property. To set the storage, use the buffer's InitializeContentEx method.
            /// </summary>
            public const string BufferStorage_string = "{D97F167A-638E-11D2-88F6-0000F87579D2}";
            /// <summary>object: This property is used to get access to the buffer's storage object.
            /// The returned pointer can be QI'd for IVsTextStorage and IVsPersistentTextImage.  
            /// This is a get-only property. To set the storage, use the buffer's InitializeContentEx method.
            /// </summary>
            public static readonly Guid BufferStorage_guid = new Guid(BufferStorage_string);

            /// <summary>object: Use this property if the file opened in the buffer is associated with list of extra files under source code control (SCC).
            /// Set this property with an implementation of IVsBufferExtraFiles in order to control how the buffer handles SCC operations.
            /// The IVsBufferExtraFiles object set will determine what files are checked out from Source Code Control (SCC) when edits are made to the buffer.
            /// This property controls the behavior of IVsTextManager2::AttemptToCheckOutBufferFromScc3 and GetBufferSccStatus3 as well as which
            /// files are passed by the buffer when it calls IVsQueryEditQuerySave2 methods.
            /// </summary>
            public const string VsBufferExtraFiles_string = "{FD494BF6-1167-4635-A20C-5C24B2D7B33D}";
            /// <summary>object: Use this property if the file opened in the buffer is associated with list of extra files under source code control (SCC).
            /// Set this property with an implementation of IVsBufferExtraFiles in order to control how the buffer handles SCC operations.
            /// The IVsBufferExtraFiles object set will determine what files are checked out from Source Code Control (SCC) when edits are made to the buffer.
            /// This property controls the behavior of IVsTextManager2::AttemptToCheckOutBufferFromScc3 and GetBufferSccStatus3 as well as which
            /// files are passed by the buffer when it calls IVsQueryEditQuerySave2 methods.
            /// </summary>
            public static readonly Guid VsBufferExtraFiles_guid = new Guid(VsBufferExtraFiles_string);

            /// <summary>bool: </summary>
            public const string VsBufferFileReload_string = "{80D2B881-81A3-4F0B-BCF0-70A0054E672F}";
            /// <summary>bool: </summary>
            public static readonly Guid VsBufferFileReload_guid = new Guid(VsBufferFileReload_string);

            /// <summary>bool: </summary>
            public const string VsInitEncodingDialogFromUserData_string = "{C2382D84-6650-4386-860F-248ECB222FC1}";
            /// <summary>bool: </summary>
            public static readonly Guid VsInitEncodingDialogFromUserData_guid = new Guid(VsInitEncodingDialogFromUserData_string);

            /// <summary>string: The ContentType for the text buffer.</summary>
            public const string VsBufferContentType_string = "{1BEB4195-98F4-4589-80E0-480CE32FF059}";
            /// <summary>string: The ContentType for the text buffer.</summary>
            public static readonly Guid VsBufferContentType_guid = new Guid(VsBufferContentType_string);

            /// <summary>string: The comma-separated list of text view roles for the text view.</summary>
            public const string VsTextViewRoles_string = "{297078FF-81A2-43D8-9CA3-4489C53C99BA}";
            /// <summary>string: The comma-separated list of text view roles for the text view.</summary>
            public static readonly Guid VsTextViewRoles_guid = new Guid(VsTextViewRoles_string);
        }
        #endregion
    }
}

