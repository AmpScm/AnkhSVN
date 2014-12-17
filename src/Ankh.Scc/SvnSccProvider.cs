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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using CommandID = System.ComponentModel.Design.CommandID;

using Ankh.Commands;
using Ankh.Scc.ProjectMap;
using Ankh.Selection;
using Ankh.Configuration;
using Ankh.VS;

namespace Ankh.Scc
{
    [GlobalService(typeof(SvnSccProvider))]
    [GlobalService(typeof(IAnkhSccService))]
    [GlobalService(typeof(ITheAnkhSvnSccProvider), true)]
    partial class SvnSccProvider : SccProvider, ITheAnkhSvnSccProvider, IVsSccControlNewSolution, IAnkhSccService, IVsSccEnlistmentPathTranslation
    {
        IFileStatusCache _statusCache;
        IAnkhOpenDocumentTracker _documentTracker;

        public SvnSccProvider(IAnkhServiceProvider context)
            : base(context)
        {
            _projectMap = new SccProjectMap(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            GetService<AnkhServiceEvents>().RuntimeStarted
                += delegate
                {
                    IAnkhCommandStates states;

                    states = GetService<IAnkhCommandStates>();

                    if (states == null || !states.SccProviderActive)
                        return;

                    // Ok, Visual Studio decided to activate the user context with our GUID
                    // This tells us VS wants us to be the active SCC
                    //
                    // This is not documented directly. But it is documented that we should
                    // enable our commands on that context

                    // Set us active; this makes VS initialize the provider
                    RegisterAsPrimarySccProvider();
                };
        }

        protected override Guid ProviderGuid
        {
            get { return AnkhId.SccProviderGuid; }
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

        /// <summary>
        /// Determines if any item in the solution are under source control.
        /// </summary>
        /// <param name="pfResult">[out] Returns non-zero (TRUE) if there is at least one item under source control; otherwise, returns zero (FALSE).</param>
        /// <returns>
        /// The method returns <see cref="F:Microsoft.VisualStudio.VSErr.S_OK"></see>.
        /// </returns>
        public override bool AnyItemsUnderSourceControl()
        {
            if (!IsActive)
                return false;

            // Return false when the solution can change to an other scc provider
            if (IsSolutionManaged)
                return true;
            
            foreach (SccProjectData data in _projectMap.Values)
            {
                if (data.IsManaged)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Called by environment to mark a particular source control package as active.
        /// </summary>
        /// <returns>
        /// The method returns <see cref="F:Microsoft.VisualStudio.VSErr.S_OK"></see>.
        /// </returns>
        protected override void SetActive(bool active)
        {
            base.SetActive(active);

            if (active)
            {
                // Send activate before scheduling glyphs to make sure the project data is
                // loaded
                GetService<IAnkhServiceEvents>().OnSccProviderActivated(EventArgs.Empty);

                // Delayed flush all glyphs of all projects when a user enables us.

                List<SccProject> allProjects = new List<SccProject>(GetAllProjects());
                allProjects.Add(SccProject.Solution);
                Monitor.ScheduleGlyphOnlyUpdate(allProjects);

                RegisterForSccCleanup();
            }
            else
            {
                // If VS asked us for custom glyphs, we can release the handle now
                DisposeGlyphList();

                // Remove all glyphs currently set
                foreach (SccProjectData pd in new List<SccProjectData>(_projectMap.Values))
                {
                    pd.NotifyGlyphsChanged();
                    pd.Dispose();
                }

                ClearSolutionGlyph();

                _projectMap.Clear();
                _fileMap.Clear();
                _unreloadable.Clear();
                _sccExcluded.Clear();

                GetService<IAnkhServiceEvents>().OnSccProviderDeactivated(EventArgs.Empty);
            }
        }

        public override void OnSolutionRefreshCommand(EventArgs e)
        {
            CommandService.PostExecCommand(AnkhCommand.Refresh);
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
        public override int RegisterSccProject(IVsSccProject2 pscp2Project, string pszSccProjectName, string pszSccAuxPath, string pszSccLocalPath, string pszProvider)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(pscp2Project, out data))
            {
                // This method is called before the OpenProject calls
                _projectMap.Add(pscp2Project, data = new SccProjectData(this, pscp2Project));
            }

            data.IsManaged = (pszProvider == AnkhId.SubversionSccName);
            data.IsRegistered = true;

            _syncMap = true;
            RegisterForSccCleanup();

            return VSErr.S_OK;
        }

        /// <summary>
        /// Called by projects registered with the source control portion of the environment before they are closed.
        /// </summary>
        /// <param name="pscp2Project">The PSCP2 project.</param>
        /// <returns></returns>
        public override int UnregisterSccProject(IVsSccProject2 pscp2Project)
        {
            SccProjectData data;
            if (_projectMap.TryGetValue(pscp2Project, out data))
            {
                data.IsRegistered = false;
            }

            return VSErr.S_OK;
        }
    }
}
