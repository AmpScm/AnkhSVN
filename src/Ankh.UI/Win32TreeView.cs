// $Id$
using System;
using Utils.Win32;

using ImageList = System.Windows.Forms.ImageList;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// Wraps a HWND to a Win32 treeview
    /// </summary>
    public class Win32TreeView : NativeWindow
    {
        public Win32TreeView( IntPtr hwnd )
        {
            this.CheckForZero( hwnd, "hwnd" );
            this.hwnd = hwnd;
        }

        /// <summary>
        /// Gets the root of the treeview.
        /// </summary>
        /// <returns></returns>
        public IntPtr GetRoot()
        {
            return (IntPtr)Win32.SendMessage( this.hwnd, Msg.TVM_GETNEXTITEM, 
                Constants.TVGN_ROOT, IntPtr.Zero );

        }

        /// <summary>
        /// Disables updates of this treeview window.
        /// </summary>
        /// <returns></returns>
        public void LockWindowUpdate()
        {
            Win32.LockWindowUpdate( this.hwnd );
        }

        /// <summary>
        /// Reenables updates of this treeview window.
        /// </summary>
        public void UnlockWindowUpdate()
        {
            Win32.LockWindowUpdate( IntPtr.Zero );
        }

        /// <summary>
        /// Gets the first child of the specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IntPtr GetChild( IntPtr item )
        {
            this.CheckForZero( item, "item" );
            return (IntPtr)Win32.SendMessage( this.hwnd, Msg.TVM_GETNEXTITEM,
                Constants.TVGN_CHILD, item );
        }

        /// <summary>
        /// Gets the next sibling of the specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IntPtr GetNextSibling( IntPtr item )
        {
            this.CheckForZero( item, "item" );
            return (IntPtr)Win32.SendMessage( this.hwnd, Msg.TVM_GETNEXTITEM, 
                Constants.TVGN_NEXT, item );
        }

        public bool RenameInProgress
        {
            get
            {
                int editControl = Win32.SendMessage( this.hwnd, Msg.TVM_GETEDITCONTROL, IntPtr.Zero, IntPtr.Zero );
                return editControl != 0;
            }
        }

        /// <summary>
        /// Whether status image list messages should be suppressed.
        /// </summary>
        public bool SuppressStatusImageChange
        {
            get 
            {
                return this.Handle != IntPtr.Zero;
            }
            set 
            {
                if ( this.Handle != IntPtr.Zero )
                {
                    this.ReleaseHandle();
                }

                if ( value )
                {                   
                    this.AssignHandle( this.hwnd ); 
                }
            }
        }



        /// <summary>
        /// Sets the imagelist to be used for the status images.
        /// </summary>
        public IntPtr StatusImageList
        {
            get
            {
                this.statusImageList = (IntPtr)Win32.SendMessage( this.hwnd, Msg.TVM_GETIMAGELIST,
                    Constants.TVSIL_STATE, IntPtr.Zero );
                return this.statusImageList;
            }
            set
            {
                this.statusImageList = value;
                Win32.SendMessage( this.hwnd, Msg.TVM_SETIMAGELIST, Constants.TVSIL_STATE,
                    value );
            }
        }

        /// <summary>
        /// Sets the imagelist to be used for the status images.
        /// </summary>
        public IntPtr ImageList
        {
            get
            {
                return (IntPtr)Win32.SendMessage( this.hwnd, Msg.TVM_GETIMAGELIST,
                    Constants.TVSIL_NORMAL, IntPtr.Zero );
            }
            set
            {
                Win32.SendMessage( this.hwnd, Msg.TVM_SETIMAGELIST, Constants.TVSIL_NORMAL,
                    value );
            }
        }

        /// <summary>
        /// Sets the status image for a specific item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="imageIndex"></param>
        public void SetStatusImage( IntPtr item, int imageIndex )
        {
            this.CheckForZero( item, "item" );

            TVITEMEX tvitem = new TVITEMEX();
            tvitem.mask = Constants.TVIF_STATE | Constants.TVIF_HANDLE;
            tvitem.hItem = item;
            // bits 12-15 indicate the state image
            tvitem.state = (uint)(imageIndex << 12);
            tvitem.stateMask = Constants.TVIS_STATEIMAGEMASK;

            Win32.SendMessage( this.hwnd, Msg.TVM_SETITEM, IntPtr.Zero, 
                tvitem ).ToInt32();
        }

        public void SetOverlayImage( IntPtr item, int imageIndex )
        {
            this.CheckForZero( item, "item" );

            TVITEMEX tvitem = new TVITEMEX();
            tvitem.mask = Constants.TVIF_STATE | Constants.TVIF_HANDLE;
            tvitem.hItem = item;
            // bits 8-11 indicate the state image
            tvitem.state = (uint)(imageIndex << 8);
            tvitem.stateMask = Constants.TVIS_OVERLAYMASK;

            Win32.SendMessage( this.hwnd, Msg.TVM_SETITEM, IntPtr.Zero, 
                tvitem ).ToInt32();
        }

        public void SetStateAndOverlayImage( IntPtr item, int overlay, int stateImage )
        {
            this.CheckForZero( item, "item" );

            TVITEMEX tvitem = new TVITEMEX();
            tvitem.mask = Constants.TVIF_STATE | Constants.TVIF_HANDLE;
            tvitem.hItem = item;
            // bits 8-11 indicate the state image
            tvitem.state = (uint)(overlay << 8) | (uint)(stateImage << 12);
            tvitem.stateMask = Constants.TVIS_OVERLAYMASK | Constants.TVIS_STATEIMAGEMASK;

            Win32.SendMessage( this.hwnd, Msg.TVM_SETITEM, IntPtr.Zero, 
                tvitem ).ToInt32();
        }

        /// <summary>
        /// Clears all status images.
        /// </summary>
        public void ClearStatusImages()
        {
            IntPtr item = this.GetRoot();
            this.RecursivelyClearStatusImages( item );
        }

        /// <summary>
        /// Use this to suppress the TVM_SETIMAGELIST message if it is for the
        /// status image list.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc( ref System.Windows.Forms.Message m )
        {
            switch ( m.Msg )
            {
                case (int)Msg.TVM_SETIMAGELIST:
                    if ( (uint)m.WParam != Constants.TVSIL_STATE)
                    {
                        base.WndProc( ref m );
                    }
                    break;
                default:
                    base.WndProc( ref m );
                    break;
            }
        }

        private void RecursivelyClearStatusImages( IntPtr item )
        {
            if ( item == IntPtr.Zero )
            {
                return;
            }
            // status image 0 is reserved for no status image at all
            this.SetStatusImage( item, 0 );

            IntPtr child = this.GetChild( item );
            while( child != IntPtr.Zero )
            {
                this.RecursivelyClearStatusImages( child );
                child = this.GetNextSibling( child );
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void CheckForZero( IntPtr ptr, string varname )
        {
            if ( ptr == IntPtr.Zero )
                throw new ArgumentNullException( varname, varname + " cannot be null" );
        }

        private IntPtr hwnd;
        private IntPtr statusImageList;
    }
}
