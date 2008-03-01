// $Id$
using System;
using EnvDTE;



using Ankh.UI;
using Utils;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Ankh.Solution;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IManagedServiceProvider = System.IServiceProvider;
using SharpSvn;
using Utils.Services;

namespace Ankh
{
    /// <summary>
    /// General context object for the Ankh addin. Contains pointers to objects
    /// required by commands.
    /// </summary>
    public class AnkhContext : IContext
    {
        /// <summary>
        /// Fired when the addin is unloading.
        /// </summary>
        public event EventHandler Unloading;

		public AnkhContext(EnvDTE._DTE dte, EnvDTE.AddIn addin, IUIShell uiShell)
			: this(dte, uiShell)
		{
			this.addin = addin;
		}

		public AnkhContext(IManagedServiceProvider serviceProvider)
			: this((EnvDTE._DTE)serviceProvider.GetService(typeof(EnvDTE._DTE)), new UIShell())
		{
			// Called from the package initializer
		}

        public AnkhContext( EnvDTE._DTE dte, IUIShell uiShell )
        {
            this.dte = dte;      
            this.uiShell = uiShell;
            this.uiShell.Context = this;

            this.dteStrategyFactory = Ankh.DteStrategyFactory.CreateDteStrategyFactory( this );

            this.errorHandler = new ErrorHandler( dte.Version, this );

            this.hostWindow = new Win32Window( new IntPtr(dte.MainWindow.HWnd) );

            this.configLoader = new Ankh.Config.ConfigLoader();

            this.LoadConfig();

            this.projectFileWatcher = new FileWatcher(this.client);

            this.outputPane = new OutputPaneWriter( dte, "AnkhSVN" );
            this.solutionExplorer = new Solution.Explorer( this.dte, this);
            this.solutionExplorer.SolutionFinishedLoading += 
                new EventHandler(this.HandleSolutionFinishedLoading);


            this.progressDialog = new ProgressDialog();             

            string iconvdir = Path.Combine( 
                Path.GetDirectoryName(this.GetType().Assembly.Location), 
                "iconv" );
            Utils.Win32.Win32.SetEnvironmentVariable( "APR_ICONV_PATH", iconvdir );

            this.ankhLoadedForSolution = false;
           
            this.SetUpEvents();    
            
            this.conflictManager = new ConflictManager(this);

            this.statusCache = new StatusCache( this.client );

            this.repositoryController = 
                new RepositoryExplorer.Controller( this );
            this.workingCopyExplorer =
                new Ankh.WorkingCopyExplorer.WorkingCopyExplorer( this );

            AnkhServices.AddService(typeof(IWorkingCopyOperations), new WorkingCopyOperations(client));
            NotificationHandler.GetHandler(this);
        }

        

        

        /// <summary>
        /// The top level automation object.
        /// </summary>
        public EnvDTE._DTE DTE
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.dte; }
        }

        /// <summary>
        /// The addin object.
        /// </summary>
		[Obsolete("Null for package")]
        public EnvDTE.AddIn AddIn
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.addin; }
        }

        /// <summary>
        /// The UI shell.
        /// </summary>
        public IUIShell UIShell
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.uiShell; }
        }

        /// <summary>
        /// The SolutionExplorer object.
        /// </summary>
        public ISolutionExplorer SolutionExplorer
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.solutionExplorer; }
        }       

        /// <summary>
        /// The output pane.
        /// </summary>
        public OutputPaneWriter OutputPane
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.outputPane; }
        }

        //        public RepositoryExplorer.Controller RepositoryController
        //        {
        //            [System.Diagnostics.DebuggerStepThrough]
        //            get{ return this.repositoryController; }
        //        }



        /// <summary>
        /// The SvnContext object used by the NSvn objects.
        /// </summary>
        public SvnClient Client
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.client; }
        }

        /// <summary>
        /// The repository explorer control.
        /// </summary>
        public RepositoryExplorer.Controller RepositoryExplorer
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.repositoryController; }
        }
 
        /// <summary>
        /// Whether a solution is open.
        /// </summary>
        public bool SolutionIsOpen
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.dte.Solution.IsOpen; }
        }

        public bool AnkhLoadedForSolution
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.ankhLoadedForSolution; }
        }


        /// <summary>
        /// The Ankh configuration.
        /// </summary>
        public Ankh.Config.Config Config
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.config; }
        }

        /// <summary>
        /// The error handler.
        /// </summary>
        public IErrorHandler ErrorHandler
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.errorHandler; }
        }

        /// <summary>
        /// The configloader.
        /// </summary>
        public Ankh.Config.ConfigLoader ConfigLoader
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.configLoader; }
        }

        /// <summary>
        /// The status cache.
        /// </summary>
        public StatusCache StatusCache
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.statusCache; }
        }

        public bool OperationRunning
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.operationRunning; }
        }

        public string SolutionDirectory
        {
            get
            {
                if ( !this.SolutionIsOpen )
                    return null; 

                // no point in doing anything if the solution dir doesn't exist
                string solutionPath = this.dte.Solution.FullName;
                if ( solutionPath == String.Empty || !File.Exists(solutionPath))
                    return null;

                return Path.GetDirectoryName( solutionPath );
            }
        }

     
        /// <summary>
        /// An IWin32Window to be used for parenting dialogs.
        /// </summary>
        public IWin32Window HostWindow
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return this.hostWindow;
            }
        }

        /// <summary>
        /// Manage issues related to conflicts.
        /// </summary>
        public ConflictManager ConflictManager
        {
            [System.Diagnostics.DebuggerStepThrough]
            get {  return this.conflictManager;  }
        }

        /// <summary>
        /// Watches the project and solution files.
        /// </summary>
        public FileWatcher ProjectFileWatcher
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.projectFileWatcher; }
        }

        public IServiceProvider ServiceProvider
        {
            [System.Diagnostics.DebuggerStepThrough]
            get 
            {
                return this.dte as IServiceProvider;
            }
        }

        public IWorkingCopyExplorer WorkingCopyExplorer
        {
            [DebuggerStepThrough]
            get { return this.workingCopyExplorer; }
        }

        public IDteStrategyFactory DteStrategyFactory
        {
            get { return this.dteStrategyFactory; }
        }




        /// <summary>
        /// Event handler for the SolutionOpened event. Can also be called at
        /// addin load time, or if Ankh is enabled for a solution.
        /// </summary>
        public bool EnableAnkhForLoadedSolution()
        {
            if ( this.SolutionLoaded() )
            {
                this.solutionEvents.InitializeForLoadedSolution();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Called when a solution is closed.
        /// </summary>
        public void SolutionClosing()
        {
            this.ankhLoadedForSolution = false;

            this.conflictManager.RemoveAllTaskItems();
            this.SolutionExplorer.Unload();

            
        }

        /// <summary>
        /// Reloads the current solution.
        /// </summary>
        /// <returns>True if the solution has been reloaded.</returns>
        public bool ReloadSolutionIfNecessary()
        {
            if ( this.projectFileWatcher.HasDirtyFiles && !this.Config.DisableSolutionReload )
            {
                if ( MessageBox.Show( this.HostWindow, 
                    "One or more of your project files have changed as a result of a " + 
                    "Subversion operation." + Environment.NewLine +
                    "It is recommended that you reload the solution now. " + Environment.NewLine +
                    "Do you want to reload the solution?", "Project files changed", 
                    MessageBoxButtons.YesNo ) == DialogResult.Yes )
                {
                    string filename = this.dte.Solution.FullName;
                    this.dte.Solution.Close( true );
                    this.dte.Solution.Open( filename );
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Should be called before starting any lengthy operation
        /// </summary>
        public void StartOperation( string description )
        {
            //TODO: maybe refactor this?
            this.operationRunning = true;
            try
            {
                this.DTE.StatusBar.Text = description + "...";
                this.DTE.StatusBar.Animate( true, vsStatusAnimation.vsStatusAnimationSync );
            }
            catch ( Exception )
            {
                // Swallow, not critical
            }

            this.OutputPane.StartActionText( description );

            this.progressDialog.Caption = description;
        }

        /// <summary>
        ///  called at the end of any lengthy operation
        /// </summary>
        public void EndOperation()
        {
            if ( this.operationRunning )
            {
                try
                {
                    this.DTE.StatusBar.Text = "Ready";
                    this.DTE.StatusBar.Animate( false, vsStatusAnimation.vsStatusAnimationSync );
                }
                catch ( Exception )
                {
                    // swallow, not critical
                }

                this.OutputPane.EndActionText();
                this.operationRunning = false;
            }
        }

        /// <summary>
        /// Miscellaneous cleanup stuff goes here.
        /// </summary>
        public void Shutdown()
        {  
            if ( this.Unloading != null )
                this.Unloading( this, EventArgs.Empty );

            if ( this.solutionEvents != null )
                this.solutionEvents.Unhook();

        }

        public ISelectionContainer Selection
        {
            get 
            {
                Debug.WriteLine( String.Format( "Active Window: {0}", this.dte.ActiveWindow.Caption ) );

                // Store the current selection so that the selection is still correct when a popup is shown, which 
                // causes the .ActiveWindow to go to some other tool window.
                if ( this.UIShell.SolutionExplorerHasFocus() )
                {
                    Debug.WriteLine( "SolutionExplorer returned as selection", "AnkhSVN" );
                    this.selectionContainer = this.SolutionExplorer;
                }
                else if ( this.UIShell.WorkingCopyExplorerHasFocus() )
                {
                    Debug.WriteLine( "WorkingCopyExplorer returned as selection", "AnkhSVN" );
                    this.selectionContainer = this.WorkingCopyExplorer;
                }
                // if none of those, the previous selection is probably still valid.

                Debug.WriteLine( this.selectionContainer.GetType(), "AnkhSVN" );
                return this.selectionContainer;
            }
        }



        #region SetUpEvents
        /// <summary>
        /// Sets up event handlers.
        /// </summary>
        private void SetUpEvents()
        {
            this.solutionEvents = new Ankh.EventSinks.SolutionEventsSink( this );
            this.solutionEvents.SolutionLoaded += new CancelEventHandler( this.HandleSolutionLoaded );
            this.solutionEvents.SolutionBeforeClosing  += new EventHandler( this.HandleSolutionClosing );
        }
        #endregion        

        private void HandleSolutionLoaded( object sender, System.ComponentModel.CancelEventArgs args )
        {
            args.Cancel = !SolutionLoaded();
        }

        private bool SolutionLoaded( )
        {
            try
            {
                if ( !this.CheckWhetherAnkhShouldLoad() )
                {
                    return false;
                }


                System.Diagnostics.Trace.WriteLine( "Solution opening", "Ankh" );

                Utils.DebugTimer timer = DebugTimer.Start();

                this.solutionExplorer.Load();

                timer.End( "Solution opened", "Ankh" );

                //MessageBox.Show( timer.ToString() );

                // Add Conflict tasks for all conflicts in solution
                this.conflictManager.CreateTaskItems();

                return true;
            }
            catch ( Exception ex )
            {
                ErrorHandler.Handle( ex );
                return false;
            }
            finally
            {
                this.EndOperation();
            }
        }

        private void HandleSolutionClosing( object sender, EventArgs args )
        {
            this.SolutionClosing();
        }

        private void HandleSolutionFinishedLoading( object sender, EventArgs args )
        {
            this.ankhLoadedForSolution = true;
        }
        /// <summary>
        /// try to load the configuration file
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                this.config = this.configLoader.LoadConfig( );
            }
            catch( Ankh.Config.ConfigException ex )
            {
                MessageBox.Show( this.HostWindow, 
                    "There is an error in your configuration file:" + 
                    Environment.NewLine + Environment.NewLine + 
                    ex.Message + Environment.NewLine + Environment.NewLine + 
                    "Please edit the " + this.configLoader.ConfigPath + 
                    " file and correct the error." + Environment.NewLine + 
                    "Ankh will now load a default configuration.", "Configuration error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error );

                // fall back on the default configuration
                this.config = this.configLoader.LoadDefaultConfig();
            }

            SetupFromConfig();

            this.ConfigLoader.ConfigFileChanged += new EventHandler( ConfigLoader_ConfigFileChanged );

        }
        
        /// <summary>
        /// Seems someone has updated the config file on disk.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConfigLoader_ConfigFileChanged( object sender, EventArgs e )
        {
            try
            {
                this.config = this.ConfigLoader.LoadConfig();
                SetupFromConfig();
                this.OutputPane.WriteLine( "Configuration reloaded." );
            }
            catch ( Ankh.Config.ConfigException ex )
            {
                this.OutputPane.WriteLine( "Configuration file has errors: " + ex.Message );
            }
            catch ( Exception ex )
            {
                this.ErrorHandler.Handle( ex );
            }
        }

        private void SetupFromConfig()
        {
            this.client = new SvnClient();
            //// should we use a custom configuration directory?
            if (this.config.Subversion.ConfigDir != null)
                this.client.LoadConfiguration(
                    Environment.ExpandEnvironmentVariables(this.config.Subversion.ConfigDir));
            else
                this.client.LoadConfigurationDefault();

            // Let SharpSvnUI handle login and SSL dialogs
            SharpSvn.UI.SharpSvnUI.Bind(client, this.HostWindow);
        }
        
        private bool CheckWhetherAnkhShouldLoad()
        {
            string solutionDir = this.SolutionDirectory;

            // if we don't have a valid solution directory, no point in going on
            if ( solutionDir == null )
                return false;

            // maybe this solution has never been loaded before with Ankh?
            if ( File.Exists( Path.Combine( solutionDir, "Ankh.Load" ) ) )
            {
                Debug.WriteLine( "Found Ankh.Load", "Ankh" );
                return true;
            }
                //  user has expressly specified that this solution should load?
            else if ( File.Exists( Path.Combine(solutionDir, "Ankh.NoLoad") ) )
            {
                Debug.WriteLine( "Found Ankh.NoLoad", "Ankh" );
                return false;
            }
            else
            {
                Debug.WriteLine( "Found neither Ankh.Load nor Ankh.NoLoad", "Ankh" );

                // is this a wc?
                // the user must manually enable Ankh if soln dir is not vc
                if ( AnkhServices.GetService<IWorkingCopyOperations>().IsWorkingCopyPath( solutionDir ) )
                    return this.QueryWhetherAnkhShouldLoad( solutionDir );
                else 
                    return false;
            }
        }

        private bool QueryWhetherAnkhShouldLoad( string solutionDir )
        {
            DialogResult res = this.uiShell.QueryWhetherAnkhShouldLoad();

            if ( res == DialogResult.Yes )
            {
                Debug.WriteLine( "Creating Ankh.Load", "Ankh" );
                string ankhLoad = Path.Combine( solutionDir, "Ankh.Load" );
                File.Create( ankhLoad ).Close();
                File.SetAttributes( ankhLoad, FileAttributes.Hidden );                
                return true;
            }
            else if ( res == DialogResult.No )
            {
                Debug.WriteLine( "Creating Ankh.NoLoad", "Ankh" );
                string ankhNoLoad = Path.Combine( solutionDir, "Ankh.NoLoad" );
                File.Create( ankhNoLoad ).Close();
                File.SetAttributes( ankhNoLoad, FileAttributes.Hidden );
            }

            return false;
        }

        #region Win32Window class
        private class Win32Window : IWin32Window
        {
            public Win32Window( IntPtr handle )
            {
                this.handle = handle;
            }
            #region IWin32Window Members

            public System.IntPtr Handle
            {
                get
                {
                    return this.handle;
                }
            }

            #endregion
            private IntPtr handle;

        }
        #endregion


        private class NullSelectionContainer : ISelectionContainer
        {
            public void RefreshSelectionParents()
            {
            }

            public void RefreshSelection()
            {
            }

            public void SyncAll()
            {
            }

            public IList GetSelectionResources( bool getChildItems )
            {
                return EmptyList;
            }

            public IList GetSelectionResources( bool getChildItems, ResourceFilterCallback filter )
            {
                return EmptyList;
            }

            public IList GetAllResources( ResourceFilterCallback filter )
            {
                return EmptyList;
            }
            public static readonly NullSelectionContainer Instance = new NullSelectionContainer();
            private SvnItem[] EmptyList = new SvnItem[]{};
        }
        

        private EnvDTE._DTE dte;
        private EnvDTE.AddIn addin;
        private IWin32Window hostWindow;

        private ISelectionContainer selectionContainer = NullSelectionContainer.Instance;

        private WorkingCopyExplorer.WorkingCopyExplorer workingCopyExplorer;
        private RepositoryExplorer.Controller repositoryController;

        private OutputPaneWriter outputPane;

        //required to ensure events will still fire
        private Ankh.EventSinks.SolutionEventsSink solutionEvents;
        private Explorer solutionExplorer = null;

        private Ankh.Config.Config config;

        private bool ankhLoadedForSolution;
        private StatusCache statusCache;

        private bool operationRunning;

        private ConflictManager conflictManager; 
        private IErrorHandler errorHandler;

        private FileWatcher projectFileWatcher;

        private ProgressDialog progressDialog;
        private SvnClient client;

        private IUIShell uiShell;
        
        private Ankh.Config.ConfigLoader configLoader;

        private IDteStrategyFactory dteStrategyFactory;
    }
}
