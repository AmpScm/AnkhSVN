// $Id$
using System;
using EnvDTE;
using System.Runtime.InteropServices;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Event sink for the DocumentEvents events.
    /// </summary>
    internal class DocumentEventsSink : EventSink
    {
        public DocumentEventsSink( AnkhContext context ) : base( context )
        {
            this.events = context.DTE.Events.get_DocumentEvents( null );
            this.events.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(
                this.DocumentSaved );
        }

        public override void Unhook()
        {
            this.events.DocumentSaved -= new _dispDocumentEvents_DocumentSavedEventHandler(
                this.DocumentSaved );
        }

        private void DocumentSaved( Document document )
        {
            try
            {
                this.Context.SolutionExplorer.UpdateStatus( document.ProjectItem );
            }
            catch( COMException )
            {
                // HACK: Swallow
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
                throw;
            }
        }

        private DocumentEvents events;
    }
}
