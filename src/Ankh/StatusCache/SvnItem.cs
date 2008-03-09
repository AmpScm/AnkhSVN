// $Id$
using System;


using System.IO;
using System.Collections;
using Ankh.UI;
using System.Diagnostics;
using SharpSvn;
using System.Collections.ObjectModel;

namespace Ankh
{
    /// <summary>
    /// Used to decide whether this particular SvnItem should be included in a collection.
    /// </summary>
    public delegate bool ResourceFilterCallback( SvnItem item );

    public enum EventBehavior
    {
        Raise,
        DontRaise,
    }

    /// <summary>
    /// Represents a version controlled path on disk, caching it's status.
    /// </summary>
    public class SvnItem
    {
        /// <summary>
        /// Fired when the status of this item changes.
        /// </summary>
        public event EventHandler Changed;

        public event EventHandler ChildrenChanged;

        public SvnItem(string path, AnkhStatus status)
        {
            this.path = path;
            this.status = status;
        }

        /// <summary>
        /// The status of this item.
        /// </summary>
        public AnkhStatus Status
        {
            get{ return this.status; }
        }

        /// <summary>
        /// The path of this item.
        /// </summary>
        public string Path
        {
            get{ return this.path; }
        }

        public string Name
        {
            get 
            {
                if ( this.Path.EndsWith("/") )
                {
                    return System.IO.Path.GetFileName( this.Path.Substring( 0, this.Path.Length - 1 ) );
                }
                else
                {
                    return System.IO.Path.GetFileName( this.Path );
                }
            }
        }

        /// <summary>
        /// Set the status of this item to the passed in status.
        /// </summary>
        /// <param name="status"></param>
        public virtual void Refresh( AnkhStatus status )
        {
            this.Refresh( status, EventBehavior.Raise );
        }


        /// <summary>
        /// Set the status of this item to the passed in status.
        /// </summary>
        /// <param name="status"></param>
        public virtual void Refresh(AnkhStatus status, EventBehavior eventBehavior)
        {
            AnkhStatus oldStatus = this.status;
            this.status = status;

            if (eventBehavior == EventBehavior.Raise)
            {
                if (!oldStatus.Equals(status))
                    this.OnChanged();
            }
        }

        /// <summary>
        /// Refresh the existing status of the item, using client.
        /// </summary>
        /// <param name="client"></param>
        public virtual void Refresh( SvnClient client )
        {
            this.Refresh( client, EventBehavior.Raise );
        }

        /// <summary>
        /// Refresh the existing status of the item, using client.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="eventBehavior">Whether to raise events.</param>
        public virtual void Refresh(SvnClient client, EventBehavior eventBehavior)
        {
            AnkhStatus oldStatus = this.status;
            Collection<SvnStatusEventArgs> statuses;
            SvnStatusArgs args = new SvnStatusArgs();
            args.Depth = SvnDepth.Empty;
            args.RetrieveAllEntries = true;
            args.ThrowOnError = false;
            if (client.GetStatus(path, args, out statuses))
                this.status = statuses.Count > 0 ? statuses[0] : AnkhStatus.None;
            else
                this.status = AnkhStatus.None;

            if (eventBehavior == EventBehavior.Raise)
            {
                if (!oldStatus.Equals(this.status))
                    this.OnChanged();
            }
        }

        /// <summary>
        /// Is this item versioned?
        /// </summary>
        public virtual bool IsVersioned
        {
            get
            {
                SvnStatus s = this.status.LocalContentStatus;
                return s == SvnStatus.Added ||
                       s == SvnStatus.Conflicted ||
                       s == SvnStatus.Merged ||
                       s == SvnStatus.Modified ||
                       s == SvnStatus.Normal ||
                       s == SvnStatus.Replaced ||
                       s == SvnStatus.Deleted ||
                    //s == StatusKind.Missing ||
                       s == SvnStatus.Incomplete;
            }
        }

        /// <summary>
        /// Is this resource modified(implies that it is versioned)?
        /// </summary>
        public virtual bool IsModified
        {
            get
            {
                SvnStatus t = this.status.LocalContentStatus;
                SvnStatus p = this.status.LocalPropertyStatus;
                return this.IsVersioned &&
                    (t != SvnStatus.Normal ||
                      (p != SvnStatus.None && p != SvnStatus.Normal));
            }
        }

        /// <summary>
        /// Is this item a directory?
        /// </summary>
        public virtual bool IsDirectory
        {
            get
            { 
                if ( this.status.WorkingCopyInfo != null )
                    return this.status.WorkingCopyInfo.NodeKind == SvnNodeKind.Directory;
                else
                    return Directory.Exists( this.path );
            }
        }

        /// <summary>
        /// Is this item a file?
        /// </summary>
        public virtual bool IsFile
        {
            get
            {
                if (this.status.WorkingCopyInfo != null)
                    return this.status.WorkingCopyInfo.NodeKind == SvnNodeKind.File;
                else
                    return File.Exists(this.path);
            }
        }

        /// <summary>
        /// Whether the item is potentially versionable.
        /// </summary>
        public virtual bool IsVersionable
        {
            get { return SvnTools.IsBelowManagedPath(path); }
        }

        /// <summary>
        /// Whether the item is read only on disk.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get
            {
                try
                {
                    return this.IsFile && File.Exists( this.Path ) &&
                        (File.GetAttributes( this.Path ) & FileAttributes.ReadOnly) != 0;
                }
                catch( IOException )
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Whether the item is locked
        /// </summary>
        /// <returns></returns>
        public virtual bool IsLocked
        {
            get
            {
                return this.Status.LocalLocked;
            }
        }


        public virtual bool IsDeleted
        {
            get { return this.status.LocalContentStatus == SvnStatus.Deleted; }
        }

        public virtual bool IsDeletedFromDisk
        {
            get { return this.status.LocalContentStatus == SvnStatus.None && !File.Exists(this.Path) && !Directory.Exists(this.Path); }
        }

        public override string ToString()
        {
            return this.path;
        }

        public void NotifyChildrenChanged()
        {
            if ( this.ChildrenChanged != null )
            {
                this.ChildrenChanged( this, EventArgs.Empty );
            }
        }


        /// <summary>
        /// Retrieves the paths from an IList of SvnItem instances.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string[] GetPaths( System.Collections.IList items )
        {
            string[] paths = new string[items.Count];
            int i = 0;
            foreach ( SvnItem item in items )
            {
                Debug.Assert( item != null, "SvnItem should not be null" );

                if ( item != null )
                {
                    paths[ i++ ] = item.Path; 
                }
            }

            return paths;
        }

        /// <summary>
        /// Filters a list of SvnItem instances using the provided callback.
        /// </summary>
        /// <param name="items">An IList containing SvnItem instances.</param>
        /// <param name="callback">A callback to be used to determine whether 
        /// an item should be included in the returned list.</param>
        /// <returns>A new IList of SvnItem instances.</returns>
        public static IList Filter( IList items, ResourceFilterCallback callback )
        {
            ArrayList list = new ArrayList( items.Count );
            foreach( SvnItem item in items )
            {
                if ( callback( item ) )
                    list.Add( item );
            }

            return list;
        }


        protected virtual void OnChanged()
        {
            if ( this.Changed != null )
                this.Changed( this, EventArgs.Empty );
        }

        private class UnversionableItem : SvnItem
        {
            public UnversionableItem() : base( "", AnkhStatus.Unversioned )
            {}

            protected override void OnChanged()
            {
                // empty
            }

            public override void Refresh(SvnClient client)
            {
                // empty
            }

            public override void Refresh(AnkhStatus status)
            {
                // empty
            }

            public override bool IsDirectory
            {
                get
                {
                    return false;
                }
            }

            public override bool IsFile
            {
                get
                {
                    return false;
                }
            }

            public override bool IsModified
            {
                get
                {
                    return false;
                }
            }

            public override bool IsVersioned
            {
                get
                {
                    return false;
                }
            }

            public override bool IsVersionable
            {
                get
                {
                    return false;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public override bool IsDeleted
            {
                get
                {
                    return false;
                }
            }

            public override bool IsDeletedFromDisk
            {
                get
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Represents an unversionable item.
        /// </summary>
        public static readonly SvnItem Unversionable = new UnversionableItem();

        /// <summary>
        /// A ResourceFilterCallback method that filters for modified items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool ModifiedFilter( SvnItem item )
        {
            return item.IsModified;
        }

        /// <summary>
        /// A ResourceFilterCallback that filters for versioned directories.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool DirectoryFilter( SvnItem item )
        {
            return item.IsVersioned && item.IsDirectory;
        }

        public static bool VersionedFilter( SvnItem item )
        {
            return item.IsVersioned;
        }

        public static bool UnversionedFilter( SvnItem item )
        {
            return !item.IsVersioned;
        }

        public static bool UnmodifiedSingleFileFilter( SvnItem item )
        {
            return item.IsVersioned && !item.IsModified && item.IsFile;
        }

        public static bool UnmodifiedItemFilter( SvnItem item )
        {
            return item.IsVersioned && !item.IsModified;
        }

        public static bool VersionedSingleFileFilter( SvnItem item )
        {
            return item.IsVersioned && item.IsFile;
        }

        public static bool LockedFilter( SvnItem item )
        {
            return item.IsLocked;
        }

        public static bool NotLockedAndLockableFilter( SvnItem item )
        {
            return item.IsVersioned && !item.IsLocked && item.IsFile;
        }

        public static bool ExistingOnDiskFilter( SvnItem item )
        {
            return !item.IsDeletedFromDisk;
        }

        public static bool NotDeletedFilter( SvnItem item )
        {
            return !item.IsDeleted && !item.IsDeletedFromDisk;
        }

        public static bool NoFilter( SvnItem item )
        {
            return true;
        }


        public static void GetPathInfo( object sender, ResolvingPathEventArgs args )
        {
            SvnItem item = (SvnItem)args.Item;
            args.IsDirectory = item.IsDirectory;
            args.Path = item.Path;
        }

        /// <summary>
        /// Filter for conflicted items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool ConflictedFilter( SvnItem item )
        {
            return item.Status.LocalContentStatus == SvnStatus.Conflicted;
        }




        private AnkhStatus status;
        private string path;
    }
}
