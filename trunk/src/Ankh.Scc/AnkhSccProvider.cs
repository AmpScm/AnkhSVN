using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SharpSvn;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Ankh.Ids;
using Ankh.Scc.ProjectMap;
using Ankh.Selection;
using Ankh.VS;

namespace Ankh.Scc
{
    [GuidAttribute(AnkhId.SccProviderId), CLSCompliant(false)]
    public interface ITheAnkhSvnSccProvider : IVsSccProvider
    {
    }

    partial class AnkhSccProvider : AnkhService, ITheAnkhSvnSccProvider, IVsSccProvider, IVsSccControlNewSolution, IAnkhSccService
    {
        bool _active;
        IFileStatusCache _statusCache;
        IAnkhOpenDocumentTracker _documentTracker;

        public AnkhSccProvider(AnkhContext context)
            : base(context)
        {
        }

        public void RegisterAsSccProvider()
        {
            IVsRegisterScciProvider rscp = (IVsRegisterScciProvider)Context.GetService<IVsRegisterScciProvider>();
            if (rscp != null)
            {
                rscp.RegisterSourceControlProvider(AnkhId.SccProviderGuid);
            }
        }

        public void RegisterAsPrimarySccProvider()
        {
            RegisterAsSccProvider();
        }

        /// <summary>
        /// Gets the status cache.
        /// </summary>
        /// <value>The status cache.</value>
        public IFileStatusCache StatusCache
        {
            get { return _statusCache ?? (_statusCache = Context.GetService<IFileStatusCache>()); }
        }

        public IAnkhOpenDocumentTracker DocumentTracker
        {
            get { return _documentTracker ?? (_documentTracker = Context.GetService<IAnkhOpenDocumentTracker>()); }
        }

        /// <summary>
        /// Determines if any item in the solution are under source control.
        /// </summary>
        /// <param name="pfResult">[out] Returns non-zero (TRUE) if there is at least one item under source control; otherwise, returns zero (FALSE).</param>
        /// <returns>
        /// The method returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>.
        /// </returns>
        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            // Set pfResult to false when the solution can change to an other scc provider
            bool oneManaged = _active && _managedSolution;

            if (_active && !oneManaged)
            {
                foreach (SccProjectData data in _projectMap.Values)
                {
                    if (data.IsManaged)
                    {
                        oneManaged = true;
                        break;
                    }
                }
            }
            pfResult = oneManaged ? 1 : 0;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called by environment to mark a particular source control package as active.
        /// </summary>
        /// <returns>
        /// The method returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>.
        /// </returns>
        public int SetActive()
        {
            if (!_active)
            {
                _active = true;


                // Disable our custom glyphs before an other SCC provider is initialized!
                IAnkhSolutionExplorerWindow solutionExplorer = GetService<IAnkhSolutionExplorerWindow>();

                if (solutionExplorer != null)
                    solutionExplorer.EnableAnkhIcons(true);

                // Delayed flush all glyphs of all projects when a user enables us.
                IProjectNotifier pn = GetService<IProjectNotifier>();

                if (pn != null)
                {
                    List<SvnProject> allProjects = new List<SvnProject>();

                    foreach (SccProjectData pd in _projectMap.Values)
                    {
                        allProjects.Add(pd.SvnProject);
                    }
                    pn.MarkDirty(allProjects);
                }
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called by environment to mark a particular source control package as inactive.
        /// </summary>
        /// <returns>
        /// The method returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>.
        /// </returns>
        public int SetInactive()
        {
            if (_active)
            {
                _active = false;

                // Disable our custom glyphs before an other SCC provider is initialized!
                IAnkhSolutionExplorerWindow solutionExplorer = GetService<IAnkhSolutionExplorerWindow>();

                if (solutionExplorer != null)
                    solutionExplorer.EnableAnkhIcons(false);

                // If VS asked us for c ustom glyphs, we can release the handle now
                if (_glyphList != null)
                {
                    _glyphList.Dispose();
                    _glyphList = null;
                }

                // Remove all glyphs currently set
                foreach (SccProjectData pd in _projectMap.Values)
                {
                    pd.SccProject.SccGlyphChanged(0, null, null, null);
                }                
            }
            
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This function determines whether the source control package is installed. 
        /// Source control packages should always return S_OK and pbInstalled = nonzero..
        /// </summary>
        /// <param name="pbInstalled">The pb installed.</param>
        /// <returns></returns>
        public int IsInstalled(out int pbInstalled)
        {
            pbInstalled = 1; // We are always installed as we have no external dependencies

            return VSConstants.S_OK;
        }

        /// <summary>
        /// This method is called by projects that are under source control 
        /// when they are first opened to register project settings.
        /// </summary>
        /// <param name="pscp2Project">The PSCP2 project.</param>
        /// <param name="pszSccProjectName">Name of the PSZ SCC project.</param>
        /// <param name="pszSccAuxPath">The PSZ SCC aux path.</param>
        /// <param name="pszSccLocalPath">The PSZ SCC local path.</param>
        /// <param name="pszProvider">The PSZ provider.</param>
        /// <returns></returns>
        public int RegisterSccProject(IVsSccProject2 pscp2Project, string pszSccProjectName, string pszSccAuxPath, string pszSccLocalPath, string pszProvider)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(pscp2Project, out data))
            {
                // This method is called before the OpenProject calls
                _projectMap.Add(pscp2Project, data = new SccProjectData(Context, pscp2Project));
            }

            data.IsManaged = (pszProvider == AnkhId.SubversionSccName);
            data.IsRegistered = true;

            _syncMap = true;
            RegisterForSccCleanup();

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called by projects registered with the source control portion of the environment before they are closed.
        /// </summary>
        /// <param name="pscp2Project">The PSCP2 project.</param>
        /// <returns></returns>
        public int UnregisterSccProject(IVsSccProject2 pscp2Project)
        {
            SccProjectData data;
            if (_projectMap.TryGetValue(pscp2Project, out data))
            {
                data.IsRegistered = false;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Gets a value indicating whether the Ankh Scc service is active
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get { return _active; }
        }

        #region // Obsolete Methods
        /// <summary>
        /// Obsolete: returns E_NOTIMPL.
        /// </summary>
        /// <param name="pbstrDirectory">The PBSTR directory.</param>
        /// <param name="pfOK">The pf OK.</param>
        /// <returns></returns>
        public int BrowseForProject(out string pbstrDirectory, out int pfOK)
        {
            pbstrDirectory = null;
            pfOK = 0;

            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Obsolete: returns E_NOTIMPL.
        /// </summary>
        /// <returns></returns>
        public int CancelAfterBrowseForProject()
        {
            return VSConstants.E_NOTIMPL;
        }
        #endregion        
    }
}
