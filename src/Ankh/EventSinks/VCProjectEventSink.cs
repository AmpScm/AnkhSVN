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
            this.vcFileType = asm.GetType(
                "Microsoft.VisualStudio.VCProjectEngine.VCFile", true );
            this.vcFilterType = asm.GetType(
                "Microsoft.VisualStudio.VCProjectEngine.VCFilter", true );
            this.vcProjectType = asm.GetType(
                "Microsoft.VisualStudio.VCProjectEngine.VCProject", true );

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
                        }
                    }
                }

                this.VCDelayedRefresh( item );
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
            }
        }

        protected void ItemRemoved( object item, object parent )
        {
            // this a project being removed?
            if ( parent != null )
                this.Context.SolutionExplorer.RefreshSelectionParents();

            this.VCDelayedRefresh( item );
        }

        /// <summary>
        /// Saves the containing project of item after an interval.
        /// </summary>
        /// <param name="item"></param>
        private void VCDelayedRefresh( object item )
        {
            // what type of item has been added?
            Type itemType;

            // a "filter" is a VC folder
            if ( this.vcFilterType.IsInstanceOfType( item ) )
                itemType = this.vcFilterType;
            else if ( this.vcFileType.IsInstanceOfType( item ) )
                itemType = this.vcFileType;
            else
                throw new ApplicationException( "Unknown item type added to project" );

            // both Filter and File objects have a "project" property
            object vcproj = itemType.GetProperty("project").GetValue(
                item, new object[]{} );

            System.Threading.Timer timer = new System.Threading.Timer(
                new TimerCallback( this.RefreshCallback ), vcproj, REFRESHDELAY, 
                Timeout.Infinite );
        }

        private void RefreshCallback( object vcproj )
        {
            this.vcProjectType.InvokeMember( "Save", 
                BindingFlags.InvokeMethod | BindingFlags.IgnoreCase, null, 
                vcproj, new object[]{} );
            
            this.Context.SolutionExplorer.Refresh( 
                this.GetProjectForVCProject( vcproj ) );
        }

        /// <summary>
        /// Gets the Project instance corresponding to a VCProject
        /// </summary>
        /// <param name="vcproj"></param>
        /// <returns></returns>
        private Project GetProjectForVCProject( object vcproj )
        {
            string name = (string)this.vcProjectType.GetProperty("ProjectFile").GetValue(
                vcproj, new object[]{} );

            foreach( Project p in this.Context.DTE.Solution.Projects )
            {
                if ( String.Compare( p.FileName, name, true ) == 0 )
                    return p;
            }
            throw new ApplicationException( "Could not find Project object for " + name );
        }

        private readonly Delegate itemAddedDelegate;
        private readonly Delegate itemRemovedDelegate;
        private readonly Type vcProjectEventsType;
        private readonly Type vcFileType;
        private readonly Type vcProjectType;
        private readonly Type vcFilterType;
        private readonly object events;
    }
}
