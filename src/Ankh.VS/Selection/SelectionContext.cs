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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using SharpSvn;

using Ankh.Scc;
using Ankh.Selection;
using Ankh.VS.SolutionExplorer;


namespace Ankh.VS.Selection
{
    /// <summary>
    /// 
    /// </summary>
    [GlobalService(typeof(ISelectionContext))]
    [GlobalService(typeof(ISccProjectWalker))]
    partial class SelectionContext : AnkhService, IVsSelectionEvents, ISelectionContext, ISelectionContextEx, ISccProjectWalker
    {
        ISvnStatusCache _svnCache;
        bool _disposed;
        uint _cookie;

        struct HierarchySelection
        {
            public IVsHierarchy hierarchy;
            public uint id;
            public IVsMultiItemSelect selection;
        }
        HierarchySelection current;
        ISelectionContainer _currentContainer;
        IVsSolution _solution;

        CachedEnumerable<SelectionItem> _selectionItems;
        CachedEnumerable<SelectionItem> _selectionItemsRecursive;
        CachedEnumerable<string> _filenames;
        CachedEnumerable<string> _filenamesRecursive;
        CachedEnumerable<SvnItem> _svnItems;
        CachedEnumerable<SvnItem> _svnItemsRecursive;
        CachedEnumerable<SccProject> _selectedProjects;
        CachedEnumerable<SccProject> _selectedProjectsRecursive;
        CachedEnumerable<SccHierarchy> _selectedHierarchies;
        CachedEnumerable<SccProject> _ownerProjects;
        Dictionary<Type, IEnumerable> _selectedItemsMap;
        readonly Hashtable _hashCache = new Hashtable();
        IVsHierarchy _miscFiles;
        bool _checkedMisc;
        bool? _isSolutionExplorer;
        bool? _isSolutionSelected;
        bool? _isSingleNodeSelection;
        string _solutionFilename;

        IVsHierarchy _filterHierarchy;
        uint _filterItem;

        public SelectionContext(IAnkhServiceProvider context)
            : base(context)
        {
            _filterItem = VSItemId.Nil;
        }

        protected override void OnInitialize()
        {
            IVsMonitorSelection monitor = GetService<IVsMonitorSelection>();

            if (monitor != null)
                Marshal.ThrowExceptionForHR(monitor.AdviseSelectionEvents(this, out _cookie));
        }

        protected ISvnStatusCache SvnCache
        {
            get { return _svnCache ?? (_svnCache = GetService<ISvnStatusCache>()); }
        }
        protected override void Dispose(bool disposing)
        {
            try
            {
                ClearCache();
                if (!_disposed)
                {
                    _disposed = true;

                    if (_cookie != 0)
                    {
                        IVsMonitorSelection monitor = GetService<IVsMonitorSelection>();

                        if (monitor != null)
                            monitor.UnadviseSelectionEvents(_cookie);
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region IVsSelectionEvents Members

        public event EventHandler<CmdUIContextChangeEventArgs> CmdUIContextChanged;

        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            if (CmdUIContextChanged != null)
                CmdUIContextChanged(this, new CmdUIContextChangeEventArgs(dwCmdUICookie, fActive != 0));

            /// Some global state change which might change UI cueues
            return VSErr.S_OK;
        }

        Disposer _disposer;
        Disposer Disposer
        {
            get { return _disposer ?? (_disposer = new Disposer()); }
        }

        public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld,
                IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
        {
            if (!_disposed)
            {
                // The current selection changed; store for future reference
                current.hierarchy = pHierNew;
                current.id = itemidNew;
                current.selection = pMISNew;
                _currentContainer = pSCNew;
            }

            if (_filterItem != VSItemId.Nil)
            {
                if (_filterItem != current.id || _filterHierarchy != current.hierarchy)
                {
                    // Clear the filter if the selection change is not to exactly the filtered item
                    _filterHierarchy = null;
                    _filterItem = VSItemId.Nil;
                }
            }

            ClearCache();

            return VSErr.S_OK;
        }

        private void ClearCache()
        {
            _selectionItems = null;
            _selectionItemsRecursive = null;
            _filenames = null;
            _filenamesRecursive = null;
            _svnItems = null;
            _svnItemsRecursive = null;
            _selectedProjects = null;
            _selectedProjectsRecursive = null;
            _selectedHierarchies = null;
            _ownerProjects = null;

            _isSolutionExplorer = null;
            _miscFiles = null;
            _checkedMisc = false;
            _selectedItemsMap = null;
            _isSolutionSelected = null;
            _isSingleNodeSelection = null;
            _solutionFilename = null;
            _hashCache.Clear();

            if (_disposer != null)
            {
                Disposer d = _disposer;
                _disposer = null;
                d.Dispose();
            }
        }

        public IVsHierarchy MiscellaneousProject
        {
            get
            {
                if (!_checkedMisc)
                {
                    _checkedMisc = true;
                    _miscFiles = VsShellUtilities.GetMiscellaneousProject(Context, false) as IVsHierarchy;
                }
                return _miscFiles;
            }
        }

        public IVsSolution Solution
        {
            get
            {
                if (_solution == null)
                    _solution = (IVsSolution)Context.GetService(typeof(SVsSolution));

                return _solution;
            }
        }

        public string SolutionFilename
        {
            get
            {
                if (_solutionFilename == null)
                {
                    if (Solution != null)
                    {
                        string solutionDir, solutionFile, solutionUserFile;
                        if (VSErr.Succeeded(Solution.GetSolutionInfo(out solutionDir, out solutionFile, out solutionUserFile)))
                        {
                            if (!string.IsNullOrEmpty(solutionFile))
                            {
                                _solutionFilename = SvnItem.IsValidPath(solutionFile) ? SvnTools.GetTruePath(solutionFile, true) : solutionFile;
                            }
                            // Assigning  _solutionFilename to "", created a race condition:
                            // SolutionFilename is queried via SolutionSettings#RefreshIfDirty (triggered from AnkhServiceEvents#SolutionOpened/SolutionClosed events)
                            // SelectionContext cache might not yet be cleared (and returns null even if a new solution is opened)
                            //
                            // Follow through AnkhIssueService#OnSolutionOpened
                            // The following logic is disabled until a better solution is implemented.
                            /*
                            if (string.IsNullOrEmpty(solutionFile))
                                _solutionFilename = "";
                            else
                                _solutionFilename = SvnItem.IsValidPath(solutionFile) ? SvnTools.GetTruePath(solutionFile, true) : solutionFile;
                            */
                        }
                    }
                }
                return string.IsNullOrEmpty(_solutionFilename) ? null : _solutionFilename;
            }
        }

        #endregion

        IVsUIHierarchyWindow _solutionExplorerWindow;
        IVsUIHierarchyWindow SolutionExplorerWindow
        {
            get
            {
                return _solutionExplorerWindow ?? (_solutionExplorerWindow = VsShellUtilities.GetUIHierarchyWindow(Context, new Guid(ToolWindowGuids80.SolutionExplorer)));
            }
        }


        protected bool MightBeSolutionExplorerSelection
        {
            get
            {
                if (!_isSolutionExplorer.HasValue)
                {
                    _isSolutionExplorer = false;
                    IVsUIHierarchyWindow hw = SolutionExplorerWindow;
                    IntPtr hierarchy;
                    IVsMultiItemSelect ms;
                    uint itemId;

                    if (hw == null)
                        return false;

                    if (!VSErr.Succeeded(hw.GetCurrentSelection(out hierarchy, out itemId, out ms)))
                        return false;

                    IVsHierarchy hier = null;
                    if (hierarchy != IntPtr.Zero)
                    {
                        hier = (IVsHierarchy)Marshal.GetObjectForIUnknown(hierarchy);
                        Marshal.Release(hierarchy);
                    }

                    if (current.id != itemId)
                        return false;

                    if (ms != current.selection)
                        return false;

                    _isSolutionExplorer = ((hier is IVsSolution) || (hier == null)) && (SolutionFilename != null);
                }

                return _isSolutionExplorer.Value;
            }
        }

        public bool IsSingleNodeSelection
        {
            get
            {
                if (!_isSingleNodeSelection.HasValue)
                {
                    if (current.id == VSItemId.Selection
                        && current.selection != null)
                    {
                        uint nItems;
                        int withinSingleHierarchy;
                        if (VSErr.Succeeded(current.selection.GetSelectionInfo(out nItems, out withinSingleHierarchy)))
                        {
                            if (nItems == 1)
                                _isSingleNodeSelection = true;
                            else
                                _isSingleNodeSelection = false;
                        }
                        else
                            _isSingleNodeSelection = true;
                    }
                    else if (current.hierarchy != null)
                    {
                        switch (current.id)
                        {
                            case VSItemId.Selection:
                            case VSItemId.Nil:
                                _isSingleNodeSelection = false;
                                break;
                            default:
                                _isSingleNodeSelection = true;
                                break;
                        }
                    }
                    else
                        _isSingleNodeSelection = IsSolutionSelected;
                }

                return _isSingleNodeSelection.Value;
            }
        }

        internal protected IEnumerable<SelectionItem> GetSelectedItems(bool recursive)
        {
            return recursive ? GetSelectedItemsRecursive() : GetSelectedItems();
        }

        protected IEnumerable<SelectionItem> GetSelectedItems()
        {
            return _selectionItems ?? (_selectionItems = new CachedEnumerable<SelectionItem>(InternalGetSelectedItems(), Disposer));
        }

        protected IEnumerable<SelectionItem> GetSelectedItemsRecursive()
        {
            return _selectionItemsRecursive ?? (_selectionItemsRecursive = new CachedEnumerable<SelectionItem>(InternalGetSelectedItemsRecursive(), Disposer));
        }

        /// <summary>
        /// Gets the selected items; yielding for each result to allow delay loading
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectionItem> InternalGetSelectedItems()
        {
            HierarchySelection sel = current; // Cache the selection to make sure we don't use an id for another hierarchy

            if (sel.id == VSItemId.Selection)
            {
                uint nItems;
                int withinSingleHierarchy;

                if (sel.selection == null || !VSErr.Succeeded(sel.selection.GetSelectionInfo(out nItems, out withinSingleHierarchy)))
                    yield break;

                uint flags = 0;

                if ((withinSingleHierarchy != 0) && sel.hierarchy != null)
                    flags = (uint)__VSGSIFLAGS.GSI_fOmitHierPtrs; // Don't marshal the hierarchy for every item

                VSITEMSELECTION[] items = new VSITEMSELECTION[nItems];

                if (!VSErr.Succeeded(sel.selection.GetSelectedItems(flags, nItems, items)))
                    yield break;

                for (int i = 0; i < nItems; i++)
                {
                    IVsHierarchy hier = items[i].pHier ?? sel.hierarchy;

                    if (hier != null)
                        yield return new SelectionItem(hier, items[i].itemid);
                    else
                    {
                        if (items[i].itemid == VSItemId.Root && MightBeSolutionExplorerSelection)
                            yield return new SelectionItem((IVsHierarchy)Solution, VSItemId.Root,
                                SelectionUtils.GetSolutionAsSccProject(Context));
                        // else skip
                    }
                }
            }
            else if (sel.id != VSItemId.Nil && (sel.hierarchy != null))
            {
                if (sel.id == _filterItem && sel.hierarchy == _filterHierarchy)
                    yield break;

                yield return new SelectionItem(sel.hierarchy, sel.id);
            }
            else if (_currentContainer == null)
            {
                // No selection, no hierarchy.... -> no selection!
            }
            else if (sel.id == VSItemId.Root)
            {
                // This is the case in the solution explorer when only the solution is selected

                // We must validate whether the window is really the solution explorer

                if (MightBeSolutionExplorerSelection)
                {
                    IVsHierarchy hier = (IVsHierarchy)Solution;

                    if (hier != null)
                        yield return new SelectionItem(hier, VSItemId.Root,
                            SelectionUtils.GetSolutionAsSccProject(Context));
                }
            }
        }

        /// <summary>
        /// Gets the selected items and its descendants; yielding for each result to allow delay loading
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectionItem> InternalGetSelectedItemsRecursive()
        {
            Dictionary<SelectionItem, SelectionItem> ticked = new Dictionary<SelectionItem, SelectionItem>();
            foreach (SelectionItem si in GetSelectedItems())
            {
                if (ticked.ContainsKey(si))
                    continue;

                ticked.Add(si, si);
                yield return si;

                foreach (SelectionItem i in GetDescendants(si, ticked, ProjectWalkDepth.AllDescendants))
                {
                    yield return i;
                }
            }
        }

        internal static uint GetItemIdFromObject(object pvar)
        {
            if (pvar == null) return VSItemId.Nil;
            if (pvar is int) return (uint)(int)pvar;
            if (pvar is uint) return (uint)pvar;
            if (pvar is short) return (uint)(short)pvar;
            if (pvar is ushort) return (uint)(ushort)pvar;
            if (pvar is byte) return (uint)(byte)pvar;
            if (pvar is sbyte) return (uint)(sbyte)pvar;
            if (pvar is long) return (uint)(long)pvar;
            if (pvar is ulong) return (uint)(ulong)pvar;
            return VSItemId.Nil;
        }

        static Guid hierarchyId = typeof(IVsHierarchy).GUID;
        /// <summary>
        /// Gets the descendants of a selection item; yielding for each result to allow delay loading
        /// </summary>
        /// <param name="si"></param>
        /// <param name="previous"></param>
        /// <returns></returns>
        private IEnumerable<SelectionItem> GetDescendants(SelectionItem si, Dictionary<SelectionItem, SelectionItem> previous, ProjectWalkDepth depth)
        {
            if (si == null)
                throw new ArgumentNullException("si");

            // A hierarchy node can have 2 identities. We only need the inner one

            uint subId;
            IntPtr hierPtr;
            int hr;
            try
            {
                hr = si.Hierarchy.GetNestedHierarchy(si.Id, ref hierarchyId, out hierPtr, out subId);
            }
            catch (Exception e)
            {
                ExternalException ee = e as ExternalException;

                if (ee != null && !VSErr.Succeeded(ee.ErrorCode))
                    hr = ee.ErrorCode; // Should have been returned instead
                else if (e is NotImplementedException)
                    hr = VSErr.E_NOTIMPL; // From Microsoft.VisualStudio.PerformanceTools.DummyVsUIHierarchy.GetNestedHierarchy()
                else
                    hr = VSErr.E_FAIL;

                hierPtr = IntPtr.Zero;
                subId = VSItemId.Nil;
            }

            if (VSErr.Succeeded(hr) && hierPtr != IntPtr.Zero)
            {
                IVsHierarchy nestedHierarchy = Marshal.GetObjectForIUnknown(hierPtr) as IVsHierarchy;
                Marshal.Release(hierPtr);

                if (nestedHierarchy == null || (nestedHierarchy == MiscellaneousProject))
                    yield break;

                if (depth <= ProjectWalkDepth.AllDescendantsInHierarchy)
                {
                    yield break; // Don't walk into sub-hierarchies
                }
                else
                    si = new SelectionItem(nestedHierarchy, subId);
            }

            if (!previous.ContainsKey(si))
            {
                previous.Add(si, si);
                yield return si;
            }

            // Note: VS2005 and earlier have all projects on the top level; from VS2008+ projects are nested
            // We can ignore that as we would include the projects anyway

            if (!RecurseInto(si, depth))
                yield break;

            object value;
            if (!VSErr.Succeeded(si.Hierarchy.GetProperty(si.Id,
                (int)__VSHPROPID.VSHPROPID_FirstChild, out value)))
            {
                yield break;
            }

            uint childId = GetItemIdFromObject(value);
            while (childId != VSItemId.Nil)
            {
                SelectionItem i = new SelectionItem(si.Hierarchy, childId);

                foreach (SelectionItem ii in GetDescendants(i, previous, depth))
                {
                    yield return ii;
                }

                if (!VSErr.Succeeded(si.Hierarchy.GetProperty(i.Id,
                    (int)__VSHPROPID.VSHPROPID_NextSibling, out value)))
                {
                    yield break;
                }

                childId = GetItemIdFromObject(value);
            }
        }

        private bool RecurseInto(SelectionItem si, ProjectWalkDepth depth)
        {
            object value;

            if (VSErr.Succeeded(si.Hierarchy.GetProperty(si.Id,
                (int)__VSHPROPID.VSHPROPID_HasEnumerationSideEffects, out value)))
            {
                if (value != null)
                {
                    bool hasSideEffects = (bool)value;

                    // Unless we are walking SCC projects, don't go deeper
                    // because we don't want side effects!

                    if (hasSideEffects)
                    {
                        if (depth == ProjectWalkDepth.AllDescendantsInHierarchy && IgnoreSideEffects(si.SccProject))
                            return true;

                        return false;
                    }
                }
            }

            if (si.SccProject == null && VSErr.Succeeded(si.Hierarchy.GetProperty(si.Id,
                (int)__VSHPROPID2.VSHPROPID_ChildrenEnumerated, out value)))
            {
                if (value != null)
                {
                    bool childrenEnumerated = (bool)value;

                    if (!childrenEnumerated)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        IProjectFileMapper _projectMap;
        IProjectFileMapper ProjectMap
        {
            [DebuggerStepThrough]
            get { return _projectMap ?? (_projectMap = GetService<IProjectFileMapper>()); }
        }

        private bool IgnoreSideEffects(IVsSccProject2 sccProject)
        {
            if (sccProject != null && ProjectMap.IgnoreEnumerationSideEffects(sccProject))
                return true;

            return false;
        }

        public Hashtable Cache
        {
            get { return _hashCache; }
        }

        #region ISelectionContext Members: GetSelected*()

        protected IEnumerable<string> GetSelectedFiles()
        {
            return _filenames ?? (_filenames = new CachedEnumerable<string>(InternalGetSelectedFiles(false), Disposer));
        }

        protected IEnumerable<string> GetSelectedFilesRecursive()
        {
            return _filenamesRecursive ?? (_filenamesRecursive = new CachedEnumerable<string>(InternalGetSelectedFiles(true), Disposer));
        }

        public IEnumerable<string> GetSelectedFiles(bool recursive)
        {
            return recursive ? GetSelectedFilesRecursive() : GetSelectedFiles();
        }

        /// <summary>
        /// Gets the selected files; yielding for each result to allow delay loading
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        IEnumerable<string> InternalGetSelectedFiles(bool recursive)
        {
            HybridCollection<string> foundFiles = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

            foreach (SelectionItem i in GetSelectedItems(recursive))
            {
                string[] files;

                if (SelectionUtils.GetSccFiles(i, out files, true, true, null))
                {
                    foreach (string file in files)
                    {
                        if (!foundFiles.Contains(file))
                        {
                            foundFiles.Add(file);

                            yield return file;
                        }
                    }
                }
            }
        }

        protected IEnumerable<SvnItem> GetSelectedSvnItems()
        {
            return _svnItems ?? (_svnItems = new CachedEnumerable<SvnItem>(InternalGetSelectedSvnItems(false), Disposer));
        }

        protected IEnumerable<SvnItem> GetSelectedSvnItemsRecursive()
        {
            return _svnItemsRecursive ?? (_svnItemsRecursive = new CachedEnumerable<SvnItem>(InternalGetSelectedSvnItems(true), Disposer));
        }

        public IEnumerable<SvnItem> GetSelectedSvnItems(bool recursive)
        {
            return recursive ? GetSelectedSvnItemsRecursive() : GetSelectedSvnItems();
        }

        IEnumerable<SvnItem> InternalGetSelectedSvnItems(bool recursive)
        {
            if (SvnCache == null)
                yield break;

            foreach (string file in GetSelectedFiles(recursive))
            {
                yield return SvnCache[file];
            }
        }

        #endregion

        #region ISelectionContext Members: Get*Projects()

        public IEnumerable<SccProject> GetOwnerProjects()
        {
            return _ownerProjects ?? (_ownerProjects = new CachedEnumerable<SccProject>(InternalGetOwnerProjects(), Disposer));
        }

        protected IEnumerable<SccProject> InternalGetOwnerProjects()
        {
            Hashtable ht = new Hashtable();
            bool searchedProjectMapper = false;
            IProjectFileMapper projectMapper = null;

            foreach (SelectionItem si in GetSelectedItems(false))
            {
                if (ht.Contains(si.Hierarchy))
                    continue;

                ht.Add(si.Hierarchy, si);

                if (si.SccProject != null)
                {
                    yield return new SccProject(null, si.SccProject);
                    continue;
                }
                else if (si.Hierarchy is IVsSccVirtualFolders)
                    continue; // Skip URL WebApplications fast

                string[] files;

                // No need to fetch special files as we only want projects!
                if (!SelectionUtils.GetSccFiles(si, out files, false, false, null) || files.Length == 0)
                    continue; // No files selected

                if (projectMapper == null && !searchedProjectMapper)
                {
                    searchedProjectMapper = true;
                    projectMapper = GetService<IProjectFileMapper>();
                }

                if (projectMapper != null)
                    foreach (string file in files)
                    {
                        foreach (SccProject project in projectMapper.GetAllProjectsContaining(file))
                        {
                            if (project.RawHandle != null)
                            {
                                if (ht.Contains(project.RawHandle))
                                    continue;

                                ht.Add(project.RawHandle, si);
                                yield return project;
                            }
                            else if (!ht.Contains(project))
                            {
                                ht.Add(project, si);
                                yield return project;
                            }
                        }
                    }
            }
        }

        protected IEnumerable<SccProject> GetSelectedProjects()
        {
            return _selectedProjects ?? (_selectedProjects = new CachedEnumerable<SccProject>(InternalGetSelectedProjects(false), Disposer));
        }

        protected IEnumerable<SccProject> GetSelectedProjectsRecursive()
        {
            return _selectedProjectsRecursive ?? (_selectedProjectsRecursive = new CachedEnumerable<SccProject>(InternalGetSelectedProjects(true), Disposer));
        }

        public IEnumerable<SccProject> GetSelectedProjects(bool recursive)
        {
            return recursive ? GetSelectedProjectsRecursive() : GetSelectedProjects();
        }

        public IEnumerable<SccHierarchy> GetSelectedHierarchies()
        {
            return _selectedHierarchies ?? (_selectedHierarchies = new CachedEnumerable<SccHierarchy>(InternalGetSelectedHierarchies(false), Disposer));
        }

        protected IEnumerable<SccProject> InternalGetSelectedProjects(bool recursive)
        {
            foreach (SelectionItem item in GetSelectedItems(recursive))
            {
                if (item.Id == VSItemId.Root)
                {
                    if (!item.IsSolution && item.SccProject != null)
                        yield return new SccProject(null, item.SccProject);
                }
            }
        }

        protected IEnumerable<SccHierarchy> InternalGetSelectedHierarchies(bool recursive)
        {
            foreach (SelectionItem item in GetSelectedItems(recursive))
            {
                if (item.Id == VSItemId.Root)
                {
                    if (!item.IsSolution && item.Hierarchy != null)
                        yield return new SccHierarchy(item.Hierarchy);
                }
            }
        }

        public bool IsSolutionSelected
        {
            get
            {
                if (_isSolutionSelected.HasValue)
                    return _isSolutionSelected.Value;

                foreach (SelectionItem item in GetSelectedItems(false))
                {
                    if (item.IsSolution)
                    {
                        _isSolutionSelected = true;
                        return true;
                    }
                }

                _isSolutionSelected = false;
                return false;
            }
        }

        public IEnumerable<T> GetSelection<T>()
            where T : class
        {
            IEnumerable enumerable;
            if (_selectedItemsMap != null && _selectedItemsMap.TryGetValue(typeof(T), out enumerable))
                return (IEnumerable<T>)enumerable;

            IEnumerable<T> v = new CachedEnumerable<T>(InternalGetSelection<T>(), Disposer);

            if (_selectedItemsMap == null)
                _selectedItemsMap = new Dictionary<Type, IEnumerable>();

            _selectedItemsMap.Add(typeof(T), v);
            return v;
        }

        IEnumerable<T> InternalGetSelection<T>()
            where T : class
        {
            ISelectionContainer sc = _currentContainer;

            uint nItems;
            if (sc == null || !VSErr.Succeeded(sc.CountObjects((uint)ShellConstants.GETOBJS_SELECTED, out nItems)))
                yield break;

            object[] objs = new object[(int)nItems];
            if (VSErr.Succeeded(sc.GetObjects((uint)ShellConstants.GETOBJS_SELECTED, nItems, objs)))
            {
                foreach (object o in objs)
                {
                    T i = o as T;

                    if (i != null)
                        yield return i;
                }
            }
        }
        #endregion


        #region ISccProjectWalker Members

        bool IncludeNoScc(ProjectWalkDepth depth)
        {
            return (depth != ProjectWalkDepth.AllDescendantsInHierarchy) && (depth != ProjectWalkDepth.SpecialFiles);
        }

        /// <summary>
        /// Gets the list of files specified by the hierarchy (IVsSccProject2 or IVsHierarchy)
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="id"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        /// <remarks>The list might contain duplicates if files are included more than once</remarks>
        public IEnumerable<string> GetSccFiles(IVsHierarchy hierarchy, uint id, ProjectWalkDepth depth, IDictionary<string, uint> map)
        {
            // Note: This command is not cached like the other commands on this object!
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            SelectionItem si = new SelectionItem(hierarchy, id);

            string[] files;
            if (!SelectionUtils.GetSccFiles(si, out files, depth >= ProjectWalkDepth.SpecialFiles, IncludeNoScc(depth), map))
                yield break;

            foreach (string file in files)
            {
                yield return SvnTools.GetNormalizedFullPath(file);
            }

            if (depth > ProjectWalkDepth.SpecialFiles)
            {
                Dictionary<SelectionItem, SelectionItem> previous = new Dictionary<SelectionItem, SelectionItem>();
                previous.Add(si, si);

                foreach (SelectionItem item in GetDescendants(si, previous, depth))
                {
                    if (!SelectionUtils.GetSccFiles(item, out files, depth >= ProjectWalkDepth.SpecialFiles, depth != ProjectWalkDepth.AllDescendantsInHierarchy, map))
                        continue;

                    foreach (string file in files)
                    {
                        yield return SvnTools.GetNormalizedFullPath(file);
                    }
                }
            }
        }

        void ISccProjectWalker.SetPrecreatedFilterItem(IVsHierarchy hierarchy, uint id)
        {
            if (id != VSItemId.Nil || _filterItem != VSItemId.Nil)
                ClearCache(); // Make sure we use the filter directly

            _filterHierarchy = hierarchy;
            _filterItem = id;
        }

        #endregion
    }

    class CmdUIContextChangeEventArgs : EventArgs
    {
        readonly uint _cookie;
        readonly bool _active;

        public CmdUIContextChangeEventArgs(uint cookie, bool active)
        {
            _cookie = cookie;
            _active = active;
        }

        public uint Cookie
        {
            get { return _cookie; }
        }

        public bool Active
        {
            get { return _active; }
        }
    }
}
