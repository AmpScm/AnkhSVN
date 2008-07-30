using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemListViewItem : ListViewItem
    {
        readonly SvnItem _svnItem;

        public FileSystemListViewItem(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            _svnItem = item;

            Text = item.Name;
        }

        public SvnItem SvnItem
        {
            get { return _svnItem; }
        }
    }
}
