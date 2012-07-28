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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Ankh.Scc;
using Ankh.VSPackage.Attributes;
using Ankh.VS;
using System.IO;
using Ankh.Scc.Native;

namespace Ankh.VSPackage
{
    [ProvideSolutionProperties(AnkhSvnPackage.SubversionPropertyCategory)]
    [ProvideSolutionProperties(AnkhId.SvnOriginName)]
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

            return VSErr.S_OK;
        }

        IAnkhSccService _scc;
        IAnkhSccService Scc
        {
            get { return _scc ?? (_scc = GetService<IAnkhSccService>()); }
        }

        // Global note: 
        // The same trick we do here for the solution (loading the package when encountering a solution property) 
        // can be done on several project types using IVsProjectStartupServices
        public int QuerySaveSolutionProps(IVsHierarchy pHierarchy, VSQUERYSAVESLNPROPS[] pqsspSave)
        {
            try
            {
                // This function is called by the IDE to determine if something needs to be saved in the solution.
                // If the package returns that it has dirty properties, the shell will callback on SaveSolutionProps

                if (Scc == null || !Scc.IsSolutionManaged)
                {
                    pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasNoProps;
                    return VSErr.S_OK;
                }

                if (pHierarchy == null)
                {
                    if (Scc.IsSolutionDirty)
                        pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasDirtyProps;
                    else
                        pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasNoDirtyProps;
                }
                else
                {
                    if (!Scc.HasProjectProperties(pHierarchy))
                        pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasNoProps;
                    else if (Scc.IsSolutionDirty)
                        pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasDirtyProps;
                    else
                        pqsspSave[0] = VSQUERYSAVESLNPROPS.QSP_HasNoDirtyProps;
                }
                return VSErr.S_OK;
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);

                return Marshal.GetHRForException(ex);
            }
        }


        public int SaveSolutionProps(IVsHierarchy pHierarchy, IVsSolutionPersistence pPersistence)
        {
            try
            {
                int hr = VSErr.S_OK;

                // This function gets called by the shell after QuerySaveSolutionProps returned QSP_HasDirtyProps

                // The package will pass in the key under which it wants to save its properties, 
                // and the IDE will call back on WriteSolutionProps

                // The properties will be saved in the Pre-Load section
                // When the solution will be reopened, the IDE will call our package to load them back before the projects in the solution are actually open
                // This could help if the source control package needs to persist information like projects translation tables, that should be read from the suo file
                // and should be available by the time projects are opened and the shell start calling IVsSccEnlistmentPathTranslation functions.
                if (Scc != null && Scc.IsSolutionManaged)
                {
                    if (pHierarchy == null)
                        hr = pPersistence.SavePackageSolutionProps(1 /* fPreLoad */, pHierarchy, this, SubversionPropertyCategory);
                    else if (Scc.HasProjectProperties(pHierarchy))
                        hr = pPersistence.SavePackageSolutionProps(1 /* fPreLoad */, pHierarchy, this, AnkhId.SvnOriginName);

                    // Once we saved our props, the solution is not dirty anymore
                    if (VSErr.Succeeded(hr))
                        Scc.IsSolutionDirty = false;
                }

                return hr;
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);

                return Marshal.GetHRForException(ex);
            }
        }

        int IVsPersistSolutionProps.WriteSolutionProps(IVsHierarchy pHierarchy, string pszKey, IPropertyBag pPropBag)
        {
            if (Scc == null)
                return VSErr.S_OK;
            else if (pPropBag == null)
                return VSErr.E_POINTER;

            try
            {
                // This method is called from the VS implementation after a request from SaveSolutionProps
                using (IPropertyMap map = new PropertyBag(pPropBag))
                {
                    switch (pszKey)
                    {
                        case SubversionPropertyCategory:
                            map.SetRawValue(ManagedPropertyName, true.ToString());
                            // BH: Don't localize this text! Changing it will change all solutions marked as managed by Ankh
                            map.SetRawValue(ManagerPropertyName, "AnkhSVN - Subversion Support for Visual Studio");
                            break;
                        case AnkhId.SvnOriginName:
                            Scc.StoreProjectProperties(pHierarchy, map);
                            break;
                    }
                }

                return VSErr.S_OK;
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);

                return Marshal.GetHRForException(ex);
            }
        }

        public int ReadSolutionProps(IVsHierarchy pHierarchy, string pszProjectName, string pszProjectMk, string pszKey, int fPreLoad, IPropertyBag pPropBag)
        {
            try
            {
                using (IPropertyMap map = new PropertyBag(pPropBag))
                {
                    bool preload = (fPreLoad != 0);

                    switch (pszKey)
                    {
                        case SubversionPropertyCategory:
                            if (preload && pHierarchy == null)
                            {
                                string value;
                                bool register;

                                if (!map.TryGetValue(ManagedPropertyName, out value))
                                    register = false;
                                else if (string.IsNullOrEmpty(value) || !bool.TryParse(value, out register))
                                    register = false;

                                if (register)
                                {
                                    Scc.RegisterAsPrimarySccProvider();

                                    Scc.LoadingManagedSolution(true);
                                }
                            }
                            break;
                        case AnkhId.SvnOriginName:
                            Scc.ReadProjectProperties(pHierarchy, pszProjectName, pszProjectMk, map);
                            break;
                    }
                }
                return VSErr.S_OK;
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);

                return Marshal.GetHRForException(ex);
            }
        }

        #region IVsPersistSolutionOpts
        const string SccPendingChangeStream = AnkhId.SubversionSccName + "Pending";
        const string SccEnlistStream = AnkhId.SubversionSccName + "Enlist";
        const string SccExcludedStream = AnkhId.SubversionSccName + "SccExcluded";

        public int LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts)
        {
            if ((grfLoadOpts & (uint)__VSLOADUSEROPTS.LUO_OPENEDDSW) != 0)
            {
                return VSErr.S_OK; // We only know .suo; let's ignore old style projects
            }

            try
            {
                pPersistence.LoadPackageUserOpts(this, SccPendingChangeStream);
                pPersistence.LoadPackageUserOpts(this, SccExcludedStream);
                pPersistence.LoadPackageUserOpts(this, SccEnlistStream);

                return VSErr.S_OK;
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);

                return Marshal.GetHRForException(ex);
            }
            finally
            {
                if (Marshal.IsComObject(pPersistence))
                    Marshal.ReleaseComObject(pPersistence); // See Package.cs from MPF for reason
            }
        }

        public int ReadUserOptions([In] IStream pOptionsStream, [In] string pszKey)
        {
            try
            {
                using (ComStreamWrapper wrapper = new ComStreamWrapper(pOptionsStream, true))
                {
                    IAnkhSccService scc;
                    switch (pszKey)
                    {
                        case SccPendingChangeStream:
                            LoadPendingChanges(wrapper);
                            break;
                        case SccEnlistStream:
                            scc = GetService<IAnkhSccService>();
                            if (scc != null)
                                scc.SerializeEnlistData(wrapper, false);
                            break;
                        case SccExcludedStream:
                            scc = GetService<IAnkhSccService>();
                            if (scc != null)
                                scc.SerializeSccExcludeData(wrapper, false);
                            break;

                        default:
                            // TODO: Add support for some service api for our services
                            break;
                    }
                }
                return VSErr.S_OK; // Our data is in subversion properties
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);

                return Marshal.GetHRForException(ex);
            }
            finally
            {
                if (Marshal.IsComObject(pOptionsStream))
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
                    if (!scc.IsActive)
                        return VSErr.S_OK;

                    pPersistence.SavePackageUserOpts(this, SccPendingChangeStream);
                    pPersistence.SavePackageUserOpts(this, SccExcludedStream);

                    if (scc.IsSolutionManaged)
                    {
                        pPersistence.SavePackageUserOpts(this, SccEnlistStream);
                    }
                }

                return VSErr.S_OK;
            }
            finally
            {
                if (Marshal.IsComObject(pPersistence))
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
                        case SccExcludedStream:
                            scc = GetService<IAnkhSccService>();
                            if (scc != null)
                                scc.SerializeSccExcludeData(wrapper, true);
                            break;
                        default:
                            // TODO: Add support for some service api for our services
                            break;
                    }
                }

                return VSErr.S_OK;
            }
            finally
            {
                if (Marshal.IsComObject(pOptionsStream))
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

            if (monitor == null)
                return;

            using (BinaryReader br = new BinaryReader(storageStream))
            {
                int n = br.ReadInt32();
                List<string> files = new List<string>();

                for (int i = 0; i < n; i++)
                {
                    files.Add(br.ReadString());
                }

                monitor.ScheduleMonitor(files);
            }
        }
        #endregion
    }
}
