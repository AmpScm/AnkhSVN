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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using ShellPackage = Microsoft.VisualStudio.Shell.Package;

using Ankh.Ids;
using Ankh.Commands;
using Ankh.UI;
using Ankh.UI.SvnLog;
using System.Windows.Forms.Design;
using Ankh.UI.RepositoryExplorer;
using Ankh.UI.WorkingCopyExplorer;
using Ankh.UI.DiffWindow;
using Ankh.Scc.UI;
using Ankh.UI.Annotate;

namespace Ankh.VSPackage
{
    // We define the toolwindows here. We implement them as some kind of
    // .Net control hosted in this container. This container makes sure
    // user settings are persisted, etc.
    [ProvideToolWindow(typeof(WorkingCopyExplorerToolWindow), Style = VsDockStyle.MDI, Transient = true)]
    [ProvideToolWindow(typeof(RepositoryExplorerToolWindow), Style = VsDockStyle.MDI, Transient = true)]
    [ProvideToolWindow(typeof(PendingChangesToolWindow), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom, Transient = false, Window = ToolWindowGuids80.Outputwindow)]
    [ProvideToolWindow(typeof(LogToolWindow), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom, Transient = true)]
    [ProvideToolWindow(typeof(DiffToolWindow), Style = VsDockStyle.Float, Transient = true)]
    [ProvideToolWindow(typeof(MergeToolWindow), Style = VsDockStyle.Float, Transient = true)]
    [ProvideToolWindowVisibility(typeof(PendingChangesToolWindow), AnkhId.SccProviderId)]
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
                case AnkhToolWindow.Diff:
                    return typeof(DiffToolWindow);
                case AnkhToolWindow.Merge:
                    return typeof(MergeToolWindow);
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

        AmbientProperties _ambientProperties;
        public AmbientProperties AmbientProperties
        {
            get
            {
                if (_ambientProperties == null)
                {
                    IUIService uis = GetService<System.Windows.Forms.Design.IUIService>();
                    _ambientProperties = new AmbientProperties();
                    Font f = (Font)((Font)uis.Styles["DialogFont"]);

                    _ambientProperties.Font = new Font(f.FontFamily, f.Size);
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
                return GetService<AnkhSvnPackage>(typeof(IAnkhPackage)).AmbientProperties;
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

        #region IAnkhUISite Members


        public bool ShowContextMenu(AnkhCommandMenu menu, int x, int y)
        {
            IAnkhCommandService cs = GetService<IAnkhCommandService>();

            cs.ShowContextMenu(menu, x, y);

            return true;
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
    }

    class AnkhToolWindowPane : ToolWindowPane, IOleCommandTarget, IVsWindowFrameNotify3, IVsWindowFrameNotify2, IVsWindowFrameNotify
    {
        readonly AnkhToolWindowHost _host;
        AnkhToolWindowControl _control;
        IAnkhToolWindowControl _twControl;
        AnkhToolWindow _toolWindow;
        uint _cookie;

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
                BitmapResourceID = 401;
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

        protected override void OnCreate()
        {
            _host.Load();
            //Control.Site = _host;
            Control.ToolWindowHost = _host;
            base.OnCreate();
        }

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            IVsWindowFrame2 wf2 = Frame as IVsWindowFrame2;

            if (wf2 != null)
            {
                wf2.Advise(this, out _cookie);
            }
        }

        public override void OnToolBarAdded()
        {
            base.OnToolBarAdded();

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
            _twControl.OnFrameDockableChanged(new FrameEventArgs(fDockable != 0, new Rectangle(x, y, w, h), (__FRAMESHOW)0));

            return VSConstants.S_OK;
        }

        public int OnMove(int x, int y, int w, int h)
        {
            _twControl.OnFrameMove(new FrameEventArgs(false, new Rectangle(x, y, w, h), (__FRAMESHOW)0));

            return VSConstants.S_OK;
        }

        public int OnShow(int fShow)
        {
            _twControl.OnFrameShow(new FrameEventArgs(false, Rectangle.Empty, (__FRAMESHOW)fShow));

            return VSConstants.S_OK;
        }

        public int OnSize(int x, int y, int w, int h)
        {
            _twControl.OnFrameSize(new FrameEventArgs(false, new Rectangle(x, y, w, h), (__FRAMESHOW)0));

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
            this.Caption = "Working Copy Explorer";

            AnkhToolWindow = AnkhToolWindow.WorkingCopyExplorer;

            this.ToolBar = new CommandID(AnkhId.CommandSetGuid, (int)AnkhCommandMenu.WorkingCopyExplorerToolBar);
            this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            // Creating the user control that will be displayed in the window
            Control = new WorkingCopyExplorerControl();
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
            this.Caption = "Repository Explorer";

            AnkhToolWindow = AnkhToolWindow.RepositoryExplorer;

            this.ToolBar = new CommandID(AnkhId.CommandSetGuid, (int)AnkhCommandMenu.RepositoryExplorerToolBar);
            this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            Control = new RepositoryExplorerControl();
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
            this.Caption = "Pending Changes";

            AnkhToolWindow = AnkhToolWindow.PendingChanges;

            this.ToolBar = new CommandID(AnkhId.CommandSetGuid, (int)AnkhToolBar.PendingChanges);
            this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            Control = new Ankh.UI.PendingChanges.PendingChangesToolControl();
        }
    }

    [Guid(AnkhId.LogToolWindowId)]
    class LogToolWindow : AnkhToolWindowPane
    {
        public LogToolWindow()
        {
            Control = new LogToolWindowControl();
            Caption = "History Viewer";

            AnkhToolWindow = AnkhToolWindow.Log;

            ToolBar = new CommandID(AnkhId.CommandSetGuid, (int)AnkhToolBar.LogViewer);
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }

    [Guid(AnkhId.DiffToolWindowId)]
    class DiffToolWindow : AnkhToolWindowPane
    {
        public DiffToolWindow()
        {
            Control = new DiffToolWindowControl();
            Caption = "Diff";

            AnkhToolWindow = AnkhToolWindow.Diff;

            //ToolBar = new CommandID(AnkhId.CommandSetGuid, (int)AnkhToolBar.LogViewer);
            //ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }

    [Guid(AnkhId.MergeToolWindowId)]
    class MergeToolWindow : AnkhToolWindowPane
    {
        public MergeToolWindow()
        {
            Control = new MergeToolWindowControl();
            Caption = "Diff";

            AnkhToolWindow = AnkhToolWindow.Merge;
        }
    }
}
