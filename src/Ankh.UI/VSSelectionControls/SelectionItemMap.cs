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
    public interface ISelectionMapOwner<T>
    {
        event EventHandler SelectionChanged;
        IList Selection { get; }
        IList AllItems { get; }

        IntPtr GetImageList();
        int GetImageListIndex(T item);
        string GetText(T item);

        object GetSelectionObject(T item);

        T GetItemFromSelectionObject(object item);
        void SetSelection(T[] items);        
    }

    public class SelectionItemMap : IVsHierarchy, IVsMultiItemSelect, ISelectionContainer
    {        
        protected abstract class MapData
        {
            internal MapData()
            {
            }

            internal abstract object GetSelectionObject(object item);
            internal abstract object GetRealObject(object selectionObject);

            internal abstract void SetSelection(object[] selection);

            internal abstract int AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie);
            internal abstract int UnadviseHierarchyEvents(uint dwCookie);
            internal abstract int GetProperty(uint itemid, int propid, out object pvar);
            internal abstract uint GetId(object item);
            internal abstract object GetItemObject(uint id);

            internal abstract object[] CreateArray(int count);

            public abstract IList Selection { get; }
            public abstract IList AllItems { get; }
        }

        sealed class MapData<T> : MapData
            where T : class
        {
            readonly ISelectionMapOwner<T> _owner;
            readonly Dictionary<uint, IVsHierarchyEvents> _eventHandlers = new Dictionary<uint, IVsHierarchyEvents>();
            readonly Dictionary<T, uint> _items = new Dictionary<T, uint>();
            readonly Dictionary<uint, T> _ids = new Dictionary<uint, T>();
            uint lvId;

            public MapData(ISelectionMapOwner<T> owner)
            {
                _owner = owner;
                _owner.SelectionChanged += new EventHandler(OwnerSelectionChanged);
            }

            public uint GetId(T item)
            {
				if (item == null)
					throw new ArgumentNullException("item");

                uint id;
                if (_items.TryGetValue(item, out id))
                    return id;

                id = ++lvId;
                _ids.Add(id, item);
                _items.Add(item, id);

                if (_eventHandlers != null)
                    foreach (IVsHierarchyEvents h in _eventHandlers.Values)
                    {
                        try
                        {
                            h.OnItemAdded(VSConstants.VSITEMID_ROOT, VSConstants.VSITEMID_ROOT, id);
                        }
                        catch { }
                    }

                return id;
            }

            internal override uint GetId(object item)
            {
                return GetId((T)item);
            }

            public T GetItem(uint id)
            {
                T value;
                if (_ids.TryGetValue(id, out value))
                    return value;
                else
                    return null;
            }

            internal override object GetItemObject(uint id)
            {
                return GetItem(id);
            }

            void OwnerSelectionChanged(object sender, EventArgs e)
            {
                if (_items.Count == 0)
                    return;

                foreach (T i in new List<T>(_items.Keys))
                {
                    if (!_owner.Selection.Contains(i))
                    {
                        uint id = (uint)_items[i];
                        _ids.Remove(id);
                        _items.Remove(i);

                        foreach (IVsHierarchyEvents h in _eventHandlers.Values)
                        {
                            try
                            {
                                h.OnItemDeleted(id);
                            }
                            catch { }
                        }
                    }
                }
            }

            internal override object GetSelectionObject(object item)
            {
                return _owner.GetSelectionObject((T)item);
            }

            internal override object GetRealObject(object selectionObject)
            {
                return _owner.GetItemFromSelectionObject(selectionObject);
            }

            internal override object[] CreateArray(int count)
            {
                return new T[count];
            }

            internal override void SetSelection(object[] selection)
            {
                _owner.SetSelection((T[])selection);
            }

            internal override int AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
            {
                if (pEventSink == null)
                {
                    pdwCookie = 0;
                    return VSConstants.E_POINTER;
                }

                _eventHandlers.Add(pdwCookie = ++lvId, pEventSink);

                return VSConstants.S_OK;
            }

            internal override int UnadviseHierarchyEvents(uint dwCookie)
            {
                if (_eventHandlers != null)
                    _eventHandlers.Remove(dwCookie);

                return VSConstants.S_OK;
            }

            internal override int GetProperty(uint itemid, int propid, out object pvar)
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
                        pvar = VSConstants.VSITEMID_NIL;
                        break;
                    case __VSHPROPID.VSHPROPID_Root:
                        pvar = VSConstants.VSITEMID_ROOT;
                        break;
                    case __VSHPROPID.VSHPROPID_TypeGuid:                    
                        pvar = typeof(SelectionItemMap).GUID;
                        break;
                    case __VSHPROPID.VSHPROPID_CmdUIGuid:
                        pvar = Guid.Empty;
                        break;
                    case __VSHPROPID.VSHPROPID_Caption:
                    case __VSHPROPID.VSHPROPID_Name:
                    case __VSHPROPID.VSHPROPID_TypeName:
						if (lv != null)
							pvar = _owner.GetText(lv);
						else
							pvar = ".";
                        break;
                    case __VSHPROPID.VSHPROPID_IconImgList:
                        pvar = (int)_owner.GetImageList();
                        break;
                    case __VSHPROPID.VSHPROPID_IconIndex:
						if(lv != null)
							pvar = _owner.GetImageListIndex(lv);
						else
							pvar = -1;
                        break;
                    case __VSHPROPID.VSHPROPID_Expandable:
                    case __VSHPROPID.VSHPROPID_Expanded:
                    case __VSHPROPID.VSHPROPID_ExpandByDefault:
                        pvar = false;
                        break;
                    case __VSHPROPID.VSHPROPID_StateIconIndex:
                        pvar = (int)VsStateIcon.STATEICON_NOSTATEICON;
                        break;
                    case __VSHPROPID.VSHPROPID_ParentHierarchy:
                    case (__VSHPROPID)__VSHPROPID2.VSHPROPID_StatusBarClientText:
                        pvar = null;
                        break;
                    case __VSHPROPID.VSHPROPID_ParentHierarchyItemid:
                        pvar = VSConstants.VSITEMID_NIL;
                        break;
                    default:
                        pvar = null;
                        return VSConstants.E_FAIL;
                }

                return VSConstants.S_OK;
            }

            public override IList Selection
            {
                get { return _owner.Selection; }
            }

            public override IList AllItems
            {
                get { return _owner.AllItems; }
            }
        }

        readonly MapData _data;

        protected SelectionItemMap(MapData mapData)
        {
            _data = mapData;
        }

        public static SelectionItemMap Create<T>(ISelectionMapOwner<T> owner)
            where T : class
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            return new SelectionItemMap(CreateData<T>(owner));
        }

        protected static MapData CreateData<T>(ISelectionMapOwner<T> owner)
            where T : class
        {
            return new MapData<T>(owner);
        }        

        int IVsHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
        {
            return _data.AdviseHierarchyEvents(pEventSink, out pdwCookie);
        }

        int IVsHierarchy.UnadviseHierarchyEvents(uint dwCookie)
        {
            return _data.UnadviseHierarchyEvents(dwCookie);
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
            return _data.GetProperty(itemid, propid, out pvar);
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
        int IVsMultiItemSelect.GetSelectionInfo(out uint pcItems, out int pfSingleHierarchy)
        {
            pcItems = (uint)_data.Selection.Count;

            pfSingleHierarchy = 1;  // If this line throws a nullreference exception, the bug is in the interop layer or the caller. 
            // Nothing we can do to fix it

            return VSConstants.S_OK;
        }

        int IVsMultiItemSelect.GetSelectedItems(uint grfGSI, uint cItems, VSITEMSELECTION[] rgItemSel)
        {
            bool omitHiers = (grfGSI == (uint)__VSGSIFLAGS.GSI_fOmitHierPtrs);

            if (cItems > _data.Selection.Count)
                return VSConstants.E_FAIL;

            for (int i = 0; i < cItems; i++)
            {
                rgItemSel[i].pHier = omitHiers ? null : this;

                if (i < _data.Selection.Count)
                    rgItemSel[i].itemid = _data.GetId(_data.Selection[i]);
            }

            return VSConstants.S_OK;
        }

        #region ISelectionContainer Members

        int ISelectionContainer.CountObjects(uint dwFlags, out uint pc)
        {
            IList src;
            if (dwFlags == (uint)Constants.GETOBJS_ALL)
                src = _data.AllItems;
            else if (dwFlags == (uint)Constants.GETOBJS_SELECTED)
                src = _data.Selection;
            else
            {
                pc = 0;
                return VSConstants.E_FAIL;
            }

            pc = (uint)src.Count;

            return VSConstants.S_OK;
        }

        int ISelectionContainer.GetObjects(uint dwFlags, uint cObjects, object[] apUnkObjects)
        {
            if (apUnkObjects == null)
                return VSConstants.E_POINTER;

            IList src;
            if (dwFlags == (uint)Constants.GETOBJS_ALL)
                src = _data.AllItems;
            else if (dwFlags == (uint)Constants.GETOBJS_SELECTED)
                src = _data.Selection;
            else
                return VSConstants.E_FAIL;

            if(src == null || cObjects > src.Count)
                return VSConstants.E_FAIL;

            for (int i = 0; i < cObjects; i++)
            {
                apUnkObjects[i] = _data.GetSelectionObject(src[i]);
            }

            return VSConstants.S_OK;
        }

        int ISelectionContainer.SelectObjects(uint cSelect, object[] apUnkSelect, uint dwFlags)
        {
            object[] items = _data.CreateArray((int)cSelect);

            if(cSelect > 0 && apUnkSelect == null)
                return VSConstants.E_POINTER;

            for(int i = 0; i < cSelect; i++)
                items[i] = _data.GetRealObject(apUnkSelect[i]);

            _data.SetSelection(items);
                
            return VSConstants.S_OK; // E_NOTIMPL kills VS from the property explorer
        }

        #endregion

        public void NotifySelectionUpdated(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            IVsTrackSelectionEx sel = (IVsTrackSelectionEx)serviceProvider.GetService(typeof(SVsTrackSelectionEx));

            int selectedCount = _data.Selection.Count;

            if (sel != null && selectedCount > 0)
            {
                IntPtr hier = Marshal.GetComInterfaceForObject(this, typeof(IVsHierarchy));
                IntPtr handle = Marshal.GetComInterfaceForObject(this, typeof(ISelectionContainer));
                try
                {
                    uint id;


                    IVsMultiItemSelect ms = null;

                    if (selectedCount == 1)
                        id = _data.GetId(_data.Selection[0]);
                    else
                    {
                        id = VSConstants.VSITEMID_SELECTION; // Look at selection instead of this item
                        ms = this; // We implement IVsMultiItemSelect
                    }

                    try
                    {
                        sel.OnSelectChangeEx(hier, id, ms, handle);
                    }
                    catch { } // Ignore listener exceptions :(
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

        [CLSCompliant(false)]
        protected object GetItem(uint id)
        {
            return _data.GetItemObject(id);
        }
    }
}
