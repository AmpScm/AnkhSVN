using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace TreeList
{
    [Designer(typeof(TreeListDesigner))]
    public class TreeList : ListView
    {
        public TreeList()
        {
            this.InitializeComponent();
            this.View = View.Details;
            this.treeListItems =  new TreeListRootItemCollection(this);
        }

        public TreeListItemCollection TreeListItems
        {
            get { return this.treeListItems; }
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

        private ImageList imageList;
        private IContainer components;

        private TreeListRootItemCollection treeListItems;

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
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }

        internal void ContractItem( TreeListItem treeListItem )
        {
            try
            {
                this.BeginUpdate();
                foreach ( TreeListItem child in treeListItem.Children )
                {
                    child.Expanded = false;

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

    }
}
