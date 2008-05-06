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
                else if (!scc.IsProjectManaged(null))
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

                if (scc != null && scc.IsProjectManaged(null))
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
                if (scc != null)
                {
                    if (scc.IsProjectManaged(null))
                    {
                        obj = true.ToString();
                        pPropBag.Write(ManagedPropertyName, ref obj);
                    
                        // BH: Don't localize this text! Changing it will change all solutions marked as managed by Ankh
                        obj = "AnkhSVN - Subversion Support for Visual Studio";
                        pPropBag.Write(ManagerPropertyName, ref obj);
                    }
                }
            }

            return VSConstants.S_OK;
        }

        public int ReadSolutionProps(IVsHierarchy pHierarchy, string pszProjectName, string pszProjectMk, string pszKey, int fPreLoad, IPropertyBag pPropBag)
        {
            // This function gets called by the shell when a solution with one of our property categories is opened in the IDE.

            if (SubversionPropertyCategory == pszKey)
            {
                IAnkhSccService scc = GetService<IAnkhSccService>();
                if (scc != null)
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
                }
            }

            return VSConstants.S_OK;
        }        

        #region IVsPersistSolutionOpts
        public int LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts)
        {
            return VSConstants.S_OK;
        }

        public int ReadUserOptions([In] IStream pOptionsStream, [In] string pszKey)
        {
            return VSConstants.S_OK; // Our data is in subversion properties
        }        

        public int SaveUserOptions([In] IVsSolutionPersistence pPersistence)
        {
            return VSConstants.S_OK;
        }

        public int WriteUserOptions([In] IStream pOptionsStream, [In] string pszKey)
        {
            return VSConstants.E_NOTIMPL;
        }
        #endregion
    }
}
