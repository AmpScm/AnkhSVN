// $Id$
//
// Copyright 2008 The AnkhSVN Project
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

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemTreeNode : TreeNode
    {
        readonly SvnItem _svnItem;
        readonly WCTreeNode _wcNode;

        public FileSystemTreeNode(SvnItem item, WCTreeNode wcNode)
        {
            if(item == null)
                throw new ArgumentNullException("item");

            _svnItem = item;
            _wcNode = wcNode;
            Text = item.Name;
        }

        public FileSystemTreeNode(string text)
            : base(text)
        {
        }

        public SvnItem SvnItem
        {
            get { return _svnItem; }
        }

        public new FileSystemTreeView TreeView
        {
            get { return (FileSystemTreeView)base.TreeView; }
        }

        public WCTreeNode WCNode
        {
            get { return _wcNode; }
        }
    }
}
