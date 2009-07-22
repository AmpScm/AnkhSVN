// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Ids;
using Ankh.Selection;

namespace Ankh.Scc
{
    partial class ProjectTracker : IVsSolutionEvents, IVsSolutionEvents2, IVsSolutionEvents3, IVsSolutionEvents4, IVsSolutionEventsProjectUpgrade
    {
        #region IVsSolutionEvents Members

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            _solutionLoaded = true;
            SccProvider.OnSolutionOpened(true);

            GetService<IAnkhServiceEvents>().OnSolutionOpened(EventArgs.Empty);

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            _solutionLoaded = false;
            SccProvider.OnStartedSolutionClose();

            IAnkhTaskManager conflicts = GetService<IAnkhTaskManager>();

            if (conflicts != null)
                conflicts.OnCloseSolution();

            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            SccProvider.OnSolutionClosed();
            SccStore.OnSolutionClosed();

            GetService<IAnkhServiceEvents>().OnSolutionClosed(EventArgs.Empty);

            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            IVsSccProject2 project = pRealHierarchy as IVsSccProject2;

            if (project != null)
            {
                SccProvider.OnProjectLoaded(project);
            }
            
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                SccProvider.OnProjectOpened(project, fAdded != 0);
            }
            //else
            //{
            //  IVsSccVirtualFolders vf = pHierarchy as IVsSccVirtualFolders; // Available for webprojects on a server
            //}

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                SccProvider.OnProjectClosed(project, fRemoved != 0);
            }

            return VSConstants.S_OK;
        }        

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            IVsSccProject2 project = pRealHierarchy as IVsSccProject2;

            if (project != null)
            {
                SccProvider.OnProjectBeforeUnload(project, pStubHierarchy);
            }

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
                SccProvider.OnProjectOpened(project, fAdded != 0);
            }

            return VSConstants.S_OK;
        }

        public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameProject(IVsHierarchy pHierarchy)
        {
            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                // SccProvider forwards this to the SccStore
                SccProvider.OnProjectRenamed(project);
            }

            return VSConstants.S_OK;
        }

        public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
        {
            pfCancel = 0;
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSolutionEventsProjectUpgrade Members

        /// <summary>
        /// Defines a method to call after a project upgrade.
        /// </summary>
        /// <param name="pHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy"></see> interface of the project.</param>
        /// <param name="fUpgradeFlag">[in] Integer. Flag indicating the nature of the upgrade. Values taken from the <see cref="T:Microsoft.VisualStudio.Shell.Interop.__VSPPROJECTUPGRADEVIAFACTORYFLAGS"></see> enumeration. Will only be PUVFF_COPYUPGRADE, PUVFF_SXSBACKUP, or PUVFF_COPYBACKUP.</param>
        /// <param name="bstrCopyLocation">[in] String containing the location of the copy upgrade (PUVFF_COPYUPGRADE) or back up copy (PUVFF_COPYBACKUP).</param>
        /// <param name="stUpgradeTime">[in] A <see cref="T:Microsoft.VisualStudio.Shell.Interop.SYSTEMTIME"></see> value. The time the upgrade was done.</param>
        /// <param name="pLogger">[in] Pointer to an <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsUpgradeLogger"></see> interface to use for logging upgrade messages.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterUpgradeProject(IVsHierarchy pHierarchy, uint fUpgradeFlag, string bstrCopyLocation, SYSTEMTIME stUpgradeTime, IVsUpgradeLogger pLogger)
        {
            IProjectFileMapper mapper = GetService<IProjectFileMapper>();
            IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();

            if(monitor == null || mapper == null)
                return VSConstants.S_OK;

            if (!string.IsNullOrEmpty(bstrCopyLocation))
                monitor.ScheduleSvnStatus(bstrCopyLocation);
                
            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                ISvnProjectInfo info = mapper.GetProjectInfo(new SvnProject(null, project));

                if (info != null && !string.IsNullOrEmpty(info.ProjectFile))
                    monitor.ScheduleSvnStatus(info.ProjectFile);
            }

            return VSConstants.S_OK;
        }

        #endregion
    }
}
