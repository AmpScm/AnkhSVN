using System;
using System.Text;

namespace Ankh.WorkingCopyExplorer
{
    class FileSystemRootItem : FileSystemDirectoryItem
    {
        public FileSystemRootItem(IAnkhServiceProvider context, WorkingCopyExplorer explorer, SvnItem svnItem ) 
            : base(context, explorer, svnItem)
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
