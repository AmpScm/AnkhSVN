using System;
using EnvDTE;
using Extensibility;

namespace Ankh.EventSinks
{
	/// <summary>
	/// Event sink for the ProjectItemsEvent
	/// </summary>
	internal class ProjectItemsEventSink : EventSink
	{
		internal ProjectItemsEventSink( ProjectItemsEvents events, 
            AnkhContext context )
            : base( context  )
		{
            this.events = events;
            events.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(
                this.ItemAdded );
            events.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
            events.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler(
                this.ItemRenamed );
        }

        public override void Unhook()
        {
            this.events.ItemAdded -= new _dispProjectItemsEvents_ItemAddedEventHandler(
                this.ItemAdded );
            this.events.ItemRemoved -= new _dispProjectItemsEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
            this.events.ItemRenamed -= new _dispProjectItemsEvents_ItemRenamedEventHandler(
                this.ItemRenamed );
        }


        protected void ItemAdded( ProjectItem item )
        {
            this.Context.SolutionExplorer.SyncWithTreeView();
        }

        protected void ItemRemoved( ProjectItem item )
        {
            string s = item.Kind;
        }

        protected void ItemRenamed( ProjectItem item, string oldName )
        {
            string s = item.Kind;
        }

        private ProjectItemsEvents events;
	}
}
