using System;
using System.Text;
using Ankh.UI;

namespace Ankh.WorkingCopyExplorer
{
    internal class FileSystemFileItem : FileSystemItem
    {
        public FileSystemFileItem( WorkingCopyExplorer explorer, SvnItem item )
            : base( null, explorer, item )
        {

        }

        public override bool IsContainer
        {
            get { return false; }
        }

        public override IFileSystemItem[] GetChildren()
        {
            try
            {
                throw new InvalidOperationException( "Cannot get children for leaf item." );
            }
            catch ( Exception ex )
            {
                this.Explorer.Context.ErrorHandler.Handle( ex );
                return new IFileSystemItem[] { };
            }
        }


    }
}
