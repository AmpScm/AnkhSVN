using System;
using System.Text;
using System.Collections;

namespace Ankh.Tools
{
    public class TreeListItemCollection : CollectionBase
    {
        public event TreeListItemsChangedEventHandler ItemRemoved;
        public event TreeListItemsChangedEventHandler ItemInserted;

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
            //this.OnChanged();
        }

        protected override void OnInsertComplete( int index, object value )
        {
            if ( this.ItemInserted != null )
            {
                this.ItemInserted( this, new TreeListItemsChangedEventArgs((TreeListItem)value, index) );
            }
        }

        protected override void OnRemoveComplete( int index, object value )
        {
            if ( this.ItemRemoved != null )
            {
                this.ItemRemoved( this, new TreeListItemsChangedEventArgs((TreeListItem)value, index ));
            }
        }
    }
}
