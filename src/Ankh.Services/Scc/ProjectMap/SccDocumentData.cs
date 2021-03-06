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
    public sealed class SccDocumentData : AnkhService, IVsFileChangeEvents, IDisposable
    {
        readonly string _name;
        bool _isFileDocument;
        uint _cookie;
        bool _isDirty;
        bool _initialUpdateCompleted;
        bool? _isPropertyDesigner;
        object _rawDocument;
        uint[] _fileChangeCookies;
        bool _disposed;
        int _reloadTick;
        DateTime? _saving;
        _VSRDTFLAGS _flags;

        public SccDocumentData(IAnkhServiceProvider context, string name)
            : base(context)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            _name = name;

            if (SvnItem.IsValidPath(name))
            {
                _isFileDocument = true;

                HookFileChanges(true);
            }
        }

        [CLSCompliant(false)]
        public void SetFlags(_VSRDTFLAGS flags)
        {
            _flags = flags;
            bool isVirtual = ((flags & (_VSRDTFLAGS.RDT_VirtualDocument | _VSRDTFLAGS.RDT_ProjSlnDocument)) == _VSRDTFLAGS.RDT_VirtualDocument);

            if (_isFileDocument && isVirtual)
            {
                HookFileChanges(false);
                _isFileDocument = false;
                _fullPath = null;
            }
        }

        /// <summary>
        /// Gets the document name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }

        string _fullPath;
        /// <summary>
        /// Gets the full path to the document
        /// </summary>
        public string FullPath
        {
            get { return _isFileDocument ? (_fullPath ?? (_fullPath = SvnTools.GetNormalizedFullPath(_name))) : null; }
        }

        /// <summary>
        /// Gets a cached value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get { return _isDirty; }
        }

        [CLSCompliant(false)]
        public uint Cookie
        {
            get { return _cookie; }
            set
            {
                if (_cookie != 0 && value != 0)
                    throw new InvalidOperationException();

                _cookie = value;
            }
        }

        bool _fetchedRaw;

        /// <summary>
        /// Document instance which usually implements <see cref="IVsPersistDocData"/> and <see cref="IVsDocDataFileChangeControl"/>
        /// </summary>
        public object RawDocument
        {
            get
            {
                if (_fetchedRaw)
                    return _rawDocument;

                return _rawDocument ?? FetchDocument();
            }
            set { _rawDocument = value; _fetchedRaw = _fetchedRaw || (value != null); }
        }

        private object FetchDocument()
        {
            if ((_flags & RDT_PendingInitialization) != 0)
                return null;

            _fetchedRaw = true;

            // Normally when a document is open we get a handle via the opendocument tracker; but we might be to soon
            IVsRunningDocumentTable rd = GetService<IVsRunningDocumentTable>(typeof(SVsRunningDocumentTable));

            if (rd == null)
                return null;

            uint id;
            IVsHierarchy hier;
            IntPtr data;
            uint cookie; // We can't store the cookie, as that would mean it is in the lookup table
            if (VSErr.Succeeded(rd.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_NoLock, Name, out hier, out id, out data, out cookie)))
            {
                if (hier != null)
                    Hierarchy = hier;
                if ((id != 0) && id != VSItemId.Nil)
                    ItemId = id;

                if (data != IntPtr.Zero)
                {
                    _rawDocument = Marshal.GetUniqueObjectForIUnknown(data);
                    Marshal.Release(data);
                }
            }

            return _rawDocument;
        }

        IVsHierarchy _hierarchy;
        [CLSCompliant(false)]
        public IVsHierarchy Hierarchy
        {
            get { return _hierarchy; }
            set { _hierarchy = value; }
        }

        uint _itemId = VSItemId.Nil;
        [CLSCompliant(false)]
        public uint ItemId
        {
            get { return _itemId; }
            set { _itemId = value; }
        }

        /// <summary>
        /// Called when initialized from existing state; instead of document creation
        /// </summary>
        public void OnCookieLoad(TryDocumentDirtyPoller poller)
        {
            _initialUpdateCompleted = true;
            _isDirty = PollDirty(poller);
        }

        public void OnSaved()
        {
            SetDirty(false);

            if (!_isFileDocument)
                return;

            IFileStatusMonitor statusMonitor = GetService<IFileStatusMonitor>();
            statusMonitor.ScheduleSvnStatus(FullPath);
        }

        public void OnClosed()
        {
            SetDirty(false); // Mark as undirty
            Dispose();
        }

        const _VSRDTFLAGS RDT_DontPollForState = (_VSRDTFLAGS)0x00020000;
        const _VSRDTFLAGS RDT_PendingInitialization = (_VSRDTFLAGS)0x00040000;
        const _VSRDTFLAGS RDT_PendingHierarchyInitialization = (_VSRDTFLAGS)0x00080000;

        [CLSCompliant(false)]
        public const __VSRDTATTRIB RDTA_DocumentInitialized = (__VSRDTATTRIB)0x00100000; // VS2013+
        [CLSCompliant(false)]
        public const __VSRDTATTRIB RDTA_HierarchyInitialized = (__VSRDTATTRIB)0x00200000; // VS2013+

        [CLSCompliant(false)]
        public void OnAttributeChange(__VSRDTATTRIB attributes, TryDocumentDirtyPoller poller)
        {
            if (0 != (attributes & RDTA_DocumentInitialized))
            {
                _flags = (_VSRDTFLAGS)((int)_flags & ~(int)RDT_PendingInitialization);
            }
            if (0 != (attributes & RDTA_HierarchyInitialized))
            {
                _flags = (_VSRDTFLAGS)((int)_flags & ~(int)RDT_PendingHierarchyInitialization);
            }

            if (0 != (attributes & __VSRDTATTRIB.RDTA_DocDataReloaded))
            {
                _reloadTick++;
                _saving = null;
                if (_initialUpdateCompleted && _isFileDocument)
                {
                    IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();

                    if (monitor != null)
                    {
                        bool wasDirty = _isDirty;
                        _isDirty = false;
                        bool dirty = PollDirty(poller);
                        if (wasDirty != IsDirty)
                        {
                            _isDirty = wasDirty;
                            SetDirty(IsDirty);
                        }

                        monitor.ScheduleGlyphUpdate(FullPath);
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

        public void SetDirty(bool dirty)
        {
            if (dirty == _isDirty)
                return;

            _isDirty = dirty;

            if (!_isFileDocument)
                return;

            IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();

            if (monitor != null)
                monitor.SetDocumentDirty(FullPath, dirty);

            ISelectionContextEx selection = GetService<ISelectionContextEx>(typeof(ISelectionContext));
            
            if (selection != null)
                selection.MaybeInstallDelayHandler();
        }

        public void CheckDirty(TryDocumentDirtyPoller poller)
        {
            if (IsDirty)
                return;

            if (PollDirty(poller))
            {
                SetDirty(true);
            }
        }

        public void Dispose()
        {
            _disposed = true;
            IAnkhOpenDocumentTracker tracker = GetService<IAnkhOpenDocumentTracker>(typeof(IAnkhOpenDocumentTracker));

            if (tracker != null)
                tracker.DoDispose(this);

            HookFileChanges(false);
        }

        public void CopyState(SccDocumentData data)
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
        /// <returns><c>true</c> if the document is reloaded, otherwise false</returns>
        public bool Reload(bool clearUndo)
        {
            if (_disposed)
                return false;

            if (!IsDocumentInitialized)
                return true; // Not loaded, so no reload necessary!

            int reloadCookie = _reloadTick;
            bool wasDirty = IsDirty;

            IVsPersistDocData vsPersistDocData = RawDocument as IVsPersistDocData;
            if (vsPersistDocData != null)
            {
                // This method is valid on all text editors and probably many other editors

                uint flags = 0;
                if (clearUndo)
                    flags |= (uint)_VSRELOADDOCDATA.RDD_RemoveUndoStack;

                bool ok;

                // Temporarily suspend our ignore 'lock' as without that documents
                // don't reload properly.

                if (_ignoring)
                    EnsureIgnored(false);

                try
                {
                    ok = SafeSucceeded(vsPersistDocData.ReloadDocData, flags);
                }
                finally
                {
                    EnsureIgnored(_ignored > 0);
                }

                if (ok)
                {
                    if (_disposed || (reloadCookie != _reloadTick) || (wasDirty != IsDirty))
                        return true;
                }
            }

            IVsPersistHierarchyItem2 vsPersistHierarchyItem2 = RawDocument as IVsPersistHierarchyItem2;
            if (vsPersistHierarchyItem2 != null)
            {
                // This route works for some project types and at least the solution
                bool assumeOk = (_rawDocument is IVsSolution);

                if (SafeSucceeded(vsPersistHierarchyItem2.ReloadItem, VSItemId.Root, (uint)0))
                {
                    if (assumeOk || _disposed || reloadCookie != _reloadTick)
                        return true;
                }
            }


            vsPersistHierarchyItem2 = Hierarchy as IVsPersistHierarchyItem2;

            if (vsPersistHierarchyItem2 != null &&
                SafeSucceeded(vsPersistHierarchyItem2.ReloadItem, ItemId, (uint)0))
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
            return SafeSucceeded(pdd.IsDocDataReloadable, out reloadable) && (reloadable != 0);
        }

        static readonly Guid ProjectPropertyPageHostGuid = new Guid("{b270807c-d8c6-49eb-8ebe-8e8d566637a1}");
        public bool IsProjectPropertyPageHost
        {
            get
            {
                if (!_isPropertyDesigner.HasValue)
                {
                    _isPropertyDesigner = false;
                    if (ItemId == VSItemId.Root
                        && (_flags & _VSRDTFLAGS.RDT_VirtualDocument) != 0)
                    {
                        IVsPersistDocData pdd = RawDocument as IVsPersistDocData;
                        Guid editorType;
                        if (pdd != null && SafeSucceeded(pdd.GetGuidEditorType, out editorType))
                        {
                            if (editorType == ProjectPropertyPageHostGuid)
                                _isPropertyDesigner = true;
                        }
                    }
                }

                return _isPropertyDesigner.Value;
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

            return SafeSucceeded(pdd2.SetDocDataReadOnly, readOnly ? 1 : 0);
        }

        public delegate bool TryDocumentDirtyPoller(SccDocumentData data, out bool dirty);

        /// <summary>
        /// Determines whether this instance is dirty
        /// </summary>
        /// <param name="tryPoll">if set to <c>true</c> return the cached value if the document says ist not dirty, false to trust the document.</param>
        /// <returns>
        /// 	<c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Gets the live data; or if that fails the cached data</remarks>
        bool PollDirty(TryDocumentDirtyPoller tryPoll)
        {
            if (!_isFileDocument)
                return false; // Not interested
            else if (!IsDocumentInitialized)
                return false; // Not initialized, so no in memory dirty state
            else if ((_flags & RDT_DontPollForState) != 0)
                return _isDirty; // Don't poll. "Don't call us, we call you"

            bool dirty;
            if (tryPoll(this, out dirty))
            {
                return dirty;
            }
            else
                return _isDirty;
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
            return SafeSucceeded(pdd2.IsDocDataReadOnly, out readOnly) && (readOnly != 0);
        }

        int _ignored;
        /// <summary>
        /// Determines whether changes to DocData in files should be ignored.
        /// </summary>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public bool IgnoreFileChanges(bool ignore)
        {
            if (!IsDocumentInitialized)
                return false;

            if (ignore)
            {
                if (_ignored > 0)
                {
                    _ignored++;
                    EnsureIgnored(true);
                    return true;
                }
                else if(_ignored == 0)
                {
                    if (EnsureIgnored(true))
                    {
                        _ignored++;
                        return true;
                    }
                }
            }
            else
            {
                if (_ignored > 1)
                {
                    _ignored--;
                    EnsureIgnored(true);
                    return true;
                }
                else if (_ignored == 1)
                {
                    if (EnsureIgnored(false))
                    {
                        _ignored = 0;
                        return true;
                    }
                }
            }

            return false;
        }

        bool _ignoring;
        bool EnsureIgnored(bool ignore)
        {
            if (!IsDocumentInitialized)
                return false;
            else if (_ignoring == ignore)
                return true;

            IVsDocDataFileChangeControl ddfcc = RawDocument as IVsDocDataFileChangeControl;

            if (ddfcc != null)
            {
                if (SafeSucceeded(ddfcc.IgnoreFileChanges, ignore ? 1 : 0))
                {
                    _ignoring = ignore;
                    return true;
                }
            }

            return false;
        }

        [CLSCompliant(false)]
        public bool SaveDocument(IVsRunningDocumentTable rdt)
        {
            if (!IsDocumentInitialized || (_flags & _VSRDTFLAGS.RDT_DontSave) != 0)
                return true;

            if (VSErr.Succeeded(rdt.SaveDocuments(0, Hierarchy, ItemId, Cookie)))
            {
                SetDirty(false);
                return true;
            }

            return false;
        }

        public DateTime? Saving
        {
            get { return _saving; }
            set { _saving = value; }
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

                List<string> items = new List<string>(GetService<IProjectFileMapper>().GetAllDocumentFiles(_name));

                uint[] cookies = new uint[items.Count];
                _fileChangeCookies = cookies;

                for (int i = 0; i < items.Count; i++)
                {
                    uint ck;
                    if (VSErr.Succeeded(fileChange.AdviseFileChange(items[i], (uint)(_VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time), this, out ck)))
                        cookies[i] = ck;
                }
            }
        }

        public int DirectoryChanged(string pszDirectory)
        {
            return VSErr.S_OK;
        }

        [CLSCompliant(false)]
        public int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
        {
            if (rgpszFile == null || cChanges == 0)
                return VSErr.S_OK;

            try
            {
                string[] nFiles = new string[cChanges];

                for (int i = 0; i < cChanges; i++)
                    nFiles[i] = SvnTools.GetNormalizedFullPath(rgpszFile[i]);

                IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();

                monitor.ScheduleSvnStatus(nFiles);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();
                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }

            return VSErr.S_OK;
        }
        #endregion

        public bool IsDocumentInitialized
        {
            get
            {
                // Documents that are not initialized yet, and ones that don't have a lock
                return (_flags & RDT_PendingInitialization) == 0;
            }
        }

        public bool NeedsNoDirtyCheck
        {
            get
            {
                //const _VSRDTFLAGS projectFlags = (_VSRDTFLAGS.RDT_ProjSlnDocument | _VSRDTFLAGS.RDT_VirtualDocument);

                if ((_flags & RDT_DontPollForState) != 0)
                    return true;
                else if (!IsDocumentInitialized)
                    return true;
                //else if ((_flags & projectFlags) == projectFlags 
                //         && !Name.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
                //                    return true;

                return false;
            }
        }
    }
}
