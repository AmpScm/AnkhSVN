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

namespace Ankh
{
    /// <summary>
    /// General context object for the Ankh addin. Contains pointers to objects
    /// required by commands.
    /// </summary>
    internal class AnkhContext
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

            this.ankhLoadedForSolution = false;

            this.SetUpEvents();

           
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
                this.StartOperation( "Synchronizing with solution explorer");

                this.solutionExplorer.Load();
                this.eventSinks = EventSinks.EventSink.CreateEventSinks( this );

                timer.End( "Solution opened", "Ankh" );
                this.ankhLoadedForSolution = true;
                //MessageBox.Show( timer.ToString() );
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
            this.DTE.StatusBar.Text = "Ready";
            this.DTE.StatusBar.Animate( false, vsStatusAnimation.vsStatusAnimationSync );

            this.OutputPane.EndActionText();
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
            // no point in doing anything if the solution dir isn't a wc
            string solutionPath = this.dte.Solution.FullName;
            if ( solutionPath == String.Empty || 
                !SvnUtils.IsWorkingCopyPath( Path.GetDirectoryName( solutionPath ) ) )
                return false;

            string adminDir = Path.Combine( Path.GetDirectoryName( solutionPath ),
                NSvn.Core.Client.AdminDirectoryName );

            // maybe this solution has never been loaded before with Ankh?
            if ( File.Exists( Path.Combine( adminDir, "Ankh.Load" ) ) )
                return true;
            else if ( File.Exists( Path.Combine(adminDir, "Ankh.NoLoad") ) )
                return false;
            else
                return this.QueryWhetherAnkhShouldLoad( adminDir );
        }

        private bool QueryWhetherAnkhShouldLoad( string adminDir )
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
                File.Create( Path.Combine(adminDir, "Ankh.Load") ).Close();
                return true;
            }
            else if ( res == DialogResult.No )
            {
                File.Create( Path.Combine(adminDir, "Ankh.NoLoad") ).Close();
            }

            return false;
        }
        

        private void CreateRepositoryExplorer()
        {   
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
