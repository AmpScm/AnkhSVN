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
using Ankh.VS;
using Ankh.Scc;

namespace Ankh.UI.WorkingCopyExplorer.Nodes
{
    class WCMyComputerNode : WCTreeNode
    {
        readonly int _imageIndex;
        public WCMyComputerNode(IAnkhServiceProvider context)
            : base(context, null)
        {
            _imageIndex = context.GetService<IFileIconMapper>().GetSpecialFolderIcon(WindowsSpecialFolder.MyComputer);
        }

        public override string Title
        {
            get { return "My Computer"; }
        }

        public override void GetResources(System.Collections.ObjectModel.Collection<SvnItem> list, bool getChildItems, Predicate<SvnItem> filter)
        {
        }

        public override IEnumerable<WCTreeNode> GetChildren()
        {
            IFileStatusCache cache = Context.GetService<IFileStatusCache>();

            foreach (string s in Environment.GetLogicalDrives())
                yield return new WCDirectoryNode(Context, this, cache[s]);
        }

        public override void Refresh(bool rescan)
        {
        }

        public override int ImageIndex
        {
            get { return _imageIndex; }
        }
    }
}
