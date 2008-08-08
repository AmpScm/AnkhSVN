﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.VS;
using System.IO;
using Ankh.UI.VSSelectionControls;

namespace Ankh.UI.RepositoryExplorer
{
    class RepositoryListItem : SmartListViewItem
    {
        readonly Uri _uri;
        SvnListEventArgs _info;

        public RepositoryListItem(RepositoryListView view, SvnListEventArgs info, IFileIconMapper iconMapper)
            : base(view)
        {
            _info = info;
            _uri = info.EntryUri;

            string name = RepositoryTreeView.UriItemName(info.EntryUri);
            Text = name;

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
                info.Entry.Time.ToString("g"));
        }

        public Uri RawUri
        {
            get { return _uri; }
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
