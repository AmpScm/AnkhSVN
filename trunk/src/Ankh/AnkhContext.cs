// $Id$
using System;
using EnvDTE;

using NSvn.Common;
using NSvn.Core;
using Ankh.UI;
using Utils;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Ankh.Solution;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace Ankh
{
    /// <summary>
    /// General context object for the Ankh addin. Contains pointers to objects
    /// required by commands.
    /// </summary>
    public class AnkhContext
    {
        /// <summary>
        /// Fired when the addin is unloading.
        /// </summary>
        public event EventHandler Unloading;


        public AnkhContext( EnvDTE._DTE dte, EnvDTE.AddIn addin )
        {
            this.dte = dte;
            this.addin = addin;

            this.hostWindow = new Win32Window( new IntPtr(dte.MainWindow.HWnd) );

            this.configLoader = new Ankh.Config.ConfigLoader();

            this.LoadConfig();

            this.outputPane = new OutputPaneWriter( dte, "AnkhSVN" );
            this.solutionExplorer = new Solution.Explorer( this.dte, this );
            this.progressDialog = new ProgressDialog();            
            this.CreateRepositoryExplorer();
            this.repositoryController = new RepositoryExplorer.Controller( this, 
                this.repositoryExplorer, this.reposExplorerWindow );

            string iconvdir = Path.Combine( 
                Path.GetDirectoryName(this.GetType().Assembly.Location), 
                "iconv" );
            Utils.Win32.Win32.SetEnvironmentVariable( "APR_ICONV_PATH", iconvdir );

            this.ankhLoadedForSolution = false;

            this.SetUpEvents();    
            
            this.conflictManager = new ConflictManager(this);

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
        public EnvDTE.AddIn AddIn
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.addin; }
        }

        /// <summary>
        /// The SolutionExplorer object.
        /// </summary>
        public Explorer SolutionExplorer
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

        public EnvDTE.Window RepositoryExplorerWindow
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.reposExplorerWindow; }
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
        /// Event handler for the SolutionOpened event. Can also be called at
        /// addin load time, or if Ankh is enabled for a solution.
        /// </summary>
        public void SolutionOpened()
        {
            try
            {
                if ( !this.CheckWhetherAnkhShouldLoad() )
                    return;

                System.Diagnostics.Trace.WriteLine( "Solution opening", "Ankh" );

                Utils.DebugTimer timer = DebugTimer.Start();
                DateTime startTime = DateTime.Now;
                this.StartOperation( "Synchronizing with solution explorer");

                this.statusCache = new StatusCache( this.Client );

                this.solutionExplorer.Load();
                this.eventSinks = EventSinks.EventSink.CreateEventSinks( this );

                timer.End( "Solution opened", "Ankh" );
                this.OutputPane.WriteLine( "Time: {0}", DateTime.Now - startTime );
                this.ankhLoadedForSolution = true;
                //MessageBox.Show( timer.ToString() );

                // Add Conflict tasks for all conflicts in solution
                this.conflictManager.CreateTaskItems(); 
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
            }
            finally
            {
                this.EndOperation();   
            }
        }

        /// <summary>
        /// Called when a solution is closed.
        /// </summary>
        public void SolutionClosing()
        {
            this.conflictManager.RemoveAllTaskItems();
            this.SolutionExplorer.Unload();

            // unhook events.
            if ( this.eventSinks != null )
            {
                foreach( EventSinks.EventSink sink in this.eventSinks )
                    sink.Unhook();
            }

            this.ankhLoadedForSolution = false;
        }

        /// <summary>
        /// Should be called before starting any lengthy operation
        /// </summary>
        public void StartOperation( string description )
        {
            //TODO: maybe refactor this?
            this.operationRunning = true;
            this.DTE.StatusBar.Text = description + "...";
            this.DTE.StatusBar.Animate( true, vsStatusAnimation.vsStatusAnimationSync );

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
                this.DTE.StatusBar.Text = "Ready";
                this.DTE.StatusBar.Animate( false, vsStatusAnimation.vsStatusAnimationSync );

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
        }


        #region SetUpEvents
        /// <summary>
        /// Sets up event handlers.
        /// </summary>
        private void SetUpEvents()
        {
            // apparently necessary to avoid the SolutionEvents object being
            // gc'd :-/
            this.solutionEvents = this.DTE.Events.SolutionEvents;
            this.solutionEvents.Opened += new 
                _dispSolutionEvents_OpenedEventHandler( this.SolutionOpened );
            this.solutionEvents.BeforeClosing += new 
                _dispSolutionEvents_BeforeClosingEventHandler( this.SolutionClosing);
        }
        #endregion        

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

            // should we use a custom configuration directory?
            if ( this.config.Subversion.ConfigDir != null )
                this.client = new SvnClient( this, 
                    Environment.ExpandEnvironmentVariables(this.config.Subversion.ConfigDir) );
            else
                this.client = new SvnClient( this );

#if ALT_ADMIN_DIR
            // should we use a custom admin directory for our working copies?
            if ( this.config.Subversion.AdminDirectoryName != null )
                NSvn.Core.Client.AdminDirectoryName = 
                    this.config.Subversion.AdminDirectoryName;
#endif
        }
        
        private bool CheckWhetherAnkhShouldLoad()
        {
            // no point in doing anything if the solution dir doesn't exist
            string solutionPath = this.dte.Solution.FullName;
            if ( solutionPath == String.Empty || !File.Exists(solutionPath))
                return false;

            string solutionDir = Path.GetDirectoryName( solutionPath );

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
                if ( SvnUtils.IsWorkingCopyPath( solutionDir ) )
                    return this.QueryWhetherAnkhShouldLoad( solutionDir );
                else 
                    return false;
            }
        }

        private bool QueryWhetherAnkhShouldLoad( string solutionDir )
        {
            string nl = Environment.NewLine;
            string msg = "Ankh has detected that the solution file for this solution " + 
                "is in a Subversion working copy." + nl + 
                "Do you want to enable Ankh for this solution?" + nl +
                "(If you select Cancel, Ankh will not be enabled, " + nl +
                "but you will " +
                "be asked this question again the next time you open the solution)";

            DialogResult res = MessageBox.Show( 
                this.HostWindow, msg, "Ankh", 
                MessageBoxButtons.YesNoCancel );
            if ( res == DialogResult.Yes )
            {
                Debug.WriteLine( "Creating Ankh.Load", "Ankh" );
                File.Create( Path.Combine(solutionDir, "Ankh.Load") ).Close();
                return true;
            }
            else if ( res == DialogResult.No )
            {
                Debug.WriteLine( "Creating Ankh.NoLoad", "Ankh" );
                File.Create( Path.Combine(solutionDir, "Ankh.NoLoad") ).Close();
            }

            return false;
        }
        

        private void CreateRepositoryExplorer()
        {   
            Debug.WriteLine( "Creating repository explorer", "Ankh" );
            object control = null;
            this.reposExplorerWindow = this.dte.Windows.CreateToolWindow( 
                this.addin, "AnkhUserControlHost.AnkhUserControlHostCtl", 
                "Repository Explorer", REPOSEXPLORERGUID, ref control );
            
            this.reposExplorerWindow.Visible = true;
            this.reposExplorerWindow.Caption = "Repository Explorer";
            
            this.objControl = (AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl)control;
            
            this.repositoryExplorer = new RepositoryExplorerControl();
            this.objControl.HostUserControl( this.repositoryExplorer );
            
            System.Diagnostics.Debug.Assert( this.repositoryExplorer != null, 
                "Could not create tool window" );
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

        

        private EnvDTE._DTE dte;
        private EnvDTE.AddIn addin;
        private IWin32Window hostWindow;

        private IList eventSinks;

        private RepositoryExplorer.Controller repositoryController;

        private OutputPaneWriter outputPane;

        //required to ensure events will still fire
        private SolutionEvents solutionEvents;
        private Explorer solutionExplorer = null;

        private Ankh.Config.Config config;

        private bool ankhLoadedForSolution;
        private StatusCache statusCache;

        private bool operationRunning;

        private ConflictManager conflictManager; 

        private ProgressDialog progressDialog;
        private SvnClient client;
        private RepositoryExplorerControl repositoryExplorer;
        private EnvDTE.Window reposExplorerWindow;
        private AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl objControl;
        private Ankh.Config.ConfigLoader configLoader;
        public static readonly string REPOSEXPLORERGUID = 
            "{1C5A739C-448C-4401-9076-5990300B0E1B}";
    }
}
