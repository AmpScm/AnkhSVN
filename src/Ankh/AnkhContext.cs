using System;
using EnvDTE;
using NSvn;

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
            this.callbackTargets = new CallbackTargets();
            SetUpEvents();
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
        public SolutionExplorer SolutionExplorer
        {
            get{ return this.solutionExplorer; }
        }

        public Notifications Notifications
        {
            get{ return this.callbackTargets.Notifications; }
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
                this.solutionExplorer = new SolutionExplorer( this.DTE );
            }
            catch( Exception ex )
            {
                System.Windows.Forms.MessageBox.Show( ex.Message + 
                    Environment.NewLine + 
                    ex.StackTrace );
                throw;
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

        private EnvDTE._DTE dte;
        private EnvDTE.AddIn addin;

        //required to ensure events will still fire
        private SolutionEvents solutionEvents;
        private DocumentEvents documentEvents;
        private ProjectItemsEvents csProjectItemsEvents;
        private ProjectItemsEvents vbProjectItemsEvents;
        private ProjectItemsEvents vcProjectItemsEvents;
        private ProjectItemsEvents vjProjectItemsEvents;
        private SolutionExplorer solutionExplorer = null;
        private CallbackTargets callbackTargets;
	}
}
