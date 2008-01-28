using System;
using System.Text;
using EnvDTE;
using System.Collections;
//using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Utils;
using Microsoft.VisualStudio.Shell.Interop;

using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Ankh.Solution;

namespace Ankh.EventSinks
{
    public class HierarchyEventsSink : EventSink
    {
        public HierarchyEventsSink( IContext context ) : base(context)
        {
            try
            {
                this.AdviseHierarchyEvents( context );
            }
            catch ( Exception ex )
            {
                context.ErrorHandler.Handle( ex );
            }
        }

        public override void Unhook()
        {
            foreach ( HierarchyEventsImpl himpl in this.hierarchyEvents )
            {
                himpl.Unadvise();
            }
        }


        private void AdviseHierarchyEvents(IContext context)
        {
            // The DTE object is an IServiceProvider (previously undocumented, but they're pretty open about it now)
            IServiceProvider sp = this.Context.ServiceProvider;

            // get hold of the SVsSolution service
            Guid serviceGuid = typeof( SVsSolution ).GUID;
            Guid interfaceGuid = typeof( IVsSolution ).GUID;

            IntPtr svcPtr;

            Marshal.ThrowExceptionForHR(sp.QueryService( ref serviceGuid, ref interfaceGuid, out svcPtr ));
            IVsSolution solution = (IVsSolution)Marshal.GetObjectForIUnknown( svcPtr );
            Guid guid = Guid.Empty;

            this.hierarchyEvents = new ArrayList();

            // Use the SVSolution service to iterate through the projects
            IEnumHierarchies enumerator;
            int hr = solution.GetProjectEnum( (uint)__VSENUMPROJFLAGS.EPF_ALLPROJECTS, ref guid, out enumerator );

            IVsHierarchy[] hierarchies = new IVsHierarchy[1];
            uint fetched;
            Marshal.ThrowExceptionForHR( enumerator.Next( 1, hierarchies, out fetched ) );
            while ( fetched == 1 )
            {
                IVsHierarchy hi = hierarchies[0];

                AddHierarchyEventsImpl( hi );
                
                Marshal.ThrowExceptionForHR( enumerator.Next( 1, hierarchies, out fetched ) );
                
            }
        }

       

        public void AddHierarchy( IVsHierarchy pHierarchy )
        {
            this.AddHierarchyEventsImpl( pHierarchy );
        }

        public void RemoveHierarchy( IVsHierarchy pHierarchy )
        {
            foreach ( HierarchyEventsImpl hei in this.hierarchyEvents )
            {
                if ( hei.Hierarchy == pHierarchy )
                {
                    hei.Unadvise();
                    this.hierarchyEvents.Remove( hei );

                    // modifying an iterated-over collection, we *must* break here.
                    break;
                }
            }
        }

        private void AddHierarchyEventsImpl( IVsHierarchy hi )
        {
            try
            {
                // Set us up to listen to all of the hierarchies' events
                this.hierarchyEvents.Add( new HierarchyEventsImpl( hi, this.Context ) );
            }
            catch ( NoProjectAutomationObjectException ex )
            {
                // this is annoying, but we can proceed
                this.Context.ErrorHandler.LogException( ex, 
                    String.Format("Cannot find automation object for project {0}.\r\nSome manual refresh of project might be necessary.", 
                    ex.ProjectName) );
            }
            catch ( COMException ex )
            {
                // this is more annoying, but we can still go on with the next project
                this.Context.ErrorHandler.LogException( ex, "Unable to add hierarchy event sink for project: {0}", ex.Message );
            }
            catch ( NotImplementedException ex )
            {
                this.Context.ErrorHandler.LogException( ex, "Unable to add hierarchy event sink for project: {0}", ex.Message );
            }
            catch ( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }
        }

        /// <summary>
        /// The actual event sink for the hierarchy events.
        /// </summary>
        private class HierarchyEventsImpl : IVsHierarchyEvents, IRefreshableProject
        {
            public HierarchyEventsImpl( IVsHierarchy hierarchy, IContext context )
            {
                this.hierarchy = hierarchy;
                this.context = context;

                // first get the name of this project
                const uint root = unchecked( (uint)(int)VSITEMID.VSITEMID_ROOT );

                object projVar;
                int hr = this.hierarchy.GetProperty( root, (int)__VSHPROPID.VSHPROPID_ExtObject, out projVar );
                if ( hr == VSConstants.S_OK && projVar is EnvDTE.Project )
                {
                    this.project = (Project)projVar;
                }
                else
                {
                    string name = VSProject.GetProjectNameFromVsHierarchy( hierarchy );
                    throw new NoProjectAutomationObjectException( name );
                }

                // and then hook us up as an event sink
                try
                {
                    hr = this.hierarchy.AdviseHierarchyEvents( this, out this.cookie );

                    // Grrr, 1.x doesn't have GetExceptionForHR
                    Marshal.ThrowExceptionForHR( hr );
                }
                catch( Exception ex )
                {
                    throw new NoProjectAutomationObjectException( VSProject.GetProjectNameFromVsHierarchy(hierarchy), ex );
                }

                
            }

            public void Unadvise()
            {
                this.hierarchy.UnadviseHierarchyEvents( this.cookie );

                // setting this to null causes .IsValid to return false
                this.project = null;
            }

            public IVsHierarchy Hierarchy
            {
                get { return this.hierarchy; }
            }


            #region IVsHierarchyEvents Members

            public int OnInvalidateIcon( IntPtr hicon )
            {
                Trace.WriteLine( "Icon invalidated for " + this.project.Name );
                return VSConstants.S_OK;
            }

            public int OnInvalidateItems( uint itemidParent )
            {
                Trace.WriteLine( "Item invalidated for " + this.project.Name );
                return VSConstants.S_OK;
            }

            public int OnItemAdded( uint itemidParent, uint itemidSiblingPrev, uint itemidAdded )
            {
                Trace.WriteLine( "Item added for " + this.project.Name );

                try
                {
                    this.context.SolutionExplorer.SetUpDelayedProjectRefresh( this );
                }
                catch ( Exception ex )
                {
                    this.context.ErrorHandler.Handle( ex );
                    return VSConstants.E_FAIL;
                }
                
                return VSConstants.S_OK;
            }

            

            public int OnItemDeleted( uint itemid )
            {
                Trace.WriteLine( "Item deleted for " + this.project.Name );

                try
                {
                    this.context.SolutionExplorer.SetUpDelayedProjectRefresh( this );
                }
                catch ( Exception ex )
                {
                    context.ErrorHandler.Handle( ex );
                    return VSConstants.E_FAIL;
                }

                return VSConstants.S_OK;
            }

            public int OnItemsAppended( uint itemidParent )
            {
                Trace.WriteLine( "Item appended for " + this.project.Name );
                return VSConstants.S_OK;
            }

            public int OnPropertyChanged( uint itemid, int propid, uint flags )
            {
                Trace.WriteLine( "Property changed for " + this.project.Name );
                return VSConstants.S_OK;
            }

            Project IRefreshableProject.Project
            {
                get { return this.project; }
            }

            bool IRefreshableProject.IsValid
            {
                get { return this.project != null; }
            }

            #endregion

            private IContext context;
            private uint cookie;
            private IVsHierarchy hierarchy;
            private Project project;
        }

        //private static void DisplayHierarchy( IVsHierarchy hi, uint item )
        //{
        //    DisplayProperties( hi, item );
        //    object var;
        //    int hr = hi.GetProperty( item, (int)__VSHPROPID.VSHPROPID_FirstChild, out var );
        //    if ( hr == VSConstants.S_OK )
        //    {
        //        DisplayHierarchy( hi, (uint)(int)var );
        //    }

        //    hr = hi.GetProperty( item, (int)__VSHPROPID.VSHPROPID_NextSibling, out var );
        //    while ( hr == VSConstants.S_OK && ((int)(uint)var) != __VSHPROPID.VSHPROPID_NIL )
        //    {
        //        DisplayHierarchy( hi, (uint)(int)var );
        //        hr = hi.GetProperty( item, (int)__VSHPROPID.VSHPROPID_NextSibling, out var );
        //    }

        //}

        private ArrayList hierarchyEvents;



        
    }
}
