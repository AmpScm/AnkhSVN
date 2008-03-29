using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Collections;

namespace Ankh.UI.PendingChanges
{
    /// <summary>
    /// Generic listview with <see cref="ISelectionContainer"/> support
    /// </summary>
    public class ListViewWithSelection : ListView, ISelectionContainer
    {
        bool _provideFullList;

        IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
            set { _serviceProvider = value; }
        }

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

        /// <summary>
        /// Notifies to external listeners selection updated.
        /// </summary>
        public void NotifySelectionUpdated()
        {
            _maybeUnselect = true;
            if (ServiceProvider != null)
            {
                ITrackSelection sel = (ITrackSelection)ServiceProvider.GetService(typeof(STrackSelection));

                if (sel != null)
                    sel.OnSelectChange(this);
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
    }
}
