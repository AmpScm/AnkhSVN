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
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Ankh.Selection
{
    [ComVisible(true), ComDefaultInterfaceAttribute(typeof(IVsMultiItemSelect))]
    public sealed class SelectionItemMap : IVsMultiItemSelect, ISelectionContainer
    {
        readonly SelectionMapHierarchy _hierarchy;
        bool _noSyncHierarchy;

        abstract class MapData
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
            internal abstract void CleanSelection();

            public abstract IList Selection { get; }
            public abstract IList AllItems { get; }

            public event EventHandler HandleDestroyed;

            protected void OnHandleDestroyed(EventArgs e)
            {
                if (HandleDestroyed != null)
                    HandleDestroyed(this, e);
            }

            internal abstract string GetCanonicalName(uint itemid);

            internal abstract Control Control { get; }
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
                if (owner == null)
                    throw new ArgumentNullException("owner");

                _owner = owner;
                _owner.HandleDestroyed += new EventHandler(OwnerHandleDestroyed);
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
                            h.OnItemAdded(VSItemId.Root, VSItemId.Root, id);
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

            void OwnerHandleDestroyed(object sender, EventArgs e)
            {
                OnHandleDestroyed(e);
            }

            internal override void CleanSelection()
            {
                foreach (T i in new List<T>(_items.Keys))
                {
                    if (!_owner.SelectionContains(i))
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
                    return VSErr.E_POINTER;
                }

                _eventHandlers.Add(pdwCookie = ++lvId, pEventSink);

                return VSErr.S_OK;
            }

            internal override int UnadviseHierarchyEvents(uint dwCookie)
            {
                if (_eventHandlers != null)
                    _eventHandlers.Remove(dwCookie);

                return VSErr.S_OK;
            }

            internal override string GetCanonicalName(uint itemid)
            {
                T lv;
                if (!_ids.TryGetValue(itemid, out lv) && itemid != VSItemId.Root)
                {
                    return null;
                }

                if (lv != null)
                    return _owner.GetCanonicalName(lv);
                else
                    return null;
            }

            internal override int GetProperty(uint itemid, int propid, out object pvar)
            {
                T lv;
                if (!_ids.TryGetValue(itemid, out lv) && itemid != VSItemId.Root)
                {
                    pvar = null;
                    return VSErr.E_FAIL;
                }

                switch ((__VSHPROPID)propid)
                {
                    case __VSHPROPID.VSHPROPID_Parent:
                    case __VSHPROPID.VSHPROPID_FirstChild:
                    case __VSHPROPID.VSHPROPID_NextSibling:
                    case __VSHPROPID.VSHPROPID_NextVisibleSibling:
                        pvar = VSItemId.Nil;
                        break;
                    case __VSHPROPID.VSHPROPID_Root:
                        pvar = VSItemId.Root;
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
                        if (lv != null)
                            pvar = _owner.GetImageListIndex(lv);
                        else
                            pvar = -1;
                        break;
                    case __VSHPROPID.VSHPROPID_Expandable:
                    case __VSHPROPID.VSHPROPID_Expanded:
                    case __VSHPROPID.VSHPROPID_ExpandByDefault:
                    case __VSHPROPID.VSHPROPID_HasEnumerationSideEffects:
                    case (__VSHPROPID)__VSHPROPID2.VSHPROPID_Container:
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
                        pvar = VSItemId.Nil;
                        break;
                    default:
                        pvar = null;
                        return VSErr.E_FAIL;
                }

                return VSErr.S_OK;
            }

            public override IList Selection
            {
                get { return _owner.Selection; }
            }

            public override IList AllItems
            {
                get { return _owner.AllItems; }
            }

            internal override Control Control
            {
                get { return _owner.Control ?? (_owner as Control); }
            }
        }

        readonly MapData _data;

        SelectionItemMap(MapData mapData)
        {
            _data = mapData;
            _data.HandleDestroyed += new EventHandler(OnDataHandleDestroyed);
            _hierarchy = new SelectionMapHierarchy(this, _data);
        }

        public static SelectionItemMap Create<T>(ISelectionMapOwner<T> owner)
            where T : class
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            return new SelectionItemMap(CreateData<T>(owner));
        }

        static MapData CreateData<T>(ISelectionMapOwner<T> owner)
            where T : class
        {
            return new MapData<T>(owner);
        }
        
        [DebuggerHidden, DebuggerNonUserCode]
        int IVsMultiItemSelect.GetSelectionInfo(out uint pcItems, out int pfSingleHierarchy)
        {
            pcItems = (uint)_data.Selection.Count;

            pfSingleHierarchy = 1;  // If this line throws a nullreference exception, the bug is in the interop layer or the caller. 
            // Nothing we can do to fix it

            return VSErr.S_OK;
        }

        int IVsMultiItemSelect.GetSelectedItems(uint grfGSI, uint cItems, VSITEMSELECTION[] rgItemSel)
        {
            bool omitHiers = (grfGSI & (uint)__VSGSIFLAGS.GSI_fOmitHierPtrs) != 0;

            IList selection = _data.Selection;

            if (cItems > selection.Count || cItems > rgItemSel.Length)
                return VSErr.E_FAIL;
            
            for (int i = 0; i < cItems; i++)
            {
                rgItemSel[i].pHier = omitHiers ? null : _hierarchy;
                rgItemSel[i].itemid = _data.GetId(selection[i]);
            }

            return VSErr.S_OK;
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
                return VSErr.E_FAIL;
            }

            pc = (uint)src.Count;

            return VSErr.S_OK;
        }

        object[] _sel;
        Hashtable _ht;

        bool _allIsSelected;
        public bool AllIsSelected
        {
            get { return _allIsSelected; }
            set { _allIsSelected = value; }
        }

        int ISelectionContainer.GetObjects(uint dwFlags, uint cObjects, object[] apUnkObjects)
        {
            if (apUnkObjects == null)
                return VSErr.E_POINTER;

            IList src;
            if (dwFlags == (uint)Constants.GETOBJS_ALL)
                src = AllIsSelected ? _data.Selection : _data.AllItems;
            else if (dwFlags == (uint)Constants.GETOBJS_SELECTED)
                src = _data.Selection;
            else
                return VSErr.E_FAIL;

            if (src == null || cObjects > src.Count)
                return VSErr.E_FAIL;

            if (_sel == null && src == _data.Selection)
            {
                if (_ht == null)
                    _ht = new Hashtable();

                // Create cache of wrapper objects
                object[] from = new object[src.Count];
                _sel = new object[src.Count];

                src.CopyTo(from, 0);

                for (int i = 0; i < from.Length; i++)
                {
                    object s = from[i];

                    _sel[i] = _ht[s] ?? _data.GetSelectionObject(s);
                }
                _ht.Clear();

                for (int i = 0; i < from.Length; i++)
                    _ht[from[i]] = _sel[i];
            }

            if (_sel != null && src == _data.Selection)
            {
                Array.Copy(_sel, apUnkObjects, cObjects);

                return VSErr.S_OK;
            }

            for (int i = 0; i < cObjects; i++)
            {
                apUnkObjects[i] = _data.GetSelectionObject(src[i]);
            }

            return VSErr.S_OK;
        }

        int ISelectionContainer.SelectObjects(uint cSelect, object[] apUnkSelect, uint dwFlags)
        {
            object[] items = _data.CreateArray((int)cSelect);

            if (cSelect > 0 && apUnkSelect == null)
                return VSErr.E_POINTER;

            for (int i = 0; i < cSelect; i++)
                items[i] = _data.GetRealObject(apUnkSelect[i]);

            _data.SetSelection(items);

            return VSErr.S_OK; // E_NOTIMPL kills VS from the property explorer
        }

        #endregion

        IntPtr _hierHandle;
        IntPtr _selHandle;
        void OnDataHandleDestroyed(object sender, EventArgs e)
        {
            Release();
        }

        void Release()
        {
            _ht = null;
            _sel = null;
            if (_hierHandle != IntPtr.Zero)
            {
                IntPtr h = _hierHandle;
                _hierHandle = IntPtr.Zero;
                Marshal.Release(h);
            }

            if (_selHandle != IntPtr.Zero)
            {
                IntPtr h = _selHandle;
                _selHandle = IntPtr.Zero;
                Marshal.Release(h);
            }
        }

        IAnkhServiceProvider _context;
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
                _tracker = null;
            }
        }

        public bool PublishHierarchy
        {
            get { return !_noSyncHierarchy; }
            set { _noSyncHierarchy = !value; }
        }

        IVsTrackSelectionEx _tracker;

        IVsTrackSelectionEx Tracker
        {
            get
            {
                if (_tracker == null && Context != null)
                {
                    _tracker = Context.GetService<IVsTrackSelectionEx>(typeof(SVsTrackSelectionEx));

                    if (_tracker == null)
                    {
                        ISelectionContextEx ex = Context.GetService<ISelectionContextEx>(typeof(ISelectionContext));

                        _tracker = ex.GetModalTracker(_data.Control);
                    }
                }

                return _tracker;
            }
        }

        public void NotifySelectionUpdated()
        {
            if (Tracker == null)
                return;

            if (!PublishHierarchy)
            {
                Tracker.OnSelectChange(this);
                return;
            }

            if (_selHandle == IntPtr.Zero)
                _selHandle = Marshal.GetComInterfaceForObject(this, typeof(ISelectionContainer));

            if (_hierHandle == IntPtr.Zero)
                _hierHandle = Marshal.GetComInterfaceForObject(_hierarchy, typeof(IVsHierarchy));

            _sel = null; // Clear wrapper cache
            _data.CleanSelection();

            int selectedCount = _data.Selection.Count;

            if (selectedCount > 0)
            {
                uint id;

                IVsMultiItemSelect ms = null;

                if (selectedCount == 1)
                    id = _data.GetId(_data.Selection[0]);
                else
                {
                    id = VSItemId.Selection; // Look at selection instead of this item
                    ms = this; // We implement IVsMultiItemSelect
                }

                try
                {
                    Tracker.OnSelectChangeEx(_hierHandle, id, ms, _selHandle);
                }
                catch { } // Ignore listener exceptions :(
            }
            else
                Tracker.OnSelectChangeEx(_hierHandle, VSItemId.Nil, null, _selHandle);
        }

        public void EnsureSelection()
        {
            if (Tracker == null)
                return;

            IntPtr ppHier;
            uint itemid;
            IVsMultiItemSelect ms;
            IntPtr ppSc;
            if (!VSErr.Succeeded(Tracker.GetCurrentSelection(out ppHier, out itemid, out ms, out ppSc)))
                return;

            if (ppHier != IntPtr.Zero)
                Marshal.Release(ppHier);

            if (ppSc != IntPtr.Zero)
            {
                object me = Marshal.GetObjectForIUnknown(ppSc);
                Marshal.Release(ppSc);

                if (me == this)
                    return;
            }

            NotifySelectionUpdated();
        }

        object GetItem(uint id)
        {
            return _data.GetItemObject(id);
        }

        [ComVisible(true), ComDefaultInterface(typeof(IVsHierarchy)), ClassInterface(ClassInterfaceType.AutoDual)]
        sealed class SelectionMapHierarchy : IVsHierarchy
        {
            readonly SelectionItemMap _map;
            readonly MapData _data;

            public SelectionMapHierarchy(SelectionItemMap map, MapData data)
            {
                if (map == null)
                    throw new ArgumentNullException("map");
                else if (data == null)
                    throw new ArgumentNullException("data");

                _map = map;
                _data = data;
            }

            int IVsHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
            {
                return _data.AdviseHierarchyEvents(pEventSink, out pdwCookie);
            }

            int IVsHierarchy.UnadviseHierarchyEvents(uint dwCookie)
            {
                return _data.UnadviseHierarchyEvents(dwCookie);
            }

            public int Close()
            {
                return VSErr.S_OK;
            }

            public int GetCanonicalName(uint itemid, out string pbstrName)
            {
                string name = _data.GetCanonicalName(itemid);

                if (name == null)
                    pbstrName = "{" + itemid + "}";
                else
                    pbstrName = name;

                return VSErr.S_OK;
            }

            int IVsHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
            {
                object v;

                if (GetProperty(itemid, propid, out v) == 0 && v is Guid)
                {
                    pguid = (Guid)v;
                    return VSErr.S_OK;
                }

                pguid = Guid.Empty;
                return VSErr.E_FAIL;
            }

            int IVsHierarchy.GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
            {
                ppHierarchyNested = IntPtr.Zero;
                pitemidNested = VSItemId.Nil;
                return VSErr.E_FAIL;
            }

            public int GetProperty(uint itemid, int propid, out object pvar)
            {
                return _data.GetProperty(itemid, propid, out pvar);
            }

            int IVsHierarchy.GetSite(out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP)
            {
                ppSP = null;
                return VSErr.S_OK;
            }

            public int ParseCanonicalName(string pszName, out uint pitemid)
            {
                pitemid = uint.Parse(pszName.TrimStart('{').TrimEnd('}'));
                return VSErr.S_OK;
            }

            public int QueryClose(out int pfCanClose)
            {
                pfCanClose = 1;
                return VSErr.S_OK;
            }

            int IVsHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
            {
                return VSErr.E_FAIL;
            }

            int IVsHierarchy.SetProperty(uint itemid, int propid, object var)
            {
                return VSErr.E_FAIL;
            }

            int IVsHierarchy.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
            {
                return VSErr.E_FAIL; // Should never be called
            }

            int IVsHierarchy.Unused0()
            {
                return VSErr.E_NOTIMPL;
            }

            int IVsHierarchy.Unused1()
            {
                return VSErr.E_NOTIMPL;
            }

            int IVsHierarchy.Unused2()
            {
                return VSErr.E_NOTIMPL;
            }

            int IVsHierarchy.Unused3()
            {
                return VSErr.E_NOTIMPL;
            }

            int IVsHierarchy.Unused4()
            {
                return VSErr.E_NOTIMPL;
            }

        }
    }
    
}
