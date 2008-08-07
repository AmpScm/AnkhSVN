using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.VSSelectionControls;
using System.IO;
using System.Diagnostics;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemListViewItem : SmartListViewItem
    {
        readonly SvnItem _svnItem;
        readonly FileSystemInfo _info;

        public FileSystemListViewItem(SmartListView view, SvnItem item, FileSystemInfo info)
            : base(view)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            
            _svnItem = item;
            _info = info;

            ImageIndex = (info is DirectoryInfo) ? View.IconMapper.DirectoryIcon : View.IconMapper.GetIcon(item.FullPath);

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

        public FileSystemInfo FSInfo
        {
            [DebuggerStepThrough]
            get { return _info; }
        }

        private void RefreshValues()
        {
            bool exists = FSInfo.Exists;
            SetValues(
                SvnItem.Name,
                Modified.ToString("g"),
                SvnItem.Extension,
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
            get { return FSInfo is DirectoryInfo; }
        }

        internal DateTime Modified
        {
            get { return FSInfo.LastWriteTime; }

        }
    }
}
