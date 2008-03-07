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
using Microsoft.VisualStudio;
using SharpSvn;

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
            this.Context.SolutionExplorer.SyncAll();
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseProject( IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel )
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseProject( IVsHierarchy pHierarchy, int fRemoved )
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterLoadProject( IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy )
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryUnloadProject( IVsHierarchy pRealHierarchy, ref int pfCancel )
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeUnloadProject( IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy )
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenSolution( object pUnkReserved, int fNewSolution )
        {
            this.SetupEventsForSolution();

            return VSConstants.S_OK;
        }       

        int IVsSolutionEvents.OnQueryCloseSolution( object pUnkReserved, ref int pfCancel )
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseSolution( object pUnkReserved )
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterCloseSolution( object pUnkReserved )
        {
            return VSConstants.S_OK;
        }
       

        #endregion

        private void SetupEventsForSolution()
        {
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
                foreach ( EventSink sink in this.eventSinks )
                    sink.Unhook();

                this.eventSinks = null;
            }
        }

        private uint cookie;
        private IVsSolution solution;
        private IList eventSinks;

        
    }
}
