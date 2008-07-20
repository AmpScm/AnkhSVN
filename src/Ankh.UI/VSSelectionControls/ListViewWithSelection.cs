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
    public class ListViewWithSelection<TListViewItem> : ListView, ISelectionMapOwner<TListViewItem>
        where TListViewItem : ListViewItem
    {
        bool _provideFullList;

		public ListViewWithSelection()
		{
			DoubleBuffered = true;
		}

        IServiceProvider _serviceProvider;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IServiceProvider SelectionPublishServiceProvider
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

            if (DesignMode)
                return;

            if (!_updatingSelection)
            {
                if (SelectedIndices.Count > 0)
                    MaybeNotifySelectionUpdated();
                else
                    _maybeUnselect = true;
            }
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

        bool _strictCheckboxesClick;
        [DefaultValue(false)]
        public bool StrictCheckboxesClick
        {
            get { return _strictCheckboxesClick; }
            set { _strictCheckboxesClick = value; }
        }

        int _inWndProc;
        protected override void WndProc(ref Message m)
        {
            _inWndProc++;
            try
            {
            if (!DesignMode)
            {
                if (m.Msg == 123) // WM_CONTEXT
                {
                    OnShowContextMenu(EventArgs.Empty);
                    return;
                }
                else if (m.Msg == 8270) // WM_REFLECTNOTIFY
                {
                    if (CheckBoxes && StrictCheckboxesClick)
                    {
                        NMHDR hdr = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));

                        if (hdr.code == -3) // Double click
                        {
                            Point mp = PointToClient(MousePosition);
                            ListViewHitTestInfo hi = HitTest(mp);

                            if (hi != null && hi.Location != ListViewHitTestLocations.StateImage)
                            {
                                OnMouseDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, mp.X, mp.Y, 0));
                                return;
                            }
                        }
                    }
                }
            }
            base.WndProc(ref m);
            }
            finally
            {
                if(0 == --_inWndProc)
                {
                    if(_shouldUpdate)
                    {
                        _shouldUpdate = false;
                        NotifySelectionUpdated();
                    }
                }
            }
        }

        public event EventHandler ShowContextMenu;
        public virtual void OnShowContextMenu(EventArgs e)
        {
            if (ShowContextMenu != null)
                ShowContextMenu(this, e);
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
   
        /// <summary>
        /// Notifies to external listeners selection updated.
        /// </summary>
        public void NotifySelectionUpdated()
        {
            _maybeUnselect = true;

            if (SelectionPublishServiceProvider != null)
            {
                SelectionMap.NotifySelectionUpdated(SelectionPublishServiceProvider);               
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

        event EventHandler ISelectionMapOwner<TListViewItem>.SelectionChanged
        {
            add { SelectedIndexChanged += value; }
            remove { SelectedIndexChanged -= value; }
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

        #endregion

        #region ISelectionMapOwner<TListViewItem> Members


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
