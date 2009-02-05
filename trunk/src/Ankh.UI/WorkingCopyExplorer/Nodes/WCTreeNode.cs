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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ankh.UI.WorkingCopyExplorer.Nodes
{
    abstract class WCTreeNode
    {
        readonly IAnkhServiceProvider _context;
        readonly Collection<FileSystemNode> _children;
        WCTreeNode _parent;

        public WCTreeNode(IAnkhServiceProvider context, WCTreeNode parent)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _parent = parent;
            _children = new Collection<FileSystemNode>();
        }

        /// <summary>
        /// Gets the index of the image.
        /// </summary>
        /// <value>The index of the image.</value>
        public abstract int ImageIndex
        {
            get;
        }

        /// <summary>
        /// Child nodes of this node
        /// </summary>
        public Collection<FileSystemNode> Children
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _children; }
        }

        /// <summary>
        /// The parent node of this node.
        /// </summary>
        public WCTreeNode Parent
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _parent; }
        }

        /// <summary>
        /// Derived classes implement this method to append their resources
        /// to the list.
        /// </summary>
        /// <param name="list"></param>
        public abstract void GetResources(Collection<SvnItem> list, bool getChildItems,
            Predicate<SvnItem> filter);

        public void Refresh()
        {
            this.Refresh(true);
        }

        public abstract void Refresh(bool rescan);

        protected void GetChildResources(Collection<SvnItem> list, bool getChildItems,
            Predicate<SvnItem> filter)
        {
            if (getChildItems)
            {
                foreach (WCTreeNode node in Children)
                    node.GetResources(list, getChildItems, filter);
            }
        }

        protected void FilterResources(IList<SvnItem> inList, IList outList, Predicate<SvnItem> filter)
        {
            foreach (SvnItem item in inList)
            {
                if (filter == null || filter(item))
                {
                    outList.Add(item);
                }
            }
        }
    }
}
