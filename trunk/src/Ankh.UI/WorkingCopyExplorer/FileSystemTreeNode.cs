// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Text;
using System.Windows.Forms;
using Ankh.UI.WorkingCopyExplorer.Nodes;
using System.IO;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemTreeNode : TreeNode
    {
        readonly WCTreeNode _wcNode;
        readonly SvnItem _item;
        public FileSystemTreeNode(WCTreeNode wcNode, SvnItem item)
        {
            if (wcNode == null)
                throw new ArgumentNullException("wcNode");

            _wcNode = wcNode;
            Text = wcNode.Title;
            _item = item;

            wcNode.TreeNode = this;
        }

        public FileSystemTreeNode(WCTreeNode wcNode)
            :this(wcNode, null)
        {
        }

        public FileSystemTreeNode(string text)
            : base(text)
        {
        }

        public new FileSystemTreeView TreeView
        {
            get { return (FileSystemTreeView)base.TreeView; }
        }

        public WCTreeNode WCNode
        {
            get { return _wcNode; }
        }

        public SvnItem SvnItem
        {
            get
            {
                WCFileSystemNode dirNode = _wcNode as WCFileSystemNode;
                if (_item == null && dirNode != null)
                    return dirNode.SvnItem;
                return _item; 
            }
        }

        public void Refresh()
        {
            if(SvnItem == null)
                return;

            if (SvnItem.IsDirectory)
            {
                try
                {
                    Directory.GetDirectories(SvnItem.FullPath);
                }
                catch(UnauthorizedAccessException)
                {
                    return;
                }
                catch(IOException)
                {
                    return;
                }
            }

            StateImageIndex = (int)TreeView.StatusMapper.GetStatusImageForSvnItem(SvnItem);
        }

        internal void SelectSubNode(string path)
        {
            Expand();

            foreach (FileSystemTreeNode tn in Nodes)
            {
                if (tn.WCNode.ContainsDescendant(path))
                    tn.SelectSubNode(path);
            }
        }
    }
}
