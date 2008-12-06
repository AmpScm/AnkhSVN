using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Scc.ProjectMap;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Ankh.Scc
{
    [GlobalService(typeof(IAnkhOpenDocumentTracker))]
    partial class OpenDocumentTracker : AnkhService, IAnkhOpenDocumentTracker, IVsRunningDocTableEvents4, IVsRunningDocTableEvents3, IVsRunningDocTableEvents2, IVsRunningDocTableEvents
    {
        readonly Dictionary<string, SccDocumentData> _docMap = new Dictionary<string, SccDocumentData>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<uint, SccDocumentData> _cookieMap = new Dictionary<uint, SccDocumentData>();
        IVsRunningDocumentTable _docTable;
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

        protected IVsRunningDocumentTable RunningDocumentTable
        {
            get { return _docTable ?? (_docTable = GetService<IVsRunningDocumentTable>(typeof(SVsRunningDocumentTable))); }
        }

        void LoadInitial()
        {
            IVsRunningDocumentTable rdt = RunningDocumentTable;
            if (rdt == null)
                return;

            IEnumRunningDocuments docEnum;
            if(!ErrorHandler.Succeeded(rdt.GetRunningDocumentsEnum(out docEnum)))
                return;

            uint[] cookies = new uint[256];
            uint nFetched;
            while (ErrorHandler.Succeeded(docEnum.Next((uint)cookies.Length, cookies, out nFetched)))
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
                if (ErrorHandler.Succeeded(rdt.AdviseRunningDocTableEvents(this, out _cookie)))
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

            if (ErrorHandler.Succeeded(RunningDocumentTable.GetDocumentInfo(cookie,
                out flags, out locks, out editLocks, out name, out hier, out itemId, out ppunkDocData)))
            {
                object document = null;

                if (ppunkDocData != IntPtr.Zero)
                {
                    document = Marshal.GetObjectForIUnknown(ppunkDocData);
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
                            Debug.Assert(data.Hierarchy == hier, "Hierarchy not the same", string.Format("File={0}", data.Name));
                            Debug.Assert(data.ItemId == itemId, "Id not the same", string.Format("File={0}; from {1} into {2}", data.Name, data.ItemId, itemId));
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
        /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"></see>. If it fails, it returns an error code.
        /// </returns>
        public int OnBeforeFirstDocumentLock(IVsHierarchy pHier, uint itemid, string pszMkDocument)
        {
            if (string.IsNullOrEmpty(pszMkDocument))
            {
                return VSConstants.S_OK; // Can't be a valid path; don't monitor
            }

            SccDocumentData data;
            if (!_docMap.TryGetValue(pszMkDocument, out data))
            {
                _docMap.Add(pszMkDocument, data = new SccDocumentData(Context, pszMkDocument));

                data.Hierarchy = pHier;
                data.ItemId = itemid;
            }

            return VSConstants.S_OK;
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
                return VSConstants.S_OK;

            SccDocumentData data;
            if (_docMap.TryGetValue(pszMkDocument, out data))
            {
                data.OnClosed(fClosedWithoutSaving != 0);
                _docMap.Remove(data.Name);

                if (data.Cookie != 0)
                    _cookieMap.Remove(data.Cookie);
            }

            return VSConstants.S_OK;
        }

        public int OnBeforeSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            SccDocumentData data;

            if (TryGetDocument(docCookie, out data))
            {
                data.OnSaved();
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Fired after a Save All command is executed.
        /// </summary>
        /// <returns></returns>
        public int OnAfterSaveAll()
        {
            // Copy the list to allow changes while we are busy
            foreach(SccDocumentData dd in new List<SccDocumentData>(_docMap.Values))
            {
                if (dd.IsDirty && !dd.GetIsDirty(false))
                {
                    // We marked this document as dirty and it still says its not dirty; 
                    // so it probably was not dirty after all
                    dd.SetDirty(false);
                }
            }

            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            SccDocumentData data;

            if (TryGetDocument(docCookie, false, out data))
            {
                data.OnAttributeChange((__VSRDTATTRIB)grfAttribs);
            }

            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            SccDocumentData data;
            if (!TryGetDocument(docCookie, true, out data))
                return VSConstants.S_OK;

            data.OnAttributeChange((__VSRDTATTRIB)grfAttribs);

            if (!string.IsNullOrEmpty(pszMkDocumentNew) && pszMkDocumentNew != pszMkDocumentOld)
            {
                // The document changed names; for SCC this is a close without saving and setting dirty state on new document

                SccDocumentData newData;

                if (!_docMap.TryGetValue(pszMkDocumentNew, out newData))
                    _docMap.Add(pszMkDocumentNew, newData = new SccDocumentData(Context, pszMkDocumentNew));
                else if (newData.Cookie != 0)
                {
                    _cookieMap.Remove(newData.Cookie);
                    newData.Cookie = 0;
                }

                if (data != null)
                {
                    newData.CopyState(data);

                    newData.Cookie = data.Cookie;
                    _cookieMap[newData.Cookie] = newData;
                    data.Cookie = 0;

                    data.Dispose();
                }

                data = newData;
            }

            if (pHierNew != null)
                data.Hierarchy = pHierNew;
            if (itemidNew != VSConstants.VSITEMID_NIL)
                data.ItemId = itemidNew;            

            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        internal void DoDispose(SccDocumentData data)
        {
            if(data == null)
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
