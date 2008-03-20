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
    partial class OpenDocumentTracker : IAnkhOpenDocumentTracker, IVsRunningDocTableEvents4, IVsRunningDocTableEvents3, IVsRunningDocTableEvents2, IVsRunningDocTableEvents
    {
        readonly AnkhContext _context;
        readonly Dictionary<string, SccDocumentData> _docMap = new Dictionary<string, SccDocumentData>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<uint, SccDocumentData> _cookieMap = new Dictionary<uint, SccDocumentData>();
        readonly AnkhSccProvider _sccProvider;
        IVsRunningDocumentTable _docTable;
        bool _hooked;
        uint _cookie;

        public OpenDocumentTracker(AnkhContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _sccProvider = context.GetService<AnkhSccProvider>();
            Hook(true);
        }

        protected IVsRunningDocumentTable RunningDocumentTable
        {
            get { return _docTable ?? (_docTable = (IVsRunningDocumentTable)_context.GetService(typeof(SVsRunningDocumentTable))); }
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
                if (ppunkDocData != IntPtr.Zero)
                    Marshal.Release(ppunkDocData);

                if (!string.IsNullOrEmpty(name))
                {
                    if (_docMap.TryGetValue(name, out data))
                    {
                        if (data.Cookie != 0)
                        {
                            _cookieMap.Remove(data.Cookie);
                            data.Cookie = 0;
                        }

                        Debug.Assert(data.Hierarchy == hier);
                        Debug.Assert(data.ItemId == itemId);
                    }
                    else
                    {
                        _docMap.Add(name, data = new SccDocumentData(_context, name));
                        data.Hierarchy = hier;
                        data.ItemId = itemId;
                    }

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
                _docMap.Add(pszMkDocument, data = new SccDocumentData(_context, pszMkDocument));

                data.Hierarchy = pHier;
                data.ItemId = itemid;
            }

            if (itemid != VSConstants.VSITEMID_NIL && itemid != VSConstants.VSITEMID_SELECTION)
            {
                IVsSccProject2 sccProject = pHier as IVsSccProject2;
                if (sccProject != null)
                {
                    _sccProvider.OnFileOpen(sccProject, pszMkDocument, itemid);
                }
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

            IVsSccProject2 sccProject = pHier as IVsSccProject2;
            if (sccProject != null)
            {
                _sccProvider.OnFileClose(sccProject, pszMkDocument, itemid);
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
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            SccDocumentData data;

            if (TryGetDocument(docCookie, out data))
            {
                data.OnAttributeChange((__VSRDTATTRIB)grfAttribs);
            }

            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            SccDocumentData data;
            if (!TryGetDocument(docCookie, out data))
                return VSConstants.S_OK;

            data.OnAttributeChange((__VSRDTATTRIB)grfAttribs);

            if (!string.IsNullOrEmpty(pszMkDocumentNew) && pszMkDocumentNew != pszMkDocumentOld)
            {
                // The document changed names; for SCC this is a close without saving and setting dirty state on new document

                SccDocumentData newData;

                if (!_docMap.TryGetValue(pszMkDocumentNew, out newData))
                    _docMap.Add(pszMkDocumentNew, newData = new SccDocumentData(_context, pszMkDocumentNew));
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
            if (itemidNew != 0)
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
