using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.WorkingCopyExplorer
{
    class FileSystemTreeNode : TreeNode
    {
        readonly SvnItem _svnItem;

        public FileSystemTreeNode(SvnItem item)
        {
            if(item == null)
                throw new ArgumentNullException("item");

            _svnItem = item;
            Text = item.Name;
        }

        public FileSystemTreeNode(string text)
            : base(text)
        {
        }

        public SvnItem SvnItem
        {
            get { return _svnItem; }
        }
    }
}
