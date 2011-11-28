// $Id$
using System;
using Ankh.UI;
using EnvDTE;
using System.Diagnostics;
using System.Windows.Forms;
using SH = SHDocVw; 
using System.IO;
using Utils.Win32;

using NSvn.Common;

namespace Ankh
{
	/// <summary>
	/// Summary description for UIShell.
	/// </summary>
	public class UIShell : IUIShell
	{
		public UIShell()
		{
			//
			// TODO: Add constructor logic here
			//
        }
        #region IUIShell Members
        public RepositoryExplorerControl RepositoryExplorer
        {
            get
            { 
                if( this.repositoryExplorerControl == null )
                    this.CreateRepositoryExplorer();
                return this.repositoryExplorerControl;
            }
        }

        public WorkingCopyExplorerControl WorkingCopyExplorer
        {
            get
            {
                if ( this.workingCopyExplorerControl == null )
                {
                    this.CreateWorkingCopyExplorer();
                }
                return this.workingCopyExplorerControl;
            }
        }

        

        public IContext Context
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.context; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.context = value; }
        }

        public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                return this.RepositoryExplorer; 
            }
        }

        public DialogResult QueryWhetherAnkhShouldLoad()
        {
            string nl = Environment.NewLine;
            string msg = "Ankh has detected that the solution file for this solution " + 
                "is in a Subversion working copy." + nl + 
                "Do you want to enable Ankh for this solution?" + nl +
                "(If you select Cancel, Ankh will not be enabled, " + nl +
                "but you will " +
                "be asked this question again the next time you open the solution)";

            //TODO: The UIShell should be responsible for maintaining the hostwindow
            return MessageBox.Show( 
                this.context.HostWindow, msg, "Ankh", 
                MessageBoxButtons.YesNoCancel );
        }

        public void ShowRepositoryExplorer(bool show)
        {
            this.repositoryExplorerWindow.Visible = show;
            if ( show )
                this.EnsureWindowSize( this.repositoryExplorerWindow );

        }

        public void ShowWorkingCopyExplorer( bool show )
        {
            this.workingCopyExplorerWindow.Visible = show;
            if ( show )
                this.EnsureWindowSize( this.workingCopyExplorerWindow );

        }
    
        public void SetRepositoryExplorerSelection(object[] selection)
        {
            this.repositoryExplorerWindow.SetSelectionContainer( ref selection );
        }

        public bool RepositoryExplorerHasFocus()
        {
            return this.Context.DTE.ActiveWindow == this.repositoryExplorerWindow;
        }

        public bool WorkingCopyExplorerHasFocus()
        {
            return this.Context.DTE.ActiveWindow == this.workingCopyExplorerWindow;
        }

        public bool SolutionExplorerHasFocus()
        {
            return this.Context.DTE.ActiveWindow.Type == vsWindowType.vsWindowTypeSolutionExplorer;
        }



        /// <summary>
        /// Shows the commit dialog, blocking until the user hits cancel or commit.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public CommitContext ShowCommitDialogModal( CommitContext ctx )
        {
            if ( this.commitDialogWindow == null ) 
                this.CreateCommitDialog();
            Debug.Assert( this.commitDialog != null );

            this.commitDialog.UrlPaths = ctx.UrlPaths;
            this.commitDialog.CommitItems = ctx.CommitItems;
            this.commitDialog.LogMessageTemplate = ctx.LogMessageTemplate;
            this.commitDialog.KeepLocks = ctx.KeepLocks;

            this.commitDialog.LogMessage = ctx.LogMessage;

            // we want to preserve the original state.
            bool originalVisibility = this.commitDialogWindow.Visible;

            this.commitDialogWindow.Visible = true;
            this.commitDialogModal = true;

            this.EnsureWindowSize( this.commitDialogWindow );

            // Fired when user clicks Commit or Cancel.
            this.commitDialog.Proceed += new EventHandler( this.ProceedCommit );

            // we need the buttons enabled now, since it's pseudo-modal.
            this.commitDialog.ButtonsEnabled = true;
            this.commitDialog.Initialize();

            // run a message pump while the commit dialog's open
            Utils.Win32.Message msg;
            while ( this.commitDialogModal && this.commitDialogWindow.Visible ) 
            {
                if ( Win32.GetMessage( out msg, IntPtr.Zero, 0, 0 ))
                {
                    Win32.TranslateMessage( out msg );
                    Win32.DispatchMessage( out msg );                    
                }
            }

            ctx.LogMessage = this.commitDialog.LogMessage;
            ctx.RawLogMessage = this.commitDialog.RawLogMessage;
            ctx.CommitItems = this.commitDialog.CommitItems;
            ctx.KeepLocks = this.commitDialog.KeepLocks;

            ctx.Cancelled = this.commitDialog.CommitDialogResult == CommitDialogResult.Cancel;

            // restore the pre-modal state.
            this.commitDialog.ButtonsEnabled = false;
            this.commitDialogWindow.Visible = originalVisibility;
            this.commitDialog.CommitItems = new object[]{};

            return ctx;
        }

        public void ToggleCommitDialog( bool show )
        {
            if ( this.commitDialogWindow == null )
                this.CreateCommitDialog();

            Debug.Assert( this.commitDialogWindow != null );

            this.commitDialogWindow.Visible = show;
            this.commitDialog.ButtonsEnabled = this.commitDialogModal;

            if ( show )
                this.EnsureWindowSize( this.commitDialogWindow );
        }



        public void ResetCommitDialog()
        {
            this.commitDialog.Reset();
        }

        /// <summary>
        /// Executes the worker.Work method while displaying a progress dialog.
        /// </summary>
        /// <param name="worker"></param>
        public bool RunWithProgressDialog( IProgressWorker worker, string caption )
        {
            ProgressRunner runner = new ProgressRunner( this.Context, worker );
            runner.Start( caption );     
       
            return !runner.Cancelled;
        }

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogResult ShowMessageBox( string text, string caption, 
            MessageBoxButtons buttons )
        {
            return MessageBox.Show( this.Context.HostWindow, text, caption,
                buttons );
        }

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogResult ShowMessageBox( string text, string caption, 
            MessageBoxButtons buttons, MessageBoxIcon icon )
        {
            return MessageBox.Show( this.Context.HostWindow, text, caption,
                buttons, icon );
        }

        /// <summary>
        /// Display a message box.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogResult ShowMessageBox( string text, string caption,
            MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton )
        {
            return MessageBox.Show( this.Context.HostWindow, text, caption,
                buttons, icon, defaultButton );
        }

        public void DisplayHtml( string caption, string html, bool reuse )
        {
            string htmlFile = Path.GetTempFileName();
            using( StreamWriter w = new StreamWriter( htmlFile, false, System.Text.Encoding.UTF8 ) )
                w.Write( html );

            // the Start Page window is a web browser
            Window browserWindow = context.DTE.Windows.Item( 
                EnvDTE.Constants.vsWindowKindWebBrowser );
            SH.WebBrowser browser = (SH.WebBrowser)browserWindow.Object;

//            if ( !reuse ) 
//                browser = this.NewBrowserWindow( browser );

            // have it show the html
            object url = "file://" + htmlFile;
            object nullObject = null;

            browser.Navigate2( ref url, ref nullObject, ref nullObject,
                ref nullObject, ref nullObject );
            browserWindow.Caption = caption;
            browserWindow.Activate();
        }

        public PathSelectorInfo ShowPathSelector( PathSelectorInfo info )
        {
            using( PathSelector selector = new PathSelector() )
            {
                // to provide information about the paths
                selector.GetPathInfo += new ResolvingPathInfoHandler(GetPathInfo);

                selector.EnableRecursive = info.EnableRecursive;
                selector.Items = info.Items;
                selector.CheckedItems = info.CheckedItems;
                selector.Recursive = info.Recurse == Recurse.Full;
                selector.SingleSelection = info.SingleSelection;
                selector.Caption = info.Caption;

                // do we need go get a revision range?
                if ( info.RevisionStart == null && info.RevisionEnd == null )
                {
                    selector.Options = PathSelectorOptions.NoRevision;
                }
                else if ( info.RevisionEnd == null )
                {
                    selector.RevisionStart = info.RevisionStart;
                    selector.Options = PathSelectorOptions.DisplaySingleRevision;
                }
                else
                {
                    selector.RevisionStart = info.RevisionStart;
                    selector.RevisionEnd = info.RevisionEnd;
                    selector.Options = PathSelectorOptions.DisplayRevisionRange;
                }

                

                // show it
                if ( selector.ShowDialog( this.Context.HostWindow ) == DialogResult.OK )
                {
                    info.CheckedItems = selector.CheckedItems;
                    info.Recurse = selector.Recursive ? Recurse.Full : Recurse.None;
                    info.RevisionStart = selector.RevisionStart;
                    info.RevisionEnd = selector.RevisionEnd;

                    return info;
                }
                else 
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Shows the lock dialog.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public LockDialogInfo ShowLockDialog( LockDialogInfo info )
        {
            using( LockDialog dlg = new LockDialog() )
            {
                dlg.GetPathInfo += new ResolvingPathInfoHandler(GetPathInfo);

                dlg.Items = info.Items;
                dlg.CheckedItems = info.CheckedItems;
                dlg.Message = info.Message;
                dlg.StealLocks = info.StealLocks;
                if ( dlg.ShowDialog( this.Context.HostWindow ) != DialogResult.OK )
                    return null;

                info.CheckedItems = dlg.CheckedItems;
                info.Message = dlg.Message;
                info.StealLocks = dlg.StealLocks;
                return info;
            }
        }

        /// <summary>
        /// Shows the log dialog.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public LogDialogInfo ShowLogDialog( LogDialogInfo info )
        {
            using( LogDialog dlg = new LogDialog() )
            {
                dlg.EnableRecursive = false;
                dlg.Items = info.Items;
                dlg.CheckedItems = info.CheckedItems;
                dlg.Options = PathSelectorOptions.DisplayRevisionRange;
                dlg.RevisionStart = info.RevisionStart;
                dlg.RevisionEnd = info.RevisionEnd;
                dlg.GetPathInfo += new ResolvingPathInfoHandler(GetPathInfo);
                dlg.StopOnCopy = info.StopOnCopy;

                if ( dlg.ShowDialog( this.Context.HostWindow ) != DialogResult.OK )
                    return null;

                info.CheckedItems = dlg.CheckedItems;
                info.StopOnCopy = dlg.StopOnCopy;
				info.RevisionStart = dlg.RevisionStart;
				info.RevisionEnd = dlg.RevisionEnd;
                
                return info;
            }
        }

        public SwitchDialogInfo ShowSwitchDialog( SwitchDialogInfo info )
        {
            using( SwitchDialog dialog = new SwitchDialog() )
            {
                dialog.GetPathInfo += new ResolvingPathInfoHandler(GetUrlPathinfo);
                dialog.Items = info.Items;
                dialog.SingleSelection = true;
                dialog.CheckedItems = info.Items;
                dialog.Options = PathSelectorOptions.DisplaySingleRevision;
                dialog.Recursive = true;

                if ( dialog.ShowDialog(this.Context.HostWindow) != DialogResult.OK )
                    return null;
                
                info.SwitchToUrl = dialog.ToUrl;
                info.Recurse = dialog.Recursive ? Recurse.Full : Recurse.None;
                info.Path = dialog.SelectedPath;
                info.RevisionStart = dialog.RevisionStart;

                return info;
            }
        }

        public string ShowNewDirectoryDialog()
        {
            using( NewDirectoryDialog dlg = new NewDirectoryDialog() )
            {
                if ( dlg.ShowDialog(this.Context.HostWindow) != DialogResult.OK )
                    return null;
                
                return dlg.DirectoryName;
            }
        }

        public RepositoryRootInfo ShowAddRepositoryRootDialog()
        {
            using( AddRepositoryRootDialog dlg = new AddRepositoryRootDialog() )
            {
                if ( dlg.ShowDialog(this.Context.HostWindow) != DialogResult.OK )
                    return null;

                return new RepositoryRootInfo( dlg.Url, dlg.Revision );
            }
        }

        public string ShowAddWorkingCopyExplorerRootDialog()
        {
            using ( AddWorkingCopyExplorerRootDialog dlg = new AddWorkingCopyExplorerRootDialog() )
            {
                if ( dlg.ShowDialog( this.context.HostWindow ) != DialogResult.OK )
                {
                    return null;
                }
                return dlg.NewRoot;
            }
        }

        #endregion

        private void CreateRepositoryExplorer()
        {   
            Debug.WriteLine( "Creating repository explorer", "Ankh" );
            ToolWindowResult result = this.context.DteStrategyFactory.GetToolWindowStrategy().
                CreateToolWindow(typeof(RepositoryExplorerControl),
                "Repository Explorer", REPOSEXPLORERGUID);

            this.repositoryExplorerWindow = result.Window;

            
            this.repositoryExplorerWindow.Visible = true;
            this.repositoryExplorerWindow.Caption = "Repository Explorer";
            try
            {
                if ( !this.repositoryExplorerWindow.AutoHides )
                {   
                    this.repositoryExplorerWindow.Width = 200;
                    this.repositoryExplorerWindow.Height = 400;
                }
            }
            catch( System.Runtime.InteropServices.COMException )
            {
                // swallow
            }
            
            this.repositoryExplorerControl = result.Control as RepositoryExplorerControl;

            // force the handle to be created
            if ( this.RepositoryExplorer.Handle == IntPtr.Zero )
            {
                throw new InvalidOperationException( "Handle should never be zero" );
            }
        }

        private void CreateCommitDialog()
        {
            Debug.WriteLine( "Creating commit dialog user control", "Ankh" );

            ToolWindowResult result = this.context.DteStrategyFactory.GetToolWindowStrategy().
                CreateToolWindow( typeof(CommitDialog), 
                "Commit", CommitDialogGuid );
            
            this.commitDialogWindow = result.Window;

            this.commitDialogWindow.Visible = true;
            this.commitDialogWindow.Visible = false;

            this.commitDialogWindow.Caption = "Commit";

            this.commitDialog = result.Control as CommitDialog;

            System.Diagnostics.Debug.Assert( this.commitDialog != null, 
                "Could not create tool window" );
            
        }

        private void CreateWorkingCopyExplorer()
        {
            Debug.WriteLine( "Creating working copy explorer tool window", "Ankh" );
            ToolWindowResult result = this.context.DteStrategyFactory.GetToolWindowStrategy().
                CreateToolWindow(typeof(WorkingCopyExplorerControl),
                "Working Copy Explorer", WorkingCopyExplorerGuid );

            this.workingCopyExplorerWindow = result.Window;

            this.workingCopyExplorerWindow.Visible = true;

            this.workingCopyExplorerWindow.Caption = "Working Copy Explorer";

            this.workingCopyExplorerControl = result.Control as WorkingCopyExplorerControl;
            System.Diagnostics.Debug.Assert( this.workingCopyExplorerControl != null,
                "Could not create tool window for WC Explorer" );
        }

        private void ProceedCommit( object sender, EventArgs e )
        {
            this.commitDialog.Proceed -= new EventHandler( this.ProceedCommit );
            this.commitDialogModal = false;
            this.commitDialogWindow.Visible = false;
        }


        private SH.WebBrowser NewBrowserWindow(SH.WebBrowser browser)
        {
            browser.NewWindow2 += new SH.DWebBrowserEvents2_NewWindow2EventHandler(this.NewWindow2);
            object nullObject = null;
            object newWindowFlag = 0x1;
            object aboutBlank = "about:blank";

            this.newBrowser = null;
            browser.Navigate2( ref aboutBlank, ref newWindowFlag, ref nullObject, 
                ref nullObject, ref nullObject );

            Debug.Assert( this.newBrowser != null );
            return this.newBrowser;                
        }

        protected static void GetPathInfo(object sender, ResolvingPathEventArgs args)
        {
            SvnItem item = (SvnItem)args.Item;
            args.IsDirectory = item.IsDirectory;
            args.Path = item.Path;
        }

        protected static void GetUrlPathinfo(object sender, ResolvingPathEventArgs args)
        {
            SvnItem item = (SvnItem)args.Item;
            args.IsDirectory = item.IsDirectory;
            args.Path = item.Status.Entry.Url;
        }

        private void EnsureWindowSize( Window window )
        {
            try
            {
                if ( window.Width < 150 )
                    window.Width = 400;

                if ( window.Height < 100 )
                    window.Height = 500;
            }
            catch( System.Runtime.InteropServices.COMException )
            {
                // swallow, no biggie
            }
        }


        private SH.WebBrowser newBrowser;
        private void NewWindow2(ref object ppDisp, ref bool Cancel)
        {
            this.newBrowser = (SH.WebBrowser)ppDisp;
        }

        private RepositoryExplorerControl repositoryExplorerControl;
        private WorkingCopyExplorerControl workingCopyExplorerControl;
        private Window repositoryExplorerWindow;
        private CommitDialog commitDialog;
        private Window commitDialogWindow;
        private Window workingCopyExplorerWindow;
        private IContext context;
        private bool commitDialogModal;
        
        public const string REPOSEXPLORERGUID = 
            "{1C5A739C-448C-4401-9076-5990300B0E1B}";
        private const string CommitDialogGuid = 
            "{08BD45A4-7716-49b0-BB41-CFEBCD098728}";
        private const string WorkingCopyExplorerGuid = 
            "{3ABCF4EA-71BB-4e1c-ABD0-39261B19EDA1}";


        
    }
}