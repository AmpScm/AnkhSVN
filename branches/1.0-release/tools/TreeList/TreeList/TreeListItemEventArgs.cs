using System;
using System.Text;

namespace Ankh.Tools
{
    public class TreeListItemEventArgs : EventArgs
    {
        public TreeListItemEventArgs( TreeListItem item )
        {
            this.item = item;
        }

        public TreeListItem Item
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return item; }
        }

        private TreeListItem item;
    }

    public class TreeListItemsChangedEventArgs : TreeListItemEventArgs
    {
        public TreeListItemsChangedEventArgs( TreeListItem item, int index ) : base(item)
        {
            this.index = index;
        }

        private int index;

        public int Index
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return index; }
        }

    }


    public delegate void TreeListItemEventHandler(object sender, TreeListItemEventArgs args);
    public delegate void TreeListItemsChangedEventHandler( object sender, TreeListItemsChangedEventArgs args );
}
