using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Ankh.UI.PendingChanges
{
    /// <summary>
    /// Generic listview with <see cref="ISelectionContainer"/> support
    /// </summary>
    public class ListViewWithSelection<TListViewItem> : ListView, ISelectionContainer
        where TListViewItem : ListViewItem
    {
        bool _provideFullList;

        IServiceProvider _serviceProvider;
        [Browsable(false)]
        public IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
            set { _serviceProvider = value; }
        }

        [Browsable(false)]
        public bool ProvideWholeListForSelection
        {
            get { return _provideFullList; }
            set { _provideFullList = value; }
        }

        bool _updatingSelection = false;
        bool _maybeUnselect = true;
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ListView.SelectedIndexChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (!_updatingSelection)
            {
                if (SelectedItems.Count > 0)
                    NotifySelectionUpdated();
                else
                    _maybeUnselect = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_maybeUnselect && SelectedItems.Count == 0)
            {
                NotifySelectionUpdated();
            }
        }

        internal virtual ListViewHierarchy CreateHierarchy()
        {
            return new ListViewHierarchy(this);
        }

        ListViewHierarchy _hier;
        internal ListViewHierarchy Hierarchy
        {
            get { return _hier ?? (_hier = CreateHierarchy()); }
        }
        /// <summary>
        /// Notifies to external listeners selection updated.
        /// </summary>
        public void NotifySelectionUpdated()
        {
            _maybeUnselect = true;
            if (ServiceProvider != null)
            {
                IVsTrackSelectionEx sel = (IVsTrackSelectionEx)ServiceProvider.GetService(typeof(SVsTrackSelectionEx));

                if (sel != null)
                {
                    IntPtr hier = Marshal.GetComInterfaceForObject(Hierarchy, typeof(IVsHierarchy));
                    IntPtr handle = Marshal.GetComInterfaceForObject(this, typeof(ISelectionContainer));
                    try
                    {
                        uint id;

                        int selectedCount = SelectedItems.Count;
                        IVsMultiItemSelect ms = null;

                        if(selectedCount == 1)
                            id = Hierarchy.GetId((TListViewItem)SelectedItems[0]);
                        else if(selectedCount > 1)
                        {
                            id = VSConstants.VSITEMID_SELECTION;
                            ms = Hierarchy; // Our hierarchy implements IVsMultiItemSelect
                        }
                        else
                            id = VSConstants.VSITEMID_NIL;

                        sel.OnSelectChangeEx(hier, id, ms, handle);
                    }
                    finally
                    {
                        Marshal.Release(handle);
                        Marshal.Release(hier);
                    }
                }
            }
        }

        public event EventHandler<RetrieveSelectionEventArgs> RetrieveSelection;

        protected virtual void OnRetrieveSelection(RetrieveSelectionEventArgs e)
        {
            if (RetrieveSelection != null)
                RetrieveSelection(this, e);
        }

        public event EventHandler<ResolveItemEventArgs> ResolveItem;

        protected virtual void OnResolveItem(ResolveItemEventArgs e)
        {
            if (ResolveItem != null)
                ResolveItem(this, e);
        }

        #region ISelectionContainer Members

        /// <summary>
        /// Returns either a count of the total number of objects available or a count of the objects in the current selection.
        /// </summary>
        /// <param name="dwFlags">[in] Flag that specifies which type of count to return. 
        /// If set to GETOBJS_ALL, CountObjects returns the total number of objects. 
        /// If set to GETOBJS_SELECTED, CountObjects returns the number of selected objects.
        /// </param>
        /// <param name="pc">[out] Pointer to the requested object count.</param>
        /// <returns></returns>
        int ISelectionContainer.CountObjects(uint dwFlags, out uint pc)
        {
            if (dwFlags == (uint)Constants.GETOBJS_ALL)
                pc = ProvideWholeListForSelection ? (uint)Items.Count : (uint)SelectedItems.Count;
            else if (dwFlags == (uint)Constants.GETOBJS_SELECTED)
                pc = (uint)SelectedItems.Count;
            else
            {
                pc = 0;
                return VSConstants.E_FAIL;
            }

            return VSConstants.S_OK;
        }

        int ISelectionContainer.GetObjects(uint dwFlags, uint cObjects, object[] apUnkObjects)
        {
            if (apUnkObjects == null)
                return VSConstants.E_POINTER;

            IList items;

            if (dwFlags == (uint)Constants.GETOBJS_ALL)
                items = ProvideWholeListForSelection ? (IList)Items : SelectedItems;
            else if (dwFlags == (uint)Constants.GETOBJS_SELECTED)
                items = SelectedItems;
            else
            {
                for (int i = 0; i < cObjects; i++)
                {
                    apUnkObjects[i] = null;
                }
                return VSConstants.E_FAIL;
            }

            for (int i = 0; i < cObjects; i++)
            {
                if (i < items.Count)
                {
                    RetrieveSelectionEventArgs e = new RetrieveSelectionEventArgs((ListViewItem)items[i]);
                    OnRetrieveSelection(e);

                    apUnkObjects[i] = e.SelectionItem ?? new NotWrapped();
                }
                else
                    apUnkObjects[i] = null;
            }

            return VSConstants.S_OK;
        }

        int ISelectionContainer.SelectObjects(uint cSelect, object[] apUnkSelect, uint dwFlags)
        {
            if (apUnkSelect == null)
                return VSConstants.E_POINTER;

            _updatingSelection = true;
            try
            {
                SelectedItems.Clear();

                for (int i = 0; i < cSelect; i++)
                {
                    ResolveItemEventArgs e = new ResolveItemEventArgs(apUnkSelect[i]);

                    OnResolveItem(e);

                    ListViewItem lvi = e.Item;

                    if (lvi != null && lvi.ListView == this)
                        lvi.Selected = true;
                }
            }
            finally
            {
                _updatingSelection = false;
            }

            NotifySelectionUpdated();

            return VSConstants.S_OK;
        }

        #endregion

        public sealed class ResolveItemEventArgs : EventArgs
        {
            readonly object _selectionItem;
            ListViewItem _item;

            public ResolveItemEventArgs(object selectionItem)
            {
                if (selectionItem == null)
                    throw new ArgumentNullException("selectionItem");

                _selectionItem = selectionItem;
            }

            /// <summary>
            /// Gets the selection item.
            /// </summary>
            /// <value>The selection item.</value>
            public object SelectionItem
            {
                get { return _selectionItem; }
            }

            /// <summary>
            /// Gets or sets the item.
            /// </summary>
            /// <value>The item.</value>
            public ListViewItem Item
            {
                get { return _item; }
                set { _item = value; }
            }
        }

        public sealed class RetrieveSelectionEventArgs : EventArgs
        {
            readonly ListViewItem _item;
            object _selection;

            public RetrieveSelectionEventArgs(ListViewItem item)
            {
                if (item == null)
                    throw new ArgumentNullException("item");

                _selection = _item = item;
            }

            /// <summary>
            /// Gets or sets the selection item.
            /// </summary>
            /// <value>The selection item.</value>
            public object SelectionItem
            {
                get { return _selection; }
                set { _selection = value; }
            }

            /// <summary>
            /// Gets the item.
            /// </summary>
            /// <value>The item.</value>
            public ListViewItem Item
            {
                get { return _item; }
            }
        }

        sealed class NotWrapped
        {
        }

        #region IVsHierarchy implementation over the ListViewItems

        internal class ListViewHierarchy : IVsHierarchy, IVsMultiItemSelect
        {
            readonly Dictionary<uint, IVsHierarchyEvents> _eventHandlers = new Dictionary<uint, IVsHierarchyEvents>();
            readonly Dictionary<TListViewItem, uint> _items = new Dictionary<TListViewItem, uint>();
            readonly Dictionary<uint, TListViewItem> _ids = new Dictionary<uint, TListViewItem>();
            readonly ListViewWithSelection<TListViewItem> _lv;
            uint lvId;

            public ListViewHierarchy(ListViewWithSelection<TListViewItem> lv)
            {
                _lv = lv;
                lv.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
            }

            void OnSelectedIndexChanged(object sender, EventArgs e)
            {
                if (_items.Count == 0)
                    return;

                foreach (TListViewItem i in new List<TListViewItem>(_items.Keys))
                {
                    if (_lv.SelectedItems.Contains(i))
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

            public uint GetId(TListViewItem item)
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

            public TListViewItem GetItem(uint id)
            {
                TListViewItem value;
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
                TListViewItem lv;
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
                        pvar = lv.Text;
                        break;
                    case __VSHPROPID.VSHPROPID_IconImgList:
                        if (_lv.SmallImageList != null) // Called on VSITEMID_ROOT
                            pvar = (int)_lv.SmallImageList.Handle;
                        else
                            pvar = 0;
                        break;
                    case __VSHPROPID.VSHPROPID_IconIndex:
                        if (lv != null && lv.ImageList != null)
                            pvar = lv.ImageIndex;
                        else
                            pvar = 0;
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


            public int GetSelectionInfo(out uint pcItems, out int pfSingleHierarchy)
            {
                pcItems = (uint)_lv.SelectedItems.Count;
                pfSingleHierarchy = 1;
                return VSConstants.S_OK;
            }

            public int GetSelectedItems(uint grfGSI, uint cItems, VSITEMSELECTION[] rgItemSel)
            {
                bool omitHiers = (grfGSI == (uint)__VSGSIFLAGS.GSI_fOmitHierPtrs);

                if (cItems != _lv.SelectedItems.Count)
                    return VSConstants.E_FAIL;

                for (int i = 0; i < cItems; i++)
                {
                    rgItemSel[i].pHier = omitHiers ? null : this;

                    if (i < _lv.SelectedItems.Count)
                        rgItemSel[i].itemid = GetId((TListViewItem)_lv.SelectedItems[i]);
                }

                return VSConstants.S_OK;
            }



            #region IListViewHierarchy Members

            

            #endregion
        }
        #endregion
    }
}
