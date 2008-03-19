using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Ankh.UI;
using Ankh.UI.Services;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VSPackage
{   
    // We define the toolwindows here. We implement them as some kind of
    // .Net control hosted in this container. This container makes sure
    // user settings are persisted, etc.
    [ProvideToolWindow(typeof(WorkingCopyExplorerToolWindow), Style=VsDockStyle.Float, Transient=false, Width=600, Height=300)]
    [ProvideToolWindow(typeof(RepositoryExplorerToolWindow), Style=VsDockStyle.Float, Transient=false, Width=600, Height=300)]
    [ProvideToolWindow(typeof(PendingChangesToolWindow), Style=VsDockStyle.Linked, Orientation=ToolWindowOrientation.Bottom, Transient=false)]
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

    class AnkhToolWindowSite : IAnkhToolWindowSite
    {
        readonly ToolWindowPane _pane;
        Container _container;

        public AnkhToolWindowSite(ToolWindowPane pane)
        {
            if (pane == null)
                throw new ArgumentNullException("pane");

            _pane = pane;
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
            set {}
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            IServiceProvider paneSp = _pane;

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

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        #endregion
    }

    public class AnkhToolWindowPane : ToolWindowPane
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

            if (Control != null)
                Control.Site = _site;
        }

        public override IWin32Window Window
        {
            get { return _control; }
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
            ((AnkhSvnPackage)Package).AnkhContext.UIShell.WorkingCopyExplorer = Control as WorkingCopyExplorerControl;
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
            ((AnkhSvnPackage)Package).AnkhContext.UIShell.RepositoryExplorer = Control as RepositoryExplorerControl;
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

            this.ToolBar = new CommandID(AnkhId.CommandSetGuid, (int)AnkhCommandMenu.PendingChangesToolBar);
            this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            Control = new Ankh.UI.PendingChanges.PendingChangesToolControl();
                //new CommitDialog();
        }

        public override void OnToolBarAdded()
        {
            base.OnToolBarAdded();
        }
    }
}
