// $Id$
using System;
using Microsoft.VisualStudio.VCProjectEngine;
using EnvDTE;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Sink for VCProjectEngineEvents events.
    /// </summary>
    internal class VCProjectEventSink : EventSink
    {
        internal VCProjectEventSink( VCProjectEngineEvents events, AnkhContext context )
            : base( context )
        {
            this.events = events;
            this.events.ItemAdded += new _dispVCProjectEngineEvents_ItemAddedEventHandler(
                this.ItemAdded );
            this.events.ItemRemoved += new _dispVCProjectEngineEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
        }

        public override void Unhook()
        {
            this.events.ItemAdded += new _dispVCProjectEngineEvents_ItemAddedEventHandler(
                this.ItemAdded );
            this.events.ItemRemoved += new _dispVCProjectEngineEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
        }

        protected void ItemAdded( object item, object parent )
        {
            VCFile file = item as VCFile;
            string s = file.ToString();
            //this.Context.SolutionExplorer.SyncWithTreeView();
        }

        protected void ItemRemoved( object item, object parent )
        {
            
        }

        private VCProjectEngineEvents events;
    }
}
