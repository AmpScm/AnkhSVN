using System;
using System.Text;
using System.Collections;
using Ankh.UI;
using SharpSvn;

namespace Ankh.WorkingCopyExplorer
{
    internal class FileSystemDirectoryItem : FileSystemItem
    {
        public FileSystemDirectoryItem( WorkingCopyExplorer explorer, SvnItem item )
            : base( null, explorer, item )
        {
            this.FindChildren();
            this.Refresh( false );
        }

        public override bool IsContainer
        {
            get { return true; }
        }

        public override Ankh.UI.IFileSystemItem[] GetChildren()
        {
            try
            {
                IFileSystemItem[] children = new IFileSystemItem[ this.Children.Count ];
                this.Children.CopyTo( children, 0 );
                return children;
            }
            catch ( Exception ex )
            {
                this.Explorer.Context.ErrorHandler.Handle( ex );
                return new IFileSystemItem[] { };
            }
        }

        public override void Refresh( bool rescan )
        {
            if ( rescan )
            {
                this.Explorer.Context.StatusCache.Status(this.svnItem.Path, SvnDepth.Infinity);
                this.FindChildren();

                this.OnItemChanged( ItemChangedType.ChildrenInvalidated );
            }
            this.CurrentStatus = MergeStatuses( this.CheckChildStatuses(), this.ThisNodeStatus() );
        }


        private void FindChildren()
        {
            this.Children.Clear();

            if ( this.SvnItem.IsVersioned )
            {
                foreach( FileSystemItem item in this.Explorer.GetFileSystemItemsForDirectory( this.SvnItem ))
                {
                    this.Children.Add(item);
                }
            }
        }
    }
}
