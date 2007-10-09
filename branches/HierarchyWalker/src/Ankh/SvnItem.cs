// $Id$
using System;
using NSvn.Core;
using NSvn.Common;
using System.IO;
using System.Collections;
using Ankh.UI;
using System.Diagnostics;

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

        public SvnItem( string path, Status status )
        {
            this.path = path;
            this.status = status;
        }

        /// <summary>
        /// The status of this item.
        /// </summary>
        public Status Status
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
        public virtual void Refresh( Status status )
        {
            this.Refresh( status, EventBehavior.Raise );
        }


        /// <summary>
        /// Set the status of this item to the passed in status.
        /// </summary>
        /// <param name="status"></param>
        public virtual void Refresh( Status status, EventBehavior eventBehavior )
        {
            Status oldStatus = this.status;
            this.status = status;

            if ( eventBehavior == EventBehavior.Raise )
            {
                if ( !oldStatus.Equals( this.status ) )
                    this.OnChanged(); 
            }
        }

        /// <summary>
        /// Refresh the existing status of the item, using client.
        /// </summary>
        /// <param name="client"></param>
        public virtual void Refresh( Client client )
        {
            this.Refresh( client, EventBehavior.Raise );
        }

        /// <summary>
        /// Refresh the existing status of the item, using client.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="eventBehavior">Whether to raise events.</param>
        public virtual void Refresh( Client client, EventBehavior eventBehavior )
        {
            Status oldStatus = this.status;
            this.status = client.SingleStatus( this.path );

            if ( eventBehavior == EventBehavior.Raise )
            {
                if ( !oldStatus.Equals( this.status ) )
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
                StatusKind s = this.status.TextStatus;
                return s == StatusKind.Added ||
                       s == StatusKind.Conflicted ||
                       s == StatusKind.Merged ||
                       s == StatusKind.Modified ||
                       s == StatusKind.Normal ||
                       s == StatusKind.Replaced ||
                       s == StatusKind.Deleted ||
                       //s == StatusKind.Missing ||
                       s == StatusKind.Incomplete;
            }
        }

        /// <summary>
        /// Is this resource modified(implies that it is versioned)?
        /// </summary>
        public virtual bool IsModified
        {
            get
            {
                StatusKind t = this.status.TextStatus;
                StatusKind p = this.status.PropertyStatus;
                return this.IsVersioned &&
                    ( t != StatusKind.Normal ||
                      (p != StatusKind.None && p != StatusKind.Normal));
            }
        }

        /// <summary>
        /// Is this item a directory?
        /// </summary>
        public virtual bool IsDirectory
        {
            get
            { 
                if ( this.status.Entry != null )
                    return this.status.Entry.Kind == NodeKind.Directory;
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
                if ( this.status.Entry != null )
                    return this.status.Entry.Kind == NodeKind.File;
                else 
                    return File.Exists( this.path );
            }
        }

        /// <summary>
        /// Whether the item is potentially versionable.
        /// </summary>
        public virtual bool IsVersionable
        {
            get
            {
                string dir;
                if ( this.IsDirectory )
                {
                    // find the parent dir
                    if ( this.path.EndsWith( System.IO.Path.DirectorySeparatorChar.ToString() ) )
                        dir = this.path.Substring(0, this.path.Length-2); 
                    else
                        dir = this.path;
                    dir = System.IO.Path.GetDirectoryName( dir );
                }
                else
                    // find the containing dir
                    dir = System.IO.Path.GetDirectoryName( this.path );

                return Directory.Exists( 
                    System.IO.Path.Combine( dir, Client.AdminDirectoryName ));
            }
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
                return this.Status.Entry != null &&
                    this.Status.Entry.LockToken != null;
            }
        }


        public virtual bool IsDeleted
        {
            get { return this.status.TextStatus == StatusKind.Deleted; }
        }

        public virtual bool IsDeletedFromDisk
        {
            get { return this.status.TextStatus == StatusKind.None && !File.Exists( this.Path ) && !Directory.Exists( this.Path ); }
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
            public UnversionableItem() : base( "", Status.None )
            {}

            protected override void OnChanged()
            {
                // empty
            }

            public override void Refresh(Client client)
            {
                // empty
            }

            public override void Refresh(Status status)
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
            return item.Status.TextStatus == StatusKind.Conflicted;
        }
 



        private Status status;
        private string path;

#if DEBUG
        internal int ScanForOrphanedTreeNodes( TextWriter writer )
        {
            if ( this.Changed == null)
            {
                return 0;
            }

            int numOrphans = 0;

            foreach ( Delegate del in this.Changed.GetInvocationList() )
            {
                Ankh.Solution.SolutionExplorerTreeNode treeNode = del.Target as Ankh.Solution.SolutionExplorerTreeNode;
                if ( treeNode == null )
                {
                    continue;
                }

                if ( treeNode.IsOrphaned() )
                {
                    numOrphans++;
                    
                }
            }

            if ( numOrphans > 0 )
            {
                writer.WriteLine( "Found {0} orphaned node{1} for item {2}", 
                    numOrphans,
                    numOrphans == 1 ? "" : "s",
                    this.Path );
            }

            return numOrphans;
        }
#endif
    }
}
