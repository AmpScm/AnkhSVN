using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.UI;
using System.Windows.Forms;
using System.ComponentModel;

namespace Ankh.VSPackage
{   
    // We define the toolwindows here. We implement them as some kind of
    // .Net control hosted in this container. This container makes sure
    // user settings are persisted, etc.
    [ProvideToolWindow(typeof(WorkingCopyExplorerToolWindow), Style=VsDockStyle.Float, Transient=false)]
    [ProvideToolWindow(typeof(RepositoryExplorerToolWindow), Style=VsDockStyle.Float, Transient=false, Width=200, Height=400)]
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
            get { return _pane.Window as Component; }
        }

        public System.ComponentModel.IContainer Container
        {
            get { return null; }
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
            if (Package != null)
                return Package.GetService(serviceType);
            else
                return null;
        }

        #endregion
    }

    /// <summary>
    /// Wrapper for the WorkingCopyExplorer in the Ankh assembly
    /// </summary>
    [Guid(AnkhId.WorkingCopyExplorerToolWindowId)]
    public class WorkingCopyExplorerToolWindow : ToolWindowPane
    {
        Control _control;

        public WorkingCopyExplorerToolWindow()
            : base(null)
        {
            this.Caption = "Working Copy Explorer";
            
            // Set the image that will appear on the tab of the window frame
			// when docked with another window.
			// The resource ID corresponds to the one defined in Resources.resx
			// while the Index is the offset in the bitmap strip. Each image in
			// the strip is 16x16.
			//this.BitmapResourceID = 301;
			//this.BitmapIndex = 3;

			// Add the toolbar by specifying the Guid/MenuID pair corresponding to
			// the toolbar definition in the vsct file.
			//this.ToolBar = new CommandID(GuidsList.guidClientCmdSet, PkgCmdId.IDM_MyToolbar);
			// Specify that we want the toolbar at the top of the window
			//this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

			// Creating the user control that will be displayed in the window
            _control = new WorkingCopyExplorerControl();
            _control.Site = new AnkhToolWindowSite(this);
        }

        public override IWin32Window Window
        {
            get { return _control; }
        }
    }

    /// <summary>
    /// Wrapper for the RepositoryExplorer in the Ankh assembly
    /// </summary>
    [Guid(AnkhId.RepositoryExplorerToolWindowId)]
    public class RepositoryExplorerToolWindow : ToolWindowPane
    {
        Control _control;
        public RepositoryExplorerToolWindow()
            : base(null)
        {
            this.Caption = "Repository Explorer";

            // Set the image that will appear on the tab of the window frame
            // when docked with another window.
            // The resource ID corresponds to the one defined in Resources.resx
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip is 16x16.
            //this.BitmapResourceID = 301;
            //this.BitmapIndex = 3;

            // Add the toolbar by specifying the Guid/MenuID pair corresponding to
            // the toolbar definition in the vsct file.
            //this.ToolBar = new CommandID(GuidsList.guidClientCmdSet, PkgCmdId.IDM_MyToolbar);
            // Specify that we want the toolbar at the top of the window
            //this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            // Creating the user control that will be displayed in the window
            _control = new RepositoryExplorerControl();
        }

        public override IWin32Window Window
        {
            get { return _control; }
        }
    }

    /// <summary>
    /// Wrapper for the Commit dialog in the Ankh assembly
    /// </summary>
    [Guid(AnkhId.PendingChangesToolWindowId)]
    public class PendingChangesToolWindow : ToolWindowPane
    {
        Control _control;

        public PendingChangesToolWindow()
            : base(null)
        {
            this.Caption = "Pending Changes";
            // Set the image that will appear on the tab of the window frame
            // when docked with another window.
            // The resource ID corresponds to the one defined in Resources.resx
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip is 16x16.
            //this.BitmapResourceID = 301;
            //this.BitmapIndex = 3;

            // Add the toolbar by specifying the Guid/MenuID pair corresponding to
            // the toolbar definition in the vsct file.
            //this.ToolBar = new CommandID(GuidsList.guidClientCmdSet, PkgCmdId.IDM_MyToolbar);
            // Specify that we want the toolbar at the top of the window
            //this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            // Creating the user control that will be displayed in the window
            _control = new CommitDialog();
        }

        public override IWin32Window Window
        {
            get { return _control; }
        }
    }
}
