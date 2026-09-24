// Copyright 2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Ankh.Commands;

namespace Ankh.UI.VSSelectionControls
{
    public class SmartTreeView : TreeView, ISupportsVSTheming
    {
        ImageList _stateImageList;
        ImageList _realStateImageList;
        bool _showLines;

        /// <summary>
        /// Initializes a new SmartTreeView
        /// </summary>
        public SmartTreeView()
        {
            ShowLines = true; // Matches default value, but updates VS2010+ behavior
        }

        public SmartTreeView(IContainer container)
        {
            if (container != null)
                container.Add(this);

            if (!DesignMode)
                ShowLines = false;
        }

        [DefaultValue(true)]
        public new bool ShowLines
        {
            get { return _showLines; }
            set
            {
                _showLines = value;
                base.ShowLines = value && false /*VSVersion.VS2008OrOlder*/;
            }
        }

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

        private const int TV_FIRST = 0x1100;
        private const int TVM_SETBKCOLOR = TV_FIRST + 29;
        private const int TVM_SETTEXTCOLOR = TV_FIRST + 30;
        private const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        private const int TVM_GETEXTENDEDSTYLE = TV_FIRST + 45;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (_stateImageList != null)
                SetStateList();

            if (SmartListView.IsXPPlus)
            {
                if (_useDarkNativeTheme)
                {
                    NativeMethods.SetWindowTheme(Handle, "DarkMode_Explorer", null);
                }
                else if (!_inVSTheming)
                    NativeMethods.SetWindowTheme(Handle, "Explorer", null);

                uint flags = (uint)NativeMethods.SendMessage(Handle, TVM_GETEXTENDEDSTYLE, IntPtr.Zero, IntPtr.Zero);

                flags |= 0x0004; // TVS_EX_DOUBLEBUFFER

                NativeMethods.SendMessage(Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)flags, (IntPtr)flags);
            }

            // SetWindowTheme() can reset the native TreeView colors even though
            // the managed BackColor/ForeColor still contain the VS palette.
            // Reapply them immediately, then once more after the current WinForms/VS
            // theming pass unwinds. IVsUIShell6.ThemeWindow/SetFixedThemeColors may
            // update the native TreeView after OnHandleCreated returns, which otherwise
            // leaves repository-browser trees with a white Windows background.
            RestoreNativeColors(this);

            IntPtr createdHandle = Handle;
            BeginInvoke((MethodInvoker)delegate
            {
                if (!IsDisposed && IsHandleCreated && Handle == createdHandle)
                    RestoreNativeColors(this);
            });
        }

        internal static void RestoreNativeColors(TreeView treeView)
        {
            if (treeView == null)
                throw new ArgumentNullException("treeView");

            if (!treeView.IsHandleCreated)
                return;

            NativeMethods.SendMessage(
                treeView.Handle,
                TVM_SETBKCOLOR,
                IntPtr.Zero,
                (IntPtr)ColorTranslator.ToWin32(treeView.BackColor));
            NativeMethods.SendMessage(
                treeView.Handle,
                TVM_SETTEXTCOLOR,
                IntPtr.Zero,
                (IntPtr)ColorTranslator.ToWin32(treeView.ForeColor));
            treeView.Invalidate();
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

        public System.Drawing.Point GetSelectionPoint()
        {
            if (this.SelectedNode != null)
            {
                int offset = this.SelectedNode.Bounds.Height / 3;
                return this.PointToScreen(new Point(this.SelectedNode.Bounds.X + offset,
                    this.SelectedNode.Bounds.Y + offset));
            }
            else
            {
                return Point.Empty;
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
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Right)
            {
                ExtendSelection(e.Location, true);
            }
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

        bool _inVSTheming;
        bool _useDarkNativeTheme;

        void ISupportsVSTheming.OnThemeChange(IAnkhServiceProvider sender, CancelEventArgs e)
        {
            _inVSTheming = !e.Cancel;

            IWinFormsThemingService themer = sender.GetService<IWinFormsThemingService>();
            AnkhThemePalette palette = themer != null ? themer.ThemePalette : null;
            bool darkSurface = palette != null
                ? palette.IsDarkSurface
                : AnkhThemePalette.IsDark(BackColor);

            _useDarkNativeTheme = SmartTreeViewThemeLogic.ShouldUseDarkNativeTheme(
                _inVSTheming,
                darkSurface,
                SystemInformation.HighContrast);

            if (palette != null)
            {
                BackColor = palette.SurfaceBackground;
                ForeColor = palette.SurfaceForeground;
            }
            else if (_inVSTheming && Parent != null)
            {
                BackColor = Parent.BackColor;
                ForeColor = Parent.ForeColor;
            }

            RecreateHandle();
        }

        static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
            public static extern int SetWindowTheme(IntPtr hWnd, String pszSubAppName, String pszSubIdList);
        }
    }
}
