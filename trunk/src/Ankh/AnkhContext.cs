// $Id$
using System;
using EnvDTE;
using NSvn;
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
    internal class AnkhContext : IWin32Window
    {
        public AnkhContext( EnvDTE._DTE dte, EnvDTE.AddIn addin )
        {
            this.dte = dte;
            this.addin = addin;
            this.context = new SvnContext( this );

            this.outputPane = new OutputPaneWriter( dte, "AnkhSVN" );

            this.repositoryController = new RepositoryExplorer.Controller();

            this.CreateRepositoryExplorer();

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

        public RepositoryExplorer.Controller RepositoryController
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.repositoryController; }
        }



        /// <summary>
        /// The SvnContext object used by the NSvn objects.
        /// </summary>
        public SvnContext Context
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.context; }
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
     
        public IntPtr Handle
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return new IntPtr(this.DTE.MainWindow.HWnd);
            }
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
            this.solutionEvents.AfterClosing += new 
                _dispSolutionEvents_AfterClosingEventHandler( this.SolutionClosed );
        }
        #endregion        

        /// <summary>
        /// Event handler for the SolutionOpened event.
        /// </summary>
        private void SolutionOpened()
        {
            try
            {
                this.solutionExplorer = new Explorer( this.DTE, this.Context );
                this.eventSinks = EventSinks.EventSink.CreateEventSinks( this );
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
            }
        }
        
        /// <summary>
        /// Called when a solution is closed.
        /// </summary>
        private void SolutionClosed()
        {
            this.solutionExplorer = null;

            // unhook events.
            foreach( EventSinks.EventSink sink in this.eventSinks )
                sink.Unhook();
        }

        private void CreateRepositoryExplorer()
        {   
            object control = null;
            this.reposExplorerWindow = this.dte.Windows.CreateToolWindow( 
                this.addin, "VSUserControlHost.VSUserControlHostCtl", 
                "Repository Explorer", REPOSEXPLORERGUID, ref control );

            this.reposExplorerWindow.Visible = true;
            this.reposExplorerWindow.Caption = "Repository Explorer";

            this.objControl = (VSUserControlHostLib.IVSUserControlHostCtl)control;

            this.repositoryExplorer = (RepositoryExplorerControl)this.objControl.HostUserControl( 
                typeof(RepositoryExplorerControl).Assembly.Location, 
                "Ankh.UI.RepositoryExplorerControl" );

            this.repositoryExplorer.Controller = this.RepositoryController;

            System.Diagnostics.Debug.Assert( this.repositoryExplorer != null, 
                "Could not create tool window" );
        }

        private EnvDTE._DTE dte;
        private EnvDTE.AddIn addin;

        private IList eventSinks;

        private RepositoryExplorer.Controller repositoryController;

        private OutputPaneWriter outputPane;

        //required to ensure events will still fire
        private SolutionEvents solutionEvents;
        private Explorer solutionExplorer = null;

        private SvnContext context;
        private RepositoryExplorerControl repositoryExplorer;
        private EnvDTE.Window reposExplorerWindow;
        private VSUserControlHostLib.IVSUserControlHostCtl objControl;
        public static readonly string REPOSEXPLORERGUID = 
            "{1C5A739C-448C-4401-9076-5990300B0E1B}";
    }
}
