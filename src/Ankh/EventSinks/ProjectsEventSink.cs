using System;
using EnvDTE;
using NSvn;

namespace Ankh.EventSinks
{
	/// <summary>
	/// Event sink for the ProjectEvents events.
	/// </summary>
	internal class ProjectsEventSink : EventSink
	{
		internal ProjectsEventSink( ProjectsEvents events, AnkhContext context ) :
            base( context )
		{
			this.events = events;
            events.ItemAdded += new _dispProjectsEvents_ItemAddedEventHandler(
                this.ItemAdded );
            events.ItemRemoved += new _dispProjectsEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
            events.ItemRenamed += new _dispProjectsEvents_ItemRenamedEventHandler(
                this.ItemRenamed );           
		}

        public override void Unhook()
        {
            this.events.ItemAdded -= new _dispProjectsEvents_ItemAddedEventHandler(
                this.ItemAdded );
            this.events.ItemRemoved -= new _dispProjectsEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
            this.events.ItemRenamed -= new _dispProjectsEvents_ItemRenamedEventHandler(
                this.ItemRenamed );
        }

        protected void ItemAdded( Project item )
        {
            this.Context.SolutionExplorer.SyncWithTreeView();
        }

        /// <summary>
        /// Schedules a Project for removal on commit.
        /// </summary>
        /// <param name="item">Projectitem to be scheduled for removal.</param>
        protected void ItemRemoved( Project item )
        {
            this.Context.SolutionExplorer.VisitResources( 
                item, new RemoveProjectVisitor() );
            this.Context.SolutionExplorer.SyncWithTreeView();
        }

        protected void ItemRenamed( Project item, string oldName )
        {
            string s = item.FileName;
        }

        /// <summary>
        /// A visitor that schedules a remove of visited items on commit.
        /// </summary>
        private class RemoveProjectVisitor : LocalResourceVisitorBase
        {
            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
            {
                resource.Remove( true );
            }
        }
        private ProjectsEvents events;
	}
}
