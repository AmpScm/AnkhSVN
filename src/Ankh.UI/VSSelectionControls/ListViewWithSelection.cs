using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace Ankh.UI.VSSelectionControls
{
    /// <summary>
    /// Generic listview with <see cref="ISelectionContainer"/> support
    /// </summary>
    /// <typeparam name="TListViewItem">The type of the list view item.</typeparam>
    public class ListViewWithSelection<TListViewItem> : SmartListView, ISelectionMapOwner<TListViewItem>
        where TListViewItem : ListViewItem
    {
        bool _provideFullList;

        public ListViewWithSelection()
        {
        }

        IAnkhServiceProvider _serviceProvider;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAnkhServiceProvider SelectionPublishServiceProvider
        {
            get { return _serviceProvider; }
            set
            {
                if (value != null && value != _serviceProvider)
                {
                    _serviceProvider = value;
                    NotifySelectionUpdated();
                }
                else
                    _serviceProvider = value;
            }
        }

        [Browsable(false), DefaultValue(false)]
        public bool ProvideWholeListForSelection
        {
            get { return _provideFullList; }
            set { _provideFullList = value; }
        }

        bool _updatingSelection;
        bool _maybeUnselect;
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ListView.SelectedIndexChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (DesignMode)
                return;

            if (SelectedIndices.Count > 0)
                MaybeNotifySelectionUpdated();
            else
                _maybeUnselect = true;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            EnsureSelection();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!DesignMode && _maybeUnselect && SelectedIndices.Count == 0)
            {
                MaybeNotifySelectionUpdated();
            }
        }

        bool _shouldUpdate;

        int _inWndProc;
        protected override void WndProc(ref Message m)
        {
            _inWndProc++;
            try
            {
                base.WndProc(ref m);
            }
            finally
            {
                if (0 == --_inWndProc)
                {
                    if (_shouldUpdate)
                    {
                        _shouldUpdate = false;
                        NotifySelectionUpdated();
                    }
                }
            }
        }

        protected override void ExtendSelection(Point p, bool rightClick)
        {
            base.ExtendSelection(p, rightClick);

            if (_shouldUpdate)
            {
                _shouldUpdate = false;
                NotifySelectionUpdated();
            }
        }

        public override void OnShowContextMenu(MouseEventArgs e)
        {
            if (_shouldUpdate)
            {
                _shouldUpdate = false;
                NotifySelectionUpdated();
            }

            base.OnShowContextMenu(e);
        }

        SelectionItemMap _selectionItemMap;
        internal virtual SelectionItemMap SelectionMap
        {
            get { return _selectionItemMap ?? (_selectionItemMap = SelectionItemMap.Create(this)); }
        }

        void MaybeNotifySelectionUpdated()
        {
            if (_inWndProc > 0)
                _shouldUpdate = true;
            else
                NotifySelectionUpdated();
        }

        public void SelectAllItems()
        {
            if (!MultiSelect)
                return;

            bool unchanged = true;
            _inWndProc++;
            try
            {
                foreach (ListViewItem i in this.Items)
                {
                    if (unchanged && !i.Selected)
                        unchanged = false;

                    i.Selected = true;
                }
            }
            finally
            {
                if (0 == --_inWndProc)
                {
                    if (_shouldUpdate || !unchanged)
                    {
                        _shouldUpdate = false;
                        NotifySelectionUpdated();
                    }
                }
            }
        }

        /// <summary>
        /// Notifies to external listeners selection updated.
        /// </summary>
        public void NotifySelectionUpdated()
        {
            if (_updatingSelection)
                return;

            _updatingSelection = true;
            try
            {
                _maybeUnselect = false;

                if (SelectionPublishServiceProvider != null)
                {
                    if (SelectionMap.Context != SelectionPublishServiceProvider)
                        SelectionMap.Context = SelectionPublishServiceProvider;

                    SelectionMap.NotifySelectionUpdated();
                }

                if (_selectionChanged != null)
                    _selectionChanged(this, EventArgs.Empty);
            }
            finally
            {
                _updatingSelection = false;
            }
        }

        protected void EnsureSelection()
        {
            if (SelectionPublishServiceProvider != null)
            {
                if (SelectionMap.Context != SelectionPublishServiceProvider)
                    SelectionMap.Context = SelectionPublishServiceProvider;

                SelectionMap.EnsureSelection();
            }
        }

        public delegate int SortItem(TListViewItem x, TListViewItem y);
        protected sealed class SortWrapper : IComparer<ListViewItem>
        {
            readonly SortItem _sorter;

            public SortWrapper(SortItem handler)
            {
                if (handler == null)
                    throw new ArgumentNullException("handler");

                _sorter = handler;
            }

            #region IComparer<ListViewItem> Members

            public int Compare(ListViewItem x, ListViewItem y)
            {
                return _sorter((TListViewItem)x, (TListViewItem)y);
            }

            #endregion
        }


        /// <summary>
        /// Occurs when converting a ListViewItem in a selection item
        /// </summary>
        public event EventHandler<RetrieveSelectionEventArgs> RetrieveSelection;

        /// <summary>
        /// Called to convert a ListViewItem in a selection item
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRetrieveSelection(RetrieveSelectionEventArgs e)
        {
            if (RetrieveSelection != null)
                RetrieveSelection(this, e);
        }

        /// <summary>
        /// Occurs when converting a selection item in a ListViewItem
        /// </summary>
        public event EventHandler<ResolveItemEventArgs> ResolveItem;

        /// <summary>
        /// Called to convert a selection item to a ListViewItem
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnResolveItem(ResolveItemEventArgs e)
        {
            if (ResolveItem != null)
                ResolveItem(this, e);
        }

        public sealed class ResolveItemEventArgs : EventArgs
        {
            readonly object _selectionItem;
            TListViewItem _item;

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
            public TListViewItem Item
            {
                get { return _item; }
                set { _item = value; }
            }
        }

        public sealed class RetrieveSelectionEventArgs : EventArgs
        {
            readonly TListViewItem _item;
            object _selection;

            public RetrieveSelectionEventArgs(TListViewItem item)
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
            public TListViewItem Item
            {
                get { return _item; }
            }
        }

        #region ISelectionMapOwner<TListViewItem> Members

        EventHandler _selectionChanged;

        event EventHandler ISelectionMapOwner<TListViewItem>.SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }

        IList ISelectionMapOwner<TListViewItem>.Selection
        {
            get
            {
                if (!VirtualMode)
                    return SelectedItems;
                else
                {
                    List<TListViewItem> lvi = new List<TListViewItem>();
                    foreach (int i in SelectedIndices)
                        lvi.Add((TListViewItem)Items[i]);
                    return lvi;
                }
            }
        }

        IList ISelectionMapOwner<TListViewItem>.AllItems
        {
            get { return _provideFullList ? (IList)Items : (IList)((ISelectionMapOwner<TListViewItem>)this).Selection; }
        }


        IntPtr ISelectionMapOwner<TListViewItem>.GetImageList()
        {
            return SmallImageList.Handle;
        }

        int ISelectionMapOwner<TListViewItem>.GetImageListIndex(TListViewItem item)
        {
            return item.ImageIndex;
        }

        string ISelectionMapOwner<TListViewItem>.GetText(TListViewItem item)
        {
            return item.Text;
        }

        object ISelectionMapOwner<TListViewItem>.GetSelectionObject(TListViewItem item)
        {
            RetrieveSelectionEventArgs sa = new ListViewWithSelection<TListViewItem>.RetrieveSelectionEventArgs(item);

            OnRetrieveSelection(sa);

            return sa.SelectionItem;
        }

        public TListViewItem GetItemFromSelectionObject(object item)
        {
            ResolveItemEventArgs ra = new ResolveItemEventArgs(item);

            OnResolveItem(ra);

            return ra.Item;
        }

        public void SetSelection(TListViewItem[] items)
        {
            foreach (TListViewItem l in SelectedItems)
            {
                if (0 > Array.IndexOf(items, l))
                    l.Selected = false;
            }

            foreach (TListViewItem l in items)
            {
                l.Selected = true;
            }
        }

        string ISelectionMapOwner<TListViewItem>.GetCanonicalName(TListViewItem item)
        {
            return GetCanonicalName(item);
        }

        protected virtual string GetCanonicalName(TListViewItem item)
        {
            return null;
        }

        #endregion
    }
    [StructLayout(LayoutKind.Sequential)]
    struct NMHDR
    {
        public IntPtr hwndFrom;
        public int idFrom;
        public int code;
    }
}
