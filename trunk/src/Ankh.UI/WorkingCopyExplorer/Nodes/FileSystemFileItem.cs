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

namespace Ankh.UI.WorkingCopyExplorer.Nodes
{
    internal class FileSystemFileItem : FileSystemNode
    {
        public FileSystemFileItem(WorkingCopyExplorerControl control, SvnItem item)
            : base(control, null, item)
        {

        }

        public override bool IsContainer
        {
            get { return false; }
        }

        public override FileSystemNode[] GetChildren()
        {
            return new FileSystemNode[] { };
        }

        public override void Refresh(bool rescan)
        {
            if (rescan)
            {
                this.SvnItem.MarkDirty();
            }
        }

        public override int ImageIndex
        {
            get { throw new NotImplementedException(); }
        }
    }
}
