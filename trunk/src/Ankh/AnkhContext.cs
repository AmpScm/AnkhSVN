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

namespace Ankh
{
    /// <summary>
    /// General context object for the Ankh addin. Contains pointers to objects
    /// required by commands.
    /// </summary>
    internal class AnkhContext
    {
        public AnkhContext( EnvDTE._DTE dte, EnvDTE.AddIn addin )
        {
            this.dte = dte;
            this.addin = addin;

            this.config = Ankh.Config.ConfigLoader.LoadConfig( AnkhContext.CONFIGDIR, 
                AnkhContext.CONFIGFILE );

            // should we use a custom configuration directory?
            if ( this.config.Subversion.ConfigDir != null )
                this.client = new SvnClient( this, 
                    Environment.ExpandEnvironmentVariables(this.config.Subversion.ConfigDir) );
            else
                this.client = new SvnClient( this );

            this.hostWindow = new Win32Window( new IntPtr(dte.MainWindow.HWnd) );

            this.outputPane = new OutputPaneWriter( dte, "AnkhSVN" );

            

            this.solutionExplorer = new Solution.Explorer( this.dte, this );

            this.progressDialog = new ProgressDialog();            

            this.CreateRepositoryExplorer();

            this.repositoryController = new RepositoryExplorer.Controller( this, 
                this.repositoryExplorer );

            // is there a solution opened?
            if ( this.dte.Solution.IsOpen )
                this.SolutionOpened();

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
        public RepositoryExplorerControl RepositoryExplorer
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.repositoryExplorer; }
        }

        public EnvDTE.Window RepositoryExplorerWindow
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.reposExplorerWindow; }
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
            Ankh.Config.ConfigLoader.SaveConfig( this.config, AnkhContext.CONFIGDIR, 
                AnkhContext.CONFIGFILE );
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
        /// Event handler for the SolutionOpened event.
        /// </summary>
        private void SolutionOpened()
        {
            try
            {
                System.Diagnostics.Trace.WriteLine( "Solution opening", "Ankh" );

                Utils.DebugTimer timer = DebugTimer.Start();

                this.StartOperation( "Synchronizing with solution explorer");

                this.solutionExplorer.Load();

                this.eventSinks = EventSinks.EventSink.CreateEventSinks( this );

                timer.End( "Solution opened", "Ankh" );
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
        private void SolutionClosing()
        {
            this.SolutionExplorer.Unload();

            // unhook events.
            if ( this.eventSinks != null )
            {
                foreach( EventSinks.EventSink sink in this.eventSinks )
                    sink.Unhook();
            }
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

        private ProgressDialog progressDialog;
        private SvnClient client;
        private RepositoryExplorerControl repositoryExplorer;
        private EnvDTE.Window reposExplorerWindow;
        private AnkhUserControlHostLib.IAnkhUserControlHostCtlCtl objControl;
        private const string CONFIGDIR = "AnkhSVN";
        private const string CONFIGFILE = "ankhsvn.xml";
        public static readonly string REPOSEXPLORERGUID = 
            "{1C5A739C-448C-4401-9076-5990300B0E1B}";
    }
}
