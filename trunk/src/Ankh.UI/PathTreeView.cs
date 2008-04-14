// $Id$
using System;
using System.Windows.Forms;
using Utils.Win32;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Ankh.VS;
using Ankh.UI.VSSelectionControls;

namespace Ankh.UI
{
    /// <summary>
    /// A treeview that displays the system icons for paths.
    /// </summary>
    [Docking(DockingBehavior.Ask)]
    [Designer("System.Windows.Forms.Design.TreeViewDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [DesignTimeVisible(true)]
    public class PathTreeView : TreeViewWithSelection<TreeNode>
    {
        IAnkhServiceProvider _context;
        public PathTreeView()
        {
        }

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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

        }

        /// <summary>
        /// Set the icon for a given node based on its path.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        public void SetIcon(TreeNode node, string path)
        {
            if (IconMapper != null)
                node.SelectedImageIndex = node.ImageIndex = IconMapper.GetIcon(path);
            else
                node.SelectedImageIndex = node.ImageIndex = -1;
        }

        internal override TreeViewWithSelection<TreeNode>.TreeViewHierarchy CreateHierarchy()
        {
            return new PathHierarchy(this);
        }

        sealed class PathHierarchy : TreeViewHierarchy
        {
            public PathHierarchy(PathTreeView ptv)
                : base(ptv)
            {
            }
        }
    }
}
