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
    [GuidAttribute(AnkhId.SccServiceId), ComVisible(true), CLSCompliant(false)]
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

        public void RegisterAsPrimarySccProvider()
        {
            IVsRegisterScciProvider rscp = GetService<IVsRegisterScciProvider>();
            if (rscp != null)
            {
                Marshal.ThrowExceptionForHR(rscp.RegisterSourceControlProvider(AnkhId.SccProviderGuid));
            }
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
            return VSErr.S_OK;
        }

        /// <summary>
        /// Called by environment to mark a particular source control package as active.
        /// </summary>
        /// <returns>
        /// The method returns <see cref="F:Microsoft.VisualStudio.VSErr.S_OK"></see>.
        /// </returns>
        public int SetActive()
        {
            _active = true;

            // Send activate before scheduling glyphs to make sure the project data is
            // loaded
            GetService<IAnkhServiceEvents>().OnSccProviderActivated(EventArgs.Empty);

            // Delayed flush all glyphs of all projects when a user enables us.

            List<SvnProject> allProjects = new List<SvnProject>(GetAllProjects());
            allProjects.Add(SvnProject.Solution);
            Monitor.ScheduleGlyphOnlyUpdate(allProjects);

            RegisterForSccCleanup();

            IAnkhConfigurationService cfg = GetService<IAnkhConfigurationService>();
            
            if (cfg != null && !cfg.Instance.DontHookSolutionExplorerRefresh)
            {
                IAnkhGlobalCommandHook cmdHook = GetService<IAnkhGlobalCommandHook>();

                if (cmdHook != null)
                    cmdHook.HookCommand(new CommandID(VSConstants.VSStd2K, (int)VSConstants.VSStd2KCmdID.SLNREFRESH),
                                        OnSlnRefresh);
            }

            return VSErr.S_OK;
        }

        private void OnSlnRefresh(object sender, EventArgs e)
        {
            CommandService.PostExecCommand(AnkhCommand.Refresh);
        }

        /// <summary>
        /// Called by environment to mark a particular source control package as inactive.
        /// </summary>
        /// <returns>
        /// The method returns <see cref="F:Microsoft.VisualStudio.VSErr.S_OK"></see>.
        /// </returns>
        public int SetInactive()
        {
            if (_active)
            {
                _active = false;
                // If VS asked us for custom glyphs, we can release the handle now
                if (_glyphList != null)
                {
                    _glyphList.Dispose();
                    _glyphList = null;
                }

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
            }

            GetService<IAnkhServiceEvents>().OnSccProviderDeactivated(EventArgs.Empty);

            IAnkhGlobalCommandHook cmdHook = GetService<IAnkhGlobalCommandHook>();

            if (cmdHook != null)
                cmdHook.UnhookCommand(new CommandID(VSConstants.VSStd2K, (int)VSConstants.VSStd2KCmdID.SLNREFRESH),
                                      OnSlnRefresh);

            return VSErr.S_OK;
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

            return VSErr.S_OK;
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

            return VSErr.S_OK;
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

            return VSErr.S_OK;
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
