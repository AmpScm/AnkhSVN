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
using System.Collections.ObjectModel;

namespace Ankh.UI.WorkingCopyExplorer.Nodes
{
    internal abstract class FileSystemNode : WCTreeNode
    {
        readonly SvnItem _item;
        readonly WorkingCopyExplorerControl _ctrl;

        public FileSystemNode(WorkingCopyExplorerControl control, FileSystemNode parent, SvnItem svnItem)
            : base(control.Context, parent)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            else if (svnItem == null)
                throw new ArgumentNullException("svnItem");

            _item = svnItem;
            _ctrl = control;
        }

        public override string Title
        {
            get { return SvnItem.Name; }
        }
        
        protected IAnkhServiceProvider Context
        {
            get { return _ctrl.Context; }
        }

        protected WorkingCopyExplorerControl Control
        {
            get { return _ctrl; }
        }

        public abstract bool IsContainer { get; }

        public virtual string Text
        {
            get
            {
                return _item.Name;
            }
        }

        public SvnItem SvnItem
        {
            get { return _item; }
        }

        public override void GetResources(Collection<SvnItem> list, bool getChildItems, Predicate<SvnItem> filter)
        {
            if (filter(_item))
            {
                list.Add(_item);
            }
            if (getChildItems)
            {
                this.GetChildResources(list, getChildItems, filter);
            }
        }




        

        public void Open(IAnkhServiceProvider context)
        {
            Control.OpenItem(context, SvnItem.FullPath);
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as FileSystemNode);
        }

        public bool Equals(FileSystemNode other)
        {
            if (other == null)
                return false;

            return String.Equals(this.SvnItem.FullPath, other.SvnItem.FullPath, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return _item.FullPath.GetHashCode();
        }

        public static FileSystemNode Create(WorkingCopyExplorerControl control, SvnItem item)
        {
            if (item.IsDirectory)
            {
                return new FileSystemDirectoryItem(control, item);
            }
            else
            {
                return new FileSystemFileItem(control, item);
            }
        }
    }
}
