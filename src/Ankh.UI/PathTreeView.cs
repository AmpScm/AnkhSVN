// $Id$
//
// Copyright 2004-2008 The AnkhSVN Project
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

// $Id$
using System;
using System.Windows.Forms;
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
        public virtual void SetIcon(TreeNode node, string path)
        {
            if (IconMapper != null)
                node.SelectedImageIndex = node.ImageIndex = IconMapper.GetIcon(path);
            else
                node.SelectedImageIndex = node.ImageIndex = -1;
        }
    }
}
