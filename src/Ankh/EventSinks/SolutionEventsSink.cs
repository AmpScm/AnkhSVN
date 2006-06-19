// $Id$
using System;
using EnvDTE;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell.Interop;

using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Event sink for solution related events.
    /// </summary>
    public class SolutionEventsSink : EventSink, IVsSolutionEvents
    {
        public event CancelEventHandler SolutionLoaded;
        public event EventHandler SolutionBeforeClosing;

        public SolutionEventsSink( IContext context )
            : base( context )
        {
            this.hierarchyEvents = new HierarchyEventsSink( this.Context );
            this.AdviseSolutionEvents();

        }

        private void AdviseSolutionEvents()
        {
            IServiceProvider sp = this.Context.ServiceProvider;
            Guid serviceGuid = typeof(SVsSolution).GUID;
            Guid interfaceGuid = typeof(IVsSolution).GUID;

            IntPtr ptr;
            Marshal.ThrowExceptionForHR(sp.QueryService( ref serviceGuid, ref interfaceGuid, out ptr ));
            this.solution = (IVsSolution)Marshal.GetObjectForIUnknown( ptr );
            Marshal.ThrowExceptionForHR( this.solution.AdviseSolutionEvents( this, out this.cookie ) );
        }

        public override void Unhook()
        {
            this.UnhookEventsForSolution();
            this.solution.UnadviseSolutionEvents( this.cookie );
        }

        public void InitializeForLoadedSolution()
        {
            this.SetupEventsForSolution();
        }

        #region IVsSolutionEvents Members

        int IVsSolutionEvents.OnAfterOpenProject( IVsHierarchy pHierarchy, int fAdded )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterOpenProject() of {0}", this.ToString() ) );

            try
            {
                // this won't be true until the solution is finally loaded
                if ( this.Context.AnkhLoadedForSolution )
                {
                    this.hierarchyEvents.AddHierarchy( pHierarchy );

                    bool shouldAdd = false;
                    VSProject newProject = null;
                    try
                    {
                        newProject = VSProject.FromVsHierarchy( this.Context, pHierarchy );
                        shouldAdd = newProject != null && !newProject.IsVersioned && newProject.IsVersionable &&
                            Context.Config.AutoAddNewFiles;
                    }
                    catch ( NoProjectAutomationObjectException ex )
                    {
                        this.Context.OutputPane.WriteLine( "Cannot determine automation object from project '{0}'.", ex.ProjectName );
                        shouldAdd = false;
                    }

                    if ( newProject != null )
                    {
                        if ( shouldAdd )
                        {
                            newProject.AddProjectToSvn(); 
                        }
                        // make sure we have an updated status for the items in that directory, otherwise they'll be seen as unversionable
                        this.Context.StatusCache.Status( newProject.ProjectDirectory );

                        this.Context.SolutionExplorer.SyncAll();
                    }
                }
            }
            catch ( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }

            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseProject( IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryCloseProject() of {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseProject( IVsHierarchy pHierarchy, int fRemoved )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnBeforeCloseProject() of {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterLoadProject( IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterLoadProject() of {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryUnloadProject( IVsHierarchy pRealHierarchy, ref int pfCancel )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryUnloadProject() of {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeUnloadProject( IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnBeforeUnloadProject() of {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenSolution( object pUnkReserved, int fNewSolution )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterOpenSolution() of {0}", this.ToString() ) );
            CancelEventArgs args = new CancelEventArgs( true );

            try
            {
                if ( this.SolutionLoaded != null )
                    this.SolutionLoaded( this, args );
                if ( !args.Cancel )
                {
                    this.SetupEventsForSolution();

                }

            }
            catch ( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
                return VSConstants.E_FAIL;
            }
            
            return VSConstants.S_OK;
        }

       

        int IVsSolutionEvents.OnQueryCloseSolution( object pUnkReserved, ref int pfCancel )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryCloseSolution() of {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseSolution( object pUnkReserved )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnBeforeCloseSolution() of {0}", this.ToString() ) );

            try
            {
                this.UnhookEventsForSolution();

                if ( this.SolutionBeforeClosing != null )
                    this.SolutionBeforeClosing( this, EventArgs.Empty );
            }
            catch ( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
                return VSConstants.E_FAIL;
            }
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterCloseSolution( object pUnkReserved )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterCloseSolution() of {0}", this.ToString() ) );

            return VSConstants.S_OK;
        }

       

        #endregion

        private void SetupEventsForSolution()
        {
            this.hierarchyEvents = new HierarchyEventsSink( this.Context );

            this.eventSinks = new ArrayList();

            this.eventSinks.Add( new TrackProjectDocumentsEventSink( this.Context ) );
            this.eventSinks.Add( new ProjectFilesEventSink( this.Context ) );
            this.eventSinks.Add( new DocumentEventsSink( this.Context ) );
            this.eventSinks.Add( new CommandsEventSink( this.Context ) );
        }

        private void UnhookEventsForSolution()
        {
            if ( this.Context.AnkhLoadedForSolution )
            {
                this.hierarchyEvents.Unhook();
                this.hierarchyEvents = null;

                foreach ( EventSink sink in this.eventSinks )
                    sink.Unhook();

                this.eventSinks = null;
            }
        }

        private HierarchyEventsSink hierarchyEvents;
        private uint cookie;
        private IVsSolution solution;
        private IList eventSinks;

        
    }
}
