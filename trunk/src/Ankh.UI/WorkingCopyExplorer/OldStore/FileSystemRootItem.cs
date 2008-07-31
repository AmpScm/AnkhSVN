using System;
using System.Text;
using Ankh.UI.WorkingCopyExplorer;

namespace Ankh.WorkingCopyExplorer
{
    class FileSystemRootItem : FileSystemDirectoryItem
    {
        public FileSystemRootItem(WorkingCopyExplorerControl control, SvnItem svnItem)
            : base(control, svnItem)
        {

        }

        public override string Text
        {
            get
            {
                return this.SvnItem.FullPath;
            }
        }
    }
}
