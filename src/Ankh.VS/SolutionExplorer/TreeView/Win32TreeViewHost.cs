using System;
using System.Text;
using System.Windows.Forms;
using Utils.Win32;
using System.Runtime.InteropServices;

namespace Ankh.UI
{
	[CLSCompliant(false)]
    public class ItemExpandedEventArgs : EventArgs
    {
        public ItemExpandedEventArgs( uint hItem, TreeViewAction action )
        {
            this.hItem = hItem;
            this.action = action;
        }

        public uint HItem
        {
            get { return hItem; }
            set { hItem = value; }
        }

        public System.Windows.Forms.TreeViewAction Action
        {
            get { return this.action; }
        }


        private uint hItem;
        private TreeViewAction action;
    }

    /// <summary>
    /// Used to wrap the host window for a treeview, so we can intercept WM_NOTIFY messages.
    /// </summary>
    public class Win32TreeViewHost : NativeWindow
    {
        [CLSCompliant(false)]
        public event EventHandler<ItemExpandedEventArgs> ItemExpanded;

        public Win32TreeViewHost( IntPtr hWnd )
        {
            this.AssignHandle( hWnd );
        }

        protected override void WndProc( ref System.Windows.Forms.Message m )
        {
            switch ( m.Msg )
            {
                case Msg.WM_NOTIFY:
                    this.WmNotify( m );
                    base.WndProc( ref m );
                    break;
                default:
                    base.WndProc( ref m );
                    break;
            }
        }

        private void WmNotify( System.Windows.Forms.Message m )
        {
            NMTREEVIEW nmtv = (NMTREEVIEW)Marshal.PtrToStructure(m.LParam, typeof(NMTREEVIEW));
            switch ( nmtv.nmhdr.code )
            {
                case unchecked( (int)Msg.TVN_ITEMEXPANDEDA ):
                case unchecked( (int)Msg.TVN_ITEMEXPANDEDW ):
                    this.OnItemExpanded( new ItemExpandedEventArgs(
                        (uint)nmtv.itemNew.hItem.ToInt32(),
                        ( nmtv.itemNew.state & Constants.TVIS_EXPANDED ) != 0 ?
                            TreeViewAction.Expand :
                            TreeViewAction.Collapse
                        ) );
                    break;
                default:
                    break;
            }
        }

        [CLSCompliant(false)]
        protected virtual void OnItemExpanded( ItemExpandedEventArgs args )
        {
            if ( this.ItemExpanded != null && args.Action == TreeViewAction.Expand )
            {
                this.ItemExpanded( this, args );
            }
        }
    }
}
