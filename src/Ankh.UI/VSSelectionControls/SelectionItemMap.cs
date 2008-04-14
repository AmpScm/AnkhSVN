using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections;
using System.Diagnostics;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;

namespace Ankh.UI.VSSelectionControls
{
    interface ISelectionMapOwner<T>
    {
        event EventHandler SelectionChanged;
        IList Selection { get; }
        IList AllItems { get; }

        IntPtr GetImageList();
        int GetImageListIndex(T item);
        string GetText(T item);

        object GetSelectionObject(T item);
    }

    internal class SelectionItemMap<T> : IVsHierarchy, IVsMultiItemSelect, ISelectionContainer
        where T : class
    {
        readonly Dictionary<uint, IVsHierarchyEvents> _eventHandlers = new Dictionary<uint, IVsHierarchyEvents>();
        readonly Dictionary<T, uint> _items = new Dictionary<T, uint>();
        readonly Dictionary<uint, T> _ids = new Dictionary<uint, T>();
        readonly ISelectionMapOwner<T> _lv;
        uint lvId;

        public SelectionItemMap(ISelectionMapOwner<T> lv)
        {
            _lv = lv;
            lv.SelectionChanged += new EventHandler(OnSelectedIndexChanged);
        }

        void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_items.Count == 0)
                return;

            foreach (T i in new List<T>(_items.Keys))
            {
                if (_lv.Selection.Contains(i))
                {
                    uint id = (uint)_items[i];
                    _ids.Remove(id);
                    _items.Remove(i);

                    foreach (IVsHierarchyEvents h in _eventHandlers.Values)
                    {
                        h.OnItemDeleted(id);
                    }
                }
            }
        }

        public uint GetId(T item)
        {
            uint id;
            if (_items.TryGetValue(item, out id))
                return id;

            id = ++lvId;
            _ids.Add(id, item);
            _items.Add(item, id);

            if (_eventHandlers != null)
                foreach (IVsHierarchyEvents h in _eventHandlers.Values)
                {
                    h.OnItemAdded(VSConstants.VSITEMID_ROOT, VSConstants.VSITEMID_ROOT, id);
                }

            return id;
        }

        public T GetItem(uint id)
        {
            T value;
            if (_ids.TryGetValue(id, out value))
                return value;
            else
                return null;
        }

        int IVsHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
        {
            if (pEventSink == null)
            {
                pdwCookie = 0;
                return VSConstants.E_POINTER;
            }

            _eventHandlers.Add(pdwCookie = ++lvId, pEventSink);

            return VSConstants.S_OK;
        }

        int IVsHierarchy.UnadviseHierarchyEvents(uint dwCookie)
        {
            if (_eventHandlers != null)
                _eventHandlers.Remove(dwCookie);

            return VSConstants.S_OK;
        }

        int IVsHierarchy.Close()
        {
            return VSConstants.S_OK;
        }

        int IVsHierarchy.GetCanonicalName(uint itemid, out string pbstrName)
        {
            pbstrName = "{" + itemid + "}";
            return VSConstants.S_OK;
        }

        int IVsHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
        {
            pguid = Guid.Empty;
            return VSConstants.E_FAIL;
        }

        int IVsHierarchy.GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
        {
            ppHierarchyNested = IntPtr.Zero;
            pitemidNested = VSConstants.VSITEMID_NIL;
            return VSConstants.E_FAIL;
        }

        int IVsHierarchy.GetProperty(uint itemid, int propid, out object pvar)
        {
            T lv;
            if (!_ids.TryGetValue(itemid, out lv) && itemid != VSConstants.VSITEMID_ROOT)
            {
                pvar = null;
                return VSConstants.E_FAIL;
            }

            switch ((__VSHPROPID)propid)
            {
                case __VSHPROPID.VSHPROPID_Parent:
                case __VSHPROPID.VSHPROPID_FirstChild:
                case __VSHPROPID.VSHPROPID_NextSibling:
                case __VSHPROPID.VSHPROPID_NextVisibleSibling:

                    pvar = unchecked((int)VSConstants.VSITEMID_NIL);
                    break;
                case __VSHPROPID.VSHPROPID_Caption:
                case __VSHPROPID.VSHPROPID_Name:
                case __VSHPROPID.VSHPROPID_TypeName:
                    pvar = _lv.GetText(lv);
                    break;
                case __VSHPROPID.VSHPROPID_IconImgList:
                    pvar = (int)_lv.GetImageList();
                    break;
                case __VSHPROPID.VSHPROPID_IconIndex:
                    pvar = _lv.GetImageListIndex(lv);
                    break;
                case __VSHPROPID.VSHPROPID_Expandable:
                case __VSHPROPID.VSHPROPID_ExpandByDefault:
                    pvar = false;
                    break;
                case __VSHPROPID.VSHPROPID_StateIconIndex:
                    pvar = 0;
                    break;
                default:
                    pvar = null;
                    return VSConstants.E_FAIL;
            }

            return VSConstants.S_OK;
        }

        Microsoft.VisualStudio.OLE.Interop.IServiceProvider _serviceProvider;
        int IVsHierarchy.GetSite(out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP)
        {
            ppSP = _serviceProvider;
            return VSConstants.S_OK;
        }

        int IVsHierarchy.ParseCanonicalName(string pszName, out uint pitemid)
        {
            pitemid = uint.Parse(pszName.TrimStart('{').TrimEnd('}'));
            return VSConstants.S_OK;
        }

        int IVsHierarchy.QueryClose(out int pfCanClose)
        {
            pfCanClose = 0;
            return VSConstants.S_OK;
        }

        int IVsHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
        {
            return VSConstants.E_FAIL;
        }

        int IVsHierarchy.SetProperty(uint itemid, int propid, object var)
        {
            return VSConstants.E_FAIL;
        }

        int IVsHierarchy.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            _serviceProvider = psp;
            return VSConstants.S_OK;
        }

        int IVsHierarchy.Unused0()
        {
            return VSConstants.E_NOTIMPL;
        }

        int IVsHierarchy.Unused1()
        {
            return VSConstants.E_NOTIMPL;
        }

        int IVsHierarchy.Unused2()
        {
            return VSConstants.E_NOTIMPL;
        }

        int IVsHierarchy.Unused3()
        {
            return VSConstants.E_NOTIMPL;
        }

        int IVsHierarchy.Unused4()
        {
            return VSConstants.E_NOTIMPL;
        }

        [DebuggerHidden]
        public int GetSelectionInfo(out uint pcItems, out int pfSingleHierarchy)
        {
            pcItems = (uint)_lv.Selection.Count;

            pfSingleHierarchy = 1;  // If this line throws a nullreference exception, the bug is in the interop layer or the caller. 
            // Nothing we can do to fix it

            return VSConstants.S_OK;
        }

        public int GetSelectedItems(uint grfGSI, uint cItems, VSITEMSELECTION[] rgItemSel)
        {
            bool omitHiers = (grfGSI == (uint)__VSGSIFLAGS.GSI_fOmitHierPtrs);

            if (cItems != _lv.Selection.Count)
                return VSConstants.E_FAIL;

            for (int i = 0; i < cItems; i++)
            {
                rgItemSel[i].pHier = omitHiers ? null : this;

                if (i < _lv.Selection.Count)
                    rgItemSel[i].itemid = GetId((T)_lv.Selection[i]);
            }

            return VSConstants.S_OK;
        }

        #region ISelectionContainer Members

        public int CountObjects(uint dwFlags, out uint pc)
        {
            IList src;
            if (dwFlags == (uint)Constants.GETOBJS_ALL)
                src = _lv.AllItems;
            else if (dwFlags == (uint)Constants.GETOBJS_SELECTED)
                src = _lv.Selection;
            else
            {
                pc = 0;
                return VSConstants.E_FAIL;
            }

            pc = (uint)src.Count;

            return VSConstants.S_OK;
        }

        public int GetObjects(uint dwFlags, uint cObjects, object[] apUnkObjects)
        {
            if (apUnkObjects == null)
                return VSConstants.E_POINTER;

            IList src;
            if (dwFlags == (uint)Constants.GETOBJS_ALL)
                src = _lv.AllItems;
            else if (dwFlags == (uint)Constants.GETOBJS_SELECTED)
                src = _lv.Selection;
            else
                return VSConstants.E_FAIL;

            if(src == null || cObjects > src.Count)
                return VSConstants.E_FAIL;

            for (int i = 0; i < cObjects; i++)
            {
                apUnkObjects[i] = _lv.GetSelectionObject((T)src[i]);
            }

            return VSConstants.S_OK;
        }

        public int SelectObjects(uint cSelect, object[] apUnkSelect, uint dwFlags)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        public void NotifySelectionUpdated(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            IVsTrackSelectionEx sel = (IVsTrackSelectionEx)serviceProvider.GetService(typeof(SVsTrackSelectionEx));

            int selectedCount = _lv.Selection.Count;

            if (sel != null && selectedCount > 0)
            {
                IntPtr hier = Marshal.GetComInterfaceForObject(this, typeof(IVsHierarchy));
                IntPtr handle = Marshal.GetComInterfaceForObject(this, typeof(ISelectionContainer));
                try
                {
                    uint id;


                    IVsMultiItemSelect ms = null;

                    if (selectedCount == 1)
                        id = GetId((T)_lv.Selection[0]);
                    else
                    {
                        id = VSConstants.VSITEMID_SELECTION; // Look at selection instead of this item
                        ms = this; // We implement IVsMultiItemSelect
                    }

                    sel.OnSelectChangeEx(hier, id, ms, handle);
                }
                finally
                {
                    Marshal.Release(handle);
                    Marshal.Release(hier);
                }
            }
            else if (sel != null)
                sel.OnSelectChangeEx(IntPtr.Zero, VSConstants.VSITEMID_NIL, null, IntPtr.Zero);

        }
    }
}
