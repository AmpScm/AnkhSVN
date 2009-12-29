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
using Ankh.Commands;
using Microsoft.Win32;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Ankh.Selection;

namespace Ankh.VS.Selection
{
    [GlobalService(typeof(IAnkhCommandStates))]
    sealed class CommandState : AnkhService, IAnkhCommandStates
    {
        IVsMonitorSelection _monitor;

        public CommandState(IAnkhServiceProvider context)
            : base(context)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            GetService<SelectionContext>(typeof(ISelectionContext)).CmdUIContextChanged += new EventHandler(OnCmdUIContextChanged);
        }

        IVsMonitorSelection Monitor
        {
            get { return _monitor ?? (_monitor = GetService<IVsMonitorSelection>()); }
        }

        void OnCmdUIContextChanged(object sender, EventArgs e)
        {
            ClearState();
        }

        void ClearState()
        {
            // Clear all caching properties
            _codeWindow = null;
            _debugging = null;
            _designMode = null;
            _dragging = null;
            _emptySolution = null;
            _fullScreenMode = null;
            _noSolution = null;
            _solutionBuilding = null;
            _solutionExists = null;
            _solutionHasMultipleProjects = null;
            _solutionHasSingleProject = null;
            _ankhActiveScc = null;
            _otherSccProviderActive = null;
        }

        uint _codeWindowId;
        bool? _codeWindow;
        public bool CodeWindow
        {
            get { return (bool)(_codeWindow ?? (_codeWindow = GetState(ref _codeWindowId, delegate() { return VSConstants.UICONTEXT_CodeWindow.ToString(); }))); }
        }


        uint _debuggingId;
        bool? _debugging;
        public bool Debugging
        {
            get { return (bool)(_debugging ?? (_debugging = GetState(ref _debuggingId, delegate() { return VSConstants.UICONTEXT_Debugging.ToString(); }))); }
        }

        uint _designModeId;
        bool? _designMode;
        public bool DesignMode
        {
            get { return (bool)(_designMode ?? (_designMode = GetState(ref _designModeId, delegate() { return VSConstants.UICONTEXT_DesignMode.ToString(); }))); }
        }

        uint _draggingId;
        bool? _dragging;
        public bool Dragging
        {
            get { return (bool)(_dragging ?? (_dragging = GetState(ref _draggingId, delegate() { return VSConstants.UICONTEXT_Dragging.ToString(); }))); }
        }

        uint _emptySolutionId;
        bool? _emptySolution;
        public bool EmptySolution
        {
            get { return (bool)(_emptySolution ?? (_emptySolution = GetState(ref _emptySolutionId, delegate() { return VSConstants.UICONTEXT_EmptySolution.ToString(); }))); }
        }

        uint _fullScreenModeId;
        bool? _fullScreenMode;
        public bool FullScreenMode
        {
            get { return (bool)(_fullScreenMode ?? (_fullScreenMode = GetState(ref _fullScreenModeId, delegate() { return VSConstants.UICONTEXT_FullScreenMode.ToString(); }))); }
        }

        uint _noSolutionId;
        bool? _noSolution;
        public bool NoSolution
        {
            get { return (bool)(_noSolution ?? (_noSolution = GetState(ref _noSolutionId, delegate() { return VSConstants.UICONTEXT_NoSolution.ToString(); }))); }
        }

        uint _solutionBuildingId;
        bool? _solutionBuilding;
        public bool SolutionBuilding
        {
            get { return (bool)(_solutionBuilding ?? (_solutionBuilding = GetState(ref _solutionBuildingId, delegate() { return VSConstants.UICONTEXT_SolutionBuilding.ToString(); }))); }
        }

        uint _solutionExistsId;
        bool? _solutionExists;
        public bool SolutionExists
        {
            get { return (bool)(_solutionExists ?? (_solutionExists = GetState(ref _solutionExistsId, delegate() { return VSConstants.UICONTEXT_SolutionExists.ToString(); }))); }
        }

        uint _solutionHasMultipleProjectsId;
        bool? _solutionHasMultipleProjects;
        public bool SolutionHasMultipleProjects
        {
            get { return (bool)(_solutionHasMultipleProjects ?? (_solutionHasMultipleProjects = GetState(ref _solutionHasMultipleProjectsId, delegate() { return VSConstants.UICONTEXT_SolutionHasMultipleProjects.ToString(); }))); }
        }

        uint _solutionHasSingleProjectId;
        bool? _solutionHasSingleProject;
        public bool SolutionHasSingleProject
        {
            get { return (bool)(_solutionHasSingleProject ?? (_solutionHasSingleProject = GetState(ref _solutionHasSingleProjectId, delegate() { return VSConstants.UICONTEXT_SolutionHasSingleProject.ToString(); }))); }
        }

        uint _ankhActiveSccId;
        bool? _ankhActiveScc;
        public bool SccProviderActive
        {
            get { return (bool)(_ankhActiveScc ?? (_ankhActiveScc = GetState(ref _ankhActiveSccId, AnkhId.SccProviderId))); }
        }

        private bool GetState(ref uint contextId, string guid)
        {
            if (contextId == 0)
            {
                Guid g = new Guid(guid);
                if (!ErrorHandler.Succeeded(Monitor.GetCmdUIContextCookie(ref g, out contextId)))
                {
                    contextId = 0;
                    return false;
                }
            }

            int active;
            return ErrorHandler.Succeeded(Monitor.IsCmdUIContextActive(contextId, out active)) && active != 0;
        }

        delegate string GetGuid();
        private bool GetState(ref uint contextId, GetGuid getGuid)
        {
            string value = null;
            if (contextId == 0)
                value = getGuid();

            return GetState(ref contextId, value);
        }

        #region IAnkhCommandStates Members

        bool? _otherSccProviderActive;
        public bool OtherSccProviderActive
        {
            get { return (bool)(_otherSccProviderActive ?? (_otherSccProviderActive = GetOtherSccProviderActive())); }
        }

        class SccData
        {
            public readonly uint _id;
            public readonly string _service;

            public SccData(uint id, string service)
            {
                _id = id;
                _service = new Guid(service).ToString();
            }
        }

        SccData[] _otherSccProviderContexts;

        bool GetOtherSccActive()
        {
            EnsureContexts();

            try
            {
                foreach (SccData scc in _otherSccProviderContexts)
                {
                    int active;
                    if (ErrorHandler.Succeeded(Monitor.IsCmdUIContextActive(scc._id, out active)) && active != 0)
                    {
                        // Ok, let's ask the service if it has any files under source control?

                        Guid gService = new Guid(scc._service);

                        IVsSccProvider pv = GetService<IAnkhQueryService>().QueryService<IVsSccProvider>(gService);

                        int iManaging;
                        if (pv != null && ErrorHandler.Succeeded(pv.AnyItemsUnderSourceControl(out iManaging)))
                        {
                            if (iManaging != 0)
                                return true;
                        }
                    }
                }
            }
            catch { }

            return false;
        }

        private void EnsureContexts()
        {
            if (_otherSccProviderContexts == null)
            {
                List<SccData> sccs = new List<SccData>();

                ILocalRegistry2 lr = GetService<ILocalRegistry2>(typeof(SLocalRegistry));

                string root;
                List<string> names = new List<string>();
                if (ErrorHandler.Succeeded(lr.GetLocalRegistryRoot(out root)))
                {
                    RegistryKey baseKey = Registry.LocalMachine;

                    // TODO: Find some way to use the VS2008 RANU api
                    if (root.EndsWith("\\UserSettings"))
                    {
                        root = root.Substring(0, root.Length - 13) + "\\Configuration";
                        baseKey = Registry.CurrentUser;
                    }

                    using (RegistryKey rk = baseKey.OpenSubKey(root + "\\SourceControlProviders", RegistryKeyPermissionCheck.ReadSubTree))
                    {
                        if (rk != null)
                        {
                            string myId = AnkhId.SccProviderGuid.ToString("B");
                            foreach (string name in rk.GetSubKeyNames())
                            {
                                if (name.Length == 38 && !myId.Equals(name, StringComparison.OrdinalIgnoreCase))
                                {
                                    try
                                    {
                                        using (RegistryKey rks = rk.OpenSubKey(name, RegistryKeyPermissionCheck.ReadSubTree))
                                        {
                                            Guid sccGuid = new Guid(name);
                                            uint id;

                                            if (ErrorHandler.Succeeded(Monitor.GetCmdUIContextCookie(ref sccGuid, out id)))
                                                sccs.Add(new SccData(id, (string)rks.GetValue("Service")));
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }

                _otherSccProviderContexts = sccs.ToArray();
            }
        }

        private bool GetOtherSccProviderActive()
        {
            if (SccProviderActive)
                return false; // We are active

            return GetRawOtherSccProviderActive();
        }

        public bool GetRawOtherSccProviderActive()
        {
            IVsSccManager2 manager = GetService<IVsSccManager2>(typeof(SVsSccManager));

            if (manager == null)
                return false;

            // If the active manager is not installed, it is not active            
            int installed = 0;
            if (!ErrorHandler.Succeeded(manager.IsInstalled(out installed)) || (installed == 0))
                return false;

            if (GetOtherSccActive())
                return true;

            return false;
        }

        #endregion
    }
}
