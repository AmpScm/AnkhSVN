// $Id$
using System;
using EnvDTE;
using System.Reflection;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Sink for VCProjectEngineEvents events.
    /// </summary>
    internal class VCProjectEventSink : EventSink
    {
        internal VCProjectEventSink( object events, AnkhContext context )
            : base( context )
        {
            this.events = events;
            // this should load the correct assembly, according to the version of VS
            Assembly asm = Assembly.Load( 
                "Microsoft.VisualStudio.VCProjectEngine");

            // the type on which VC project events are dispatched
            this.vcProjectEventsType = asm.GetType( 
                "Microsoft.VisualStudio.VCProjectEngine._dispVCProjectEngineEvents_Event", 
                true );

            // the ItemAdded event handler
            Type itemAddedType = asm.GetType( "Microsoft.VisualStudio.VCProjectEngine._dispVCProjectEngineEvents_ItemAddedEventHandler", false );
            this.itemAddedDelegate = Delegate.CreateDelegate( itemAddedType, this, "ItemAdded" );

            this.vcProjectEventsType.GetEvent( "ItemAdded" ).AddEventHandler( events, 
                this.itemAddedDelegate );

            // the ItemRemoved event handler
            Type itemRemovedType = asm.GetType( "Microsoft.VisualStudio.VCProjectEngine._dispVCProjectEngineEvents_ItemRemovedEventHandler", false );
            this.itemRemovedDelegate = Delegate.CreateDelegate( itemRemovedType, this, "ItemRemoved" );

            this.vcProjectEventsType.GetEvent( "ItemRemoved" ).AddEventHandler( events, 
                this.itemRemovedDelegate );

        }

//
        public override void Unhook()
        {
            this.vcProjectEventsType.GetEvent( "ItemAdded" ).RemoveEventHandler( 
                this.events, this.itemAddedDelegate );
            this.vcProjectEventsType.GetEvent( "ItemRemoved" ).RemoveEventHandler( 
                this.events, this.itemRemovedDelegate );
               
        }

        protected void ItemAdded( object item, object parent )
        {
            // is this a project being added?
            if ( parent == null ) 
            {
                EventSink.AddingProject = true;
            }

            // is there a project currently being added?
            if ( EventSink.AddingProject )
                return;

            this.Context.SolutionExplorer.RefreshSelectionParents();
        }

        protected void ItemRemoved( object item, object parent )
        {
            // this a project being removed?
            if ( parent != null )
                this.Context.SolutionExplorer.RefreshSelectionParents();
        }

        private readonly Delegate itemAddedDelegate;
        private readonly Delegate itemRemovedDelegate;
        private readonly Type vcProjectEventsType;
        private readonly object events;
    }
}
