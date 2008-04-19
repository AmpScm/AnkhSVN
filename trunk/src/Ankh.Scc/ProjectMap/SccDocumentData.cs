﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Selection;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Ankh.Scc.ProjectMap
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Name={Name}, Dirty={IsDirty}")]
    sealed class SccDocumentData
    {
        readonly IAnkhServiceProvider _context;
        readonly string _name;
        readonly bool _isFileDocument;
        uint _cookie;
        bool _isDirty;
        bool _initialUpdateCompleted;
        object _rawDocument;

        internal SccDocumentData(IAnkhServiceProvider context, string name)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            _context = context;
            _name = name;

            IFileStatusCache fcc = GetService<IFileStatusCache>();

            if (fcc != null && fcc.IsValidPath(name))
            {
                _isFileDocument = true;
                ISvnItemStateUpdate sisu = fcc[name];

                if (sisu != null)
                    sisu.SetDocumentOpen(true);
            }
        }

        T GetService<T>()
            where T : class
        {
            return _context.GetService<T>();
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

                if (data != null)
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
            _isDirty = GetIsDirty();
        }

        internal void OnSaved()
        {
            SetDirty(false);

            if(_isFileDocument)
                return;
            
            IFileStatusMonitor statusMonitor = GetService<IFileStatusMonitor>();
            statusMonitor.ScheduleSvnStatus(Name);
        }

        internal void OnClosed(bool closedWithoutSaving)
        {
            if (closedWithoutSaving && _isDirty)
            {
                _isDirty = false;
                UpdateGlyph();
            }

            IFileStatusCache fcc = GetService<IFileStatusCache>();
            if (fcc != null)
            {
                ISvnItemStateUpdate sisu = fcc[Name];

                if (sisu != null)
                    sisu.SetDocumentOpen(false);
            }

            Dispose();
        }

        internal void OnAttributeChange(__VSRDTATTRIB attributes)
        {
            if (0 != (attributes & __VSRDTATTRIB.RDTA_DocDataReloaded))
            {
                if (_initialUpdateCompleted)
                {
                    IFileStatusCache statusCache = _context.GetService<IFileStatusCache>();

                    if (statusCache != null)
                    {
                        if (statusCache.IsValidPath(Name))
                        {
                            statusCache.MarkDirty(Name);
                            UpdateGlyph();
                        }
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
            UpdateGlyph();
        }

        internal void CheckDirty()
        {
            if (IsDirty)
                return;

            bool wasDirty = IsDirty;
            bool dirty = GetIsDirty();

            if (dirty != wasDirty)
            {
                _isDirty = wasDirty;

                if (!_isFileDocument)
                    return;

                IFileStatusCache fcc = GetService<IFileStatusCache>();

                if (fcc != null)
                {
                    ISvnItemStateUpdate sisu = fcc[Name];

                    if (sisu != null)
                        sisu.SetDocumentDirty(dirty);
                }

                UpdateGlyph();
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

        internal void Dispose()
        {
            OpenDocumentTracker tracker = (OpenDocumentTracker)_context.GetService<IAnkhOpenDocumentTracker>();

            if (tracker != null)
                tracker.DoDispose(this);
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
        /// <returns></returns>
        public bool Reload(bool clearUndo)
        {
            IVsPersistDocData vsPersistDocData = RawDocument as IVsPersistDocData;
            if (vsPersistDocData != null)
            {
                // This method is valid on all text editors and probably many other editors

                if (ErrorHandler.Succeeded(vsPersistDocData.ReloadDocData(clearUndo ? (uint)_VSRELOADDOCDATA.RDD_RemoveUndoStack : 0)))
                    return true;
            }

            IVsPersistHierarchyItem2 vsPersistHierarchyItem2 = RawDocument as IVsPersistHierarchyItem2;
            if (vsPersistHierarchyItem2 != null)
            {
                // This route works for some project types and at least the solution
                if (ErrorHandler.Succeeded(vsPersistHierarchyItem2.ReloadItem(VSConstants.VSITEMID_ROOT, 0)))
                    return true;
            }

            
            vsPersistHierarchyItem2 = Hierarchy as IVsPersistHierarchyItem2;

            if (vsPersistHierarchyItem2 != null &&
                ErrorHandler.Succeeded(vsPersistHierarchyItem2.ReloadItem(ItemId, 0)))
            {
                // Our parent reloaded us
                return true;
            }

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
        /// <returns>
        /// 	<c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Gets the live data; or if that fails the cached data</remarks>
        internal bool GetIsDirty()
        {
            IVsPersistDocData pdd = RawDocument as IVsPersistDocData;

            if(pdd == null)
                return _isDirty;

            int dirty;
            return ErrorHandler.Succeeded(pdd.IsDocDataDirty(out dirty)) ? (dirty != 0) : _isDirty;
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
                else if(_ignored == 0)
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
            return ErrorHandler.Succeeded(rdt.SaveDocuments(0, Hierarchy, ItemId, Cookie));
        }
    }
}
