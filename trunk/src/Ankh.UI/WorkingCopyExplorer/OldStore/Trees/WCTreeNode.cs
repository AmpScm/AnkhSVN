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
using System.Text;
using System.Collections;

using SharpSvn;

namespace Ankh
{
    public abstract class WCTreeNode
    {
        public WCTreeNode(WCTreeNode parent)
        {
            this.parent = parent;
            this.children = new ArrayList();
        }  

        /// <summary>
        /// Child nodes of this node
        /// </summary>
        public IList Children
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.children; }
        }

        /// <summary>
        /// The parent node of this node.
        /// </summary>
        public WCTreeNode Parent
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.parent; }
        }

        /// <summary>
        /// Derived classes implement this method to append their resources
        /// to the list.
        /// </summary>
        /// <param name="list"></param>
        public abstract void GetResources(IList list, bool getChildItems,
            Predicate<SvnItem> filter);

        public void Refresh()
        {
            this.Refresh(true);
        }

        public abstract void Refresh(bool rescan);

        protected void GetChildResources(System.Collections.IList list, bool getChildItems,
            Predicate<SvnItem> filter)
        {
            if (getChildItems)
            {
                foreach (WCTreeNode node in this.Children)
                    node.GetResources(list, getChildItems, filter);
            }
        }

        protected void FilterResources(IList inList, IList outList, Predicate<SvnItem> filter)
        {
            foreach (SvnItem item in inList)
            {
                if (filter == null || filter(item))
                {
                    outList.Add(item);
                }
            }
        }

        private WCTreeNode parent;
        private IList children;
    }
}
