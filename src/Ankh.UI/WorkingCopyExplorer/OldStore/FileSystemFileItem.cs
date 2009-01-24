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
using Ankh.UI;
using Ankh.UI.WorkingCopyExplorer;

namespace Ankh.WorkingCopyExplorer
{
    internal class FileSystemFileItem : FileSystemItem
    {
        public FileSystemFileItem(WorkingCopyExplorerControl control, SvnItem item)
            : base(control, null, item)
        {

        }

        public override bool IsContainer
        {
            get { return false; }
        }

        public override FileSystemItem[] GetChildren()
        {
            return new FileSystemItem[] { };
        }

        public override void Refresh(bool rescan)
        {
            if (rescan)
            {
                this.SvnItem.MarkDirty();
            }
        }
    }
}
