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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using System.Security.Permissions;
using Ankh.Scc.UI;
using Ankh.Ids;

namespace Ankh.UI.PendingChanges
{
    /// <summary>
    /// This class is used to implement CodeEditorUserControl
    /// </summary>
    /// <seealso cref="UserControl"/>
    public partial class VSTextEditor : Control
    {
        CodeEditorWindow _nativeWindow;
        IAnkhServiceProvider _context;
        BorderStyle _borderStyle;
        Guid? _forceLanguageService;

        public VSTextEditor()
        {
            BackColor = SystemColors.Window;
        }

        public VSTextEditor(IContainer container)
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
            set { _forceLanguageService = value; }
        }

        bool _showHorizontalScrollBar;

        [Localizable(false), DefaultValue(false)]
        public bool ShowHorizontalScrollBar
        {
            get { return _showHorizontalScrollBar; }
            set
            {
                _showHorizontalScrollBar = value;
                if (_nativeWindow != null)
                {
                    _nativeWindow.ShowHorizontalScrollBar = value;
                    UpdateSize();
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

                return _nativeWindow.LineHeight;
            }
        }

        public event EventHandler<TextViewScrollEventArgs> Scroll;

        public void OpenFile(string path)
        {
            if (_nativeWindow == null)
                throw new InvalidOperationException("Code editor not initialized");

            _nativeWindow.LoadFile(path);
        }

        public void ReplaceContents(string pathToReplaceWith)
        {
            if (_nativeWindow == null)
                throw new InvalidOperationException("Code editor not initialized");

            _nativeWindow.ReplaceContents(pathToReplaceWith);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (DesignMode)
                return;

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

        void InitializeForm(VSContainerForm ownerForm)
        {
            if (CommandTarget == null)
            {
                Init(ownerForm.Context, true);
            }

            IAnkhVSContainerForm cf = ownerForm;

            cf.AddCommandTarget(CommandTarget);
            cf.AddWindowPane(WindowPane);
            cf.ContainerMode |= VSContainerMode.TranslateKeys | VSContainerMode.UseTextEditorScope;

            if (!string.IsNullOrEmpty(_text))
            {
                Text = _text;
            }
        }

        bool _inToolWindow;
        void InitializeToolWindow(IAnkhToolWindowControl toolWindow)
        {
            _inToolWindow = true;
            Init(toolWindow.ToolWindowHost, false);
            toolWindow.ToolWindowHost.AddCommandTarget(CommandTarget);
        }

        bool _inDocumentForm;
        void InitializeDocumentForm(VSEditorControl documentForm)
        {
            _inDocumentForm = true;
            Init(documentForm.Context, false);
            documentForm.AddCommandTarget(CommandTarget);
        }

        #region Methods

        /// <summary>
        /// Creation and initialization of <see cref="CodeEditorNativeWindow"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="allowModal">if set to <c>true</c> [allow modal].</param>
        public void Init(IAnkhServiceProvider context, bool allowModal)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            IOleServiceProvider serviceProvider = context.GetService<IOleServiceProvider>();
            _nativeWindow = new CodeEditorWindow(_context, this);
            _nativeWindow.Init(allowModal, ForceLanguageService);
            _nativeWindow.ShowHorizontalScrollBar = ShowHorizontalScrollBar;
            _nativeWindow.Size = ClientSize;
            _nativeWindow.SetReadOnly(_readOnly);

            _nativeWindow.Scroll += new EventHandler<TextViewScrollEventArgs>(codeEditorNativeWindow_Scroll);
        }

        void codeEditorNativeWindow_Scroll(object sender, TextViewScrollEventArgs e)
        {
            if (Scroll != null)
                Scroll(sender, e);
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

        [CLSCompliant(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IOleCommandTarget CommandTarget
        {
            get { return _nativeWindow; }
        }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams createParams = base.CreateParams;
                // style = 0x56010000 = WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS | WS_CLIPCHILDREN | WS_TABSTOP
                // exstyle = 0x10000 = WS_EX_CONTROLPARENT

                // Remove border settings
                createParams.ExStyle &= ~0x00000200; // WS_EX_CLIENTEDGE
                createParams.Style &= ~0x00800000; // WS_BORDER
                switch (_borderStyle)
                {
                    case BorderStyle.FixedSingle:
                        createParams.Style |= 0x00800000; // WS_BORDER
                        return createParams;

                    case BorderStyle.Fixed3D:
                        createParams.ExStyle |= 0x00000200; // WS_EX_CLIENTEDGE
                        return createParams;
                }
                return createParams;
            }
        }

        [Category("Appearance"), DefaultValue(BorderStyle.None)]
        public BorderStyle BorderStyle
        {
            get
            {
                return _borderStyle;
            }
            set
            {
                if (_borderStyle != value)
                {
                    if (!Enum.IsDefined(typeof(BorderStyle), _borderStyle))
                    {
                        throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
                    }
                    _borderStyle = value;
                    if (!DesignMode)
                        base.UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Gets the window pane.
        /// </summary>
        /// <value>The window pane.</value>
        [CLSCompliant(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IVsWindowPane WindowPane
        {
            get
            {
                if (_nativeWindow != null)
                    return _nativeWindow.WindowPane;
                else
                    return null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
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
            Keys key = (keyData & ~Keys.Modifiers);

            switch (key)
            {
                case Keys.Tab:
                    {
                        if (_readOnly)
                            return false; // Allow using tab for dialog navigation

                        Keys mods = keyData & Keys.Modifiers;
                        if ((mods & (Keys.Control | Keys.Alt)) != 0)
                        {
                            if ((mods & (Keys.Control | Keys.Alt)) == Keys.Control && TopLevelControl is Form)
                            {
                                Control c = this;
                                bool found = false;

                                bool forward = (mods & Keys.Shift) == 0;

                                while (!found && c != null)
                                {
                                    ContainerControl cc = c.GetContainerControl() as ContainerControl;

                                    if (cc == null)
                                        break;

                                    if (cc.SelectNextControl(this, forward, true, true, false))
                                    {
                                        found = true;
                                    }

                                    c = cc;
                                }

                                if (!found)
                                {
                                    ContainerControl cc = c.TopLevelControl as ContainerControl;

                                    if (cc != null)
                                        cc.SelectNextControl(this, forward, true, true, true);
                                }
                            }
                            return false;
                        }
                    }
                    break;
                case Keys.Escape:
                    Form f = TopLevelControl as Form;

                    if (f != null && f.Modal)
                        return false; // Escape should close a dialog
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

        #endregion

        string _text;

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                if (_nativeWindow != null)
                    return _text = _nativeWindow.Text;
                else
                    return _text;
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
        [CLSCompliant(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IVsFindTarget FindTarget
        {
            get
            {
                if (_nativeWindow != null)
                    return _nativeWindow.FindTarget;

                return null;
            }
        }
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
        private IntPtr editorHwnd;

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
                ErrorHandler.ThrowOnFailure(lines.GetLastLineIndex(out endLine, out endIndex));
                ErrorHandler.ThrowOnFailure(lines.GetLineText(0, 0, endLine, endIndex, out text));
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

                ErrorHandler.ThrowOnFailure(lines.GetLastLineIndex(out endLine, out endIndex));

                IntPtr pText = Marshal.StringToCoTaskMemUni(value);
                try
                {
                    if (_ro)
                        InternalSetReadOnly(false);

                    ErrorHandler.ThrowOnFailure(lines.ReloadLines(0, 0, endLine, endIndex, pText, value.Length, null));
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pText);

                    if (_ro)
                        InternalSetReadOnly(true);
                }

                ErrorHandler.ThrowOnFailure(_textView.SetCaretPos(0, 0)); // Move cursor to 0,0
                ErrorHandler.ThrowOnFailure(_textView.SetScrollPosition(0, 0)); // Scroll horizontally
                ErrorHandler.ThrowOnFailure(_textView.SetScrollPosition(1, 0)); // Scroll vertically     
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
                return ErrorHandler.Succeeded(lines.ReplaceLines(s[0].Y, s[0].X, s[1].Y, s[1].X, pText, text.Length, null));
            }
            finally
            {
                Marshal.FreeCoTaskMem(pText);
            }


        }

        Point[] GetSelection()
        {
            int y1, x1, y2, x2;
            if (!ErrorHandler.Succeeded(_textView.GetSelection(out y1, out x1, out y2, out x2)))
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

            if (!ErrorHandler.Succeeded(_textView.GetCaretPos(out y, out x)))
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
                                if (ErrorHandler.Succeeded(_textBuffer.GetLanguageServiceID(out lid)))
                                {
                                    lp = new LANGPREFERENCES2[1];
                                    lp[0].guidLang = lid;
                                }
                            }

                            if (ErrorHandler.Succeeded(manager.GetUserPreferences2(null, items, lp, null)))
                            {
                                // Only hide the horizontal scrollbar if one would have been visible 
                                if (items[0].fHorzScrollbar != 0 && (lp == null || lp[0].fWordWrap == 0))
                                    height += SystemInformation.HorizontalScrollBarHeight;
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

        #endregion

        #region Methods

        /// <summary>
        /// Used to create Code Window
        /// </summary>
        /// <param name="parentHandle">Parent window handle</param>
        /// <param name="place">The place.</param>
        /// <param name="allowModel">if set to <c>true</c> [allow model].</param>
        /// <param name="codeWindow">Represents a multiple-document interface (MDI) child that contains one or more code views.</param>
        private void CreateCodeWindow(IntPtr parentHandle, Rectangle place, bool allowModel, Guid? forceLanguageService, out IVsCodeWindow codeWindow)
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

            if (allowModel)
                initViewFlags |= (uint)TextViewInitFlags2.VIF_ACTIVEINMODALSTATE;

            IVsCodeWindowEx codeWindowEx = codeWindow as IVsCodeWindowEx;
            ErrorHandler.ThrowOnFailure(codeWindowEx.Initialize((uint)_codewindowbehaviorflags.CWB_DISABLEDROPDOWNBAR |
                (uint)_codewindowbehaviorflags.CWB_DISABLESPLITTER,
                0, null, null, initViewFlags, initView));

            // set buffer
            _textBuffer = CreateLocalInstance<IVsTextBuffer>(typeof(VsTextBufferClass), _serviceProvider);

            if (forceLanguageService.HasValue)
            {
                Guid languageService = forceLanguageService.Value;

                ErrorHandler.ThrowOnFailure(_textBuffer.SetLanguageServiceID(ref languageService));
            }

            ErrorHandler.ThrowOnFailure(codeWindow.SetBuffer((IVsTextLines)_textBuffer));

            // create pane window
            _windowPane = (IVsWindowPane)codeWindow;
            ErrorHandler.ThrowOnFailure(_windowPane.SetSite(_serviceProvider));

            ErrorHandler.ThrowOnFailure(_windowPane.CreatePaneWindow(parentHandle, place.X, place.Y, place.Width, place.Height, out editorHwnd));

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
                    ErrorHandler.ThrowOnFailure(frame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref CMDUIGUID_TextEditor));
                }
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
        public void Init(bool allowModal, Guid? forceLanguageService)
        {
            //Create window            
            CreateCodeWindow(_container.Handle, _container.ClientRectangle, allowModal, forceLanguageService, out _codeWindow);
            commandTarget = _codeWindow as IOleCommandTarget;

            IVsTextView textView;
            ErrorHandler.ThrowOnFailure(_codeWindow.GetPrimaryView(out textView));

            _languageService = forceLanguageService;
            _textView = textView;
            NativeMethods.ShowWindow(editorHwnd, 4); // 4 = SW_SHOWNOACTIVATE

            HookEvents(true);
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
            if (ErrorHandler.Succeeded(_textBuffer.GetStateFlags(out state)))
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
            {
                return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
            }
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
            {
                return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
            }
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

        static class NativeMethods
        {
            [DllImport("user32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern IntPtr SetFocus(IntPtr hWnd);

            [DllImport("user32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.U1)]
            internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

            [DllImport("user32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.U1)]
            internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            //internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            internal static extern bool IsWindow(IntPtr hWnd);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            internal static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            internal static extern IntPtr GetDesktopWindow();
        }

        internal void ReplaceContents(string pathToReplaceWith)
        {
            IVsTextBuffer tempBuffer = CreateLocalInstance<IVsTextBuffer>(typeof(VsTextBufferClass), _serviceProvider);
            try
            {
                IVsPersistDocData2 tempDocData = (IVsPersistDocData2)tempBuffer;
                tempDocData.LoadDocData(pathToReplaceWith);

                IVsTextStream tempStream = (IVsTextStream)tempBuffer;

                int size;
                ErrorHandler.ThrowOnFailure(tempStream.GetSize(out size));

                IntPtr buffer = Marshal.AllocCoTaskMem((size + 1) * sizeof(char));
                try
                {
                    if (_ro)
                        InternalSetReadOnly(false);
                    ErrorHandler.ThrowOnFailure(tempStream.GetStream(0, size, buffer));

                    IVsTextStream destStream = (IVsTextStream)_textBuffer;
                    int oldDestSize;
                    ErrorHandler.ThrowOnFailure(destStream.GetSize(out oldDestSize));

                    destStream.ReplaceStream(0, oldDestSize, buffer, size);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(buffer);
                    Marshal.ReleaseComObject(tempBuffer);

                    if (_ro)
                        InternalSetReadOnly(true);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(tempBuffer);
            }
        }

        internal void LoadFile(string path)
        {
            IVsPersistDocData2 docData = (IVsPersistDocData2)_textBuffer;
            ErrorHandler.ThrowOnFailure(docData.LoadDocData(path));
        }

        internal int LineHeight
        {
            get
            {
                int height;
                ErrorHandler.ThrowOnFailure(_textView.GetLineHeight(out height));
                return height;
            }
        }

        void IVsTextViewEvents.OnChangeCaretLine(IVsTextView pView, int iNewLine, int iOldLine)
        {
        }


        /// <summary>
        /// Occurs when the text view scrolled
        /// </summary>
        public event EventHandler<TextViewScrollEventArgs> Scroll;
        void IVsTextViewEvents.OnChangeScrollInfo(IVsTextView pView, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
        {
            if (Scroll != null)
            {
                ScrollOrientation orientation = iBar == 1 ? ScrollOrientation.VerticalScroll : ScrollOrientation.HorizontalScroll;
                TextViewScrollEventArgs ea = new TextViewScrollEventArgs(orientation, iMinUnit, iMaxUnits, iVisibleUnits, iFirstVisibleUnit);

                Scroll(this, ea);
            }
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
    }

    public class TextViewScrollEventArgs : EventArgs
    {

        readonly ScrollOrientation _orientation;
        readonly int _iMinUnit;
        readonly int _iMaxUnits;
        readonly int _iVisibleUnits;
        readonly int _iFirstVisibleUnit;

        public TextViewScrollEventArgs(ScrollOrientation orientiation, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
        {
            _orientation = orientiation;

            _iMinUnit = iMinUnit;
            _iMaxUnits = iMaxUnits;
            _iVisibleUnits = iVisibleUnits;
            _iFirstVisibleUnit = iFirstVisibleUnit;
        }

        public ScrollOrientation Orientation
        {
            get { return _orientation; }
        }

        public int MinUnit
        {
            get { return _iMinUnit; }
        }

        public int MaxUnit
        {
            get { return _iMaxUnits; }
        }

        public int VisibleUnits
        {
            get { return _iVisibleUnits; }
        }

        public int FirstVisibleUnit
        {
            get { return _iFirstVisibleUnit; }
        }
    }
}

