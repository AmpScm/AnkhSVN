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
            this.context = new SvnContext( this );

            this.CreateRepositoryExplorer();

            this.SetUpEvents();
        }

        

        /// <summary>
        /// The top level automation object.
        /// </summary>
        public EnvDTE._DTE DTE
        {
            get{ return this.dte; }
        }

        /// <summary>
        /// The addin object.
        /// </summary>
        public EnvDTE.AddIn AddIn
        {
            get{ return this.addin; }
        }

        /// <summary>
        /// The SolutionExplorer object.
        /// </summary>
        public Explorer SolutionExplorer
        {
            get{ return this.solutionExplorer; }
        }

        /// <summary>
        /// The SvnContext object used by the NSvn objects.
        /// </summary>
        public SvnContext Context
        {
            get{ return this.context; }
        }

        /// <summary>
        /// The repository explorer control.
        /// </summary>
        public RepositoryExplorerControl RepositoryExplorer
        {
            get{ return this.repositoryExplorer; }
        }

        public EnvDTE.Window RepositoryExplorerWindow
        {
            get{ return this.reposExplorerWindow; }
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

            

            this.documentEvents = this.DTE.Events.get_DocumentEvents( null );
            this.documentEvents.DocumentSaved += new 
                _dispDocumentEvents_DocumentSavedEventHandler( this.DocumentSaved );

            this.csProjectItemsEvents = (ProjectItemsEvents)
                this.DTE.Events.GetObject( "CSharpProjectItemsEvents" );

            this.csProjectItemsEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(
                this.ItemAdded );



//            this.vcProjectItemsEvents = (ProjectItemsEvents)
//                this.DTE.Events.GetObject( "VCProjectItemsEvents" );
//
//            this.vcProjectItemsEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(
//                this.ItemAdded );

//            this.vbProjectItemsEvents = (ProjectItemsEvents)
//                this.DTE.Events.GetObject( "VBProjectItemsEvents" );
//            
//            this.vbProjectItemsEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(
//                this.ItemAdded );
//
//            this.vjProjectItemsEvents = (ProjectItemsEvents)
//                this.DTE.Events.GetObject( "VJSharpProjectItemsEvents" );
//
//            this.vjProjectItemsEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(
//                this.ItemAdded );

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
            }
            catch( Exception ex )
            {
                Connect.HandleError( ex );
            }
        }
        
        /// <summary>
        /// Called when a solution is closed.
        /// </summary>
        private void SolutionClosed()
        {
            this.solutionExplorer = null;
        }

        /// <summary>
        /// Called when a document is saved.
        /// </summary>
        private void DocumentSaved( Document document )
        {
            if ( this.SolutionExplorer != null )
                this.SolutionExplorer.UpdateStatus( document.ProjectItem );
        }

        /// <summary>
        ///  Called when a document is added
        /// </summary>
        private void ItemAdded( ProjectItem item )
        {
//            System.Windows.Forms.MessageBox.Show( "moo" );
            if ( this.SolutionExplorer != null )
                this.SolutionExplorer.SyncWithTreeView();
        }

//        private void CreateRepositoryExplorer()
//        {
//            
//            ProgIdAttribute attr = (ProgIdAttribute)
//                typeof(RepositoryExplorerControl).GetCustomAttributes(
//                typeof(ProgIdAttribute), false )[0];
//
//            object control = null;
//
//            this.reposExplorerWindow = this.dte.Windows.CreateToolWindow( 
//                this.addin, attr.Value, 
//                "Repository Explorer", REPOSEXPLORERGUID, ref control );
//
//            System.Diagnostics.Debug.Assert( control != null, 
//                "Could not create tool window" );
//            this.reposExplorerWindow.Visible = true;
//
//            this.repositoryExplorer = (RepositoryExplorerControl)control;
//            this.repositoryExplorer.Visible = true;
//        }

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

            System.Diagnostics.Debug.Assert( this.repositoryExplorer != null, 
                "Could not create tool window" );
            

            //this.repositoryExplorer.Visible = true;
        }

        private EnvDTE._DTE dte;
        private EnvDTE.AddIn addin;

        //required to ensure events will still fire
        private SolutionEvents solutionEvents;
        private DocumentEvents documentEvents;
        private ProjectItemsEvents csProjectItemsEvents;
        private ProjectItemsEvents vbProjectItemsEvents;
        private ProjectItemsEvents vcProjectItemsEvents;
        private ProjectItemsEvents vjProjectItemsEvents;
        private Explorer solutionExplorer = null;

        private SvnContext context;
        private RepositoryExplorerControl repositoryExplorer;
        private EnvDTE.Window reposExplorerWindow;
        private VSUserControlHostLib.IVSUserControlHostCtl objControl;
        public static readonly string REPOSEXPLORERGUID = 
            "{1C5A739C-448C-4401-9076-5990300B0E1B}";
	}
}
