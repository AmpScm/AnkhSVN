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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using ShellPackage = Microsoft.VisualStudio.Shell.Package;

using Ankh.Commands;
using Ankh.Scc.UI;
using Ankh.UI;
using Ankh.UI.DiffWindow;
using Ankh.UI.RepositoryExplorer;
using Ankh.UI.SvnInfoGrid;
using Ankh.UI.SvnLog;
using Ankh.UI.WorkingCopyExplorer;

namespace Ankh.VSPackage
{
    // We define the toolwindows here. We implement them as some kind of
    // .Net control hosted in this container. This container makes sure
    // user settings are persisted, etc.
    [ProvideToolWindow(typeof(WorkingCopyExplorerToolWindow), Style = VsDockStyle.MDI, Transient = true)]
    [ProvideToolWindow(typeof(RepositoryExplorerToolWindow), Style = VsDockStyle.MDI, Transient = true)]
    [ProvideToolWindow(typeof(PendingChangesToolWindow), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom, Transient = false, Window = ToolWindowGuids80.Outputwindow)]
    [ProvideToolWindow(typeof(LogToolWindow), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom, Transient = true)]
    [ProvideToolWindow(typeof(SvnInfoToolWindow), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Right, Transient = false, Window = ToolWindowGuids80.PropertiesWindow)]
    [ProvideToolWindowVisibility(typeof(PendingChangesToolWindow), AnkhId.SccProviderId)]
    [ProvideToolWindowVisibility(typeof(SvnInfoToolWindow), AnkhId.SccProviderId)]
    public partial class AnkhSvnPackage
    {
        public void ShowToolWindow(AnkhToolWindow window)
        {
            ShowToolWindow(window, 0, true);
        }

        Type GetPaneType(AnkhToolWindow toolWindow)
        {
            switch (toolWindow)
            {
                case AnkhToolWindow.RepositoryExplorer:
                    return typeof(RepositoryExplorerToolWindow);
                case AnkhToolWindow.WorkingCopyExplorer:
                    return typeof(WorkingCopyExplorerToolWindow);
                case AnkhToolWindow.PendingChanges:
                    return typeof(PendingChangesToolWindow);
                case AnkhToolWindow.Log:
                    return typeof(LogToolWindow);
                case AnkhToolWindow.SvnInfo:
                    return typeof(SvnInfoToolWindow);
                default:
                    throw new ArgumentOutOfRangeException("toolWindow");
            }
        }

        public void ShowToolWindow(AnkhToolWindow toolWindow, int id, bool create)
        {
            ToolWindowPane pane = FindToolWindow(GetPaneType(toolWindow), id, create);
            
            IVsWindowFrame frame = pane.Frame as IVsWindowFrame;
            if (frame == null)
            {
                throw new InvalidOperationException("FindToolWindow failed");
            }
            // Bring the tool window to the front and give it focus
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
        }

        public void CloseToolWindow(AnkhToolWindow toolWindow, int id, FrameCloseMode close)
        {
            ToolWindowPane pane = FindToolWindow(GetPaneType(toolWindow), id, false);

            if (pane == null)
                return;

            IVsWindowFrame frame = pane.Frame as IVsWindowFrame;
            if (frame == null)
                return;

            ErrorHandler.ThrowOnFailure(frame.CloseFrame((uint) close));
        }

        AmbientProperties _ambientProperties;
        public AmbientProperties AmbientProperties
        {
            get
            {
                if (_ambientProperties == null)
                {
                    IUIService uis = GetService<System.Windows.Forms.Design.IUIService>();
                    _ambientProperties = new AmbientProperties();
                    Font f = (Font)uis.Styles["DialogFont"];

                    _ambientProperties.Font = new Font(f.Name, f.Size);
                }
                return _ambientProperties;
            }
        }
    }

    class AnkhToolWindowHost : ISite, IAnkhToolWindowHost, IOleCommandTarget
    {
        readonly List<IOleCommandTarget> _targets = new List<IOleCommandTarget>();
        readonly AnkhToolWindowPane _pane;
        Container _container;
        string _originalTitle;
        string _title;

        public AnkhToolWindowHost(AnkhToolWindowPane pane)
        {
            if (pane == null)
                throw new ArgumentNullException("pane");

            _pane = pane;
        }

        internal void Load()
        {
            _originalTitle = _title = _pane.Caption;
        }
        #region IAnkhToolWindowSite Members

        IAnkhPackage _package;
        public IAnkhPackage Package
        {
            get
            {
                if (_package != null)
                    return _package;

                if (_pane != null && _pane.Package != null)
                    _package = (IAnkhPackage)_pane.Package;
                else
                    _package = (IAnkhPackage)ShellPackage.GetGlobalService(typeof(IAnkhPackage));

                return _package;
            }
        }

        public IVsWindowFrame Frame
        {
            get { return ((IVsWindowFrame)_pane.Frame); }
        }

        public IVsWindowPane Pane
        {
            get { return _pane; }
        }

        public void AddCommandTarget(IOleCommandTarget target)
        {
            if (!_targets.Contains(target))
                _targets.Add(target);
        }

        public string Title
        {
            get { return _title; }
            set { _pane.Caption = _title = value; }
        }

        public string OriginalTitle
        {
            get { return _originalTitle; }
        }

        #endregion

        #region ISite Members

        public System.ComponentModel.IComponent Component
        {
            get { return _pane.Window as IComponent; }
        }

        public System.ComponentModel.IContainer Container
        {
            get { return _container ?? (_container = new Container()); }
        }

        public bool DesignMode
        {
            get { return false; }
        }

        public string Name
        {
            get { return ToString(); }
            set { }
        }

        #endregion

        #region IServiceProvider Members
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(AmbientProperties))
            {
                if (Package != null)
                    return Package.AmbientProperties;
                else
                    return null;
            }

            System.IServiceProvider paneSp = _pane;

            object ob = paneSp.GetService(serviceType);

            if (ob != null)
                return ob;
            else if (Package != null)
                return Package.GetService(serviceType);
            else
                return null;
        }

        #endregion

        #region IAnkhServiceProvider Members

        [DebuggerStepThrough]
        public T GetService<T>()
            where T : class
        {
            return (T)GetService(typeof(T));
        }

        [DebuggerStepThrough]
        public T GetService<T>(Type serviceType)
            where T : class
        {
            return (T)GetService(serviceType);
        }

        #endregion

        IOleCommandTarget _baseTarget;
        #region IOleCommandTarget Members

        CommandMapper _mapper;

        CommandMapper Mapper
        {
            get { return _mapper ?? (_mapper = GetService<CommandMapper>()); }
        }

        AnkhContext _context;
        AnkhContext AnkhContext
        {
            get { return _context ?? (_context = AnkhContext.Create(this)); }
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            foreach (IOleCommandTarget target in _targets)
            {
                int hr = target.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                if (hr != (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED && hr != (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP)
                    return hr;
            }

            IOleCommandTarget t = _baseTarget ?? (_baseTarget = (IOleCommandTarget)_pane.BaseGetService(typeof(IOleCommandTarget)));

            if (t != null)
                return t.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            else
                return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            foreach (IOleCommandTarget target in _targets)
            {
                int hr = target.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

                if (hr != (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED && hr != (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP)
                    return hr;
            }

            IOleCommandTarget t = _baseTarget ?? (_baseTarget = (IOleCommandTarget)_pane.BaseGetService(typeof(IOleCommandTarget)));

            int r = (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;

            if (t != null)
                r = t.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

            if (ErrorHandler.Succeeded(r))
                return r;
            else
                return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
        }

        #endregion

        #region IAnkhToolWindowSite Members

        public Guid KeyboardContext
        {
            get { return GetGuid(__VSFPROPID.VSFPROPID_InheritKeyBindings); }
            set { SetGuid(__VSFPROPID.VSFPROPID_InheritKeyBindings, value); }
        }

        public Guid CommandContext
        {
            get { return GetGuid(__VSFPROPID.VSFPROPID_CmdUIGuid); }
            set { SetGuid(__VSFPROPID.VSFPROPID_CmdUIGuid, value); }
        }

        private Guid GetGuid(__VSFPROPID id)
        {
            Guid gResult;
            if (ErrorHandler.Succeeded(Frame.GetGuidProperty((int)id, out gResult)))
                return gResult;
            else
                return Guid.Empty;
        }

        private void SetGuid(__VSFPROPID id, Guid value)
        {
            ErrorHandler.ThrowOnFailure(Frame.SetGuidProperty((int)id, ref value));
        }

        #endregion

        #region IAnkhToolWindowHost Members


        public bool IsOnScreen
        {
            get
            {
                IVsWindowFrame frame = Frame;
                if (frame != null)
                {
                    int onScreen;

                    if (ErrorHandler.Succeeded(frame.IsOnScreen(out onScreen)) && onScreen != 0)
                        return true;
                }

                return false;
            }
        }

        #endregion
    }

    class AnkhToolWindowPane : ToolWindowPane, IOleCommandTarget, IVsWindowFrameNotify3, IVsWindowFrameNotify2, IVsWindowFrameNotify
    {
        readonly AnkhToolWindowHost _host;
        AnkhToolWindowControl _control;
        IAnkhToolWindowControl _twControl;
        AnkhToolWindow _toolWindow;
        AnkhToolBar _extraToolBar;

        protected AnkhToolWindowPane()
            : base(null)
        {
            _host = new AnkhToolWindowHost(this);
        }

        public AnkhToolWindow AnkhToolWindow
        {
            get { return _toolWindow; }
            protected set
            {
                _toolWindow = value;
                BitmapResourceID = VSVersion.VS11OrLater ? 701 : 702;
                BitmapIndex = (int)_toolWindow;
            }
        }

        protected AnkhToolWindowControl Control
        {
            get { return _control; }
            set
            {
                Debug.Assert(_control == null);
                _control = value;
                _twControl = (IAnkhToolWindowControl)value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_serviceEvents != null)
                        _serviceEvents.ThemeChanged -= OnThemeChanged;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        bool _created;
        public override IWin32Window Window
        {
            get
            {
                if (!_created)
                {
                    _created = true;
                    if (!_control.IsHandleCreated)
                    {
                        Size sz = _control.Size;
                        _control.Location = new Point(-15000, -15000); // Far, far away
                        _control.Size = new Size(0, 0); // And just 1 pixel

                        _control.Visible = true; // If .Visible = false no window is created!
                        _control.CreateControl();
                        _control.Visible = false; // And hide the window now or we hijack the focus. See issue #507
                        _control.Size = sz;
                    }
                }
                return _control;
            }
        }

        [DebuggerStepThrough]
        protected T GetService<T>()
            where T : class
        {
            return GetService(typeof(T)) as T;
        }

        protected override object GetService(Type serviceType)
        {
            if (serviceType == typeof(IOleCommandTarget))
                return _host;
            else
                return base.GetService(serviceType);
        }

        internal object BaseGetService(Type serviceType)
        {
            return base.GetService(serviceType);
        }

        AnkhServiceEvents _serviceEvents;
        protected override void OnCreate()
        {
            _host.Load();
            //Control.Site = _host;
            Control.ToolWindowHost = _host;
            base.OnCreate();

            _serviceEvents = _host.GetService<AnkhServiceEvents>();
            if (_serviceEvents != null)
                _serviceEvents.ThemeChanged += OnThemeChanged;
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            if (_twControl != null)
                _twControl.OnThemeChanged(e);
        }

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();
        }

        public override void OnToolBarAdded()
        {
            base.OnToolBarAdded();

            if (ExtraToolBarId != AnkhToolBar.None)
            {
                IVsWindowFrame frame = (IVsWindowFrame)this.Frame;

                object obj;

                if (frame != null
                    && ErrorHandler.Succeeded(frame.GetProperty((int)__VSFPROPID.VSFPROPID_ToolbarHost, out obj)))
                {
                    IVsToolWindowToolbarHost host = obj as IVsToolWindowToolbarHost;

                    Guid g = AnkhId.CommandSetGuid;
                    host.AddToolbar((VSTWT_LOCATION)ToolBarLocation, ref g, (uint)ExtraToolBarId);
                }
            }

            _twControl.OnFrameCreated(EventArgs.Empty);
        }

        #region IVsWindowFrameNotify* Members

        public int OnClose(ref uint pgrfSaveOptions)
        {
            _twControl.OnFrameClose(EventArgs.Empty);

            return VSConstants.S_OK;
        }

        public int OnDockableChange(int fDockable, int x, int y, int w, int h)
        {
            _twControl.OnFrameDockableChanged(new FrameEventArgs(fDockable != 0, new Rectangle(x, y, w, h), (__FRAMESHOW)(-1)));

            return VSConstants.S_OK;
        }

        public int OnMove(int x, int y, int w, int h)
        {
            _twControl.OnFrameMove(new FrameEventArgs(false, new Rectangle(x, y, w, h), (__FRAMESHOW)(-1)));

            return VSConstants.S_OK;
        }

        public int OnShow(int fShow)
        {
            _twControl.OnFrameShow(new FrameEventArgs(false, Rectangle.Empty, (__FRAMESHOW)fShow));

            return VSConstants.S_OK;
        }

        public int OnSize(int x, int y, int w, int h)
        {
            _twControl.OnFrameSize(new FrameEventArgs(false, new Rectangle(x, y, w, h), (__FRAMESHOW)(-1)));

            return VSConstants.S_OK;
        }

        #endregion

        #region IVsWindowFrameNotify Members

        public int OnDockableChange(int fDockable)
        {
            return OnDockableChange(fDockable, 0, 0, 0, 0);
        }

        public int OnMove()
        {
            return OnMove(0, 0, 0, 0);
        }

        public int OnSize()
        {
            return OnSize(0, 0, 0, 0);
        }

        public AnkhToolBar ToolBarId
        {
            get { return (ToolBar != null) ? (AnkhToolBar)ToolBar.ID : AnkhToolBar.None; }
            set { ToolBar = value != AnkhToolBar.None ? new CommandID(AnkhId.CommandSetGuid, (int)value) : null; }
        }

        public AnkhToolBar ExtraToolBarId
        {
            get { return _extraToolBar; }
            set { _extraToolBar = value; }
        }

        #endregion
    }

    /// <summary>
    /// Wrapper for the WorkingCopyExplorer in the Ankh assembly
    /// </summary>
    [Guid(AnkhId.WorkingCopyExplorerToolWindowId)]
    class WorkingCopyExplorerToolWindow : AnkhToolWindowPane
    {
        public WorkingCopyExplorerToolWindow()
        {
            Caption = Resources.WorkingCopyExplorerToolWindowTitle;
            Control = new WorkingCopyExplorerControl();

            AnkhToolWindow = AnkhToolWindow.WorkingCopyExplorer;

            ToolBarId = (AnkhToolBar)AnkhCommandMenu.WorkingCopyExplorerToolBar;
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }

    /// <summary>
    /// Wrapper for the RepositoryExplorer in the Ankh assembly
    /// </summary>
    [Guid(AnkhId.RepositoryExplorerToolWindowId)]
    class RepositoryExplorerToolWindow : AnkhToolWindowPane
    {
        public RepositoryExplorerToolWindow()
        {
            Caption = Resources.RepositoryExplorerToolWindowTitle;
            Control = new RepositoryExplorerControl();

            AnkhToolWindow = AnkhToolWindow.RepositoryExplorer;

            ToolBarId = (AnkhToolBar)AnkhCommandMenu.RepositoryExplorerToolBar;
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }

    /// <summary>
    /// Wrapper for the Commit dialog in the Ankh assembly
    /// </summary>
    [Guid(AnkhId.PendingChangesToolWindowId)]
    class PendingChangesToolWindow : AnkhToolWindowPane
    {
        public PendingChangesToolWindow()
        {
            Caption = Resources.PendingChangesToolWindowTitle;
            Control = new Ankh.UI.PendingChanges.PendingChangesToolControl();

            AnkhToolWindow = AnkhToolWindow.PendingChanges;

            ToolBarId = AnkhToolBar.PendingChanges;
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }

    [Guid(AnkhId.LogToolWindowId)]
    class LogToolWindow : AnkhToolWindowPane
    {
        public LogToolWindow()
        {
            Caption = Resources.LogToolWindowTitle;
            Control = new LogToolWindowControl();

            AnkhToolWindow = AnkhToolWindow.Log;

            ToolBarId = AnkhToolBar.LogViewer;
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }

    [Guid(AnkhId.SvnInfoToolWindowId)]
    class SvnInfoToolWindow : AnkhToolWindowPane
    {
        public SvnInfoToolWindow()
        {
            Caption = Resources.SubversionInfoToolWindowTitle;
            Control = new SvnInfoGridControl();

            AnkhToolWindow = AnkhToolWindow.SvnInfo;

            ToolBarId = AnkhToolBar.SvnInfoCombo;
            ExtraToolBarId = AnkhToolBar.SvnInfo;
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }
}
