// $Id$
using System;
using NSvn;
using EnvDTE;
using System.IO;

namespace Ankh.EventSinks
{
	/// <summary>
	/// Base class serving as event sink for all item related events.
	/// </summary>
	internal abstract class ItemEventSink : EventSink
	{
		protected ItemEventSink( AnkhContext context ) : base( context )
		{
			// empty
		}
     
 

        protected void ItemAdded( ProjectItem item )
        {
            try
            {
                this.Context.SolutionExplorer.Refresh( item.ContainingProject );
            }
            catch( Exception ex )
            {
                Connect.HandleError( ex );
                throw;
            }
        }

        /// <summary>
        /// Schedules a Projectitem for removal on commit.
        /// </summary>
        /// <param name="item">Projectitem to be scheduled for removal.</param>
        protected void ItemRemoved( ProjectItem item )
        {
            try
            {
                //this.Context.OutputPane.StartActionText( "Delete" );
                this.Context.SolutionExplorer.VisitResources( 
                    item, new RemoveProjectVisitor(), false );
                this.Context.SolutionExplorer.Refresh ( item.ContainingProject );
                //this.Context.OutputPane.EndActionText();

            }
            catch ( Exception ex )
            {
                Connect.HandleError( ex );
                throw;
            }
        }

        protected void ItemRenamed( ProjectItem item, string oldName )
        {
            try 
            {
                this.Context.OutputPane.StartActionText( "Renaming" );
                this.Context.SolutionExplorer.VisitResources(
                    item, new RenameProjectVisitor( item.get_FileNames(0) ), true );
                this.Context.SolutionExplorer.Refresh( item.ContainingProject );
                this.Context.OutputPane.EndActionText();
            }
            catch ( Exception ex)
            {
                Connect.HandleError( ex );
                throw;
            }
        }

        /// <summary>
        /// A visitor that schedules a remove of visited item on commit
        /// </summary>
        private class RemoveProjectVisitor : LocalResourceVisitorBase
        {
            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
            {
                // Checks if file doesn't exists. 
                if ( !File.Exists( resource.Path ) )
                {
                    resource.Remove( true );
                }
                
            }
        }

        /// <summary>
        /// A visitor that renames visited items.
        /// </summary>
        private class RenameProjectVisitor : LocalResourceVisitorBase
        {
            private string newPath;
            //Initialises the Visitor with the new name of the item to move.
            public RenameProjectVisitor( string newName )
            {
                this.newPath = newName;
            }

            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
            {
                File.Move( this.newPath, resource.Path );
                resource.Move( this.newPath, true );
            }
        }
       
        
	}
}
