// $Id$
using System;
using EnvDTE;
using System.Reflection;
using System.Threading;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Sink for VCProjectEngineEvents events.
    /// </summary>
    public class VCProjectEventSink : EventSink
    {
        public VCProjectEventSink( object events, IContext context )
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
            this.vcFileType = asm.GetType(
                "Microsoft.VisualStudio.VCProjectEngine.VCFile", true );
            
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
            try
            {
                // is this a project being added?
                if ( parent == null ) 
                {
                    EventSink.AddingProject = true;
                }

                // is there a project currently being added?
                if ( EventSink.AddingProject )
                    return;

                EventSink.AddingProject = false;

                // should we auto-add this?
                if ( this.Context.Config.AutoAddNewFiles )
                {
                    // must be a file
                    if ( this.vcFileType.IsInstanceOfType( item ) )
                    {
                        PropertyInfo fullPath = this.vcFileType.GetProperty( "FullPath" );
                        string path = (string)fullPath.GetValue( item, new object[]{} );

                        SvnItem svnItem = this.Context.StatusCache[path];
                        if ( !svnItem.IsVersioned && svnItem.IsVersionable )
                        {
                            this.Context.Client.Add( path, false );
                            this.Context.SolutionExplorer.UpdateSelection();
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }
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
        private readonly Type vcFileType;
        private readonly object events;
    }
}
