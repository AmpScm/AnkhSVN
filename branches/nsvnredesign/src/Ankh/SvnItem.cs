// $Id$
using System;
using NSvn.Core;

namespace Ankh
{

    internal delegate void StatusChanged( object sender, EventArgs e );

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
        }

        /// <summary>
        /// Represents an unversionable item.
        /// </summary>
        public static readonly SvnItem Unversionable = new UnversionableItem();



        private Status status;
        private string path;
    }
}
