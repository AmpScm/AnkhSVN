using System;
using EnvDTE;

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

        /// <summary>
        /// Event handler for the SolutionOpened event.
        /// </summary>
        private void SolutionOpened()
        {
            try
            {
                System.Windows.Forms.MessageBox.Show( "Solution opened" );
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



        private EnvDTE._DTE dte;
        private EnvDTE.AddIn addin;

        //required to ensure events will still fire
        private SolutionEvents solutionEvents;
        private SolutionExplorer solutionExplorer = null;

	}
}
