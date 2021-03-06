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
using System.IO;
using System.Windows.Forms;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.Configuration;
using Ankh.Scc.SccUI;
using Ankh.Selection;
using Ankh.VS;
using System.Runtime.InteropServices;
using SharpSvn;



namespace Ankh.Scc
{
    partial class ProjectTracker : IVsSolutionEvents, IVsSolutionEvents2, IVsSolutionEvents3, IVsSolutionEvents4, IVsSolutionEventsProjectUpgrade
    {
        #region IVsSolutionEvents Members

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            _solutionLoaded = true;
            SccEvents.OnSolutionOpened(true);

            GetService<IAnkhServiceEvents>().OnSolutionOpened(EventArgs.Empty);

            if (!SccProvider.IsActive)
                return VSErr.S_OK;
            try
            {
                VerifySolutionNaming();

                IAnkhSolutionSettings ss = GetService<IAnkhSolutionSettings>();

                if (ss != null && ss.ProjectRoot != null)
                {
                    string rootDir = Path.GetPathRoot(ss.ProjectRoot);
                    if (rootDir.Length == 3 && rootDir.EndsWith(":\\", StringComparison.OrdinalIgnoreCase))
                    {
                        DriveInfo di = new DriveInfo(rootDir);
                        bool oldFs = false;

                        switch ((di.DriveFormat ?? "").ToUpperInvariant())
                        {
                            case "FAT32":
                            case "FAT":
                                oldFs = true;
                                break;
                        }

                        if (oldFs)
                        {
                            IAnkhConfigurationService cs = GetService<IAnkhConfigurationService>();

                            if (!cs.GetWarningBool(AnkhWarningBool.FatFsFound))
                            {
                                using (SccFilesystemWarningDialog dlg = new SccFilesystemWarningDialog())
                                {
                                    dlg.Text = Path.GetFileName(ss.SolutionFilename);
                                    if (DialogResult.OK == dlg.ShowDialog(Context))
                                    {
                                        cs.SetWarningBool(AnkhWarningBool.FatFsFound, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler.IsEnabled(ex))
                    handler.OnError(ex);
                else
                    throw;
            }

            return VSErr.S_OK;
        }

        private void VerifySolutionNaming()
        {
            IVsSolution sol = GetService<IVsSolution>(typeof(SVsSolution));

            string dir, path, user;

            if (sol == null
                || !VSErr.Succeeded(sol.GetSolutionInfo(out dir, out path, out user))
                || string.IsNullOrEmpty(path))
            {
                return;
            }

            string trueSln = SvnTools.GetTruePath(path, true) ?? SvnTools.GetNormalizedFullPath(path);

            if (trueSln == path)
                return; // Nothing to do for us

            IVsRunningDocumentTable rdt = GetService<IVsRunningDocumentTable>(typeof(SVsRunningDocumentTable));

            if (rdt == null)
                return;

            Guid IID_hier = typeof(IVsHierarchy).GUID;
            IntPtr hier = IntPtr.Zero;
            IntPtr unk = Marshal.GetIUnknownForObject(sol);
            IntPtr ppunkDocData = IntPtr.Zero;
            try
            {
                IVsHierarchy slnHier;
                uint pitemid;
                uint pdwCookie;

                if (!VSErr.Succeeded(rdt.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_EditLock, path, out slnHier, out pitemid, out ppunkDocData, out pdwCookie)))
                    return;
                if (!VSErr.Succeeded(Marshal.QueryInterface(unk, ref IID_hier, out hier)))
                {
                    hier = IntPtr.Zero;
                    return;
                }

                if (VSErr.Succeeded(rdt.RenameDocument(path, trueSln, hier, VSItemId.Root)))
                {
                    int hr;

                    hr = rdt.SaveDocuments((uint)(__VSRDTSAVEOPTIONS.RDTSAVEOPT_ForceSave | __VSRDTSAVEOPTIONS.RDTSAVEOPT_SaveNoChildren),
                                           slnHier, pitemid, pdwCookie);

                    hr = sol.SaveSolutionElement((uint)(__VSSLNSAVEOPTIONS.SLNSAVEOPT_ForceSave), (IVsHierarchy)sol, pdwCookie);

                    //GC.KeepAlive(hr);
                }
                if (ppunkDocData != IntPtr.Zero)
                {
                    object doc = Marshal.GetObjectForIUnknown(ppunkDocData);
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.Release(unk);
                if (hier != IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.Release(hier);
                if (ppunkDocData != IntPtr.Zero)
                    Marshal.Release(hier);
            }
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            _solutionLoaded = false;
            if (SccProvider.IsActive)
                SccEvents.OnStartedSolutionClose();

            return VSErr.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            if (SccProvider.IsActive)
            {
                SccEvents.OnSolutionClosed();
            }

            GetService<IAnkhServiceEvents>().OnSolutionClosed(EventArgs.Empty);

            return VSErr.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            if (!SccProvider.IsActive)
                return VSErr.S_OK;

            IVsSccProject2 project = pRealHierarchy as IVsSccProject2;

            if (project != null)
            {
                SccEvents.OnProjectLoaded(project);
            }
            
            return VSErr.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            if (!SccProvider.IsActive)
                return VSErr.S_OK;

            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                SccEvents.OnProjectOpened(project, fAdded != 0);
            }
            //else
            //{
            //  IVsSccVirtualFolders vf = pHierarchy as IVsSccVirtualFolders; // Available for webprojects on a server
            //}

            return VSErr.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            if (!SccProvider.IsActive)
                return VSErr.S_OK;

            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                SccEvents.OnProjectClosed(project, fRemoved != 0);
            }

            return VSErr.S_OK;
        }        

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            if (!SccProvider.IsActive)
                return VSErr.S_OK;

            IVsSccProject2 project = pRealHierarchy as IVsSccProject2;

            if (project != null)
            {
                SccEvents.OnProjectBeforeUnload(project, pStubHierarchy);
            }

            return VSErr.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            pfCancel = 0;
            return VSErr.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSErr.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSErr.S_OK;
        }

        #endregion

        #region IVsSolutionEvents2 Members


        public int OnAfterMergeSolution(object pUnkReserved)
        {
            return VSErr.S_OK;
        }

        #endregion

        #region IVsSolutionEvents3 Members


        public int OnAfterClosingChildren(IVsHierarchy pHierarchy)
        {
            return VSErr.S_OK;
        }

        public int OnAfterOpeningChildren(IVsHierarchy pHierarchy)
        {
            return VSErr.S_OK;
        }

        public int OnBeforeClosingChildren(IVsHierarchy pHierarchy)
        {
            return VSErr.S_OK;
        }

        public int OnBeforeOpeningChildren(IVsHierarchy pHierarchy)
        {
            return VSErr.S_OK;
        }

        #endregion

        #region IVsSolutionEvents4 Members

        public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            if (!SccProvider.IsActive)
                return VSErr.S_OK;

            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                SccEvents.OnProjectOpened(project, fAdded != 0);
            }

            return VSErr.S_OK;
        }

        public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
        {
            return VSErr.S_OK;
        }

        public int OnAfterRenameProject(IVsHierarchy pHierarchy)
        {
            if (!SccProvider.IsActive)
                return VSErr.S_OK;

            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                // SccProvider forwards this to the SccStore
                SccEvents.OnProjectRenamed(project);
            }

            return VSErr.S_OK;
        }

        public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
        {
            pfCancel = 0;
            return VSErr.S_OK;
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
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSErr.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int OnAfterUpgradeProject(IVsHierarchy pHierarchy, uint fUpgradeFlag, string bstrCopyLocation, SYSTEMTIME stUpgradeTime, IVsUpgradeLogger pLogger)
        {
            if (!SccProvider.IsActive)
                return VSErr.S_OK;

            IProjectFileMapper mapper = GetService<IProjectFileMapper>();
            IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();

            if(monitor == null || mapper == null)
                return VSErr.S_OK;

            if (SccProvider.IsSafeSccPath(bstrCopyLocation))
                monitor.ScheduleSvnStatus(bstrCopyLocation);
                
            IVsSccProject2 project = pHierarchy as IVsSccProject2;

            if (project != null)
            {
                ISccProjectInfo info = mapper.GetProjectInfo(new SccProject(null, project));

                if (info != null && !string.IsNullOrEmpty(info.ProjectFile))
                    monitor.ScheduleSvnStatus(info.ProjectFile);
            }

            return VSErr.S_OK;
        }

        #endregion
    }
}
