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
using Ankh.UI.VSSelectionControls;
using Ankh.VS;

namespace Ankh.UI.PendingChanges.Conflicts
{
    class ConflictListView : ListViewWithSelection<ConflictListItem>
    {
        IAnkhServiceProvider _context;

        public ConflictListView()
        {
            Initialize();
        }

        void Initialize()
        {
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
                SelectionPublishServiceProvider = value;
                if (value != null)
                {
                    IFileIconMapper mapper = value.GetService<IFileIconMapper>();
                    SmallImageList = mapper.ImageList;
                }
            }
        }

        protected override string GetCanonicalName(ConflictListItem item)
        {
            return item.PendingChange.FullPath;
        }
    }
}
