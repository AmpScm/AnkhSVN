// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using System.IO;
using Ankh.Scc;
using Ankh.VS;

namespace Ankh.UI.WorkingCopyExplorer.Nodes
{
    abstract class WCFileSystemNode : WCTreeNode
    {
        readonly SvnItem _item;
        public SvnItem SvnItem { get { return _item; } }
        protected WCFileSystemNode(IAnkhServiceProvider context, WCTreeNode parent, SvnItem item)
            :base(context, parent)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _item = item;
        }

        ISvnStatusCache _statusCache;
        protected ISvnStatusCache StatusCache
        {
            get { return _statusCache ?? (_statusCache = Context.GetService<ISvnStatusCache>()); }
        }
 
        public override string Title
        {
            get { return string.IsNullOrEmpty(_item.Name) ? _item.FullPath : _item.Name; }
        }
    }

    class WCFileNode : WCFileSystemNode
    {
        public WCFileNode(IAnkhServiceProvider context, WCTreeNode parent, SvnItem item)
            : base(context, parent, item)
        {
        }

        public override bool IsContainer
        {
            get
            {
                return false;
            }
        }

        public override int ImageIndex
        {
            get
            {
                IFileIconMapper iconMap = Context.GetService<IFileIconMapper>();
                return iconMap.GetIcon(SvnItem.FullPath);
            }
        }

        protected override void RefreshCore(bool rescan)
        {
            if(SvnItem == null)
                return;

            if(rescan)
                StatusCache.MarkDirtyRecursive(SvnItem.FullPath);

            if (TreeNode != null)
                TreeNode.Refresh();
        }

        public override IEnumerable<WCTreeNode> GetChildren()
        {
            yield break;
        }

        internal override bool ContainsDescendant(string path)
        {
            return false;
        }
    }

    class WCDirectoryNode : WCFileSystemNode
    {
        public WCDirectoryNode(IAnkhServiceProvider context, WCTreeNode parent, SvnItem item)
            : base(context, parent, item)
        {
        }

        public override int ImageIndex
        {
            get 
            {
                IFileIconMapper iconMap = Context.GetService<IFileIconMapper>();
                return iconMap.GetIcon(SvnItem.FullPath);
            }
        }

        protected override void RefreshCore(bool rescan)
        {
            if (SvnItem == null)
                return;

            if (rescan)
                StatusCache.MarkDirtyRecursive(SvnItem.FullPath);

            if (TreeNode != null)
                TreeNode.Refresh();
        }

        public override IEnumerable<WCTreeNode> GetChildren()
        {
            foreach (SccFileSystemNode node in SccFileSystemNode.GetDirectoryNodes(SvnItem.FullPath))
            {
                if (node.IsHiddenOrSystem)
                    continue;

                if ((node.Attributes & FileAttributes.Offline) != 0)
                    continue;

                if (node.IsDirectory)
                    yield return new WCDirectoryNode(Context, this, StatusCache[node.FullPath]);
                else
                    yield return new WCFileNode(Context, this, StatusCache[node.FullPath]);
            }
        }

        internal override bool ContainsDescendant(string path)
        {
            return StatusCache[path].IsBelowPath(SvnItem);
        }
    }
}
