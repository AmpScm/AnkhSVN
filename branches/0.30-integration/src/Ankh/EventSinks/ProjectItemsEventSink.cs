// $Id$
using System;
using EnvDTE;
using Extensibility;
using NSvn;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Event sink for the ProjectItemsEvent
    /// </summary>
    internal class ProjectItemsEventSink : ItemEventSink
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

        private ProjectItemsEvents events;

    }
}
