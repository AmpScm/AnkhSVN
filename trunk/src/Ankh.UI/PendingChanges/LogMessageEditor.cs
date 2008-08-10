//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System;
using System.Collections.Generic;
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
using IServiceProvider = System.IServiceProvider;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using System.Security.Permissions;



namespace Ankh.UI.PendingChanges
{
    /// <summary>
    /// This class is used to implement CodeEditorUserControl
    /// </summary>
    /// <seealso cref="UserControl"/>
    public partial class LogMessageEditor : Control
    {
        IAnkhServiceProvider _context;
        private CodeEditorNativeWindow codeEditorNativeWindow;
        BorderStyle _borderStyle;
        bool _fixUI;

        public LogMessageEditor()
        {
        }

        public LogMessageEditor(IContainer container)
            : this()
        {
            container.Add(this);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            Control c = TopLevelControl;

            VSContainerForm ownerForm = c as VSContainerForm;
            if (ownerForm == null || !ownerForm.Modal)
                return;

            if (CommandTarget == null)
            {
                Init(ownerForm.Context, true);
            }

            IAnkhVSContainerForm cf = ownerForm;

            cf.AddCommandTarget(CommandTarget);
            cf.AddWindowPane(WindowPane);
            cf.ContainerMode |= VSContainerMode.TranslateKeys | VSContainerMode.UseTextEditorScope;
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
            codeEditorNativeWindow = new CodeEditorNativeWindow(_context, this);
            codeEditorNativeWindow.Init(allowModal);
            codeEditorNativeWindow.Size = Size;

            _fixUI = true; // Fix the font issues in the next size changed            
        }

        [CLSCompliant(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IOleCommandTarget CommandTarget
        {
            get { return codeEditorNativeWindow; }
        }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 65536;
                createParams.ExStyle &= -513;
                createParams.Style &= -8388609;
                switch (_borderStyle)
                {
                    case BorderStyle.FixedSingle:
                        createParams.Style |= 8388608;
                        return createParams;

                    case BorderStyle.Fixed3D:
                        createParams.ExStyle |= 512;
                        return createParams;
                }
                return createParams;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if(!DesignMode && !e.ClipRectangle.IsEmpty)
            {
                _fixUI = true;
                ForceUIUpdate();
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
            get { return codeEditorNativeWindow.WindowPane; }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (codeEditorNativeWindow != null)
                    {
                        codeEditorNativeWindow.Dispose();
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
            FixUI();

            Keys key = (keyData & ~Keys.Modifiers);

            switch (key)
            {
                case Keys.Tab:
                    {
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
            if (codeEditorNativeWindow != null)
            {
                codeEditorNativeWindow.Focus();
            }
        }

        /// <summary>
        /// Overrides OnSizeChanged method to handle OnSizeChanged event
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            ForceUIUpdate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.LocationChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            ForceUIUpdate();
        }

        public void ForceUIUpdate()
        {
            if (codeEditorNativeWindow != null)
            {
                codeEditorNativeWindow.Size = ClientRectangle.Size;

                FixUI();
            }
        }

        void FixUI()
        {
            if (!_fixUI || _context == null)
                return;

            IntPtr hwndTop;

            IVsUIShell shell = _context.GetService<IVsUIShell>(typeof(SVsUIShell));

            if (shell != null && ErrorHandler.Succeeded(shell.GetDialogOwnerHwnd(out hwndTop)))
            {
                if (!CodeEditorNativeWindow.NativeMethods.IsWindow(hwndTop) ||
                    (hwndTop == CodeEditorNativeWindow.NativeMethods.GetDesktopWindow()))
                {
                    // For some reason VS gives an invalid window (the desktop) while loading
                    return;
                }

                // Send WM_SYSCOLORCHANGE to the toplevel window to fix the font in the editor :(
                CodeEditorNativeWindow.NativeMethods.PostMessage(hwndTop, 21, IntPtr.Zero, IntPtr.Zero);
                _fixUI = false;
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
                ForceUIUpdate();
        }

        #endregion

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                if (codeEditorNativeWindow != null)
                    return codeEditorNativeWindow.Text;
                else
                    return null;
            }
            set
            {
                if (codeEditorNativeWindow != null)
                    codeEditorNativeWindow.Text = value;
            }
        }

        /// <summary>
        /// Pastes the specified text at the current location
        /// </summary>
        /// <param name="text">The text.</param>
        public void PasteText(string text)
        {
            codeEditorNativeWindow.PasteText(text);
        }
    }

    /// <summary>
    /// This class inherits from NativeWindow class, that provides a low-level encapsulation of a window handle and a window procedure
    /// </summary>
    /// <seealso cref="NativeWindow"/>
    internal class CodeEditorNativeWindow : IOleCommandTarget, IDisposable
    {
        #region Fields

        readonly Control _container;
        readonly IAnkhServiceProvider _context;
        readonly IOleServiceProvider _serviceProvider;
        IVsWindowPane _windowPane;

        /// <summary>
        /// Editor window handle
        /// </summary>
        private IntPtr editorHwnd;

        /// <summary>
        /// Command window handle
        /// </summary>
        private IntPtr commandHwnd;

        /// <summary>
        /// The IOleCommandTarget interface enables objects and their containers to dispatch commands to each other.
        /// For example, an object's toolbars may contain buttons for commands such as Print, Print Preview, Save, New, and Zoom. 
        /// </summary>
        private IOleCommandTarget commandTarget;

        /// <summary>
        /// Reference to VsCodeWindow object
        /// </summary>
        private IVsCodeWindow codeWindow;

        IVsTextBuffer textBuffer;
        IVsTextView _textView;

        #endregion

        public CodeEditorNativeWindow(IAnkhServiceProvider context, Control container)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (container == null)
                throw new ArgumentNullException("container");

            _context = context;
            _container = container;
            _serviceProvider = context.GetService<IOleServiceProvider>();
        }

        public string Text
        {
            get
            {
                if (textBuffer == null)
                    return null;

                IVsTextLines lines = textBuffer as IVsTextLines;

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
                if (textBuffer == null)
                    return;

                if (string.IsNullOrEmpty(value))
                    value = "";

                IVsTextLines lines = textBuffer as IVsTextLines;

                if (lines == null)
                    return;

                int endLine, endIndex;

                ErrorHandler.ThrowOnFailure(lines.GetLastLineIndex(out endLine, out endIndex));

                IntPtr pText = Marshal.StringToCoTaskMemUni(value);
                try
                {
                    ErrorHandler.ThrowOnFailure(lines.ReloadLines(0, 0, endLine, endIndex, pText, value.Length, null));
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pText);
                }
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

            IVsTextLines lines = textBuffer as IVsTextLines;

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
                    NativeMethods.SetWindowPos(editorHwnd, IntPtr.Zero, 0, 0,
                        value.Width, value.Height, 0x16); // 0x16 = SWP_NOACTIVATE | SWP_NOZORDER | SWP_NOMOVE
                }
                if (commandHwnd != IntPtr.Zero)
                {
                    NativeMethods.SetWindowPos(this.commandHwnd, IntPtr.Zero, 0, 0,
                        value.Width, value.Height, 0x16); // 0x16 = SWP_NOACTIVATE | SWP_NOZORDER | SWP_NOMOVE
                }
            }
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
        private void CreateCodeWindow(IntPtr parentHandle, Rectangle place, bool allowModel, out IVsCodeWindow codeWindow)
        {
            ILocalRegistry localRegistry = QueryService<ILocalRegistry>(typeof(SLocalRegistry));

            // create code window
            Guid guidVsCodeWindow = typeof(VsCodeWindowClass).GUID;
            codeWindow = CreateObject(localRegistry, guidVsCodeWindow, typeof(IVsCodeWindow).GUID) as IVsCodeWindow;

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
                (uint)TextViewInitFlags.VIF_VSCROLL |
                (uint)TextViewInitFlags2.VIF_SUPPRESSBORDER |
                (uint)TextViewInitFlags2.VIF_SUPPRESS_STATUS_BAR_UPDATE |
                (uint)TextViewInitFlags2.VIF_SUPPRESSTRACKCHANGES;

            if (allowModel)
                initViewFlags |= (uint)TextViewInitFlags2.VIF_ACTIVEINMODALSTATE;

            IVsCodeWindowEx codeWindowEx = codeWindow as IVsCodeWindowEx;
            int hr = codeWindowEx.Initialize((uint)_codewindowbehaviorflags.CWB_DISABLEDROPDOWNBAR |
                (uint)_codewindowbehaviorflags.CWB_DISABLESPLITTER,
                0, null, null, initViewFlags, initView);

            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            // set buffer
            Guid guidVsTextBuffer = typeof(VsTextBufferClass).GUID;
            textBuffer = (IVsTextBuffer)CreateObject(localRegistry, guidVsTextBuffer, typeof(IVsTextBuffer).GUID);
            Guid CLSID_LogMessageService = typeof(LogMessageLanguageService).GUID;

            hr = textBuffer.SetLanguageServiceID(ref CLSID_LogMessageService);
            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            hr = codeWindow.SetBuffer((IVsTextLines)textBuffer);
            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            // create pane window
            _windowPane = (IVsWindowPane)codeWindow;

            hr = _windowPane.SetSite(_serviceProvider);
            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            hr = _windowPane.CreatePaneWindow(parentHandle, place.X, place.Y, place.Width, place.Height, out editorHwnd);
            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

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

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="localRegistry">Establishes a locally-registered COM object relative to the local Visual Studio registry hive</param>
        /// <param name="clsid">GUID if object to be created</param>
        /// <param name="iid">GUID assotiated with specified System.Type</param>
        /// <returns>An object</returns>
        private object CreateObject(ILocalRegistry localRegistry, Guid clsid, Guid iid)
        {
            object objectInstance;
            IntPtr unknown = IntPtr.Zero;

            int hr = localRegistry.CreateInstance(clsid, null, ref iid, (uint)CLSCTX.CLSCTX_INPROC_SERVER, out unknown);

            Marshal.ThrowExceptionForHR(hr);

            try
            {
                objectInstance = Marshal.GetObjectForIUnknown(unknown);
            }
            finally
            {
                if (unknown != IntPtr.Zero)
                {
                    Marshal.Release(unknown);
                }
            }

            // Try to site object instance
            IObjectWithSite objectWithSite = objectInstance as IObjectWithSite;
            if (objectWithSite != null)
                objectWithSite.SetSite(_serviceProvider);

            return objectInstance;
        }

        /// <summary>
        /// Sets focus to Editor's Window
        /// </summary>
        public void Focus()
        {
            NativeMethods.SetFocus(editorHwnd);
        }

        /// <summary>
        /// Window initialization
        /// </summary>
        /// <param name="serviceProvider">IOleServiceProvider</param>
        /// <param name="parent">Control, that can be used to create other controls</param>
        public void Init(bool allowModal)
        {
            //Create window            
            CreateCodeWindow(_container.Handle, _container.ClientRectangle, allowModal, out codeWindow);
            commandTarget = codeWindow as IOleCommandTarget;

            IVsTextView textView;
            int hr = codeWindow.GetPrimaryView(out textView);

            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            _textView = textView;
            commandHwnd = textView.GetWindowHandle();

            NativeMethods.ShowWindow(editorHwnd, 4); // 4 = SW_SHOWNOACTIVATE
        }

        /// <summary>
        ///  This method is used to get service of specified type
        /// </summary>
        /// <typeparam name="InterfaceType">A name of the interface which the caller wishes to receive for the service</typeparam>
        /// <param name="serviceType">A name of the requested service</param>
        /// <returns></returns>
        private InterfaceType QueryService<InterfaceType>(Type serviceType)
            where InterfaceType : class
        {
            Guid serviceGuid = serviceType.GUID;
            Guid interfaceGuid = typeof(InterfaceType).GUID;

            IntPtr unknown = IntPtr.Zero;

            InterfaceType service = null;

            int hr = _serviceProvider.QueryService(ref serviceGuid, ref interfaceGuid, out unknown);

            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            try
            {
                service = (InterfaceType)Marshal.GetObjectForIUnknown(unknown);
            }
            finally
            {
                if (unknown != IntPtr.Zero)
                {
                    Marshal.Release(unknown);
                }
            }

            return service;
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Releases all resources, other than memory, used by the CodeEditorUserControl
        /// </summary>
        public void Dispose()
        {
            if (codeWindow != null)
            {
                codeWindow.Close();
                codeWindow = null;
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

        internal static class NativeMethods
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
    }
}
