using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel;
using System.Drawing;

namespace Ankh.UI.VSSelectionControls
{
    public class TreeViewWithSelection<TNode> : TreeView, ISelectionMapOwner<TNode>
        where TNode : TreeNode
    {
        EventHandler _selectionChanged;

        public TreeViewWithSelection()
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

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            if (DesignMode)
                return;

            NotifySelectionUpdated();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Enter"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            EnsureSelection();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ExtendSelection(e.Location, true);
            }

            base.OnMouseDown(e);
        }

        protected virtual void ExtendSelection(Point p, bool rightClick)
        {
            TreeViewHitTestInfo hi = HitTest(p);
            
            bool onItem = hi.Node != null && hi.Location != TreeViewHitTestLocations.None;

            if (rightClick)
            {
                if (hi.Node != SelectedNode)
                {
                    SelectedNode = hi.Node;                    
                }
            }
        }

        int _inWndProc;
        bool _shouldUpdate;
        protected override void WndProc(ref Message m)
        {
            _inWndProc++;

            try
            {
                if (!DesignMode)
                {
                    if (m.Msg == 123) // WM_CONTEXTMENU
                    {
                        uint pos = unchecked((uint)m.LParam);

                        Select();

                        OnShowContextMenu(new MouseEventArgs(Control.MouseButtons, 1,
                            unchecked((short)(ushort)(pos & 0xFFFF)),
                            unchecked((short)(ushort)(pos >> 16)), 0));

                        return;
                    }
                }
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

        public event MouseEventHandler ShowContextMenu;
        public virtual void OnShowContextMenu(MouseEventArgs e)
        {
            if (_shouldUpdate)
            {
                _shouldUpdate = false;
                NotifySelectionUpdated();
            }

            if (ShowContextMenu != null)
                ShowContextMenu(this, e);
        }

        /// <summary>
        /// Notifies to external listeners selection updated.
        /// </summary>
        public virtual void NotifySelectionUpdated()
        {
            EnsureSelection();
        }

        protected void EnsureSelection()
        {
            if (SelectionPublishServiceProvider != null)
            {
                if (SelectionMap.Context != SelectionPublishServiceProvider)
                    SelectionMap.Context = SelectionPublishServiceProvider;

                SelectionMap.NotifySelectionUpdated();
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
            TNode _item;

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
            public TNode Item
            {
                get { return _item; }
                set { _item = value; }
            }
        }

        SelectionItemMap _selectionMap;
        internal SelectionItemMap SelectionMap
        {
            get { return _selectionMap ?? (_selectionMap = SelectionItemMap.Create(this)); }
        }

        public sealed class RetrieveSelectionEventArgs : EventArgs
        {
            readonly TNode _item;
            object _selection;

            public RetrieveSelectionEventArgs(TNode item)
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
            public TNode Item
            {
                get { return _item; }
            }
        }

        event EventHandler ISelectionMapOwner<TNode>.SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }

        System.Collections.IList ISelectionMapOwner<TNode>.Selection
        {
            get
            {
                TreeNode n = SelectedNode;

                if (n != null)
                    return new object[] { SelectedNode };
                else
                    return new object[0];
            }
        }

        System.Collections.IList ISelectionMapOwner<TNode>.AllItems
        {
            get { return ((ISelectionMapOwner<TNode>)this).Selection; }
        }

        public IntPtr GetImageList()
        {
            if (ImageList != null)
                return ImageList.Handle;
            else
                return IntPtr.Zero;
        }

        public int GetImageListIndex(TNode item)
        {
            return item.ImageIndex;
        }

        public string GetText(TNode item)
        {
            return item.Text;
        }

        public object GetSelectionObject(TNode item)
        {
            RetrieveSelectionEventArgs sa = new RetrieveSelectionEventArgs(item);

            OnRetrieveSelection(sa);

            return sa.SelectionItem;
        }

        #region ISelectionMapOwner<TNode> Members


        public TNode GetItemFromSelectionObject(object item)
        {
            return null;
        }

        public void SetSelection(TNode[] items)
        {
        }

        #endregion

        string ISelectionMapOwner<TNode>.GetCanonicalName(TNode item)
        {
            return GetCanonicalName(item);
        }

        protected virtual string GetCanonicalName(TNode item)
        {
            return null;
        }
    }
}
