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
using SharpSvn;
using Ankh.VS;
using System.IO;
using Ankh.UI.VSSelectionControls;
using Ankh.Scc;

namespace Ankh.UI.RepositoryExplorer
{
    class RepositoryListItem : SmartListViewItem
    {
        SvnListEventArgs _info;
        SvnOrigin _origin;

        public RepositoryListItem(RepositoryListView view, SvnListEventArgs info, SvnOrigin dirOrigin, IFileIconMapper iconMapper)
            : base(view)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            else if (dirOrigin == null)
                throw new ArgumentNullException("dirOrigin");

            _info = info;
            _origin = new SvnOrigin(info.EntryUri, dirOrigin);

            string name = SvnTools.GetFileName(info.EntryUri);

            bool isFile = (info.Entry.NodeKind == SvnNodeKind.File);

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

            SetValues(
                name,
                extension,
                info.Entry.Revision.ToString(),
                info.Entry.Author,
                IsFolder ? "" : info.Entry.FileSize.ToString(),
                info.Entry.Time.ToLocalTime().ToString("g"),
                (info.Lock != null) ? info.Lock.Owner : "");
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

        public SvnListEventArgs Info
        {
            get { return _info; }
        }

        public bool IsFolder
        {
            get { return Info.Entry.NodeKind == SvnNodeKind.Directory; }
        }
    }
}
