// $Id$
//
// Copyright 2004-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using Ankh.Scc;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.UI.VSSelectionControls;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Ankh.UI.PathSelector
{
    /// <summary>
    /// Represents a TreeView that can be used to choose from a set of paths.
    /// </summary>
    partial class PathSelectionTreeView : PathTreeView
    {
        Predicate<SvnItem> _checkedFilter;
        ICollection<SvnItem> _items;
        TreeNode _checkedNode;
        bool _singleCheck;
        bool _recursive;

        public PathSelectionTreeView()
        {
            this.CheckBoxes = true;
            this.SingleCheck = false;
            this.Recursive = false;
            this._items = new SvnItem[] { };
        }

        protected override void WndProc(ref Message m)
        {
            if (!DesignMode)
            {
                if (m.Msg == 0x0203) // WM_LBUTTONDBLCLK
                {
                    if (CheckBoxes)
                    {
                            Point mp = PointToClient(MousePosition);
                            TreeViewHitTestInfo hi = HitTest(mp);

                            if (hi != null && 
                                hi.Location == TreeViewHitTestLocations.StateImage && 
                                !((PathTreeNode)hi.Node).Enabled)
                            {
                                m.Result = new IntPtr(1);
                                return;
                            }
                    }
                }
            }
            base.WndProc(ref m);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICollection<SvnItem> Items
        {
            get { return this._items; }
            set
            {
                this._items = value;
                this.BuildTree();
            }
        }
     
        /// <summary>
        /// Whether there should be only one single check. Default is false.
        /// </summary>
        public bool SingleCheck
        {
            get { return this._singleCheck; }
            set { this._singleCheck = value; }
        }

        /// <summary>
        /// Whether a check is supposed to be perceived as recursive.
        /// </summary>
        public bool Recursive
        {
            get { return this._recursive; }
            set
            {
                this._recursive = value;
                BeginUpdate();
                this.UncheckChildren(this.Nodes, value);
                EndUpdate();
            }
        }

        public IEnumerable<SvnItem> CheckedItems
        {
            get
            {
                return GetCheckedItems(this.Nodes);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public event Predicate<SvnItem> CheckedFilter
        {
            add
            {
                _checkedFilter += value;
                SetCheckedItems(Nodes);
            }
            remove
            {
                _checkedFilter -= value;
                SetCheckedItems(Nodes);
            }
        }

        /// <summary>
        /// Keep track of the last checked node and make sure only
        /// one node is checked at one time if singleCheck is true.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);

            // unchecking?
            if (!e.Node.Checked)
            {
                if (e.Action == TreeViewAction.ByKeyboard || e.Action == TreeViewAction.ByMouse)
                {
                    this._checkedNode = null;
                    // reenable children?
                    if (this.Recursive)
                        this.ToggleChildren(e.Node, true);
                }
                return;
            }

            // make sure only one node is checked if SingleCheck is true
            if (this.SingleCheck && this._checkedNode != null)
            {
                this._checkedNode.Checked = false;
            }

            // keep track of the last checked node
            
                this._checkedNode = e.Node;

            // disable children?
            if (this.Recursive)
                this.ToggleChildren(e.Node, false);


        }

        /// <summary>
        /// Make sure you can't check disabled nodes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            base.OnBeforeCheck(e);
            if (!((PathTreeNode)e.Node).Enabled)
                e.Cancel = true;
        }

        protected void ResolveIcon(TreeNode node)
        {

            SvnItem item = (SvnItem)node.Tag;
            if (item.IsDirectory)
            {
                node.SelectedImageIndex = node.ImageIndex = this.FolderIndex;
            }
            else
                this.SetIcon(node, item.FullPath);
        }



        /// <summary>
        /// Retrieves a list of the checked items.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private IEnumerable<SvnItem> GetCheckedItems(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                    yield return (SvnItem)node.Tag;

                foreach (SvnItem i in GetCheckedItems(node.Nodes))
                    yield return i;
            }
        }

        /// <summary>
        /// Sets the checked items.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="items"></param>
        void SetCheckedItems(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = SvnItemFilters.Evaluate((SvnItem)node.Tag, _checkedFilter);

                this.SetCheckedItems(node.Nodes);
            }
        }


        /// <summary>
        /// Recursively uncheck and disable the children of checked nodes
        /// </summary>
        /// <param name="nodes"></param>
        private void UncheckChildren(TreeNodeCollection nodes, bool recursive)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked)
                    this.ToggleChildren(node, !recursive);
                else
                    this.UncheckChildren(node.Nodes, recursive);
            }
        }

        /// <summary>
        /// Enables or disables the children of a specific node.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="enabled"></param>
        private void ToggleChildren(TreeNode parent, bool enabled)
        {
            this.BeginUpdate();
            this.DoToggleChildren(parent, enabled);
            this.EndUpdate();
        }

        /// <summary>
        /// Helper for above.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="enabled"></param>
        private void DoToggleChildren(TreeNode parent, bool enabled)
        {
            foreach (PathTreeNode node in parent.Nodes)
            {
                node.Checked = enabled ? node.Checked : false;

                this.ToggleChildren(node, enabled);
            }
        }


        /// <summary>
        /// Builds the treeview from the objects in this.paths
        /// </summary>
        private void BuildTree()
        {
            try
            {
                this.BeginUpdate();
                this.Nodes.Clear();

                if (this._items.Count == 0)
                    return;

                foreach (SvnItem item in this._items)
                    this.AddNode(item);

                this.TrimTree();

                this.ExpandAll();
            }
            finally
            {
                this.EndUpdate();
            }
        }

        /// <summary>
        /// Adds a node to the right place in the tree.
        /// </summary>
        /// <param name="nodeName"></param>
        private void AddNode(SvnItem item)
        {
            TreeNodeCollection nodes = this.Nodes;

            string fullPath = item.FullPath;
            string[] components;

            int nStart = 0;
            if (Context != null)
            {
                IAnkhSolutionSettings ss = Context.GetService<IAnkhSolutionSettings>();

                if (ss != null)
                {
                    string root = ss.ProjectRootWithSeparator;

                    if (!string.IsNullOrEmpty(root) && fullPath.StartsWith(root))
                        nStart = root.Length - 1;
                }
            }

            if (nStart == 0)
                components = fullPath.Split(this.PathSeparator[0]);
            else
            {
                components = fullPath.Substring(nStart).Split(this.PathSeparator[0]);
                components[0] = fullPath.Substring(0, nStart) + components[0];
            }

            PathTreeNode node = null;
            foreach (string component in components)
            {
                node = this.GetNode(nodes, component);
                nodes = node.Nodes;
            }

            // leaf nodes should be black and enabled
            if (node != null)
            {
                node.Enabled = true;
                node.Tag = item;
                this.ResolveIcon(node);
            }
        }

        private PathTreeNode GetNode(TreeNodeCollection nodes, string pathComponent)
        {
            foreach (PathTreeNode node in nodes)
            {
                if (String.Compare(node.Text, pathComponent, true) == 0)
                    return node;
            }

            PathTreeNode newNode = new PathTreeNode(pathComponent);
            nodes.Add(newNode);

            // non-leaf nodes default to gray and are disabled
            newNode.Enabled = false;
            newNode.SelectedImageIndex = newNode.ImageIndex = FolderIndex;
            return newNode;
        }

        /// <summary>
        /// Simplify the tree so top level node hierarchies with only 1 child
        /// per node are reduced to a single node
        /// </summary>
        private void TrimTree()
        {
            foreach (TreeNode topLevelNode in this.Nodes)
            {
                if (topLevelNode.Nodes.Count == 1)
                {
                    StringBuilder nodeName = new StringBuilder(topLevelNode.Text);

                    // Recurse down the tree, adding nodes while relevant
                    // We don't want to add the last child element
                    TreeNode activeNode = topLevelNode;
                    while (activeNode.Nodes.Count == 1 && activeNode.Nodes[0].Nodes.Count != 0)
                    {
                        activeNode = activeNode.Nodes[0];

                        nodeName.Append(System.IO.Path.DirectorySeparatorChar);
                        nodeName.Append(activeNode.Text);
                    }

                    // Rename the last node to include the full path
                    activeNode.Text = nodeName.ToString();

                    // Replace the top level node with the current one
                    this.Nodes.Remove(topLevelNode);
                    this.Nodes.Add(activeNode);
                }
            }
        }

        IAnkhVSColor ColorProvider
        {
            get
            {
                return Context == null ? null : Context.GetService<IAnkhVSColor>();
            }
        }


        Color _enabledColor;
        Color EnabledColor
        {
            get
            {
                if (_enabledColor == Color.Empty)
                {
                    Color color;
                    if (ColorProvider != null &&
                        ColorProvider.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_TEXT_ACTIVE, out color))
                        _enabledColor = color;
                    else
                        _enabledColor = Color.Black;
                }
                return _enabledColor;
            }
        }

        Color _disabledColor;
        Color DisabledColor
        {
            get
            {
                if (_disabledColor == Color.Empty)
                {
                    Color color;
                    if (ColorProvider != null &&
                        ColorProvider.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_TEXT_INACTIVE, out color))
                        _disabledColor = color;
                    else
                        _disabledColor = Color.Gray;
                }
                return _disabledColor;
            }
        }

        class PathTreeNode : TreeNode
        {
            public PathTreeNode(string text)
                :base(text)
            {
            }
            bool _enabled;
            public bool Enabled
            {
                get { return _enabled; }
                set 
                {
                    _enabled = value;
                    ForeColor = value ? ((PathSelectionTreeView)TreeView).EnabledColor : ((PathSelectionTreeView)TreeView).DisabledColor;
                }
            }
        }
    }
}
