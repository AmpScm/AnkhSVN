using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;

using Ankh.Ids;
using Ankh.Commands;
using Ankh.UI;
using Ankh.Selection;
using Ankh.UI.SvnLog;
using Ankh.WorkingCopyExplorer;
using System.Windows.Forms.Design;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.VSPackage
{
    // We define the toolwindows here. We implement them as some kind of
    // .Net control hosted in this container. This container makes sure
    // user settings are persisted, etc.
    [ProvideToolWindow(typeof(WorkingCopyExplorerToolWindow), Style = VsDockStyle.Float, Transient = false, Width = 600, Height = 300)]
    [ProvideToolWindow(typeof(RepositoryExplorerToolWindow), Style = VsDockStyle.Float, Transient = false, Width = 600, Height = 300)]
    [ProvideToolWindow(typeof(PendingChangesToolWindow), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom, Transient = false, Window = ToolWindowGuids80.Outputwindow)]
    [ProvideToolWindow(typeof(LogToolWindow), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom, Transient = false)]
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
    }

    class AnkhToolWindowSite : IAnkhToolWindowSite, IOleCommandTarget
    {
        readonly AnkhToolWindowPane _pane;
        Container _container;
        string _originalTitle;
        string _title;

        public AnkhToolWindowSite(AnkhToolWindowPane pane)
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

        public IAnkhPackage Package
        {
            get { return (IAnkhPackage)_pane.Package; }
        }

        public IVsWindowFrame Frame
        {
            get { return ((IVsWindowFrame)_pane.Frame); }
        }

        public IVsWindowPane Pane
        {
            get { return _pane; }
        }

        public IOleCommandTarget CommandTarget
        {
            get { return _target; }
            set { _target = value; }
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
        AmbientProperties _ambientProperties;
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(AmbientProperties))
            {
                if (_ambientProperties == null)
                {
                    IUIService uis = GetService<System.Windows.Forms.Design.IUIService>();
                    _ambientProperties = new AmbientProperties();
                    _ambientProperties.Font = (System.Drawing.Font)uis.Styles["DialogFont"];
                }
                return _ambientProperties;
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
            return ShowContextMenu(new CommandID(AnkhId.CommandSetGuid, (int)menu), x, y);
        }

        public bool ShowContextMenu(CommandID menu, int x, int y)
        {
            IMenuCommandService mcs = (IMenuCommandService)GetService(typeof(IMenuCommandService));

            try
            {
                mcs.ShowContextMenu(menu, x, y);
            }
            catch (COMException)
            {
                return false;
            }

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

        IOleCommandTarget _target;
        IOleCommandTarget _baseTarget;
        #region IOleCommandTarget Members

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            IOleCommandTarget t = _target ?? _baseTarget ?? (_baseTarget = (IOleCommandTarget)_pane.BaseGetService(typeof(IOleCommandTarget)));

            if (t != null)
                return t.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            else
                return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
        }

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

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            IOleCommandTarget t = _target ?? _baseTarget ?? (_baseTarget = (IOleCommandTarget)_pane.BaseGetService(typeof(IOleCommandTarget)));
            int r = (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;

            if (t != null)
                r = t.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

            if (((r == (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED) || (r == (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP))
                && pguidCmdGroup == AnkhId.CommandSetGuid)
            {
                r = Mapper.QueryStatus(AnkhContext, cCmds, prgCmds, pCmdText);
            }

            return r;
        }

        #endregion
    }

    public class AnkhToolWindowPane : ToolWindowPane, IOleCommandTarget
    {
        readonly AnkhToolWindowSite _site;
        Control _control;

        protected AnkhToolWindowPane()
            : base(null)
        {
            _site = new AnkhToolWindowSite(this);
        }

        protected Control Control
        {
            get { return _control; }
            set
            {
                Debug.Assert(_control == null);
                _control = value;
            }
        }

        public override void OnToolBarAdded()
        {
            base.OnToolBarAdded();

            _site.Load();

            if (Control != null)
                Control.Site = _site;
        }

        public override IWin32Window Window
        {
            get { return _control; }
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
                return _site;
            else
                return base.GetService(serviceType);
        }

        internal object BaseGetService(Type serviceType)
        {
            return base.GetService(serviceType);
        }
    }

    /// <summary>
    /// Wrapper for the WorkingCopyExplorer in the Ankh assembly
    /// </summary>
    [Guid(AnkhId.WorkingCopyExplorerToolWindowId)]
    public class WorkingCopyExplorerToolWindow : AnkhToolWindowPane
    {
        public WorkingCopyExplorerToolWindow()
        {
            this.Caption = "Working Copy Explorer";

            this.BitmapResourceID = 401;
            this.BitmapIndex = 0;

            this.ToolBar = new CommandID(AnkhId.CommandSetGuid, (int)AnkhCommandMenu.WorkingCopyExplorerToolBar);
            this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            // Creating the user control that will be displayed in the window
            Control = new WorkingCopyExplorerControl();
        }

        public override void OnToolBarAdded()
        {
            base.OnToolBarAdded();

            IExplorersShell shell = GetService<Ankh.WorkingCopyExplorer.IExplorersShell>();

            if (shell != null)
            {
                shell.WorkingCopyExplorer = Control as WorkingCopyExplorerControl;
            }
        }
    }

    /// <summary>
    /// Wrapper for the RepositoryExplorer in the Ankh assembly
    /// </summary>
    [Guid(AnkhId.RepositoryExplorerToolWindowId)]
    public class RepositoryExplorerToolWindow : AnkhToolWindowPane
    {
        public RepositoryExplorerToolWindow()
        {
            this.Caption = "Repository Explorer";

            this.BitmapResourceID = 401;
            this.BitmapIndex = 1;

            this.ToolBar = new CommandID(AnkhId.CommandSetGuid, (int)AnkhCommandMenu.RepositoryExplorerToolBar);
            this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            Control = new RepositoryExplorerControl();
        }

        public override void OnToolBarAdded()
        {
            base.OnToolBarAdded();

            IExplorersShell shell = GetService<Ankh.WorkingCopyExplorer.IExplorersShell>();

            if (shell != null)
            {
                shell.RepositoryExplorer = Control as RepositoryExplorerControl;
            }
        }
    }

    /// <summary>
    /// Wrapper for the Commit dialog in the Ankh assembly
    /// </summary>
    [Guid(AnkhId.PendingChangesToolWindowId)]
    public class PendingChangesToolWindow : AnkhToolWindowPane
    {
        public PendingChangesToolWindow()
        {
            this.Caption = "Pending Changes";

            this.BitmapResourceID = 401;
            this.BitmapIndex = 2;

            this.ToolBar = new CommandID(AnkhId.CommandSetGuid, (int)AnkhToolBar.PendingChanges);
            this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            Control = new Ankh.UI.PendingChanges.PendingChangesToolControl();
            //new CommitDialog();
        }

        public override void OnToolBarAdded()
        {
            base.OnToolBarAdded();
        }
    }

    [Guid(AnkhId.LogToolWindowId)]
    public class LogToolWindow : AnkhToolWindowPane
    {
        public LogToolWindow()
        {
            Control = new LogToolControl();
            Caption = "History Viewer";

            ToolBar = new CommandID(AnkhId.CommandSetGuid, (int)AnkhToolBar.LogViewer);
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }
    }
}
