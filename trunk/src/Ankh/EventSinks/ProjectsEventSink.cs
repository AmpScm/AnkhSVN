using System;
using EnvDTE;

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

        protected void ItemRemoved( Project item )
        {
            string s = item.FileName;
        }

        protected void ItemRenamed( Project item, string oldName )
        {
            string s = item.FileName;
        }

        private ProjectsEvents events;
	}
}
