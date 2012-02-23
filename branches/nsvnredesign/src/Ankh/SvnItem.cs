// $Id$
using System;
using NSvn.Core;

namespace Ankh
{

    internal delegate void StatusChanged( object sender, EventArgs e );

    
    /// <summary>
    /// Used to decide whether this particular SvnItem should be included in a collection.
    /// </summary>
    internal delegate bool ResourceFilterCallback( SvnItem item );

    /// <summary>
    /// Represents a version controlled path on disk, caching it's status.
    /// </summary>
    internal class SvnItem
    {
        /// <summary>
        /// Fired when the status of this item changes.
        /// </summary>
        public event StatusChanged Changed;

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


        /// <summary>
        /// Set the status of this item to the passed in status.
        /// </summary>
        /// <param name="status"></param>
        public virtual void Refresh( Status status )
        {
            Status oldStatus = this.status;
            this.status = status;

            if ( !oldStatus.Equals( this.status ) )
                this.OnChanged();
        }

        /// <summary>
        /// Refresh the existing status of the item, using client.
        /// </summary>
        /// <param name="client"></param>
        public virtual void Refresh( Client client )
        {
            Status oldStatus = this.status;
            this.status = client.SingleStatus( this.path );

            if ( !oldStatus.Equals( this.status ) )
                this.OnChanged();
        }

        /// <summary>
        /// Is this item versioned?
        /// </summary>
        public virtual bool IsVersioned
        {
            get
            { 
                return this.status.TextStatus != StatusKind.None &&
                    this.status.TextStatus != StatusKind.Unversioned;
            }
        }

        /// <summary>
        /// Is this resource modified(implies that it is versioned)?
        /// </summary>
        public virtual bool IsModified
        {
            get
            {
                return (this.Status.TextStatus != StatusKind.Normal &&
                  this.Status.TextStatus != StatusKind.Unversioned &&
                  this.status.TextStatus != StatusKind.None) ||
                (this.Status.PropertyStatus != StatusKind.Normal &&
                this.Status.PropertyStatus != StatusKind.None);
            }
        }

        /// <summary>
        /// Is this item a directory?
        /// </summary>
        public virtual bool IsDirectory
        {
            get{ return this.status.Entry.Kind == NodeKind.Directory; }
        }

        /// <summary>
        /// Is this item a file?
        /// </summary>
        public virtual bool IsFile
        {
            get{ return this.status.Entry.Kind == NodeKind.File; }
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
            foreach( SvnItem item in items )
                paths[i++] = item.Path;

            return paths;
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




        }

        /// <summary>
        /// Represents an unversionable item.
        /// </summary>
        public static readonly SvnItem Unversionable = new UnversionableItem();



        private Status status;
        private string path;
    }
}