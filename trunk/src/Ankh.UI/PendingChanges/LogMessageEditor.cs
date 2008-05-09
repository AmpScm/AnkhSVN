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
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using System.ComponentModel;
using System.Collections.Generic;

namespace Ankh.UI.PendingChanges
{
    /// <summary>
    /// This class is used to implement CodeEditorUserControl
    /// </summary>
    /// <seealso cref="UserControl"/>
    public partial class LogMessageEditor : UserControl
    {
        IAnkhServiceProvider _context;
        private CodeEditorNativeWindow codeEditorNativeWindow;
        IntPtr _hwndTop;
        bool _fixUI;

        #region Methods

        /// <summary>
        /// Creation and initialization of <see cref="CodeEditorNativeWindow"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Init(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            IOleServiceProvider serviceProvider = context.GetService<IOleServiceProvider>();
            codeEditorNativeWindow = new CodeEditorNativeWindow(_context, this);
            codeEditorNativeWindow.Init();
            codeEditorNativeWindow.Area = this.ClientRectangle;

            _fixUI = true; // Fix the font issues in the next size changed            
        }

        [CLSCompliant(false)]
        public IOleCommandTarget CommandTarget
        {
            get { return codeEditorNativeWindow; }
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
            if (_fixUI)
                FixUI();

            // Since we process each pressed keystroke, the return value is always true.
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

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Overrides OnSizeChanged method to handle OnSizeChanged event
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (codeEditorNativeWindow != null)
            {
                codeEditorNativeWindow.Area = this.ClientRectangle;

                
                    FixUI();
            }
        }

        void FixUI()
        {
            if (!_fixUI)
                return;

            if (_hwndTop == IntPtr.Zero)
            {
                IVsUIShell shell = _context.GetService<IVsUIShell>(typeof(SVsUIShell));

                if (shell != null && ErrorHandler.Succeeded(shell.GetDialogOwnerHwnd(out _hwndTop)))
                {
                    if (!CodeEditorNativeWindow.NativeMethods.IsWindow(_hwndTop) ||
                        (_hwndTop == CodeEditorNativeWindow.NativeMethods.GetDesktopWindow()))
                    {
                        // For some reason VS gives an invalid window (the desktop) while loading
                        _hwndTop = IntPtr.Zero;
                    }
                }
                else
                    _hwndTop = IntPtr.Zero;

            }

            if (_hwndTop != IntPtr.Zero)
            {
                // Send WM_SYSCOLORCHANGE to the toplevel window to fix the font in the editor :(
                CodeEditorNativeWindow.NativeMethods.PostMessage(_hwndTop, 21, IntPtr.Zero, IntPtr.Zero);
                _fixUI = false;
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (_fixUI && codeEditorNativeWindow != null && Visible)
                FixUI();
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
    internal class CodeEditorNativeWindow : NativeWindow, IOleCommandTarget, IDisposable
    {
        #region Fields

        readonly Control _container;
        readonly IAnkhServiceProvider _context;
        readonly IOleServiceProvider _serviceProvider;

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
        public Rectangle Area
        {
            set
            {
                if (editorHwnd != IntPtr.Zero)
                {
                    NativeMethods.SetWindowPos(editorHwnd, IntPtr.Zero, value.X, value.Y,
                        value.Width, value.Height, 0x16); // 0x16 = SWP_NOACTIVATE | SWP_NOZORDER | SWP_NOMOVE
                }
                if (commandHwnd != IntPtr.Zero)
                {
                    NativeMethods.SetWindowPos(this.commandHwnd, IntPtr.Zero, value.X, value.Y,
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
        /// <param name="codeWindow">Represents a multiple-document interface (MDI) child that contains one or more code views.</param>
        private void CreateCodeWindow(IntPtr parentHandle, Rectangle place, out IVsCodeWindow codeWindow)
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

            IVsCodeWindowEx codeWindowEx = codeWindow as IVsCodeWindowEx;
            int hr = codeWindowEx.Initialize((uint)_codewindowbehaviorflags.CWB_DISABLEDROPDOWNBAR |
                (uint)_codewindowbehaviorflags.CWB_DISABLESPLITTER,
                0, null, null,
                (int)TextViewInitFlags.VIF_SET_WIDGET_MARGIN |
                (int)TextViewInitFlags.VIF_SET_SELECTION_MARGIN |
                (int)TextViewInitFlags.VIF_VSCROLL |
                (int)TextViewInitFlags2.VIF_SUPPRESSBORDER |
                (int)TextViewInitFlags2.VIF_SUPPRESS_STATUS_BAR_UPDATE |
                (int)TextViewInitFlags2.VIF_SUPPRESSTRACKCHANGES,
                initView);

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
            IVsWindowPane windowPane = (IVsWindowPane)codeWindow;

            hr = windowPane.SetSite(_serviceProvider);
            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            hr = windowPane.CreatePaneWindow(parentHandle, place.X, place.Y, place.Width, place.Height, out editorHwnd);
            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
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

            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

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
        public void Init()
        {
            //Create window            
            CreateCodeWindow(_container.Handle, _container.ClientRectangle, out codeWindow);
            commandTarget = codeWindow as IOleCommandTarget;

            IVsTextView textView;
            int hr = codeWindow.GetPrimaryView(out textView);

            if (!ErrorHandler.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            _textView = textView;
            commandHwnd = textView.GetWindowHandle();

            //Assign a handle to this window
            AssignHandle(commandHwnd);
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

        /// <summary>
        /// Releases the handle, previously assigned in Init method
        /// </summary>
        public override void DestroyHandle()
        {
            ReleaseHandle();
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

        #region IOleCommandTarget Members
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
            if (_container.ContainsFocus)
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
            if (_container.ContainsFocus)
                return commandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            else
            {
                return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
            }
        }

        #endregion


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

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            internal static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            internal static extern bool IsWindow(IntPtr hWnd);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            internal static extern IntPtr GetDesktopWindow();
        }
    }
}
