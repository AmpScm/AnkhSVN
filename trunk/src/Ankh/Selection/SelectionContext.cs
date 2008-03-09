using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Collections;
using Microsoft.VisualStudio.OLE.Interop;
using IServiceProvider = System.IServiceProvider;
using Microsoft.VisualStudio.Shell;
using Ankh.SolutionExplorer;

namespace Ankh.Selection
{
    public interface ISelectionContext
    {
        ICollection<string> GetSelectedFiles();
        ICollection<string> GetSelectedFiles(bool recursive);
        ICollection<SvnItem> GetSelectedSvnItems();
        ICollection<SvnItem> GetSelectedSvnItems(bool recursive);

        string SolutionFilename { get; }

    }

    /// <summary>
    /// 
    /// </summary>
    class SelectionContext : IVsSelectionEvents, IDisposable, ISelectionContext
    {
        readonly IServiceProvider _context;
        readonly StatusCache _cache;
        readonly SolutionExplorerWindow _solutionExplorer;
        bool _disposed;
        uint _cookie;

        uint _currentItem;
        IVsHierarchy _currentHierarchy;
        IVsMultiItemSelect _currentSelection;
        ISelectionContainer _currentContainer;
        IVsSolution _solution;
        string[] _filenames;
        string[] _filenamesRecursive;
        SvnItem[] _svnItems;
        SvnItem[] _svnItemsRecursive;
        bool _deteminedSolutionExplorer;
        bool _isSolutionExplorer;
        string _solutionFilename;

        public SelectionContext(IServiceProvider environment, StatusCache cache, SolutionExplorerWindow solutionExplorer)
        {
            if (environment == null)
                throw new ArgumentNullException("environment");
            else if (cache == null)
                throw new ArgumentNullException("cache");
            else if (solutionExplorer == null)
                throw new ArgumentNullException("solutionExplorer");

            _context = environment;
            _cache = cache;
            _solutionExplorer = solutionExplorer;

            IVsMonitorSelection monitor = (IVsMonitorSelection)environment.GetService(typeof(IVsMonitorSelection));

            if (monitor != null)
                Marshal.ThrowExceptionForHR(monitor.AdviseSelectionEvents(this, out _cookie));
            else
                _context = null;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                IVsMonitorSelection monitor = (IVsMonitorSelection)_context.GetService(typeof(IVsMonitorSelection));

                Marshal.ThrowExceptionForHR(monitor.UnadviseSelectionEvents(_cookie));
                ClearCache();
            }
        }

        #region IVsSelectionEvents Members

        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            /// Some global state change which might change UI cueues
            return VSConstants.S_OK;
        }

        public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            // Some property changed
            return VSConstants.S_OK;
        }

        public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld,
                IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
        {
            if (!_disposed)
            {
                // The current selection changed; store for future reference
                _currentHierarchy = pHierNew;
                _currentItem = itemidNew;
                _currentSelection = pMISNew;
                _currentContainer = pSCNew;
            }

            ClearCache();

            return VSConstants.S_OK;
        }

        private void ClearCache()
        {
            _filenames = null;
            _svnItems = null;
            _filenamesRecursive = null;
            _svnItemsRecursive = null;
            _deteminedSolutionExplorer = false;
            _isSolutionExplorer = false;
            _solution = null;
            _solutionFilename = null;
        }

        public IVsSolution Solution
        {
            get
            {
                if (_solution == null)
                    _solution = (IVsSolution)_context.GetService(typeof(SVsSolution));

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
                        if (Solution.GetSolutionInfo(out solutionDir, out solutionFile, out solutionUserFile) == VSConstants.S_OK)
                        {
                            _solutionFilename = solutionFile;
                        }
                    }
                }
                return _solutionFilename;
            }
        }

        #endregion

        protected class SelectionItem : IEquatable<SelectionItem>
        {
            readonly IVsHierarchy _hierarchy;
            IVsSccProject2 _sccProject;
            readonly uint _id;

            public SelectionItem(IVsHierarchy hierarchy, uint id)
            {
                _hierarchy = hierarchy;
                _id = id;
            }

            public SelectionItem(IVsHierarchy hierarchy, uint id, IVsSccProject2 project)
            {
                _hierarchy = hierarchy;
                _sccProject = project;
                _id = id;
            }

            public IVsHierarchy Hierarchy
            {
                get { return _hierarchy; }
            }

            public virtual IVsSccProject2 SccProject
            {
                get
                {
                    if (_sccProject == null)
                        _sccProject = _hierarchy as IVsSccProject2;

                    return _sccProject;
                }
            }

            public uint Id
            {
                get { return _id; }
            }

            public virtual bool IsSolution
            {
                get { return false; }
            }

            #region IEquatable<SelectionItem> Members

            public bool Equals(SelectionItem other)
            {
                if (other == null)
                    return false;

                return (other.Id == Id) && (other.Hierarchy == Hierarchy);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SelectionItem);
            }

            public override int GetHashCode()
            {
                return _hierarchy.GetHashCode() ^ _id.GetHashCode();
            }

            #endregion
        }

        protected IEnumerable<SelectionItem> GetSelectedItems(bool recursive)
        {
            return recursive ? GetSelectedItemsRecursive() : GetSelectedItems();
        }

        protected bool MightBeSolutionExplorerSelection
        {
            get
            {
                if (!_deteminedSolutionExplorer)
                {
                    _deteminedSolutionExplorer = true;
                    IVsUIHierarchyWindow hw = _solutionExplorer.HierarchyWindow;
                    IntPtr hierarchy;
                    IVsMultiItemSelect ms;
                    uint itemId;

                    if (hw.GetCurrentSelection(out hierarchy, out itemId, out ms) != VSConstants.S_OK)
                        return _isSolutionExplorer = false;

                    IVsHierarchy hier = null;
                    if (hierarchy != IntPtr.Zero)
                    {
                        hier = (IVsHierarchy)Marshal.GetObjectForIUnknown(hierarchy);
                        Marshal.Release(hierarchy);
                    }

                    if (_currentItem != itemId)
                        return _isSolutionExplorer = false;

                    if (ms != _currentSelection)
                        return _isSolutionExplorer = false;

                    _isSolutionExplorer = (hier is IVsSolution);
                }

                return _isSolutionExplorer;
            }
        }

        protected IEnumerable<SelectionItem> GetSelectedItems()
        {
            if (_currentSelection != null)
            {
                uint nItems;
                int withinSingleHierarchy;
                Marshal.ThrowExceptionForHR(_currentSelection.GetSelectionInfo(out nItems, out withinSingleHierarchy));

                uint flags = 0;

                bool singleHierarchy = (withinSingleHierarchy != 0);

                if (singleHierarchy && _currentHierarchy != null)
                    flags = (uint)__VSGSIFLAGS.GSI_fOmitHierPtrs; // Don't marshal the hierarchy for every item

                VSITEMSELECTION[] items = new VSITEMSELECTION[nItems];

                Marshal.ThrowExceptionForHR(_currentSelection.GetSelectedItems(flags, nItems, items));

                for (int i = 0; i < nItems; i++)
                {
                    SelectionItem si = new SelectionItem(singleHierarchy ? _currentHierarchy : items[i].pHier, items[i].itemid);

                    if (si.Hierarchy != null)
                        yield return si;
                    else if (si.Id == VSConstants.VSITEMID_ROOT && MightBeSolutionExplorerSelection)
                        yield return new SelectionItem((IVsHierarchy)Solution, si.Id, SelectionUtils.GetSolutionAsSccProject(_context));
                    // else skip
                }
            }
            else if (_currentHierarchy != null)
            {
                yield return new SelectionItem(_currentHierarchy, _currentItem);
            }
            else if (_currentContainer == null)
            {
                // No selection, no hierarchy.... -> no selection!
            }
            else if (_currentItem == VSConstants.VSITEMID_ROOT)
            {
                // This is the case in the solution explorer when only the solution is selected

                // We must validate whether the window is really the solution explorer

                if (MightBeSolutionExplorerSelection)
                {
                    yield return new SelectionItem((IVsHierarchy)Solution, _currentItem, SelectionUtils.GetSolutionAsSccProject(_context));
                }
            }
        }

        protected IEnumerable<SelectionItem> GetSelectedItemsRecursive()
        {
            Dictionary<SelectionItem, SelectionItem> ticked = new Dictionary<SelectionItem, SelectionItem>();
            foreach (SelectionItem si in GetSelectedItems())
            {
                if (ticked.ContainsKey(si))
                    continue;

                ticked.Add(si, si);
                yield return si;

                foreach (SelectionItem i in GetDescendants(si, ticked))
                {
                    yield return i;
                }
            }
        }

        private static uint GetItemIdFromObject(object pvar)
        {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int) return (uint)(int)pvar;
            if (pvar is uint) return (uint)pvar;
            if (pvar is short) return (uint)(short)pvar;
            if (pvar is ushort) return (uint)(ushort)pvar;
            if (pvar is long) return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }

        private IEnumerable<SelectionItem> GetDescendants(SelectionItem si, Dictionary<SelectionItem, SelectionItem> previous)
        {
            if (si.Hierarchy == null)
                yield break;

            Guid hierarchyId = typeof(IVsHierarchy).GUID;
            IntPtr hierPtr;
            uint id;

            int hr = si.Hierarchy.GetNestedHierarchy(si.Id, ref hierarchyId, out hierPtr, out id);

            if (hr == VSConstants.S_OK && hierPtr != IntPtr.Zero)
            {
                IVsHierarchy nestedHierarchy = Marshal.GetObjectForIUnknown(hierPtr) as IVsHierarchy;
                Marshal.Release(hierPtr); // we are responsible to release the refcount on the out IntPtr parameter
                if (nestedHierarchy != null)
                {
                    // Display name and type of the node in the Output Window
                    SelectionItem i = new SelectionItem(nestedHierarchy, id);

                    if (previous.ContainsKey(i))
                        yield break;

                    previous.Add(i, i);

                    yield return i;

                    foreach (SelectionItem ii in GetDescendants(i, previous))
                    {
                        yield return ii;
                    }
                }
            }

            // Note: There is a bug with firstchild on solutions pre vs2008, in that it contains
            // all projects on the top level instead of below solution folders. But we can ignore 
            // that as we would include the projects anyway
            object child;
            Marshal.ThrowExceptionForHR(si.Hierarchy.GetProperty(si.Id,
                (int)__VSHPROPID.VSHPROPID_FirstChild, out child));

            uint childId = GetItemIdFromObject(child);
            while (childId != VSConstants.VSITEMID_NIL)
            {
                SelectionItem i = new SelectionItem(si.Hierarchy, childId);
                if (!previous.ContainsKey(i))
                {
                    previous.Add(i, i);
                    yield return i;
                }

                foreach (SelectionItem ii in GetDescendants(i, previous))
                {
                    yield return ii;
                }

                Marshal.ThrowExceptionForHR(si.Hierarchy.GetProperty(i.Id,
                    (int)__VSHPROPID.VSHPROPID_NextSibling, out child));

                childId = GetItemIdFromObject(child);
            }
        }


        #region ISelectionContext Members

        public ICollection<string> GetSelectedFiles()
        {
            return GetSelectedFiles(false);
        }

        public ICollection<string> GetSelectedFiles(bool recursive)
        {
            if (recursive && (_filenamesRecursive != null))
                return _filenamesRecursive;
            else if (!recursive && (_filenames != null))
                return _filenames;

            List<string> filenames = new List<string>();

            // Selection can be generated by several objects. 
            // E.g. the solution provider, a document, our own toolwindows..
            foreach (SelectionItem i in GetSelectedItems(recursive))
            {
                string[] files;

                if (SelectionUtils.GetSccFiles(i.Hierarchy, i.SccProject, i.Id, out files, true) == VSConstants.S_OK)
                    filenames.AddRange(files);
            }

            if (recursive)
                return _filenamesRecursive = filenames.ToArray();
            else
                return _filenames = filenames.ToArray();
        }

        public ICollection<SvnItem> GetSelectedSvnItems()
        {
            return GetSelectedSvnItems(false);
        }

        public ICollection<SvnItem> GetSelectedSvnItems(bool recursive)
        {
            if (recursive && (_svnItemsRecursive != null))
                return _svnItemsRecursive;
            else if (!recursive && (_svnItems != null))
                return _svnItems;

            List<SvnItem> items = new List<SvnItem>();
            foreach (string file in GetSelectedFiles(recursive))
            {
                SvnItem i = _cache[file];

                if (i != null)
                    items.Add(i);
            }

            if (recursive)
                return _svnItemsRecursive = items.ToArray();
            else
                return _svnItems = items.ToArray();
        }

        #endregion
    }
}