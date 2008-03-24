// $Id$
using System;
using Utils.Win32;

using ImageList = System.Windows.Forms.ImageList;
using System.Windows.Forms;
using System.Diagnostics;

namespace Ankh.UI
{
    /// <summary>
    /// Wraps a HWND to a Win32 treeview
    /// </summary>
    public class Win32TreeView : NativeWindow
    {
        readonly IntPtr _hwnd;
        IntPtr _statusImageList;

        public Win32TreeView(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                throw new ArgumentNullException("hwnd");

            _hwnd = hwnd;
        }

        public bool IsValid
        {
            get { return true; }
        }

        public bool RenameInProgress
        {
            get
            {
                IntPtr editControl = Win32.SendMessage(_hwnd, Msg.TVM_GETEDITCONTROL, IntPtr.Zero, IntPtr.Zero);
                return editControl != IntPtr.Zero;
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
                if ((this.Handle != IntPtr.Zero) == value)
                    return;

                if (value)
                    this.AssignHandle(_hwnd);
                else
                    this.ReleaseHandle();
            }
        }

        /// <summary>
        /// Sets the imagelist to be used for the status images.
        /// </summary>
        public IntPtr StatusImageList
        {
            get
            {
                _statusImageList = (IntPtr)Win32.SendMessage(_hwnd, Msg.TVM_GETIMAGELIST,
                    Constants.TVSIL_STATE, IntPtr.Zero);
                return _statusImageList;
            }
            set
            {
                _statusImageList = value;
                Win32.SendMessage(_hwnd, Msg.TVM_SETIMAGELIST, Constants.TVSIL_STATE,
                    value);
            }
        }

        /// <summary>
        /// Sets the imagelist to be used for the status images.
        /// </summary>
        public IntPtr ImageList
        {
            get
            {
                return (IntPtr)Win32.SendMessage(_hwnd, Msg.TVM_GETIMAGELIST,
                    Constants.TVSIL_NORMAL, IntPtr.Zero);
            }
            set
            {
                Win32.SendMessage(_hwnd, Msg.TVM_SETIMAGELIST, Constants.TVSIL_NORMAL,
                    value);
            }
        }

        /// <summary>
        /// Use this to suppress the TVM_SETIMAGELIST message if it is for the
        /// status image list.
        /// </summary>
        /// <param name="m"></param>
        [DebuggerNonUserCode]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == (int)Msg.TVM_SETIMAGELIST && m.WParam == Constants.TVSIL_STATE)
            {
                return;
            }

            // When you see a crash here; it probably is not in AnkhSVN itself
            // but at somebody else handling events raised from the solution explorer
            base.WndProc(ref m);
        }
    }
}
