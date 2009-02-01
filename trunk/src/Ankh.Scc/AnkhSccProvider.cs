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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using SharpSvn;

using Ankh.Ids;
using Ankh.Scc.ProjectMap;
using Ankh.Selection;
using Ankh.VS;

namespace Ankh.Scc
{
    [GuidAttribute(AnkhId.SccServiceId), CLSCompliant(false)]
    public interface ITheAnkhSvnSccProvider : IVsSccProvider
    {
    }

    [GlobalService(typeof(AnkhSccProvider))]
    [GlobalService(typeof(IAnkhSccService))]
    [GlobalService(typeof(ITheAnkhSvnSccProvider), true)]
    partial class AnkhSccProvider : AnkhService, ITheAnkhSvnSccProvider, IVsSccProvider, IVsSccControlNewSolution, IAnkhSccService, IVsSccEnlistmentPathTranslation
    {
        bool _active;
        IFileStatusCache _statusCache;
        IAnkhOpenDocumentTracker _documentTracker;
        AnkhSccSettingStorage _sccSettings;

        public AnkhSccProvider(IAnkhServiceProvider context)
            : base(context)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            GetService<AnkhServiceEvents>().RuntimeStarted
                += delegate
                {
                    TryRegisterSccProvider();
                };
        }

        public void RegisterAsSccProvider()
        {
            _tryRegisteredBefore = true;
            IVsRegisterScciProvider rscp = GetService<IVsRegisterScciProvider>();
            if (rscp != null)
            {
                ErrorHandler.ThrowOnFailure(rscp.RegisterSourceControlProvider(AnkhId.SccProviderGuid));
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
        IFileStatusCache StatusCache
        {
            get { return _statusCache ?? (_statusCache = GetService<IFileStatusCache>()); }
        }

        IAnkhOpenDocumentTracker DocumentTracker
        {
            get { return _documentTracker ?? (_documentTracker = GetService<IAnkhOpenDocumentTracker>()); }
        }
        
        AnkhSccSettingStorage SccStore
        {
            get { return _sccSettings ?? (_sccSettings = GetService<AnkhSccSettingStorage>(typeof(ISccSettingsStore))); }
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
            bool oneManaged = _active && IsSolutionManaged;

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

                // Delayed flush all glyphs of all projects when a user enables us.
                IProjectNotifier pn = GetService<IProjectNotifier>();

                if (pn != null)
                {
                    List<SvnProject> allProjects = new List<SvnProject>(GetAllProjects());
                    allProjects.Add(SvnProject.Solution);

                    pn.MarkDirty(allProjects);
                }
            }

            _ensureIcons = true;
            RegisterForSccCleanup();

            GetService<IAnkhServiceEvents>().OnSccProviderActivated(EventArgs.Empty);

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

                ClearSolutionGlyph();
            }

            GetService<IAnkhServiceEvents>().OnSccProviderDeactivated(EventArgs.Empty);

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
