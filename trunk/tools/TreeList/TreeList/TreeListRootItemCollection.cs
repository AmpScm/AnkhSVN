using System;
using System.Text;

namespace TreeList
{
    public class TreeListRootItemCollection : TreeListItemCollection
    {
        public TreeListRootItemCollection( TreeList treeList )
        {
            this.treeList = treeList;
        }

        protected override void OnInsertComplete( int index, object value )
        {
            this.treeList.Items.Insert( index, (TreeListItem)value );
        }

        protected override void OnRemoveComplete( int index, object value )
        {
            this.treeList.Items.Remove( (TreeListItem)value );
        }

        private TreeList treeList;
	
    }
}
