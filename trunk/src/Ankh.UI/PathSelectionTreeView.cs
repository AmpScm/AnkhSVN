// $Id$
using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using Utils;
using System.Collections.Generic;
using System.ComponentModel;
using Ankh.Scc;

namespace Ankh.UI
{
    /// <summary>
    /// Represents a TreeView that can be used to choose from a set of paths.
    /// </summary>
    public partial class PathSelectionTreeView : PathTreeView
    {
        Predicate<SvnItem> _checkedFilter;


        public PathSelectionTreeView()
        {
            this.CheckBoxes = true;
            this.SingleCheck = false;
            this.Recursive = false;
            this.items = new SvnItem[] { };
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICollection<SvnItem> Items
        {
            get { return this.items; }
            set
            {
                this.items = value;
                this.BuildTree();
            }
        }

        /// <summary>
        /// Whether the paths used are URLs.
        /// </summary>
        public bool UrlPaths
        {
            get
            {
                return this.PathSeparator == "/";
            }
            set { this.PathSeparator = value ? "/" : "\\"; }
        }

        /// <summary>
        /// Whether there should be only one single check. Default is false.
        /// </summary>
        public bool SingleCheck
        {
            get { return this.singleCheck; }
            set { this.singleCheck = value; }
        }

        /// <summary>
        /// Whether a check is supposed to be perceived as recursive.
        /// </summary>
        public bool Recursive
        {
            get { return this.recursive; }
            set
            {
                this.recursive = value;
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
        public Predicate<SvnItem> CheckedFilter
        {
            get { return _checkedFilter; }
            set
            {
                _checkedFilter = value;
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
                this.checkedNode = null;
                // reenable children?
                if (this.Recursive)
                    this.ToggleChildren(e.Node, true);
                return;
            }

            // make sure only one node is checked if SingleCheck is true
            if (this.SingleCheck && this.checkedNode != null)
            {
                this.checkedNode.Checked = false;
            }

            // keep track of the last checked node
            this.checkedNode = e.Node;

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
            e.Cancel = e.Node.ForeColor == DisabledColor;
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
            foreach (TreeNode node in parent.Nodes)
            {
                node.Checked = enabled ? node.Checked : false;
                node.ForeColor = enabled ? EnabledColor : DisabledColor;

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

                if (this.items.Count == 0)
                    return;

                foreach (SvnItem item in this.items)
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

            // special treatment for URLs - we want the hostname in one go.
            if (this.UrlPaths)
                components = UriUtils.Split(fullPath);
            else
            {
                int nStart = 0;
                if (Context != null)
                {
                    Ankh.VS.IAnkhSolutionSettings ss = Context.GetService<Ankh.VS.IAnkhSolutionSettings>();

                    if (ss != null)
                    {
                        string root = ss.ProjectRootWithSeparator ?? "";

                        if (fullPath.StartsWith(root))
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
            }

            TreeNode node = null;
            foreach (string component in components)
            {
                node = this.GetNode(nodes, component);
                nodes = node.Nodes;
            }

            // leaf nodes should be black and enabled
            if (node != null)
            {
                node.ForeColor = EnabledColor;
                node.Tag = item;
                this.ResolveIcon(node);
            }
        }

        private TreeNode GetNode(TreeNodeCollection nodes, string pathComponent)
        {
            foreach (TreeNode node in nodes)
            {
                if (String.Compare(node.Text, pathComponent, true) == 0)
                    return node;
            }

            TreeNode newNode = nodes.Add(pathComponent);

            // non-leaf nodes default to gray and are disabled
            newNode.ForeColor = DisabledColor;
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

        private ICollection<SvnItem> items;
        private TreeNode checkedNode;
        private bool singleCheck;
        private bool recursive;

        private static readonly Color EnabledColor = Color.Black;
        private static readonly Color DisabledColor = Color.Gray;

    }
}
