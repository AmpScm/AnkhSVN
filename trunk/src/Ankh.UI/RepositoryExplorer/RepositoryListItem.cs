// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using SharpSvn;
using Ankh.VS;
using System.IO;
using Ankh.UI.VSSelectionControls;
using Ankh.Scc;

namespace Ankh.UI.RepositoryExplorer
{
    class RepositoryListItem : SmartListViewItem
    {
        SvnDirEntry _entry;
        SvnOrigin _origin;

        public RepositoryListItem(RepositoryListView view, SharpSvn.Remote.ISvnRepositoryListItem listItem, SvnOrigin dirOrigin, IFileIconMapper iconMapper)
            : base(view)
        {
            if (listItem == null)
                throw new ArgumentNullException("listItem");
            else if (dirOrigin == null)
                throw new ArgumentNullException("dirOrigin");

            SvnDirEntry entry = listItem.Entry;
            Uri entryUri = listItem.Uri;

            _entry = entry;
            _origin = new SvnOrigin(entryUri, dirOrigin);

            string name = SvnTools.GetFileName(entryUri);

            bool isFile = (entry.NodeKind == SvnNodeKind.File);

            string extension = isFile ? Path.GetExtension(name) : "";

            if (iconMapper != null)
            {
                if (isFile)
                    ImageIndex = iconMapper.GetIconForExtension(extension);
                else
                {
                    ImageIndex = iconMapper.DirectoryIcon;
                }
            }

            SvnLockInfo lockInfo = null;
            SvnListEventArgs lea = listItem as SvnListEventArgs;
            if (lea != null)
                lockInfo = lea.Lock;

            SetValues(
                name,
                IsFolder ? RepositoryStrings.ExplorerDirectoryName : view.Context.GetService<IFileIconMapper>().GetFileType(extension),
                entry.Revision.ToString(),
                entry.Author,
                IsFolder ? "" : entry.FileSize.ToString(),
                entry.Time.ToLocalTime().ToString("g"),
                (lockInfo != null) ? lockInfo.Owner : "");
        }

        /// <summary>
        /// Gets the list view.
        /// </summary>
        /// <value>The list view.</value>
        protected internal new RepositoryListView ListView
        {
            get { return (RepositoryListView)base.ListView; }
        }

        /// <summary>
        /// Gets the raw URI.
        /// </summary>
        /// <value>The raw URI.</value>
        public Uri RawUri
        {
            get { return _origin.Uri; }
        }

        /// <summary>
        /// Gets the origin.
        /// </summary>
        /// <value>The origin.</value>
        public SvnOrigin Origin
        {
            get { return _origin; }
        }

        public SvnDirEntry Entry
        {
            get { return _entry; }
        }

        public bool IsFolder
        {
            get { return Entry.NodeKind == SvnNodeKind.Directory; }
        }
    }
}
