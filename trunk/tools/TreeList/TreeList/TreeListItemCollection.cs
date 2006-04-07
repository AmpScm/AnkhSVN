using System;
using System.Text;
using System.Collections;

namespace TreeList
{
    public class TreeListItemCollection : CollectionBase
    {
        public event EventHandler Changed;

        public TreeListItem this[ int index ]
        {
            get { return (TreeListItem)this.List[ index ]; }
            set { this.List[ index ] = value; }
        }

        public int Add( TreeListItem item )
        {
            return this.List.Add( item );
        }

        public int IndexOf( TreeListItem item )
        {
            return this.List.IndexOf( item );
        }

        public void Insert( int index, TreeListItem item )
        {
            this.List.Insert( index, item );
        }

        public void Remove( TreeListItem item )
        {
            this.List.Remove( item );
        }

        public bool Contains( TreeListItem item )
        {
            return this.List.Contains( item );
        }

        protected override void OnClearComplete()
        {
            this.OnChanged();
        }

        protected override void OnInsertComplete( int index, object value )
        {
            this.OnChanged();
        }

        protected override void OnRemoveComplete( int index, object value )
        {
            this.OnChanged();
        }

        private void OnChanged()
        {
            if ( this.Changed != null )
            {
                this.Changed( this, EventArgs.Empty );
            }

        }
    }
}
