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
            int hr = solution.GetProjectEnum( (uint)VSENUMPROJFLAGS.EPF_ALLPROJECTS, ref guid, out enumerator );

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

        private void AddHierarchyEventsImpl( IVsHierarchy hi )
        {
            try
            {
                // Set us up to listen to all of the hierarchies' events
                this.hierarchyEvents.Add( new HierarchyEventsImpl( hi, this.Context ) );
                throw new NoProjectAutomationObjectException();
            }
            catch ( NoProjectAutomationObjectException ex )
            {
                // this is annoying, but we can proceed
                this.Context.ErrorHandler.LogException( ex, 
                    "Cannot find automation object for project. Some manual refresh of project might be necessary." );
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

        private class NoProjectAutomationObjectException : Exception
        {
        }
        /// <summary>
        /// The actual event sink for the hierarchy events.
        /// </summary>
        private class HierarchyEventsImpl : IVsHierarchyEvents
        {
            public HierarchyEventsImpl( IVsHierarchy hierarchy, IContext context )
            {
                this.hierarchy = hierarchy;
                this.context = context;

                // first get the name of this project
                const uint root = unchecked( (uint)(int)VSITEMID.VSITEMID_ROOT );

                object var;
                int hr = this.hierarchy.GetProperty( root, (int)VSHPROPID.VSHPROPID_Name, out var );
                System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(hr);

                this.project = this.GetProjectForName( (string)var );
                // VS 2005 web projects use the project dir for their name
                if ( this.project == null )
                {
                    hr = this.hierarchy.GetProperty( root, (int)VSHPROPID.VSHPROPID_ProjectDir, out var );
                    System.Runtime.InteropServices.Marshal.ThrowExceptionForHR( hr );
                    this.project = GetProjectForName( (string)var );
                }

                if ( this.project == null )
                {
                    throw new NoProjectAutomationObjectException();
                }

                // and then hook us up as an event sink
                hr = this.hierarchy.AdviseHierarchyEvents( this, out this.cookie );
                System.Runtime.InteropServices.Marshal.ThrowExceptionForHR( hr );

                
            }

            public void Unadvise()
            {
                this.hierarchy.UnadviseHierarchyEvents( this.cookie );
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
                    this.SetUpRefreshCallback();
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
                    this.SetUpRefreshCallback();
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

            private Project GetProjectForName( string name )
            {
                foreach ( Project project in Enumerators.EnumerateProjects( this.context.DTE ) )
                {
                    Project proj = CheckProjectForName( project, name );
                    if ( proj != null )
                        return proj;

                }
                return null;
            }

            private Project CheckProjectForName( Project project, string name )
            {
                if ( project.Name == name )
                    return project;
                else
                {
                    string projectDir = PathUtils.NormalizePath( project.Name );
                    string projectName = PathUtils.NormalizePath( name );
                    if ( projectDir == projectName )
                        return project;
                }
                foreach ( ProjectItem item in Enumerators.EnumerateProjectItems(project.ProjectItems) )
                {
                    Project subProject = null;

                    if ( item.SubProject != null )
                    {
                        subProject = item.SubProject;
                    }
                    else if ( item.Object is Project )
                    {
                        subProject = item.Object as Project;
                    }

                    if ( subProject != null )
                    {
                        subProject = this.CheckProjectForName( subProject, name );
                        if ( subProject != null )
                        {
                            return subProject;
                        }
                    }                   
                }

                return null;
            }



            private System.Threading.Timer timer;
            /// <summary>
            /// Since the ItemAdded event is fired before IVTDPE.OnAfterAddedFilesEx, we need to set up a 
            /// refresh after a certain interval.
            /// </summary>
            private void SetUpRefreshCallback()
            {
                lock ( this )
                {
                    // Avoid multiple refreshes if more things are added simultaneously
                    if ( !this.refreshPending )
                    {
                        this.refreshPending = true;
                        this.timer = new System.Threading.Timer(
                           new TimerCallback( this.RefreshCallback ), null, REFRESHDELAY ,
                           Timeout.Infinite );
                    }
                }
            }

            private void RefreshCallback( object state )
            {
                if ( this.context.UIShell.SynchronizingObject.InvokeRequired )
                {
                    this.context.UIShell.SynchronizingObject.Invoke( new System.Threading.TimerCallback( this.RefreshCallback ),
                        new object[] { null } );
                    return;
                }

                this.context.SolutionExplorer.Refresh( this.project );

                lock ( this )
                {
                    this.refreshPending = false;
                }
            }

            #endregion

            private IContext context;
            private uint cookie;
            private IVsHierarchy hierarchy;
            private Project project;

            private bool refreshPending;
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
