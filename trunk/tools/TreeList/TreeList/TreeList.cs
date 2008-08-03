using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Ankh.Tools
{
    [Designer(typeof(TreeListDesigner))]
    public class TreeList : ListView
    {

        public event TreeListItemEventHandler BeforeCollapse;
        public event TreeListItemEventHandler AfterCollapse;
        public event TreeListItemEventHandler BeforeExpand;
        public event TreeListItemEventHandler AfterExpand;

        public TreeList()
        {
            this.InitializeComponent();
            this.View = View.Details;
            this.treeListItems =  new TreeListItemCollection();

            this.treeListItems.ItemInserted += new TreeListItemsChangedEventHandler( treeListItems_ItemInserted );
            this.treeListItems.ItemRemoved += new TreeListItemsChangedEventHandler( treeListItems_ItemRemoved );
        }

        

        public new TreeListItemCollection Items
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.treeListItems; }
        }

        internal ListViewItemCollection BaseItems
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return base.Items; }
        }

        

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( TreeList ) );
            this.imageList = new System.Windows.Forms.ImageList( this.components );
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream = ( (System.Windows.Forms.ImageListStreamer)( resources.GetObject( "imageList.ImageStream" ) ) );
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // TreeList
            // 
            this.SmallImageList = this.imageList;
            this.ResumeLayout( false );

        }

        protected override void OnMouseDown( MouseEventArgs e )
        {
            base.OnMouseDown( e );

            if ( e.Button != MouseButtons.Left )
            {
                return;
            }

            TreeListItem item = this.GetItemAt( e.X, e.Y ) as TreeListItem;

            if (item != null)
	        {
                if ( e.Clicks == 2 || 
                    ( e.Clicks == 1 && item.StateImageHitTest( new Point( e.X, e.Y ) ) ) )
                {
                    item.Expanded = !item.Expanded;
                }
            }
        }

        

        internal void ExpandItem( TreeListItem treeListItem )
        {
            try
            {
                this.BeginUpdate();
                int index = base.Items.IndexOf( treeListItem );
                foreach ( TreeListItem child in treeListItem.Children )
                {
                    base.Items.Insert( ++index, child );
                    child.Level = treeListItem.Level + 1;

                    if ( child.Expanded )
                    {
                        this.ExpandItem( child );
                    }
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }

        internal void CollapseItem( TreeListItem treeListItem )
        {
            try
            {
                this.BeginUpdate();
                foreach ( TreeListItem child in treeListItem.Children )
                {
                    if ( child.Expanded )
                    {
                        CollapseItem( child ); 
                    }

                    int index = base.Items.IndexOf( child );
                    base.Items.RemoveAt( index  );
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }

        internal void EnsureHandleCreated()
        {
            if ( !this.IsHandleCreated )
            {
                this.CreateHandle();
            }
        }

        internal void treeListItems_ItemRemoved( object sender, TreeListItemsChangedEventArgs args )
        {
            if ( args.Item.Expanded )
            {
                this.CollapseItem( args.Item );
            }
            this.BaseItems.Remove( args.Item );
        }

        internal void treeListItems_ItemInserted( object sender, TreeListItemsChangedEventArgs args )
        {
            int index;
            // Optimize for the very common case of inserting at the end.
            if ( args.Index == (this.Items.Count - 1) )
            {
                index = this.BaseItems.Count;
            }
            else
            {
                index = this.FindIndexForInsertion( args.Index, args.Item ); 
            }
            this.BaseItems.Insert( index, args.Item );
        }

        internal virtual void OnAfterCollapse( TreeListItem treeListItem )
        {
            if ( this.AfterCollapse != null )
            {
                this.AfterCollapse( this, new TreeListItemEventArgs(treeListItem) );
            }
        }

        internal virtual void OnBeforeCollapse( TreeListItem treeListItem )
        {
            if ( this.BeforeCollapse != null )
            {
                this.BeforeCollapse( this, new TreeListItemEventArgs(treeListItem) );
            }

        }

        internal virtual void OnAfterExpand( TreeListItem treeListItem )
        {
            if ( this.AfterExpand != null )
            {
                this.AfterExpand( this, new TreeListItemEventArgs(treeListItem) );
            }
        }

        internal virtual void OnBeforeExpand( TreeListItem treeListItem )
        {
            if ( this.BeforeExpand != null )
            {
                this.BeforeExpand( this, new TreeListItemEventArgs(treeListItem) );
            }
        }

        private int FindIndexForInsertion( int index, TreeListItem treeListItem )
        {
            if ( index == 0 )
            {
                return 0;
            }

            // find the index of the top level treelistitem in the underlying collection
            int baseItemIndex = this.BaseItems.IndexOf( this.Items[index - 1]) + 1;


            if ( baseItemIndex == this.BaseItems.Count )
            {
                return baseItemIndex;
            }

            //if ( baseItemIndex == this.Items.Count )
            //{
            //    return this.BaseItems.Count;
            //}

            // go through subitems to find the index *after* the top level node and all its children
            TreeListItem baseItem; 
            do
            {
                baseItem = (TreeListItem)this.BaseItems[baseItemIndex];
                if ( baseItem.Level == 0 )
                {
                    break;
                }
                baseItemIndex++;
            } while (baseItemIndex < this.BaseItems.Count);
            return baseItemIndex;
        }

        private ImageList imageList;
        private IContainer components;

        private TreeListItemCollection treeListItems;


        
    }
}
