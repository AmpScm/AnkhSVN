// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Ankh.Ids;
using Ankh.Scc;
using Ankh.VSPackage.Attributes;
using Ankh.VS;
using System.IO;

namespace Ankh.VSPackage
{
    [ProvideSolutionProperties(AnkhSvnPackage.SubversionPropertyCategory)]
    partial class AnkhSvnPackage : IVsPersistSolutionProps
    {
        const string SubversionPropertyCategory = AnkhId.SubversionSccName;
        const string ManagedPropertyName = "Svn-Managed";
        const string ManagerPropertyName = "Manager";

        public int OnProjectLoadFailure(IVsHierarchy pStubHierarchy, string pszProjectName, string pszProjectMk, string pszKey)
        {
            IAnkhSccService scc = GetService<IAnkhSccService>();

            if (scc != null)
                scc.IsSolutionDirty = true; // We should save our settings again

            return VSConstants.S_OK;
        }

        // Global note: 
        // The same trick we do here for the solution (loading the package when encountering a solution property) 
        // can be done on several project types using IVsProjectStartupServices

        public int QuerySaveSolutionProps(IVsHierarchy pHierarchy, VSQUERYSAVESLNPROPS[] pqsspSave)
        {
            // This function is called by the IDE to determine if something needs to be saved in the solution.
            // If the package returns that it has dirty properties, the shell will callback on SaveSolutionProps

            if (pHierarchy == null)
            {
                // We will write solution properties only for the solution

                IAnkhSccService scc = GetService<IAnkhSccService>();

                if (scc == null)
                {
                    // Nothing to save, nothing loaded
                    pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasNoProps;
                }
                else if (scc.IsSolutionDirty)
                {
                    // Something changed -> Save
                    pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasDirtyProps;
                }
                else if (!scc.IsSolutionManaged)
                {
                    // Nothing changed and unmanaged; not adding anything
                    pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasNoProps;
                }
                else
                {
                    // Nothing changed
                    pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasNoDirtyProps;
                }
            }

            return VSConstants.S_OK;
        }


        public int SaveSolutionProps(IVsHierarchy pHierarchy, IVsSolutionPersistence pPersistence)
        {
            // This function gets called by the shell after QuerySaveSolutionProps returned QSP_HasDirtyProps

            // The package will pass in the key under which it wants to save its properties, 
            // and the IDE will call back on WriteSolutionProps

            // The properties will be saved in the Pre-Load section
            // When the solution will be reopened, the IDE will call our package to load them back before the projects in the solution are actually open
            // This could help if the source control package needs to persist information like projects translation tables, that should be read from the suo file
            // and should be available by the time projects are opened and the shell start calling IVsSccEnlistmentPathTranslation functions.
            if (pHierarchy == null) // Only save the property on the solution itself
            {
                IAnkhSccService scc = GetService<IAnkhSccService>();

                if (scc != null && scc.IsSolutionManaged)
                {
                    // Calls WriteSolutionProps for us
                    pPersistence.SavePackageSolutionProps(1, pHierarchy, this, SubversionPropertyCategory);
                }

                // Once we saved our props, the solution is not dirty anymore
                scc.IsSolutionDirty = false;
            }

            return VSConstants.S_OK;
        }

        public int WriteSolutionProps(IVsHierarchy pHierarchy, string pszKey, IPropertyBag pPropBag)
        {
            if (pszKey == SubversionPropertyCategory)
            {
                // Ankh will only save two properties in the solution, to indicate that solution is controlled
                object obj;

                IAnkhSccService scc = GetService<IAnkhSccService>();
                if (scc == null || !scc.IsSolutionManaged)
                    return VSConstants.S_OK;

                obj = true.ToString();
                pPropBag.Write(ManagedPropertyName, ref obj);

                // BH: Don't localize this text! Changing it will change all solutions marked as managed by Ankh
                obj = "AnkhSVN - Subversion Support for Visual Studio";
                pPropBag.Write(ManagerPropertyName, ref obj);

                scc.WriteEnlistments(pPropBag);
            }

            return VSConstants.S_OK;
        }

        public int ReadSolutionProps(IVsHierarchy pHierarchy, string pszProjectName, string pszProjectMk, string pszKey, int fPreLoad, IPropertyBag pPropBag)
        {
            // This function gets called by the shell when a solution with one of our property categories is opened in the IDE.

            if (pHierarchy == null && pszKey == SubversionPropertyCategory)
            {
                IAnkhSccService scc = GetService<IAnkhSccService>();
                if (scc != null && fPreLoad != 0)
                {
                    // We were called to read the key written by this source control provider
                    // Make sure we were marked as primary Scc provider on this solution

                    object pVar;
                    pPropBag.Read(ManagedPropertyName, out pVar, null, 0, null);

                    string val = pVar as string;

                    bool result = false;
                    if (val != null && bool.TryParse(val, out result) && result)
                    {
                        // (This is how automatic source control provider switching on solution opening should be implemented)

                        scc.RegisterAsPrimarySccProvider();
                    }

                    if (result)
                    {
                        // TODO: One day we might implement AnkhSvn as secondary solution here

                        scc.LoadingManagedSolution(result);
                    }

                    scc.LoadEnlistments(pPropBag);
                }
            }

            return VSConstants.S_OK;
        }

        #region IVsPersistSolutionOpts
        const string SccPendingChangeStream = AnkhId.SubversionSccName + "Pending";
        const string SccEnlistStream = AnkhId.SubversionSccName + "Enlist";

        public int LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts)
        {
            if ((grfLoadOpts & (uint)__VSLOADUSEROPTS.LUO_OPENEDDSW) != 0)
            {
                return VSConstants.S_OK; // We only know .suo; let's ignore old style projects
            }

            try
            {
                pPersistence.LoadPackageUserOpts(this, SccPendingChangeStream);
                pPersistence.LoadPackageUserOpts(this, SccEnlistStream);

                return VSConstants.S_OK;
            }
            finally
            {
                Marshal.ReleaseComObject(pPersistence); // See Package.cs from MPF for reason
            }
        }

        public int ReadUserOptions([In] IStream pOptionsStream, [In] string pszKey)
        {
            try
            {
                using (ComStreamWrapper wrapper = new ComStreamWrapper(pOptionsStream, true))
                {
                    switch (pszKey)
                    {
                        case SccPendingChangeStream:
                            LoadPendingChanges(wrapper);
                            break;
                        case SccEnlistStream:
                            IAnkhSccService scc = GetService<IAnkhSccService>();
                            if (scc != null)
                                scc.SerializeEnlistData(wrapper, false);
                            break;

                        default:
                            // TODO: Add support for some service api for our services
                            break;
                    }
                }
                return VSConstants.S_OK; // Our data is in subversion properties
            }
            finally
            {
                Marshal.ReleaseComObject(pOptionsStream); // See Package.cs from MPF for reason
            }
        }

        public int SaveUserOptions([In] IVsSolutionPersistence pPersistence)
        {
            try
            {
                IAnkhSccService scc = GetService<IAnkhSccService>();
                if (scc != null)
                {
                    if (scc.IsActive && scc.IsSolutionManaged)
                    {
                        pPersistence.SavePackageUserOpts(this, SccPendingChangeStream);
                        pPersistence.SavePackageUserOpts(this, SccEnlistStream);
                    }
                }

                return VSConstants.S_OK;
            }
            finally
            {
                Marshal.ReleaseComObject(pPersistence); // See Package.cs from MPF for reason
            }
        }

        public int WriteUserOptions([In] IStream pOptionsStream, [In] string pszKey)
        {
            try
            {
                using (ComStreamWrapper wrapper = new ComStreamWrapper(pOptionsStream))
                {
                    IAnkhSccService scc;
                    switch (pszKey)
                    {
                        case SccPendingChangeStream:
                            WritePendingChanges(wrapper);
                            break;
                        case SccEnlistStream:
                            scc = GetService<IAnkhSccService>();
                            if (scc != null)
                                scc.SerializeEnlistData(wrapper, true);
                            break;
                        default:
                            // TODO: Add support for some service api for our services
                            break;
                    }
                }

                return VSConstants.S_OK;
            }
            finally
            {
                Marshal.ReleaseComObject(pOptionsStream); // See Package.cs from MPF for reason
            }
        }

        private void WritePendingChanges(Stream storageStream)
        {
            IPendingChangesManager pendingChanges = GetService<IPendingChangesManager>();

            using (BinaryWriter bw = new BinaryWriter(storageStream))
            {
                List<PendingChange> changes = (pendingChanges != null) ? new List<PendingChange>(pendingChanges.GetAll()) : null;

                if (changes == null)
                    bw.Write((int)0);
                else
                {
                    bw.Write((int)changes.Count);

                    foreach (PendingChange pc in changes)
                    {
                        bw.Write(pc.FullPath);
                    }
                }
            }
        }

        private void LoadPendingChanges(Stream storageStream)
        {
            IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();

            if(monitor == null)
                return;

            using (BinaryReader br = new BinaryReader(storageStream))
            {
                int n = br.ReadInt32();
                List<string> files = new List<string>();

                for(int i = 0; i < n; i++)
                {
                    files.Add(br.ReadString());
                }

                monitor.ScheduleMonitor(files);                    
            }
        }
        #endregion
    }
}
