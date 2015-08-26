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
using Ankh.Configuration;
using Ankh.UI;

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
            GetService<AnkhServiceEvents>().ThemeChanged += OnThemeChanged;

            IVsShell shell = GetService<IVsShell>(typeof(SVsShell));

            if (shell != null)
            {
                object v;

                if (!VSErr.Succeeded(shell.GetProperty((int)__VSSPROPID.VSSPROPID_Zombie, out v)))
                    _zombie = false;
                else
                    _zombie = (v is bool) && ((bool)v);

                if (!VSErr.Succeeded(shell.AdviseShellPropertyChanges(this, out _shellPropsCookie)))
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

            if (!VSErr.Succeeded(Monitor.GetCmdUIContextCookie(ref cmdContextId, out cookie)))
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

        CmdStateCacheItem _sccManagerLoaded;
        public bool SccManagerLoaded
        {
            get { return (_sccManagerLoaded ?? (_sccManagerLoaded = GetCache(typeof(ISccManagerLoaded).GUID))).Active; }
        }

        CmdStateCacheItem _sccEnlistingInProject;
        public bool SccEnlistingInProject
        {
            get { return (_sccEnlistingInProject ?? (_sccEnlistingInProject = GetCache(typeof(IEnlistingInProject).GUID))).Active; }
        }

        CmdStateCacheItem _sccEnableOpenFromScc;
        public bool SccEnableOpenFromScc
        {
            get { return (_sccEnableOpenFromScc ?? (_sccEnableOpenFromScc = GetCache(new Guid("795635A1-4522-11d1-8DCE-00AA00A3F593")))).Active; }
        }

        CmdStateCacheItem _notBuildingAndNotDebugging;
        public bool NotBuildingAndNotDebugging
        {
            get { return (_notBuildingAndNotDebugging ?? (_notBuildingAndNotDebugging = GetCache(new Guid("48EA4A80-F14E-4107-88FA-8D0016F30B9C")))).Active; }
        }

        CmdStateCacheItem _solutionOrProjectUpgrading;
        public bool SolutionOrProjectUpgrading
        {
            get { return (_solutionOrProjectUpgrading ?? (_solutionOrProjectUpgrading = GetCache(new Guid("EF4F870B-7B85-4F29-9D15-CE1ABFBE733B")))).Active; }
        }

        /*CmdStateCacheItem _dataSourceWindowSupported;
        public bool DataSourceWindowSupported
        {
            get { return (_dataSourceWindowSupported ?? (_dataSourceWindowSupported = GetCache(new Guid("95C314C4-660B-4627-9F82-1BAF1C764BBF")))).Active; }
        }*/

        CmdStateCacheItem _dataSourceWindowAutoVisible;
        public bool DataSourceWindowAutoVisible
        {
            get { return (_dataSourceWindowAutoVisible ?? (_dataSourceWindowAutoVisible = GetCache(new Guid("2E78870D-AC7C-4460-A4A1-3FE37D00EF81")))).Active; }
        }

        CmdStateCacheItem _toolboxInitialized;
        public bool ToolboxInitialized
        {
            get { return (_toolboxInitialized ?? (_toolboxInitialized = GetCache(new Guid("DC5DB425-F0FD-4403-96A1-F475CDBA9EE0")))).Active; }
        }

        CmdStateCacheItem _solutionExistsAndNotBuildingAndNotDebugging;
        public bool SolutionExistsAndNotBuildingAndNotDebugging
        {
            get { return (_solutionExistsAndNotBuildingAndNotDebugging ?? (_solutionExistsAndNotBuildingAndNotDebugging = GetCache(new Guid("D0E4DEEC-1B53-4CDA-8559-D454583AD23B")))).Active; }
        }
        
        CmdStateCacheItem _solutionExistsAndFullyLoaded;
        public bool SolutionExistsAndFullyLoaded
        {
            // VS2010+
            get { return (_solutionExistsAndFullyLoaded ?? (_solutionExistsAndFullyLoaded = GetCache(new Guid("10534154-102D-46E2-ABA8-A6BFA25BA0BE")))).Active; }
        }

        CmdStateCacheItem _solutionOpening;
        public bool SolutionOpening
        {
            // VS2010+
            get { return (_solutionOpening ?? (_solutionOpening = GetCache(new Guid("D2567162-F94F-4091-8798-A096E61B8B50")))).Active; }
        }

        CmdStateCacheItem _projectRetargeting;
        public bool ProjectRetargeting
        {
            // VS2010+
            get { return (_projectRetargeting ?? (_projectRetargeting = GetCache(new Guid("DE039A0E-C18F-490c-944A-888B8E86DA4B")))).Active; }
        }

        CmdStateCacheItem _historicalDebugging;
        public bool HistoricalDebugging
        {
            // VS2010+
            get { return (_historicalDebugging ?? (_historicalDebugging = GetCache(new Guid("D1B1E38F-1A7E-4236-AF55-6FA8F5FA76E6")))).Active; }
        }

        /*CmdStateCacheItem _dataSourceWizardSuppressed;
        public bool DataSourceWizardSuppressed
        {
            // VS2010+
            get { return (_dataSourceWizardSuppressed ?? (_dataSourceWizardSuppressed = GetCache(new Guid("5705AD15-40EE-4426-AD3E-BA750610D599")))).Active; }
        }*/

        /*CmdStateCacheItem _standardPreviewerConfigurationChanging;
        public bool StandardPreviewerConfigurationChanging
        {
            // VS11+
            get { return (_standardPreviewerConfigurationChanging ?? (_standardPreviewerConfigurationChanging = GetCache(new Guid("6D3CAD8E-9129-4ec0-929E-69B6F5D4400D")))).Active; }
        }*/

        CmdStateCacheItem _solutionHasImmersiveProject;
        public bool SolutionHasImmersiveProject
        {
            // VS11+
            get { return (_solutionHasImmersiveProject ?? (_solutionHasImmersiveProject = GetCache(new Guid("7CAC4AE1-2E6B-4B02-A91C-71611E86F273")))).Active; }
        }

        CmdStateCacheItem _firstLaunchSetup;
        public bool FirstLaunchSetup
        {
            // VS11+
            get { return (_firstLaunchSetup ?? (_firstLaunchSetup = GetCache(new Guid("E7B2B2DB-973B-4CE9-A8D7-8498895DEA73")))).Active; }
        }

        CmdStateCacheItem _osWindows8OrHigher;
        public bool OsWindows8OrHigher
        {
            // VS11+
            get { return (_osWindows8OrHigher ?? (_osWindows8OrHigher = GetCache(new Guid("67CFF80C-0863-4202-A4E4-CE80FDF8506E")))).Active; }
        }

        CmdStateCacheItem _backgroundProjectLoad;
        public bool BackgroundProjectLoad
        {
            get { return (_backgroundProjectLoad ?? (_backgroundProjectLoad = GetCache(new Guid("DC769521-31A2-41A5-9BBB-210B5D63568D")))).Active; }
        }

        CmdStateCacheItem _twSolutionExplorer;
        public bool SolutionExplorerActive
        {
            get { return (_twSolutionExplorer ?? (_twSolutionExplorer = GetCache(new Guid(ToolWindowGuids80.SolutionExplorer)))).Active; }
        }

        CmdStateCacheItem _twClassViewer;
        public bool ClassViewerActive
        {
            get { return (_twClassViewer ?? (_twClassViewer = GetCache(new Guid(ToolWindowGuids80.ClassView)))).Active; }
        }

        CmdStateCacheItem _twPendingChanges;
        public bool PendingChangesActive
        {
            get { return (_twPendingChanges ?? (_twPendingChanges = GetCache(AnkhId.PendingChangeContextGuid))).Active; }
        }

        public bool ShiftDown
        {
            get { return (0 != (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift)); }
        }

        #region Theme State
        bool? _themed;
        bool _themeDark;
        bool _themeLight;
        
        void OnThemeChanged(object sender, EventArgs e)
        {
            _themed = null;
        }

        public bool ThemeDark
        {
            get
            {
                if (!_themed.HasValue)
                    LoadThemeData();

                return _themeDark;
            }
        }

        public bool ThemeLight
        {
            get
            {
                if (!_themed.HasValue)
                    LoadThemeData();

                return _themeLight;
            }
        }

        public bool ThemeDefined
        {
            get
            {
                if (!_themed.HasValue)
                    LoadThemeData();

                return _themed.Value;
            }
        }


        void LoadThemeData()
        {
            if (!VSVersion.VS2012OrLater)
            {
                _themed = false;
                return;
            }

            _themeLight = _themeDark = false;
            IAnkhConfigurationService config = GetService<IAnkhConfigurationService>();
            Guid themeGuid;

            if (config == null
                || !GetService<IWinFormsThemingService>().GetCurrentTheme(out themeGuid))
            {
                _themed = false;
                return;
            }

            if (themeGuid == Guid.Empty)
            {
                // Before the first theme switch no theme is set, but the light theme
                // is used anyway
                _themed = true;
                _themeLight = true;
                _themeDark = true;
                return;
            }
                
            using (RegistryKey rk = config.OpenVSInstanceKey("Extensions\\AnkhSVN\\Themes"))
            {
                object v;

                if (rk != null)
                    v = rk.GetValue(themeGuid.ToString("B"));
                else
                    v = null;

                if (v is int)
                {
                    _themed = true;
                    int vv = (int)v;

                    _themeLight = (vv & 0x01) != 0;
                    _themeDark = (vv & 0x02) != 0;
                }
                else
                    _themed = true;
            }
        }

        
        #endregion

        #region Cache Item
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
                _active = VSErr.Succeeded(monitor.IsCmdUIContextActive(_cookie, out active)) && active != 0;
            }

            public bool Active
            {
                get { return _active; }
                set { _active = value; }
            }
        }
        #endregion

        #region Scc State

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
                        if (pv != null && VSErr.Succeeded(pv.AnyItemsUnderSourceControl(out iManaging)))
                        {
                            if (iManaging != 0)
                                return true;
                        }
                        break;
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
                if (VSErr.Succeeded(lr.GetLocalRegistryRoot(out root)))
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
            if (!VSErr.Succeeded(manager.IsInstalled(out installed)) || (installed == 0))
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
                        {
                            IAnkhServiceEvents se = GetService<IAnkhServiceEvents>();
                            if (se != null)
                                se.OnUIShellActivate(EventArgs.Empty);
                        }
                    }
                    break;
            }

            return VSErr.S_OK;
        }

        #endregion
    }
}
