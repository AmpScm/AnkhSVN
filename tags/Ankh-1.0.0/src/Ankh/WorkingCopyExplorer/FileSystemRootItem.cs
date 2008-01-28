using System;
using System.Text;

namespace Ankh.WorkingCopyExplorer
{
    class FileSystemRootItem : FileSystemDirectoryItem
    {
        public FileSystemRootItem( WorkingCopyExplorer explorer, SvnItem svnItem ) : base(explorer, svnItem)
        {
            
        }

        public override string Text
        {
            get
            {
                return this.svnItem.Path;
            }
        }
    }
}
