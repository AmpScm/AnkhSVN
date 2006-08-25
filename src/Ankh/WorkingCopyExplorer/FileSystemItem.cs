using System;
using System.Text;
using Ankh.UI;
using System.IO;
using NSvn.Core;
using Utils;
using System.Collections;

namespace Ankh.WorkingCopyExplorer
{
    internal abstract class FileSystemItem : TreeNode, Ankh.UI.IFileSystemItem
    {
        public event ItemChangedEventHandler ItemChanged;

        public FileSystemItem( FileSystemItem parent, WorkingCopyExplorer explorer, SvnItem svnItem ) : base(parent)
        {
            this.explorer = explorer;
            this.svnItem = svnItem;

            this.svnItem.Changed += new EventHandler( this.ChildOrResourceChanged );

            this.CurrentStatus = MergeStatuses( this.svnItem );
        }

       

        public abstract bool IsContainer{ get; }

        public virtual string Text
        {
            get
            {
                return this.svnItem.Name;
            }
        }

        public SvnItem SvnItem
        {
            get { return this.svnItem; }
        }

        public WorkingCopyExplorer Explorer
        {
            get { return this.explorer; }
        }

        public override void GetResources( IList list, bool getChildItems, ResourceFilterCallback filter )
        {
            if ( filter( this.svnItem ) )
            {
                list.Add( this.svnItem );
            }
            if ( getChildItems )
            {
                this.GetChildResources( list, getChildItems, filter );
            }
        }

        


        public abstract IFileSystemItem[] GetChildren();

        public void Open()
        {
            try
            {
                this.explorer.OpenItem( this.SvnItem.Path );
            }
            catch ( Exception ex )
            {
                this.Explorer.Context.ErrorHandler.Handle( ex );
            }
        }
       
        [TextProperty( "TextStatus", Order = 0, TextWidth = 18 )]
        public StatusKind TextStatus
        {
            get { return this.svnItem.Status.TextStatus; }
        }

        [TextProperty( "PropertyStatus", Order = 1, TextWidth = 18 )]
        public StatusKind PropertyStatus
        {
            get { return this.svnItem.Status.PropertyStatus; }
        }

        [TextProperty("Locked", Order=2, TextWidth=10)]
        public string Locked
        {
            get
            {
                return this.svnItem.IsLocked ? "Yes" : "";
            }
        }

        [TextProperty( "Read-only", Order = 3, TextWidth = 10 )]
        public string ReadOnly
        {
            get
            {
                return this.svnItem.IsReadOnly ? "Yes" : "";
            }
        }

        [StateImageProperty]
        public int StateImage
        {
            get
            {
                try
                {
                    return StatusImages.GetStatusImageForNodeStatus( this.CurrentStatus );
                }
                catch ( Exception ex )
                {
                    this.explorer.Context.ErrorHandler.Handle( ex );
                    return 0;
                }
            }
        }


        public override bool Equals( object obj )
        {
            try
            {
                FileSystemItem other = obj as FileSystemItem;
                if ( other == null )
                {
                    return false;
                }
                return PathUtils.AreEqual( this.SvnItem.Path, other.SvnItem.Path );
            }
            catch ( Exception ex )
            {
                this.explorer.Context.ErrorHandler.Handle( ex );
                return false;
            }
        }

        public override int GetHashCode()
        {
            try
            {
                return this.svnItem.Path.GetHashCode();
            }
            catch ( Exception ex )
            {
                this.explorer.Context.ErrorHandler.Handle( ex );
                return 0;
            }
        }

        internal void Refresh(SvnClient client)
        {
            this.svnItem.Refresh(client);
        }

        public static FileSystemItem Create( WorkingCopyExplorer explorer, SvnItem item )
        {
            if ( item.IsDirectory )
            {
                return new FileSystemDirectoryItem( explorer, item );
            }
            else
            {
                return new FileSystemFileItem( explorer, item );
            }
        }

        protected override void DoDispose()
        {
            this.svnItem.Changed -= new EventHandler( this.ChildOrResourceChanged );
        }

        protected override bool RemoveTreeNodeIfResourcesDeleted()
        {
            // TODO: Implement
            return false;
        }

        protected override void CheckForSvnDeletions()
        {
            // TODO: Implement
        }

        protected override void SvnDelete()
        {
            // TODO: Implement
        }

        protected override NodeStatus ThisNodeStatus()
        {
            return MergeStatuses( this.svnItem );
        }

        protected virtual void OnItemChanged( ItemChangedType changeType )
        {
            if ( this.ItemChanged != null )
            {
                this.ItemChanged( this, new ItemChangedEventArgs(changeType) );
            }
        }

        protected override void OnChanged()
        {
            base.OnChanged();

            this.OnItemChanged( ItemChangedType.StatusChanged );
        }
        


        private WorkingCopyExplorer explorer;
        protected SvnItem svnItem;

        

    }
}
