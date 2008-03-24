// $Id$
using System;
using EnvDTE;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell.Interop;

using IServiceProvider = System.IServiceProvider;
using Microsoft.VisualStudio;
using SharpSvn;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Event sink for solution related events.
    /// </summary>
    public class SolutionEventsSink : EventSink, IVsSolutionEvents
    {
        public event EventHandler AfterOpenSolution;
        public event EventHandler BeforeCloseSolution;

        public SolutionEventsSink( IAnkhServiceProvider context )
            : base( context )
        {
            this.AdviseSolutionEvents();
        }

        private void AdviseSolutionEvents()
        {
            this.solution = (IVsSolution)Context.GetService(typeof(SVsSolution));

            Marshal.ThrowExceptionForHR( this.solution.AdviseSolutionEvents( this, out this.cookie ) );
        }

        public override void Unhook()
        {
            this.solution.UnadviseSolutionEvents( this.cookie );
        }

        #region IVsSolutionEvents Members

        int IVsSolutionEvents.OnAfterOpenProject( IVsHierarchy pHierarchy, int fAdded )
        {
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
            if (AfterOpenSolution != null)
                AfterOpenSolution(this, EventArgs.Empty);

            return VSConstants.S_OK;
        }       

        int IVsSolutionEvents.OnQueryCloseSolution( object pUnkReserved, ref int pfCancel )
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseSolution( object pUnkReserved )
        {
            if (BeforeCloseSolution != null)
                BeforeCloseSolution(this, EventArgs.Empty);
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterCloseSolution( object pUnkReserved )
        {
            return VSConstants.S_OK;
        }
       

        #endregion

        private uint cookie;
        private IVsSolution solution;
    }
}
