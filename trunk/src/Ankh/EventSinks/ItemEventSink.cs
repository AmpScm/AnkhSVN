// $Id$
using System;
using NSvn;
using EnvDTE;

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
            this.Context.OutputPane.StartActionText( "Delete" );
            this.Context.SolutionExplorer.VisitResources( 
                item, new RemoveProjectVisitor(), false );
            this.Context.SolutionExplorer.Refresh ( item.ContainingProject );
            this.Context.OutputPane.EndActionText();
        }

        protected void ItemRenamed( ProjectItem item, string oldName )
        {
            string s = item.Kind;
        }

        /// <summary>
        /// A visitor that schedules a remove of visited item on commit
        /// </summary>
        private class RemoveProjectVisitor : LocalResourceVisitorBase
        {
            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
            {
                resource.Remove( true );
            }
        }
	}
}
