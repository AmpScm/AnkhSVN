using System;
using System.Text;

namespace Ankh.Tools
{
    public class TreeListRootItemCollection : TreeListItemCollection
    {
        public TreeListRootItemCollection( TreeList treeList )
        {
            this.treeList = treeList;
        }

        protected override void OnInsertComplete( int index, object value )
        {
            
            this.treeList.BaseItems.Insert( index, (TreeListItem)value );
        }

        protected override void OnRemoveComplete( int index, object value )
        {
            this.treeList.BaseItems.Remove( (TreeListItem)value );
        }

        private TreeList treeList;
	
    }
    

}
