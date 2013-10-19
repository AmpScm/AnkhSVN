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
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

using Ankh.Scc.ProjectMap;


namespace Ankh.Scc
{
    [GlobalService(typeof(IAnkhOpenDocumentTracker))]
    partial class OpenDocumentTracker : AnkhService, IAnkhOpenDocumentTracker, IVsRunningDocTableEvents4, IVsRunningDocTableEvents3, IVsRunningDocTableEvents2, IVsRunningDocTableEvents
    {
        readonly Dictionary<string, SccDocumentData> _docMap = new Dictionary<string, SccDocumentData>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<uint, SccDocumentData> _cookieMap = new Dictionary<uint, SccDocumentData>();
        readonly SccDocumentData.TryDocumentDirtyPoller _poller;
        bool _hooked;
        uint _cookie;

        public OpenDocumentTracker(IAnkhServiceProvider context)
            : base(context)
        {
            _poller = VSVersion.VS2012OrLater ? (SccDocumentData.TryDocumentDirtyPoller)TryPollDirty : TryPollDirtyFallback;
        }

        protected override void OnInitialize()
        {
            Hook(true);
            LoadInitial();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                    Hook(false);
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        IVsRunningDocumentTable _docTable;
        protected IVsRunningDocumentTable RunningDocumentTable
        {
            [DebuggerStepThrough]
            get { return _docTable ?? (_docTable = GetService<IVsRunningDocumentTable>(typeof(SVsRunningDocumentTable))); }
        }

        SvnSccProvider _sccProvider;
        protected SvnSccProvider SccProvider
        {
            [DebuggerStepThrough]
            get { return _sccProvider ?? (_sccProvider = GetService<SvnSccProvider>()); }
        }

        ProjectTracker _projectTracker;
        protected ProjectTracker ProjectTracker
        {
            [DebuggerStepThrough]
            get { return _projectTracker ?? (_projectTracker = GetService<ProjectTracker>(typeof(IAnkhProjectDocumentTracker))); }
        }

        void LoadInitial()
        {
            IVsRunningDocumentTable rdt = RunningDocumentTable;
            if (rdt == null)
                return;

            IEnumRunningDocuments docEnum;
            if (!VSErr.Succeeded(rdt.GetRunningDocumentsEnum(out docEnum)))
                return;

            uint[] cookies = new uint[256];
            uint nFetched;
            while (VSErr.Succeeded(docEnum.Next((uint)cookies.Length, cookies, out nFetched)))
            {
                if (nFetched == 0)
                    break;

                for (int i = 0; i < nFetched; i++)
                {
                    SccDocumentData data;
                    if (TryGetDocument(cookies[i], out data))
                    {
                        data.OnCookieLoad(_poller);
                    }
                }
            }
        }

        void Hook(bool enable)
        {
            if (enable == _hooked)
                return;

            IVsRunningDocumentTable rdt = RunningDocumentTable;

            if (rdt == null)
                return;

            if (enable)
            {
                if (VSErr.Succeeded(rdt.AdviseRunningDocTableEvents(this, out _cookie)))
                    _hooked = true;
            }
            else
            {
                _docMap.Clear();
                _cookieMap.Clear();

                _hooked = false;
                rdt.UnadviseRunningDocTableEvents(_cookie);
            }
        }

        bool TryGetDocument(uint cookie, out SccDocumentData data)
        {
            return TryGetDocument(cookie, false, out data);
        }

        bool TryGetDocument(uint cookie, bool create, out SccDocumentData data)
        {
            if (cookie == 0)
            {
                data = null;
                return false;
            }

            if (_cookieMap.TryGetValue(cookie, out data))
                return true;

            if (!create)
            {
                data = null;
                return false;
            }

            string name;
            uint flags;
            IVsHierarchy hier;
            uint itemId;
            object document;

            if (TryGetDocumentInfo(cookie, out name, out flags, out hier, out itemId, out document))
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (_docMap.TryGetValue(name, out data))
                    {
                        if (data.Cookie != 0)
                        {
                            _cookieMap.Remove(data.Cookie);
                            data.Cookie = 0;
                        }
                    }
                    else
                    {
                        _docMap.Add(name, data = new SccDocumentData(Context, name));

                        if (hier != null)
                        {
                            data.Hierarchy = hier;
                            data.ItemId = itemId;
                        }
                    }

                    data.SetFlags((_VSRDTFLAGS)flags);

                    if (document != null)
                        data.RawDocument = document;

                    data.Cookie = cookie;
                    _cookieMap.Add(cookie, data);
                }
            }
            else
                data = null;

            return (data != null);
        }

        static class VSReflection
        {
            public delegate uint GetDocumentCookie(string moniker);
            public delegate string GetDocumentMoniker(uint cookie);
            public delegate uint GetDocumentFlags(uint cookie);
            public delegate bool IsDocumentDirty(uint cookie);
        }
        VSReflection.GetDocumentCookie GetDocumentCookie_cb;
        VSReflection.GetDocumentMoniker GetDocumentMoniker_cb;
        VSReflection.GetDocumentFlags GetDocumentFlags_cb;
        VSReflection.IsDocumentDirty IsDocumentDirty_cb;
        bool _documentInfo_init;

        void DocumentInfoInit()
        {
            if (VSVersion.VS2013OrLater)
            {
                Type IVsRunningDocumentTable4_type = VSAssemblies.VSShellInterop12.GetType("Microsoft.VisualStudio.Shell.Interop.IVsRunningDocumentTable4", false);

                GetDocumentCookie_cb = GetInterfaceDelegate<VSReflection.GetDocumentCookie>(IVsRunningDocumentTable4_type, RunningDocumentTable);
                GetDocumentMoniker_cb = GetInterfaceDelegate<VSReflection.GetDocumentMoniker>(IVsRunningDocumentTable4_type, RunningDocumentTable);
                GetDocumentFlags_cb = GetInterfaceDelegate<VSReflection.GetDocumentFlags>(IVsRunningDocumentTable4_type, RunningDocumentTable);
            }

            if (VSVersion.VS2012OrLater)
            {
                Type IVsRunningDocumentTable3_type = VSAssemblies.VSShellInterop11.GetType("Microsoft.VisualStudio.Shell.Interop.IVsRunningDocumentTable3", false);

                IsDocumentDirty_cb = GetInterfaceDelegate<VSReflection.IsDocumentDirty>(IVsRunningDocumentTable3_type, RunningDocumentTable);
            }
        }

        private bool TryGetDocumentInfo(uint cookie, out string name, out uint flags, out IVsHierarchy hier, out uint itemId, out object document)
        {
            if (!_documentInfo_init)
            {
                _documentInfo_init = true;
                DocumentInfoInit();
            }

            if (GetDocumentMoniker_cb != null)
            {
                // Allow VS2013 to delayload windows
                try
                {
                    name = GetDocumentMoniker_cb(cookie);
                    flags = GetDocumentFlags_cb(cookie);

                    hier = null;
                    itemId = VSConstants.VSITEMID_NIL;
                    document = null;
                    return true;
                }
                catch
                { }
            }

            IntPtr ppunkDocData;
            uint locks;
            uint editLocks;

            if (VSErr.Succeeded(RunningDocumentTable.GetDocumentInfo(cookie,
                out flags, out locks, out editLocks, out name, out hier, out itemId, out ppunkDocData)))
            {
                if (ppunkDocData != IntPtr.Zero)
                {
                    document = Marshal.GetUniqueObjectForIUnknown(ppunkDocData);
                    Marshal.Release(ppunkDocData);
                }
                else
                    document = null;

                return true;
            }
            else
            {
                hier = null;
                itemId = VSConstants.VSITEMID_NIL;
                document = null;
                return false;
            }
        }

        /// <summary>
        /// Called before a document is locked in the Running Document Table (RDT) for the first time.
        /// </summary>
        /// <param name="pHier">[in] The <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy"></see> object that owns the document about to be locked.</param>
        /// <param name="itemid">[in] The item ID in the hierarchy. This is a unique identifier or it can be one of the following values: <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_NIL"></see>, <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_ROOT"></see>, or <see cref="F:Microsoft.VisualStudio.VSConstants.VSITEMID_SELECTION"></see>.</param>
        /// <param name="pszMkDocument">[in] The path to the document about to be locked.</param>
        /// <returns>
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSErr.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int OnBeforeFirstDocumentLock(IVsHierarchy pHier, uint itemid, string pszMkDocument)
        {
            if (string.IsNullOrEmpty(pszMkDocument))
            {
                return VSErr.S_OK; // Can't be a valid path; don't monitor
            }

            SccDocumentData data;
            if (!_docMap.TryGetValue(pszMkDocument, out data))
            {
                _docMap.Add(pszMkDocument, data = new SccDocumentData(Context, pszMkDocument));

                data.Hierarchy = pHier;
                data.ItemId = itemid;
            }

            return VSErr.S_OK;
        }

        /// <summary>
        /// Called when [after last document unlock].
        /// </summary>
        /// <param name="pHier">The p hier.</param>
        /// <param name="itemid">The itemid.</param>
        /// <param name="pszMkDocument">The PSZ mk document.</param>
        /// <param name="fClosedWithoutSaving">The f closed without saving.</param>
        /// <returns></returns>
        public int OnAfterLastDocumentUnlock(IVsHierarchy pHier, uint itemid, string pszMkDocument, int fClosedWithoutSaving)
        {
            if (string.IsNullOrEmpty(pszMkDocument))
                return VSErr.S_OK;

            SccDocumentData data;
            if (_docMap.TryGetValue(pszMkDocument, out data))
            {
                data.OnClosed();
                _docMap.Remove(data.Name);

                if (data.Cookie != 0)
                    _cookieMap.Remove(data.Cookie);
            }

            return VSErr.S_OK;
        }

        public int OnBeforeSave(uint docCookie)
        {
            SccDocumentData data;
            if (TryGetDocument(docCookie, out data))
            {
                data.Saving = DateTime.Now;
            }

            return VSErr.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            SccDocumentData data;

            if (TryGetDocument(docCookie, out data))
            {
                data.OnSaved();
                data.Saving = null;

                if (data.IsProjectPropertyPageHost)
                {
                    ProjectPropertyPageFixup(data);
                }
            }
            return VSErr.S_OK;
        }

        /// <summary>
        /// Fired after a Save All command is executed.
        /// </summary>
        /// <returns></returns>
        public int OnAfterSaveAll()
        {
            return VSErr.S_OK;
        }


        internal const __VSRDTATTRIB RDTA_DocumentInitialized = (__VSRDTATTRIB)0x00100000; // VS2013+
        internal const __VSRDTATTRIB RDTA_HierarchyInitialized = (__VSRDTATTRIB)0x00200000; // VS2013+
        const uint HandledRDTAttributes = (uint)(__VSRDTATTRIB.RDTA_DocDataReloaded
                                                 | __VSRDTATTRIB.RDTA_DocDataIsDirty
                                                 | __VSRDTATTRIB.RDTA_DocDataIsNotDirty
                                                 | /*__VSRDTATTRIB3.*/RDTA_DocumentInitialized
                                                 | /*__VSRDTATTRIB3.*/RDTA_HierarchyInitialized);

        const uint TrackedRDTAttributes = HandledRDTAttributes
                                          | (uint)(__VSRDTATTRIB.RDTA_ItemID
                                          | __VSRDTATTRIB.RDTA_Hierarchy
                                          | __VSRDTATTRIB.RDTA_MkDocument);

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            if ((grfAttribs & HandledRDTAttributes) == 0)
                return VSErr.S_OK; // Not interested

            SccDocumentData data;

            if (TryGetDocument(docCookie, out data))
            {
                __VSRDTATTRIB attribs = (__VSRDTATTRIB)grfAttribs;

                bool wasInitialized = data.IsDocumentInitialized;
                data.OnAttributeChange(attribs, _poller);

                if (!wasInitialized
                    && GetDocumentFlags_cb != null
                    && (attribs & (RDTA_DocumentInitialized | RDTA_HierarchyInitialized)) != 0)
                {
                    uint newFlags = GetDocumentFlags_cb(data.Cookie);
                    data.SetFlags((_VSRDTFLAGS)newFlags);
                }
            }

            return VSErr.S_OK;
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            if ((grfAttribs & TrackedRDTAttributes) == 0)
                return VSErr.S_OK; // Not interested

            SccDocumentData data;
            if (!TryGetDocument(docCookie, true, out data))
                return VSErr.S_OK;

            __VSRDTATTRIB attribs = (__VSRDTATTRIB)grfAttribs;

            {
                bool wasInitialized = data.IsDocumentInitialized;

                data.OnAttributeChange(attribs, _poller);

                if (!wasInitialized
                    && GetDocumentFlags_cb != null
                    && (attribs & (RDTA_DocumentInitialized | RDTA_HierarchyInitialized)) != 0)
                {
                    uint newFlags = GetDocumentFlags_cb(data.Cookie);
                    data.SetFlags((_VSRDTFLAGS)newFlags);
                }
            }


            if ((attribs & __VSRDTATTRIB.RDTA_ItemID) == __VSRDTATTRIB.RDTA_ItemID)
            {
                data.ItemId = itemidNew;
            }

            if ((attribs & __VSRDTATTRIB.RDTA_Hierarchy) == __VSRDTATTRIB.RDTA_Hierarchy)
            {
                data.Hierarchy = pHierNew;
            }

            if ((attribs & __VSRDTATTRIB.RDTA_MkDocument) == __VSRDTATTRIB.RDTA_MkDocument
                && !string.IsNullOrEmpty(pszMkDocumentNew))
            {
                if (data.Name != pszMkDocumentNew)
                {
                    // The document changed names; Handle this as opening a new document

                    SccDocumentData newData;

                    if (!_docMap.TryGetValue(pszMkDocumentNew, out newData))
                    {
                        newData = new SccDocumentData(Context, pszMkDocumentNew);
                        newData.CopyState(data);
                        newData.Cookie = docCookie;
                        data.Dispose();

                        _docMap.Add(pszMkDocumentNew, newData);
                    }
                    else
                    {
                        data.Dispose(); // Removes old item from docmap and cookie map if necessary
                    }

                    _cookieMap[newData.Cookie] = newData;
                    data = newData;
                }

                if (!string.IsNullOrEmpty(pszMkDocumentOld) && pszMkDocumentNew != pszMkDocumentOld)
                {
                    if (SvnItem.IsValidPath(pszMkDocumentNew) && SvnItem.IsValidPath(pszMkDocumentOld))
                    {
                        string oldFile = SvnTools.GetNormalizedFullPath(pszMkDocumentOld);
                        string newFile = SvnTools.GetNormalizedFullPath(pszMkDocumentNew);
                        ProjectTracker.OnDocumentSaveAs(oldFile, newFile);
                    }
                }
            }

            return VSErr.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSErr.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSErr.S_OK;
        }

        bool TryPollDirty(SccDocumentData data, out bool dirty)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (!_documentInfo_init)
            {
                _documentInfo_init = true;
                DocumentInfoInit();
            }

            if (data.Cookie == 0 && GetDocumentCookie_cb != null)
            {
                try
                {
                    uint cookie = GetDocumentCookie_cb(data.Name);

                    if (cookie != 0)
                    {
                        data.Cookie = cookie;
                        _cookieMap[cookie] = data;
                    }
                }
                catch
                { }
            }
            if (IsDocumentDirty_cb != null && data.Cookie != 0)
            {
                try
                {
                    dirty = IsDocumentDirty_cb(data.Cookie);
                    return true;
                }
                catch
                { }
            }

            return TryPollDirtyFallback(data, out dirty);
        }

        bool TryPollDirtyFallback(SccDocumentData data, out bool dirty)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            bool done = false;

            int dv;
            IVsWindowFrame wf;
            object rawDoc = data.RawDocument;

            if (rawDoc != null)
            {
                IVsPersistDocData pdd;
                IPersistFileFormat pff;
                IVsPersistHierarchyItem phi;

                // Implemented by most editors
                if (null != (pdd = rawDoc as IVsPersistDocData))
                {
                    if (SafeSucceeded(pdd.IsDocDataDirty, out dv))
                    {
                        if (dv != 0)
                        {
                            dirty = true;
                            return true;
                        }

                        done = true;
                    }
                }

                // Implemented by the common project types (Microsoft Project Base)
                if (!done && null != (pff = rawDoc as IPersistFileFormat))
                {
                    if (SafeSucceeded(pff.IsDirty, out dv))
                    {
                        if (dv != 0)
                        {
                            dirty = true;
                            return true;
                        }

                        done = true;
                    }
                }

                // Project based documents will probably handle this
                if (!done && null != (phi = data.Hierarchy as IVsPersistHierarchyItem) && rawDoc != null)
                {
                    IntPtr docHandle = Marshal.GetIUnknownForObject(rawDoc);
                    try
                    {
                        if (VSErr.Succeeded(phi.IsItemDirty(data.ItemId, docHandle, out dv)))
                        {
                            if (dv != 0)
                            {
                                dirty = true;
                                return true;
                            }

                            done = true;
                        }
                    }
                    catch
                    {
                        // MPF throws a cast exception when docHandle doesn't implement IVsPersistDocData..
                        // which we tried before getting here*/
                    }
                    finally
                    {
                        Marshal.Release(docHandle);
                    }
                }
            }

            // Literally look if the frame window has a modified *
            if (!done && TryGetOpenDocumentFrame(data, out wf) && wf != null)
            {
                object ok;
                if (VSErr.Succeeded(wf.GetProperty((int)__VSFPROPID2.VSFPROPID_OverrideDirtyState, out ok)))
                {
                    if (ok == null)
                    { }
                    else if (ok is bool) // Implemented by VS as bool
                    {
                        if ((bool)ok)
                        {
                            dirty = true;
                            return true;
                        }
                    }
                }
            }

            dirty = false;
            return false;
        }

        private bool TryGetOpenDocumentFrame(SccDocumentData data, out IVsWindowFrame wf)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Hierarchy == null)
            {
                wf = null;
                return false;
            }
            Guid gV = Guid.Empty;
            IVsUIHierarchy hier;
            uint[] openId = new uint[1];

            int open;

            IVsUIShellOpenDocument so = GetService<IVsUIShellOpenDocument>(typeof(SVsUIShellOpenDocument));
            wf = null;

            if (so == null)
                return false;

            try
            {
                return VSErr.Succeeded(so.IsDocumentOpen(data.Hierarchy as IVsUIHierarchy, data.ItemId, data.Name, ref gV,
                    (uint)__VSIDOFLAGS.IDO_IgnoreLogicalView, out hier, openId, out wf, out open))
                    && (open != 0)
                    && (wf != null);
            }
            catch
            {
                return false;
            }
        }

        internal void DoDispose(SccDocumentData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Debug.Assert(_docMap[data.Name] == data);

            _docMap.Remove(data.Name);

            if (data.Cookie != 0)
            {
                Debug.Assert(_cookieMap[data.Cookie] == data);

                _cookieMap.Remove(data.Cookie);
                data.Cookie = 0;
            }
        }
    }
}
