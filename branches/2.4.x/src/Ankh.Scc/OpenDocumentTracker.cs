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
        bool _hooked;
        uint _cookie;

        public OpenDocumentTracker(IAnkhServiceProvider context)
            : base(context)
        {
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
                        data.OnCookieLoad();
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

        bool TryGetDocument(uint cookie, bool forUpdate, out SccDocumentData data)
        {
            if (cookie == 0)
            {
                data = null;
                return false;
            }

            if (_cookieMap.TryGetValue(cookie, out data))
                return true;

            uint flags;
            uint locks;
            uint editLocks;
            string name;
            IVsHierarchy hier;
            uint itemId;
            IntPtr ppunkDocData;

            if (VSErr.Succeeded(RunningDocumentTable.GetDocumentInfo(cookie,
                out flags, out locks, out editLocks, out name, out hier, out itemId, out ppunkDocData)))
            {
                object document = null;

                if (ppunkDocData != IntPtr.Zero)
                {
                    document = Marshal.GetUniqueObjectForIUnknown(ppunkDocData);
                    Marshal.Release(ppunkDocData);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    if (_docMap.TryGetValue(name, out data))
                    {
                        if (data.Cookie != 0)
                        {
                            _cookieMap.Remove(data.Cookie);
                            data.Cookie = 0;
                        }

                        if (!forUpdate)
                        {
                            Debug.Assert(data.Hierarchy == hier, "Hierarchy not the same", string.Format("File={0}", data.FullPath));
                            Debug.Assert(data.ItemId == itemId, "Id not the same", string.Format("File={0}; from {1} into {2}", data.FullPath, data.ItemId, itemId));
                        }
                    }
                    else
                    {
                        _docMap.Add(name, data = new SccDocumentData(Context, name));
                        data.Hierarchy = hier;
                        data.ItemId = itemId;
                    }

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
            }
            return VSErr.S_OK;
        }

        /// <summary>
        /// Fired after a Save All command is executed.
        /// </summary>
        /// <returns></returns>
        public int OnAfterSaveAll()
        {
            // Copy the list to allow changes while we are busy
            foreach (SccDocumentData dd in new List<SccDocumentData>(_docMap.Values))
            {
                if (dd.IsDirty && !dd.GetIsDirty(false))
                {
                    // We marked this document as dirty and it still says its not dirty; 
                    // so it probably was not dirty after all
                    dd.SetDirty(false);
                }
            }

            return VSErr.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            SccDocumentData data;

            if (TryGetDocument(docCookie, false, out data))
            {
                data.OnAttributeChange((__VSRDTATTRIB)grfAttribs);
            }

            return VSErr.S_OK;
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            SccDocumentData data;
            if (!TryGetDocument(docCookie, true, out data))
                return VSErr.S_OK;

            __VSRDTATTRIB attribs = (__VSRDTATTRIB)grfAttribs;

            data.OnAttributeChange(attribs);

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
