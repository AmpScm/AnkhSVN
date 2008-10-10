using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Ankh.UI.Blame
{
    class EditorHost : Control, IVsTextViewEvents
    {
        IVsTextBuffer _textBuffer;
        IVsCodeWindow _codeWindow;
        IntPtr _hwndCodeWindow;

        uint _textEventsCookie;

        IOleServiceProvider _serviceProvider;
        IAnkhServiceProvider _context;
        BlameToolWindowControl _control;

        static Guid codeWindowClassId = typeof(VsCodeWindowClass).GUID;
        static Guid codeWindowId = typeof(IVsCodeWindow).GUID;
        static Guid textBufferClassId = typeof(VsTextBufferClass).GUID;
        static Guid textLinesId = typeof(IVsTextLines).GUID;
        static Guid textViewEventsId = typeof(IVsTextViewEvents).GUID;

        public EditorHost()
        {
        }

        internal void Init(BlameToolWindowControl control, IAnkhServiceProvider context)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            if (context == null)
                throw new ArgumentNullException("context");

            _control = control;
            _context = context;
            _serviceProvider = GetService<IOleServiceProvider>();

            HostEditor();
        }

        public void LoadFile(string projectFile, string exportedFile)
        {
            IAnkhPackage package = GetService<IAnkhPackage>();
            IVsTextBuffer tempBuffer = (IVsTextBuffer)package.CreateInstance(ref textBufferClassId, ref textLinesId, typeof(IVsTextBuffer));
            ((IObjectWithSite)tempBuffer).SetSite(_serviceProvider);

            IVsPersistDocData2 tempDocData = (IVsPersistDocData2)tempBuffer;
            tempDocData.LoadDocData(exportedFile);

            IVsPersistDocData2 docData = (IVsPersistDocData2)_textBuffer;
            docData.LoadDocData(projectFile);
            

            int size;
            IVsTextStream tempStream = (IVsTextStream)tempBuffer;
            ErrorHandler.ThrowOnFailure(tempStream.GetSize(out size));
            
            IntPtr buffer = Marshal.AllocCoTaskMem((size + 1) * sizeof(char));
            try
            {
                ErrorHandler.ThrowOnFailure(tempStream.GetStream(0, size, buffer));
                
                IVsTextStream destStream = (IVsTextStream)_textBuffer;
                int oldDestSize;
                ErrorHandler.ThrowOnFailure(destStream.GetSize(out oldDestSize));

                destStream.ReplaceStream(0, oldDestSize, buffer, size);
            }
            finally
            {
                Marshal.FreeCoTaskMem(buffer);
            }

            docData.SetDocDataReadOnly(1);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            
                
        }

        T GetService<T>(Type t)
            where T: class
        {
            return _context.GetService<T>(t);
        }
        T GetService<T>()
            where T:class
        {
            return _context.GetService<T>();
        }


     

        private void HostEditor()
        {

            IAnkhPackage package = GetService<IAnkhPackage>();
            _textBuffer = (IVsTextBuffer)package.CreateInstance(ref textBufferClassId, ref textLinesId, typeof(IVsTextBuffer));
            ((IObjectWithSite)_textBuffer).SetSite(_serviceProvider);
            _codeWindow = (IVsCodeWindow)package.CreateInstance(ref codeWindowClassId, ref codeWindowId, typeof(IVsCodeWindow));

            


            INITVIEW[] initviewArray = new INITVIEW[1];
            initviewArray[0].fSelectionMargin = 0;
            initviewArray[0].fWidgetMargin = 0;
            initviewArray[0].fVirtualSpace = 0;
            initviewArray[0].fDragDropMove = 1;
            ((IVsCodeWindowEx)_codeWindow).Initialize(
                (uint)(_codewindowbehaviorflags.CWB_DISABLEDROPDOWNBAR | _codewindowbehaviorflags.CWB_DISABLESPLITTER),
                VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter,
                "",
                "",
                ((uint)(TextViewInitFlags.VIF_SET_WIDGET_MARGIN | 
                    TextViewInitFlags.VIF_SET_SELECTION_MARGIN | 
                    TextViewInitFlags.VIF_SET_VIRTUAL_SPACE |
                    TextViewInitFlags.VIF_SET_DRAGDROPMOVE)) |
                ((uint)(
                    TextViewInitFlags2.VIF_READONLY |
                    TextViewInitFlags2.VIF_SUPPRESSTRACKCHANGES |
                    TextViewInitFlags2.VIF_SUPPRESSBORDER)),
                initviewArray);

            _codeWindow.SetBuffer((IVsTextLines)this._textBuffer);
            IVsWindowPane codeWindow = (IVsWindowPane)_codeWindow;
            codeWindow.SetSite(_serviceProvider);
            codeWindow.CreatePaneWindow(Handle, 0, 0, Width, Height, out _hwndCodeWindow);

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

            HookEvents(true);

        }

        void HookEvents(bool hook)
        {
            if (_codeWindow == null)
                return;
            IVsTextView view;
            _codeWindow.GetPrimaryView(out view);
            IConnectionPointContainer container = view as IConnectionPointContainer;
            if (container != null)
            {
                IConnectionPoint point;
                container.FindConnectionPoint(ref textViewEventsId, out point);
                if (point != null)
                {
                    if (hook)
                    {
                        point.Advise(this, out _textEventsCookie);
                    }
                    else
                    {
                        point.Unadvise(_textEventsCookie);
                        _textEventsCookie = 0;
                    }
                }
            }
        }

        internal int GetLineHeight()
        {
            IVsTextView textView;
            ErrorHandler.ThrowOnFailure(_codeWindow.GetPrimaryView(out textView));

            int height;
            ErrorHandler.ThrowOnFailure(textView.GetLineHeight(out height));

            return height;
        }


        public void OnChangeCaretLine(IVsTextView pView, int iNewLine, int iOldLine)
        {
        }

        public void OnChangeScrollInfo(IVsTextView pView, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
        {
            if(iBar == 1) // Vertical scroll
                _control.NotifyScroll(iMinUnit, iMaxUnits, iVisibleUnits, iFirstVisibleUnit);
        }

        public void OnKillFocus(IVsTextView pView)
        {
        }

        public void OnSetBuffer(IVsTextView pView, IVsTextLines pBuffer)
        {
        }

        public void OnSetFocus(IVsTextView pView)
        {
        }
    }
}
