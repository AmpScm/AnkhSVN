using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using System.Runtime.Remoting.Contexts;
using Ankh.Scc;
using System.Collections.Generic;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemTreeView : TreeViewWithSelection<FileSystemTreeNode>, IWorkingCopyExplorerSubControl
    {
        public event EventHandler SelectedItemChanged;

        public FileSystemTreeView()
        {
            this.HideSelection = false;
        }


        public IFileSystemItem SelectedItem
        {
            get
            {
                return this.SelectedNode != null ? this.SelectedNode.Tag as IFileSystemItem : null;
            }
            set
            {
                // Assume for now that it can only change to a direct child of the current node
                if (this.SelectedItem != null)
                {
                    this.SelectedNode.Expand();

                    foreach (TreeNode childNode in this.SelectedNode.Nodes)
                    {
                        if (childNode.Tag.Equals(value))
                        {
                            this.SelectedNode = childNode;
                        }
                    }
                }
            }
        }

        public void AddRoot(IFileSystemItem rootItem)
        {
            this.AddNode(this.Nodes, rootItem);

            // select this node if it's the first one added
            if (this.Nodes.Count == 1)
            {
                this.SelectedNode = this.Nodes[0];
            }
        }

        public System.Drawing.Point GetSelectionPoint()
        {
            if (this.SelectedNode != null)
            {
                int offset = this.SelectedNode.Bounds.Height / 3;
                return this.PointToScreen(new Point(this.SelectedNode.Bounds.X + offset,
                    this.SelectedNode.Bounds.Y + offset));
            }
            else
            {
                return Point.Empty;
            }
        }

        public IFileSystemItem[] GetSelectedItems()
        {
            if (this.SelectedItem != null)
            {
                return new IFileSystemItem[] { this.SelectedItem };
            }
            else
            {
                return new IFileSystemItem[] { };
            }
        }

        internal void RemoveRoot(IFileSystemItem root)
        {
            TreeNode nodeToRemove = null;
            foreach (TreeNode node in this.Nodes)
            {
                if (node.Tag == root)
                {
                    nodeToRemove = node;
                    break;
                }
            }

            if (nodeToRemove != null)
            {
                nodeToRemove.Remove();
            }
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.SelectedNode = this.GetNodeAt(e.X, e.Y);
            }

            base.OnMouseDown(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            if (this.SelectedItemChanged != null)
            {
                this.SelectedItemChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);

            if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Tag == DummyTag)
            {
                this.FillNode(e.Node);
            }
        }

        private void FillNode(TreeNode treeNode)
        {
            // get rid of the dummy node or existing nodes
            treeNode.Nodes.Clear();

            IFileSystemItem item = treeNode.Tag as IFileSystemItem;

            foreach (IFileSystemItem child in item.GetChildren())
            {
                if (child.IsContainer)
                {
                    this.AddNode(treeNode.Nodes, child);
                }
            }
        }

        IAnkhServiceProvider _context;
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                if (_context != value)
                {
                    _context = value;
                    OnContextChanged(EventArgs.Empty);
                }
            }
        }

        IFileIconMapper _mapper;

        protected virtual void OnContextChanged(EventArgs e)
        {
            if (!DesignMode && Context != null)
            {
                _mapper = Context.GetService<IFileIconMapper>();

                if (IconMapper != null)
                    ImageList = IconMapper.ImageList;
            }
        }

        protected IFileIconMapper IconMapper
        {
            get { return _mapper; }
        }

        public int FolderIndex
        {
            get
            {
                if (IconMapper != null)
                    return IconMapper.DirectoryIcon;
                else
                    return -1;
            }
        }

        private void AddNode(TreeNodeCollection nodes, IFileSystemItem child)
        {
            child.ItemChanged += new EventHandler<ItemChangedEventArgs>(child_ItemChanged);
            FileSystemTreeNode ftn = new FileSystemTreeNode(child.SvnItem);
            nodes.Add(ftn);
            ftn.Tag = child;
            ftn.SelectedImageIndex = ftn.ImageIndex = this.FolderIndex;

            FileSystemTreeNode dummy = new FileSystemTreeNode("DUMMY");
            ftn.Nodes.Add(dummy);
            dummy.Tag = DummyTag;
        }

        void child_ItemChanged(object sender, ItemChangedEventArgs e)
        {
            IFileSystemItem item = sender as IFileSystemItem;
            if (e.ItemChangedType == ItemChangedType.ChildrenInvalidated && item != null)
            {
                this.HandleItemChanged(item);
            }
        }

        private void HandleItemChanged(IFileSystemItem item)
        {
            TreeNode node = this.SearchForNodeRecursivelyByTag(this.Nodes, item);
            if (node != null)
            {
                this.RecursivelyUnhookFromEvents(node.Nodes);
                this.FillNode(node);
            }
        }

        private void RecursivelyUnhookFromEvents(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                IFileSystemItem item = node.Tag as IFileSystemItem;
                if (item != null)
                {
                    item.ItemChanged -= new EventHandler<ItemChangedEventArgs>(this.child_ItemChanged);
                }

                RecursivelyUnhookFromEvents(node.Nodes);
            }
        }

        private TreeNode SearchForNodeRecursivelyByTag(TreeNodeCollection coll, object tag)
        {
            foreach (TreeNode node in coll)
            {
                if (node.Tag == tag)
                {
                    return node;
                }
                TreeNode foundNode = SearchForNodeRecursivelyByTag(node.Nodes, tag);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }
            return null;
        }

        protected override void OnRetrieveSelection(TreeViewWithSelection<FileSystemTreeNode>.RetrieveSelectionEventArgs e)
        {
            SvnItem item = e.Item.SvnItem;

            if (item != null)
            {
                e.SelectionItem = new SvnItemData(Context, item);
                return;
            }

            base.OnRetrieveSelection(e);
        }

        protected override void OnResolveItem(TreeViewWithSelection<FileSystemTreeNode>.ResolveItemEventArgs e)
        {
            SvnItemData dt = e.SelectionItem as SvnItemData;
            if (dt != null)
            {
                foreach (FileSystemTreeNode tn in AllNodes)
                {
                    if (tn.SvnItem == dt.SvnItem)
                    {
                        e.Item = tn;
                        return;
                    }
                }
            }

            base.OnResolveItem(e);
        }

        protected override string GetCanonicalName(FileSystemTreeNode item)
        {
            SvnItem i = item.SvnItem;

            if (i != null)
            {
                string name = i.FullPath;

                if (i.IsDirectory && !name.EndsWith("\\"))
                    name += "\\"; // VS usualy ends folders with '\\'

                return name;
            }

            return base.GetCanonicalName(item);
        }

        protected IEnumerable<FileSystemTreeNode> AllNodes
        {
            get { return GetAllNodes(Nodes); }
        }

        private IEnumerable<FileSystemTreeNode> GetAllNodes(TreeNodeCollection Nodes)
        {
            foreach (FileSystemTreeNode fst in Nodes)
            {
                yield return fst;

                foreach (FileSystemTreeNode f in GetAllNodes(fst.Nodes))
                    yield return fst;
            }
        }
        private static readonly object DummyTag = new object();
    }
}
