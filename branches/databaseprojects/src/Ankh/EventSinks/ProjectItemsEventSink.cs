//// $Id$
//using System;
//using EnvDTE;
//using Extensibility;
//
//
//namespace Ankh.EventSinks
//{
//    /// <summary>
//    /// Event sink for the ProjectItemsEvent
//    /// </summary>
//    public class ProjectItemsEventSink : ItemEventSink
//    {
//        public ProjectItemsEventSink( ProjectItemsEvents events, 
//            IContext context )
//            : base( context  )
//        {
//            this.events = events;
//            events.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(
//                this.ItemAdded );
//            events.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(
//                this.ItemRemoved );
//            events.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler(
//                this.ItemRenamed );
//        }
//
//        public override void Unhook()
//        {
//            this.events.ItemAdded -= new _dispProjectItemsEvents_ItemAddedEventHandler(
//                this.ItemAdded );
//            this.events.ItemRemoved -= new _dispProjectItemsEvents_ItemRemovedEventHandler(
//                this.ItemRemoved );
//            this.events.ItemRenamed -= new _dispProjectItemsEvents_ItemRenamedEventHandler(
//                this.ItemRenamed );
//        }
//
//        private ProjectItemsEvents events;
//
//    }
//}
