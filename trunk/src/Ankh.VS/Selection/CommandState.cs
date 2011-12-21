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
    sealed class CommandState : AnkhService, IAnkhCommandStates, IVsShellPropertyEvents
    {
        readonly Dictionary<uint, CmdStateCacheItem> _cookieMap = new Dictionary<uint, CmdStateCacheItem>();
        IVsMonitorSelection _monitor;
        uint _shellPropsCookie;
        bool _zombie;

        public CommandState(IAnkhServiceProvider context)
            : base(context)
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            GetService<SelectionContext>(typeof(ISelectionContext)).CmdUIContextChanged += OnCmdUIContextChanged;

            IVsShell shell = GetService<IVsShell>(typeof(SVsShell));

            if (shell != null)
            {
                object v;

                if (!ErrorHandler.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_Zombie, out v)))
                    _zombie = false;
                else
                    _zombie = (v is bool) && ((bool)v);

                if (!ErrorHandler.Succeeded(shell.AdviseShellPropertyChanges(this, out _shellPropsCookie)))
                    _shellPropsCookie = 0;
            }

            // We might already have cached some stale values!
            foreach (CmdStateCacheItem i in _cookieMap.Values)
            {
                i.Reload(Monitor);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_shellPropsCookie != 0)
                {
                    uint ck = _shellPropsCookie;
                    _shellPropsCookie = 0;

                    IVsShell shell = GetService<IVsShell>(typeof(SVsShell));
                    if (shell != null)
                        shell.UnadviseShellPropertyChanges(ck);
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        IVsMonitorSelection Monitor
        {
            get { return _monitor ?? (_monitor = GetService<IVsMonitorSelection>()); }
        }

        void OnCmdUIContextChanged(object sender, CmdUIContextChangeEventArgs e)
        {
            CmdStateCacheItem item;
            if (_cookieMap.TryGetValue(e.Cookie, out item))
                item.Active = e.Active;

            ClearState();
        }

        void ClearState()
        {
            // Clear all caching properties
            _otherSccProviderActive = null;
        }

        CmdStateCacheItem _codeWindow;
        public bool CodeWindow
        {
            get { return (_codeWindow ?? (_codeWindow = GetCache(VSConstants.UICONTEXT_CodeWindow))).Active; }
        }

        private CmdStateCacheItem GetCache(Guid cmdContextId)
        {
            uint cookie;

            if (!ErrorHandler.Succeeded(Monitor.GetCmdUIContextCookie(ref cmdContextId, out cookie)))
                return new CmdStateCacheItem(Monitor, 0);

            CmdStateCacheItem item;

            if (!_cookieMap.TryGetValue(cookie, out item))
            {
                _cookieMap[cookie] = item = new CmdStateCacheItem(Monitor, cookie);
            }

            return item;
        }

        CmdStateCacheItem _debugging;
        public bool Debugging
        {
            get { return (_debugging ?? (_debugging = GetCache(VSConstants.UICONTEXT_Debugging))).Active; }
        }

        CmdStateCacheItem _designMode;
        public bool DesignMode
        {
            get { return (_designMode ?? (_designMode = GetCache(VSConstants.UICONTEXT_DesignMode))).Active; }
        }

        CmdStateCacheItem _dragging;
        public bool Dragging
        {
            get { return (_dragging ?? (_dragging = GetCache(VSConstants.UICONTEXT_Dragging))).Active; }
        }

        CmdStateCacheItem _emptySolution;
        public bool EmptySolution
        {
            get { return (_emptySolution ?? (_emptySolution = GetCache(VSConstants.UICONTEXT_EmptySolution))).Active; }
        }

        CmdStateCacheItem _fullScreenMode;
        public bool FullScreenMode
        {
            get { return (_fullScreenMode ?? (_fullScreenMode = GetCache(VSConstants.UICONTEXT_FullScreenMode))).Active; }
        }

        CmdStateCacheItem _noSolution;
        public bool NoSolution
        {
            get { return (_noSolution ?? (_noSolution = GetCache(VSConstants.UICONTEXT_NoSolution))).Active; }
        }

        CmdStateCacheItem _solutionBuilding;
        public bool SolutionBuilding
        {
            get { return (_solutionBuilding ?? (_solutionBuilding = GetCache(VSConstants.UICONTEXT_SolutionBuilding))).Active; }
        }

        CmdStateCacheItem _solutionExists;
        public bool SolutionExists
        {
            get { return (_solutionExists ?? (_solutionExists = GetCache(VSConstants.UICONTEXT_SolutionExists))).Active; }
        }

        CmdStateCacheItem _solutionHasMultipleProjects;
        public bool SolutionHasMultipleProjects
        {
            get { return (_solutionHasMultipleProjects ?? (_solutionHasMultipleProjects = GetCache(VSConstants.UICONTEXT_SolutionHasMultipleProjects))).Active; }
        }

        CmdStateCacheItem _solutionHasSingleProject;
        public bool SolutionHasSingleProject
        {
            get { return (_solutionHasSingleProject ?? (_solutionHasSingleProject = GetCache(VSConstants.UICONTEXT_SolutionHasSingleProject))).Active; }
        }

        CmdStateCacheItem _ankhActiveScc;
        public bool SccProviderActive
        {
            get { return (_ankhActiveScc ?? (_ankhActiveScc = GetCache(AnkhId.SccProviderGuid))).Active; }
        }

        sealed class CmdStateCacheItem
        {
            readonly uint _cookie;
            bool _active;
            public CmdStateCacheItem(IVsMonitorSelection monitor, uint cookie)
            {
                if (monitor == null)
                    throw new ArgumentNullException("monitor");

                _cookie = cookie;

                Reload(monitor);
            }

            internal void Reload(IVsMonitorSelection monitor)
            {
                int active;
                _active = ErrorHandler.Succeeded(monitor.IsCmdUIContextActive(_cookie, out active)) && active != 0;
            }

            public bool Active
            {
                get { return _active; }
                set { _active = value; }
            }
        }

        #region IAnkhCommandStates Members

        bool? _otherSccProviderActive;
        public bool OtherSccProviderActive
        {
            get { return (bool)(_otherSccProviderActive ?? (_otherSccProviderActive = GetOtherSccProviderActive())); }
        }

        class SccData
        {
            public readonly CmdStateCacheItem _cache;
            public readonly string _service;

            public SccData(CmdStateCacheItem cache, string service)
            {
                if (cache == null)
                    throw new ArgumentNullException("cache");
                else if (service == null)
                    throw new ArgumentNullException("service");

                _cache = cache;
                _service = new Guid(service).ToString();
            }

            public bool Active
            {
                get { return _cache.Active; }
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
                    if (scc.Active)
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
                                            string service = rks.GetValue("Service") as string;

                                            if (!string.IsNullOrEmpty(service))
                                            {
                                                Guid sccGuid = new Guid(name);
                                                CmdStateCacheItem cache = GetCache(new Guid(name));

                                                sccs.Add(new SccData(cache, service));
                                            }
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

        public bool UIShellAvailable
        {
            get { return !_zombie; }
        }

        #endregion

        #region IVsShellPropertyEvents Members

        public int OnShellPropertyChange(int propid, object var)
        {
            switch ((__VSSPROPID)propid)
            {
                case __VSSPROPID.VSSPROPID_Zombie:
                    if (var is bool)
                    {
                        _zombie = (bool)var;

                        if (!_zombie)
                            GetService<IAnkhServiceEvents>().OnUIShellActivate(EventArgs.Empty);
                    }
                    break;
            }

            return VSConstants.S_OK;
        }

        #endregion
    }
}
