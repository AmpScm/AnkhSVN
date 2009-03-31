// $Id$
//
// Copyright 2006-2009 The AnkhSVN Project
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
using System.ComponentModel;
using Ankh.Commands;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemTreeView : TreeViewWithSelection<FileSystemTreeNode>
    {
        public FileSystemTreeView()
        {
            this.HideSelection = false;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WCTreeNode SelectedItem
        {
            get
            {
                FileSystemTreeNode selected = this.SelectedNode as FileSystemTreeNode;

                if(selected != null)
                    return selected.WCNode;

                return null;
            }
        }

        public void SelectSubNode(SvnItem item)
        {
            if (item == null)
                return;

            FileSystemTreeNode fstn = SelectedNode as FileSystemTreeNode;
            if(fstn != null)
            {
                if(fstn.SvnItem == item)
                    return;
            }

            SelectedNode.Expand();

            foreach (FileSystemTreeNode tn in SelectedNode.Nodes)
            {
                if (tn.SvnItem == item)
                {
                    SelectedNode = tn;
                    return;
                }
            }

            // Walk up recursively
            SelectSubNode(item.Parent);

            // Walk back down (step down is taken by setting SelectedNode one deeper above)
            SelectSubNode(item);
        }

        IStatusImageMapper _statusMapper;
        internal IStatusImageMapper StatusMapper
        {
            get { return _statusMapper ?? (_statusMapper = Context.GetService<IStatusImageMapper>()); }
        }

        public void AddRoot(WCTreeNode rootItem)
        {
            TreeNode tn = this.AddNode(this.Nodes, rootItem);

            this.SelectedNode = tn;
        }

        public override void OnShowContextMenu(MouseEventArgs e)
        {
            base.OnShowContextMenu(e);

            Point screen;
            if (e.X == -1 && e.Y == -1)
            {
                screen = GetSelectionPoint();
                if(screen.IsEmpty)
                    return;
            }
            else
            {
                screen = e.Location;
            }

            IAnkhCommandService sc = Context.GetService<IAnkhCommandService>();

            sc.ShowContextMenu(AnkhCommandMenu.WorkingCopyExplorerContextMenu, screen);
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);

            if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Tag == DummyTag)
            {
                this.FillNode(e.Node);
            }
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);

            if (e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Tag == DummyTag)
            {
                this.FillNode(e.Node);
            }
        }

        internal void FillNode(TreeNode treeNode)
        {
            // get rid of the dummy node or existing nodes
            treeNode.Nodes.Clear();

            WCTreeNode item = treeNode.Tag as WCTreeNode;

            foreach (WCTreeNode child in item.GetChildren())
            {
                if (child.IsContainer)
                {
                    AddNode(treeNode.Nodes, child);
                }
            }
        }

        IAnkhServiceProvider _context;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

            if (IconMapper != null)
                ImageList = IconMapper.ImageList;

            if (StateImageList == null)
                StateImageList = StatusMapper.StatusImageList;
            
            SelectionPublishServiceProvider = Context;
        }

        protected IFileIconMapper IconMapper
        {
            get { return _mapper ?? (_mapper = Context.GetService<IFileIconMapper>()); }
        }

        [Browsable(false)]
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

        private TreeNode AddNode(TreeNodeCollection nodes, WCTreeNode child)
        {
            FileSystemTreeNode normalTreeNode = new FileSystemTreeNode(child);
            normalTreeNode.Tag = child;
            normalTreeNode.SelectedImageIndex = normalTreeNode.ImageIndex = child.ImageIndex;
            nodes.Add(normalTreeNode);
            normalTreeNode.Refresh();

            FileSystemTreeNode d = new FileSystemTreeNode("DUMMY");
            normalTreeNode.Nodes.Add(d);
            d.Tag = DummyTag;
            return normalTreeNode;
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

        internal void ClearSolutionRoot()
        {
            foreach (FileSystemTreeNode tn in Nodes)
            {
                if (tn.WCNode is WCSolutionNode)
                    tn.Remove();
            }
        }

        internal void ClearRoots()
        {
            Nodes.Clear();
        }

        IEnumerable<FileSystemTreeNode> NodesInSearchOrder
        {
            get
            {
                foreach (FileSystemTreeNode tn in Nodes)
                    if (tn.WCNode is WCSolutionNode)
                        yield return tn;

                foreach (FileSystemTreeNode tn in Nodes)
                    if (tn.WCNode is WCDirectoryNode)
                        yield return tn;

                foreach (FileSystemTreeNode tn in Nodes)
                    if (tn.WCNode is WCMyComputerNode)
                        yield return tn;
            }
        }

        internal void BrowsePath(string path)
        {
            foreach (FileSystemTreeNode tn in NodesInSearchOrder)
            {
                if (tn.WCNode.ContainsDescendant(path))
                {
                    tn.SelectSubNode(path);
                    return;
                }
            }
        }
    }
}
