using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Ankh.UI.VSSelectionControls
{
    public class SmartTreeView : TreeView
    {
        ImageList _stateImageList;
        ImageList _realStateImageList;

        [DefaultValue(null)]
        public new ImageList StateImageList
        {
            get { return _stateImageList; }
            set
            {
                if (value != _stateImageList)
                {
                    if(_stateImageList != null)
                        _stateImageList.RecreateHandle -= new EventHandler(OnStateImageList_RecreateHandle);

                    _stateImageList = value;
                    _stateImageList.RecreateHandle += new EventHandler(OnStateImageList_RecreateHandle);

                    if (IsHandleCreated)
                        SetStateList();
                }
            }
        }

        void OnStateImageList_RecreateHandle(object sender, EventArgs e)
        {
            SetStateList();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_stateImageList != null)
                    {
                        _stateImageList.RecreateHandle -= new EventHandler(OnStateImageList_RecreateHandle);
                        _stateImageList = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (_stateImageList != null)
                SetStateList();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (_realStateImageList != null)
            {
                _realStateImageList.Dispose();
                _realStateImageList = null;
            }
        }

        private void SetStateList()
        {
            if (_stateImageList == null)
                return;

            if (_realStateImageList != null)
            {
                _realStateImageList.Dispose();
                _realStateImageList = null;
            }

            _realStateImageList = new ImageList();
            _realStateImageList.ImageSize = _stateImageList.ImageSize;
            
            if (_stateImageList.Images.Count > 0)
            {
                Image[] list = new Image[_stateImageList.Images.Count+1];
                int n = 0;
                list[n++] = _stateImageList.Images[0];

                foreach(Image i in _stateImageList.Images)
                    list[n++] = i;

                _realStateImageList.Images.AddRange(list);

                NativeMethods.SendMessage(Handle, 0x1109, (IntPtr)2, _realStateImageList.Handle);

                n = 0;
                foreach(Image i in list)
                {
                    if(n++ > 0)
                        i.Dispose(); // Release temporary bitmaps
                }
            }
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ExtendSelection(e.Location, true);
            }

            base.OnMouseDown(e);
        }

        protected virtual void ExtendSelection(Point p, bool rightClick)
        {
            TreeViewHitTestInfo hi = HitTest(p);

            bool onItem = hi.Node != null && hi.Location != TreeViewHitTestLocations.None;

            if (rightClick)
            {
                if (hi.Node != SelectedNode)
                {
                    SelectedNode = hi.Node;
                }
            }
        }

        static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        }
    }
}
