using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using AnkhSvn.Ids;

namespace Ankh.Scc
{
    partial class ProjectDocumentTracker : IVsSolutionEvents, IVsSolutionEvents2, IVsSolutionEvents3, IVsSolutionEvents4
    {
        #region IVsSolutionEvents Members

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            _sccProvider.OnSolutionOpened();

            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            _sccProvider.OnSolutionClosed();

            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            IVsSccProject2 project = pRealHierarchy as IVsSccProject2;

            if (project != null)
            {
                _sccProvider.OnProjectLoaded(project);
            }
            
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                _sccProvider.OnProjectOpened(project, fAdded != 0);
            }

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                _sccProvider.OnProjectClosed(project, fRemoved != 0);
            }

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            pfCancel = 0;
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSolutionEvents2 Members


        public int OnAfterMergeSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSolutionEvents3 Members


        public int OnAfterClosingChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpeningChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeClosingChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeOpeningChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSolutionEvents4 Members

        public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                _sccProvider.OnProjectOpened(project, fAdded != 0);
            }

            return VSConstants.S_OK;
        }

        public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameProject(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
