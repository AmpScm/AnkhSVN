using System;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace TreeList
{
    public class TreeListItem : ListViewItem
    {
        public TreeListItem( string text ) : base(text)
        {
            this.children.Changed += new EventHandler( children_Changed );
        }

        

        public TreeListItem()
        {
            // empty
        }

        public TreeListItemCollection Children
        {
            get { return this.children; }
        }

        private TreeList TreeList
        {
            get { return (TreeList)this.ListView; }
        }


        public bool Expanded
        {
            get 
            { 
                return this.expanded; 
            }
            set 
            {
                if ( this.expanded != value )
                {
                    if ( value )
                    {
                        this.TreeList.ExpandItem( this );
                    }
                    else
                    {
                        this.TreeList.ContractItem( this );
                    }

                }
                expanded = value;
                this.UpdateImage();
            }
        }

        //public TreeList TreeList
        //{
        //    get { return (TreeList)this.ListView; }
        //}

        public int Level
        {
            get { return this.level; }
            set 
            { 
                this.level = value;
                this.SetIndent( this.Index, this.level );
            }
        }

        internal void SetIndent( int index, int indent )
        {
            
            this.TreeList.EnsureHandleCreated();

            Win32.LVITEM lvItem = new Win32.LVITEM();
            lvItem.mask = Win32.ListViewConstants.LVIF_INDENT;
            lvItem.iItem = index;
            lvItem.iIndent = indent;
            int err = Win32.Functions.SendMessage( this.ListView.Handle, Win32.ListViewConstants.LVM_SETITEMA, 0, out lvItem );
        }

        internal bool StateImageHitTest( Point point )
        {
            Win32.LVHITTESTINFO info = new Win32.LVHITTESTINFO();
            info.pt.x = point.X;
            info.pt.y = point.Y;

            Win32.Functions.SendMessage( this.ListView.Handle, Win32.ListViewConstants.LVM_SUBITEMHITTEST, 0, out info );
            return info.flags == Win32.ListViewConstants.LVHT_ONITEMICON;
        }

        void children_Changed( object sender, EventArgs e )
        {
            UpdateImage();
        }

        private void UpdateImage()
        {
            if ( this.Children.Count > 0 )
            {
                this.ImageIndex = this.Expanded ? ImageIndexes.Expanded : ImageIndexes.Contracted;
            }
        }
        

        private TreeListItemCollection children = new TreeListItemCollection();
        private int level;
        private bool expanded;

        
    }
}
