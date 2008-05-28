using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.VS;
using System.IO;

namespace Ankh.UI.RepositoryExplorer
{
    class RepositoryListItem : ListViewItem
    {
        readonly Uri _uri;
        SvnListEventArgs _info;

        public RepositoryListItem(SvnListEventArgs info, IFileIconMapper iconMapper)
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

            SubItems.Add(extension);
            SubItems.Add(info.Entry.Revision.ToString());
            SubItems.Add(info.Entry.Author);
            if (info.Entry.NodeKind == SvnNodeKind.File)
                SubItems.Add(info.Entry.FileSize.ToString());
            else
                SubItems.Add("");

            DateTime time = info.Entry.Time.ToLocalTime();

            SubItems.Add(time.ToShortDateString() + " " + time.ToShortTimeString());
        }

        public Uri RawUri
        {
            get { return _uri; }
        }
    }
}
