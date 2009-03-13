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
using Ankh.UI.VSSelectionControls;
using System.IO;
using System.Diagnostics;
using Ankh.VS;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemListViewItem : SmartListViewItem
    {
        readonly SvnItem _svnItem;

        public FileSystemListViewItem(SmartListView view, SvnItem item)
            : base(view)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            
            _svnItem = item;

            ImageIndex = View.IconMapper.GetIcon(item.FullPath);

            RefreshValues();
        }

        FileSystemDetailsView View
        {
            get { return base.ListView as FileSystemDetailsView; }
        }

        public SvnItem SvnItem
        {
            [DebuggerStepThrough]
            get { return _svnItem; }
        }
    
        private void RefreshValues()
        {
            bool exists = SvnItem.Exists;
            string name = string.IsNullOrEmpty(SvnItem.Name) ? SvnItem.FullPath : SvnItem.Name;

            SetValues(
                name,
                Modified.ToString("g"),
                View.Context.GetService<IFileIconMapper>().GetFileType(SvnItem),
                SvnItem.Status.LocalContentStatus.ToString(),
                SvnItem.Status.LocalPropertyStatus.ToString(),
                SvnItem.Status.IsLockedLocal ? Ankh.UI.PendingChanges.PCStrings.LockedValue : "",
                SvnItem.Status.Revision.ToString(),
                SvnItem.Status.LastChangeTime.ToLocalTime().ToString(),
                SvnItem.Status.LastChangeRevision.ToString(),
                SvnItem.Status.LastChangeAuthor,
                SvnItem.Status.IsCopied.ToString(),
                SvnItem.IsConflicted.ToString(),
                SvnItem.FullPath
                );

            StateImageIndex = (int)View.StatusMapper.GetStatusImageForSvnItem(SvnItem);
        }

        internal bool IsDirectory
        {
            get { return SvnItem.IsDirectory; }
        }

        internal DateTime Modified
        {
            get { return SvnItem.Modified; }

        }
    }
}
