using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel;

namespace Ankh.UI.VSSelectionControls
{
    public class TreeViewWithSelection<TNode> : TreeView, ISelectionMapOwner<TNode>
        where TNode : TreeNode
    {
        bool _maybeUnselect;
        EventHandler _selectionChanged;


        IServiceProvider _serviceProvider;
        [Browsable(false)]
        public IServiceProvider ServiceProvider
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
        /// Notifies to external listeners selection updated.
        /// </summary>
        public void NotifySelectionUpdated()
        {
            _maybeUnselect = true;

            if (ServiceProvider != null)
            {
                Hierarchy.NotifySelectionUpdated(ServiceProvider);
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

        internal virtual TreeViewHierarchy CreateHierarchy()
        {
            return new TreeViewHierarchy(this);
        }

        TreeViewHierarchy _hier;
        internal TreeViewHierarchy Hierarchy
        {
            get { return _hier ?? (_hier = CreateHierarchy()); }
        }

        internal class TreeViewHierarchy : SelectionItemMap<TNode>
        {
            public TreeViewHierarchy(TreeViewWithSelection<TNode> lv)
                : base(lv)
            {
            }
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
            return item;
        }
    }
}
