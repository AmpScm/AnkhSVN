// $Id$
//
// Copyright 2006-2008 The AnkhSVN Project
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
using System.Drawing;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms;

using Ankh.Scc;
using Ankh.UI.VSSelectionControls;
using Ankh.UI.WorkingCopyExplorer.Nodes;
using Ankh.VS;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemTreeView : TreeViewWithSelection<FileSystemTreeNode>
    {
        public event EventHandler SelectedItemChanged;

        public FileSystemTreeView()
        {
            this.HideSelection = false;
        }


        public FileSystemNode SelectedItem
        {
            get
            {
                return this.SelectedNode != null ? this.SelectedNode.Tag as FileSystemNode : null;
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

        public void AddRoot(FileSystemNode rootItem)
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

        public FileSystemNode[] GetSelectedItems()
        {
            if (this.SelectedItem != null)
            {
                return new FileSystemNode[] { this.SelectedItem };
            }
            else
            {
                return new FileSystemNode[] { };
            }
        }

        internal void RemoveRoot(FileSystemNode root)
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

            FileSystemNode item = treeNode.Tag as FileSystemNode;

            foreach (FileSystemNode child in item.GetChildren())
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
            if (DesignMode || Context == null)
                return;

            SelectionPublishServiceProvider = Context;            

            if (IconMapper != null)
                ImageList = IconMapper.ImageList;
        }

        protected IFileIconMapper IconMapper
        {
            get { return _mapper ?? (_mapper = Context.GetService<IFileIconMapper>()); }
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

        private void AddNode(TreeNodeCollection nodes, FileSystemNode child)
        {
            FileSystemTreeNode ftn = new FileSystemTreeNode(child.SvnItem);
            nodes.Add(ftn);
            if (ftn.Parent == null)
                ftn.Text = child.SvnItem.FullPath;
            ftn.Tag = child;
            ftn.SelectedImageIndex = ftn.ImageIndex = this.FolderIndex;

            FileSystemTreeNode dummy = new FileSystemTreeNode("DUMMY");
            ftn.Nodes.Add(dummy);
            dummy.Tag = DummyTag;
        }

        private void HandleItemChanged(FileSystemNode item)
        {
            TreeNode node = this.SearchForNodeRecursivelyByTag(this.Nodes, item);
            if (node != null)
            {
                this.FillNode(node);
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
