// $Id$
using System;
using EnvDTE;
using System.Runtime.InteropServices;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Event sink for the DocumentEvents events.
    /// </summary>
    public class DocumentEventsSink : EventSink
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
                if ( document.ProjectItem != null )
                {
                    for ( short i = 1; i <= document.ProjectItem.FileCount; i++ )
                    {
                        string filename = document.ProjectItem.get_FileNames(i);
                        SvnItem item = this.Context.StatusCache[ filename ];
                        item.Refresh( this.Context.Client );
                    }
                }
            }
            catch( COMException ex )
            {
                System.Diagnostics.Debug.WriteLine( 
                    "Exception thrown in DocumentSaved: " + ex, "Ankh" );
                // swallow
            }
            catch( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }
        }

        private DocumentEvents events;
    }
}
