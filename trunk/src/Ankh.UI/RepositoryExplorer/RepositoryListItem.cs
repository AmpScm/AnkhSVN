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

        public RepositoryListItem(RepositoryListView view, SvnListEventArgs info, IFileIconMapper iconMapper)
            : base(view)
        {
            _info = info;
            _origin = new SvnOrigin(new SvnUriTarget(info.EntryUri, info.Entry.Revision), info.RepositoryRoot);

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
