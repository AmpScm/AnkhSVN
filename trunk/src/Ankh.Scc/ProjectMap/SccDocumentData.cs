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
using Microsoft.VisualStudio.Shell.Interop;

using SharpSvn;
using Ankh.Selection;

namespace Ankh.Scc.ProjectMap
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Name={Name}, Dirty={IsDirty}")]
    sealed class SccDocumentData : IVsFileChangeEvents, IDisposable
    {
        readonly IAnkhServiceProvider _context;
        readonly string _name;
        readonly bool _isFileDocument;
        uint _cookie;
        bool _isDirty;
        bool _initialUpdateCompleted;
        bool? _isPropertyDesigner;
        object _rawDocument;
        uint[] _fileChangeCookies;
        bool _disposed;
        int _reloadTick;

        internal SccDocumentData(IAnkhServiceProvider context, string name)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            _context = context;
            _name = name;

            if (SvnItem.IsValidPath(name))
            {
                _isFileDocument = true;

                HookFileChanges(true);
            }
        }

        [DebuggerStepThrough]
        T GetService<T>()
            where T : class
        {
            return _context.GetService<T>();
        }

        [DebuggerStepThrough]
        T GetService<T>(Type serviceType)
            where T : class
        {
            return _context.GetService<T>(serviceType);
        }

        /// <summary>
        /// Gets the document name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets a cached value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get { return _isDirty; }
        }

        public uint Cookie
        {
            get { return _cookie; }
            internal set
            {
                if (_cookie != 0 && value != 0)
                    throw new InvalidOperationException();

                _cookie = value;
            }
        }

        /// <summary>
        /// Document instance which usually implements <see cref="IVsPersistDocData"/> and <see cref"IVsDocDataFileChangeControl"/>
        /// </summary>
        public object RawDocument
        {
            get { return _rawDocument ?? (_rawDocument = FetchDocument()); }
            internal set { _rawDocument = value; }
        }

        private object FetchDocument()
        {
            // Normally when a document is open we get a handle via the opendocument tracker; but we might be to soon
            IVsRunningDocumentTable rd = (IVsRunningDocumentTable)_context.GetService(typeof(SVsRunningDocumentTable));

            if (rd == null)
                return null;

            uint id;
            IVsHierarchy hier;
            IntPtr data;
            uint cookie; // We can't store the cookie, as that would mean it is in the lookup table
            if (ErrorHandler.Succeeded(rd.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_NoLock, Name, out hier, out id, out data, out cookie)))
            {
                if (hier != null)
                    Hierarchy = hier;
                if ((id != 0) && id != VSConstants.VSITEMID_NIL)
                    ItemId = id;

                if (data != IntPtr.Zero)
                {
                    _rawDocument = Marshal.GetObjectForIUnknown(data);
                    Marshal.Release(data);
                }
            }

            return _rawDocument;
        }

        IVsHierarchy _hierarchy;
        internal IVsHierarchy Hierarchy
        {
            get { return _hierarchy; }
            set { _hierarchy = value; }
        }

        uint _itemId;
        internal uint ItemId
        {
            get { return _itemId; }
            set { _itemId = value; }
        }

        /// <summary>
        /// Called when initialized from existing state; instead of document creation
        /// </summary>
        internal void OnCookieLoad()
        {
            _initialUpdateCompleted = true;
            _isDirty = GetIsDirty(true);
        }

        internal void OnSaved()
        {
            SetDirty(false);

            if (!_isFileDocument)
                return;

            IFileStatusMonitor statusMonitor = GetService<IFileStatusMonitor>();
            statusMonitor.ScheduleSvnStatus(Name);
        }

        internal void OnClosed(bool closedWithoutSaving)
        {
            SetDirty(false); // Mark as undirty
            Dispose();
        }

        internal void OnAttributeChange(__VSRDTATTRIB attributes)
        {
            if (0 != (attributes & __VSRDTATTRIB.RDTA_DocDataReloaded))
            {
                _reloadTick++;
                if (_initialUpdateCompleted && _isFileDocument)
                {
                    IFileStatusMonitor monitor = _context.GetService<IFileStatusMonitor>();

                    if (monitor != null)
                    {
                        monitor.ScheduleSvnStatus(Name);
                    }
                }
                else
                    _initialUpdateCompleted = true;
            }

            if (0 != (attributes & __VSRDTATTRIB.RDTA_DocDataIsDirty))
            {
                _initialUpdateCompleted = true;
                SetDirty(true);

            }
            else if (0 != (attributes & __VSRDTATTRIB.RDTA_DocDataIsNotDirty))
            {
                _initialUpdateCompleted = true;
                SetDirty(false);
            }
        }

        internal void SetDirty(bool dirty)
        {
            if (dirty == _isDirty)
                return;

            _isDirty = dirty;

            if (!_isFileDocument)
                return;

            // Make sure the dirty notification doesn't steal focus
            GetService<ISelectionContextEx>(typeof(ISelectionContext)).MaybeInstallDelayHandler();

            IFileStatusCache fcc = GetService<IFileStatusCache>();

            if (fcc != null)
            {
                ISvnItemStateUpdate sisu = fcc[Name];

                if (sisu != null)
                    sisu.SetDocumentDirty(dirty);
            }

            UpdateGlyph();
        }

        internal void CheckDirty()
        {
            if (IsDirty)
                return;

            bool wasDirty = IsDirty;
            bool dirty = GetIsDirty(true);

            if (dirty != wasDirty)
            {
                SetDirty(dirty);
            }
        }

        void UpdateGlyph()
        {
            if (!_isFileDocument)
                return;
            IFileStatusMonitor monitor = _context.GetService<IFileStatusMonitor>();

            if (monitor != null)
                monitor.ScheduleGlyphUpdate(Name);
        }

        public void Dispose()
        {
            _disposed = true;
            OpenDocumentTracker tracker = (OpenDocumentTracker)_context.GetService<IAnkhOpenDocumentTracker>();

            if (tracker != null)
                tracker.DoDispose(this);

            HookFileChanges(false);
        }

        internal void CopyState(SccDocumentData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            _initialUpdateCompleted = data._initialUpdateCompleted;
            _isDirty = data._isDirty;
            _itemId = data._itemId;
            _hierarchy = data._hierarchy;
        }

        /// <summary>
        /// Reloads the document and optionally clears the undo state
        /// </summary>
        /// <param name="clearUndo">if set to <c>true</c> [clear undo].</param>
        /// <param name="ignoreNextChange">if set to <c>true</c> [ignore next change].</param>
        /// <returns><c>true</c> if the document is reloaded, otherwise false</returns>
        public bool Reload(bool clearUndo, bool ignoreNextChange)
        {
            if (_disposed)
                return false;

            int reloadCookie = _reloadTick;
            bool wasDirty = IsDirty;

            IVsPersistDocData vsPersistDocData = RawDocument as IVsPersistDocData;
            if (vsPersistDocData != null)
            {
                // This method is valid on all text editors and probably many other editors

                try
                {
                    uint flags = 0;
                    if (clearUndo)
                        flags |= (uint)_VSRELOADDOCDATA.RDD_RemoveUndoStack;
                    if (ignoreNextChange)
                        flags |= (uint)_VSRELOADDOCDATA.RDD_IgnoreNextFileChange;

                    if (ignoreNextChange)
                        IgnoreFileChanges(true);

                    if (ErrorHandler.Succeeded(vsPersistDocData.ReloadDocData(flags)))
                    {
                        if (_disposed || (reloadCookie != _reloadTick) || (wasDirty != IsDirty))
                            return true;
                    }
                }
                catch
                { }
            }

            IVsPersistHierarchyItem2 vsPersistHierarchyItem2 = RawDocument as IVsPersistHierarchyItem2;
            if (vsPersistHierarchyItem2 != null)
            {
                // This route works for some project types and at least the solution
                try
                {
                    bool assumeOk = (_rawDocument is IVsSolution);

                    if (ErrorHandler.Succeeded(vsPersistHierarchyItem2.ReloadItem(VSConstants.VSITEMID_ROOT, 0)))
                    {
                        if (assumeOk || _disposed || reloadCookie != _reloadTick)
                            return true;
                    }
                }
                catch
                { }
            }


            vsPersistHierarchyItem2 = Hierarchy as IVsPersistHierarchyItem2;

            try
            {
                if (vsPersistHierarchyItem2 != null &&
                    ErrorHandler.Succeeded(vsPersistHierarchyItem2.ReloadItem(ItemId, 0)))
                {
                    // Our parent reloaded us
                    return true;
                }
            }
            catch
            { }

            return false; // We can't be reloaded by ourselves.. Let our caller reload our parent instead
        }

        /// <summary>
        /// Gets a value indicating whether this document is reloadable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is reloadable; otherwise, <c>false</c>.
        /// </value>
        public bool IsReloadable()
        {
            IVsPersistDocData pdd = RawDocument as IVsPersistDocData;

            if (pdd == null)
                return false;

            int reloadable;
            return ErrorHandler.Succeeded(pdd.IsDocDataReloadable(out reloadable)) && (reloadable != 0);
        }

        static readonly Guid ProjectPropertyPageHostGuid = new Guid("{b270807c-d8c6-49eb-8ebe-8e8d566637a1}");
        public bool IsProjectPropertyPageHost
        {
            get
            {
                if (!_isPropertyDesigner.HasValue && ItemId == VSConstants.VSITEMID_ROOT)
                {
                    _isPropertyDesigner = false;

                    IVsPersistDocData pdd = RawDocument as IVsPersistDocData;
                    Guid editorType;
                    if (pdd != null && ErrorHandler.Succeeded(pdd.GetGuidEditorType(out editorType)))
                    {
                        if (editorType == ProjectPropertyPageHostGuid)
                            _isPropertyDesigner = true;
                    }
                }

                return _isPropertyDesigner.HasValue && _isPropertyDesigner.Value;
            }
        }

        /// <summary>
        /// Sets the read only state of a document
        /// </summary>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        /// <returns></returns>
        public bool SetReadOnly(bool readOnly)
        {
            IVsPersistDocData2 pdd2 = RawDocument as IVsPersistDocData2;

            if (pdd2 == null)
                return false;

            return ErrorHandler.Succeeded(pdd2.SetDocDataReadOnly(readOnly ? 1 : 0));
        }

        /// <summary>
        /// Determines whether this instance is dirty
        /// </summary>
        /// <param name="fallback">if set to <c>true</c> return the cached value if the document says ist not dirty, false to trust the document.</param>
        /// <returns>
        /// 	<c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Gets the live data; or if that fails the cached data</remarks>
        internal bool GetIsDirty(bool fallback)
        {
            if (!_isFileDocument)
                return false; // Not interested

            IVsPersistDocData pdd = RawDocument as IVsPersistDocData;

            int dirty;
            if (pdd != null && ErrorHandler.Succeeded(pdd.IsDocDataDirty(out dirty)) && (dirty != 0))
                return true;

            IVsUIShellOpenDocument so = GetService<IVsUIShellOpenDocument>(typeof(SVsUIShellOpenDocument));

            Guid gV = Guid.Empty;
            IVsUIHierarchy hier;
            uint[] openId = new uint[1];
            IVsWindowFrame wf;
            int open;
            if (ErrorHandler.Succeeded(so.IsDocumentOpen(Hierarchy as IVsUIHierarchy, ItemId, this.Name, ref gV, (uint)__VSIDOFLAGS.IDO_IgnoreLogicalView,
                out hier, openId, out wf, out open)) && (open != 0) && wf != null)
            {
                if (wf != null)
                {
                    object ok;
                    if (ErrorHandler.Succeeded(wf.GetProperty((int)__VSFPROPID2.VSFPROPID_OverrideDirtyState, out ok)))
                    {
                        if (ok == null)
                        { }
                        else if (ok is bool) // Implemented by VS as bool
                        {
                            if ((bool)ok)
                                return true;
                        }
                    }
                }
            }

            return fallback && _isDirty;
        }

        /// <summary>
        /// Determines whether the document is readonly
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is read only]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsReadOnly()
        {
            IVsPersistDocData2 pdd2 = RawDocument as IVsPersistDocData2;

            if (pdd2 == null)
                return false;

            int readOnly;
            return ErrorHandler.Succeeded(pdd2.IsDocDataReadOnly(out readOnly)) && (readOnly != 0);
        }

        int _ignored;
        /// <summary>
        /// Determines whether changes to DocData in files should be ignored.
        /// </summary>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public bool IgnoreFileChanges(bool ignore)
        {
            IVsDocDataFileChangeControl ddfcc = RawDocument as IVsDocDataFileChangeControl;

            if (ddfcc == null)
                return false;

            if (ignore)
            {
                if (_ignored > 0)
                {
                    _ignored++;
                    return true;
                }
            }
            else
            {
                if (_ignored > 1)
                {
                    _ignored--;
                    return true;
                }
                else if (_ignored == 0)
                    return false; // We were not ignoring (Are we 100% reloaded?)
            }

            if (ErrorHandler.Succeeded(ddfcc.IgnoreFileChanges(ignore ? 1 : 0)))
            {
                if (ignore)
                    _ignored++;
                else
                    _ignored--;

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Tells the data document (object implementing IVsPersistDocData) to release 
        /// any hold it has on its storage (i.e. release any file system locks on its file).
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// After a call to HandsOff the document is either closed 
        /// (by reloading a changed project or solution file), or reloaded by calling ReloadDocData or ReloadItem.
        /// 
        /// If the data document hasn't been modified, HandsOnDocDataStorage will be called.
        /// 
        /// Most calls to the data document are expected to fail when the object is in hands off mode. 
        /// The only methods expected to work are IsDocDataReloadable and IsDocDataDirty.
        /// </remarks>
        public bool HandsOff()
        {
            IVsPersistDocData3 pdd3 = RawDocument as IVsPersistDocData3;

            if (pdd3 == null)
                return false;

            return ErrorHandler.Succeeded(pdd3.HandsOffDocDataStorage());
        }

        /// <summary>
        /// Enables the data document to place locks on its storage.
        /// </summary>
        public bool HandsOn()
        {
            IVsPersistDocData3 pdd3 = RawDocument as IVsPersistDocData3;

            if (pdd3 == null)
                return false;

            return ErrorHandler.Succeeded(pdd3.HandsOnDocDataStorage());
        }

        internal bool SaveDocument(IVsRunningDocumentTable rdt)
        {
            if (ErrorHandler.Succeeded(rdt.SaveDocuments(0, Hierarchy, ItemId, Cookie)))
            {
                SetDirty(false);
                return true;
            }

            return false;
        }

        #region IVsFileChangeEvents Members
        private void HookFileChanges(bool reHook)
        {
            IVsFileChangeEx fileChange = null;
            if (_fileChangeCookies != null)
            {
                fileChange = GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));
                uint[] list = _fileChangeCookies;
                _fileChangeCookies = null;

                foreach (uint u in list)
                    if (u != 0)
                        fileChange.UnadviseFileChange(u);
            }

            if (reHook && _isFileDocument)
            {
                if (fileChange == null)
                    fileChange = GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));

                List<SvnItem> items = new List<SvnItem>(GetService<AnkhSccProvider>(typeof(ITheAnkhSvnSccProvider)).GetAllDocumentItems(_name));

                uint[] cookies = new uint[items.Count];
                _fileChangeCookies = cookies;

                for (int i = 0; i < items.Count; i++)
                {
                    uint ck;
                    if (ErrorHandler.Succeeded(fileChange.AdviseFileChange(items[i].FullPath, (uint)(_VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time), this, out ck)))
                        cookies[i] = ck;
                }
            }
        }

        public int DirectoryChanged(string pszDirectory)
        {
            return VSConstants.S_OK;
        }

        public int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
        {
            if (rgpszFile != null)
            {
                string[] nFiles = new string[cChanges];

                for (int i = 0; i < cChanges; i++)
                    nFiles[i] = SvnTools.GetNormalizedFullPath(rgpszFile[i]);

                IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();

                monitor.ScheduleSvnStatus(nFiles);
            }

            return VSConstants.S_OK;
        }
        #endregion
    }
}
