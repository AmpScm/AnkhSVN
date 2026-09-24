// Copyright 2008-2009 The AnkhSVN Project
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Ankh.Commands;

namespace Ankh.UI.VSSelectionControls
{
    public enum SortIcon
    {
        None,
        Ascending,
        Descending
    }

    public class SmartListView : ListView, ISupportsVSTheming
    {
        readonly Collection<SmartColumn> _groupColumns = new Collection<SmartColumn>();
        readonly Collection<SmartColumn> _sortColumns = new Collection<SmartColumn>();
        readonly Collection<SmartColumn> _allColumns = new Collection<SmartColumn>();
        readonly SortedList<SmartGroup, ListViewGroup> _groups;
        ISmartValueComparer _topSorter;
        ISmartValueComparer _finalSorter;
        bool _autoSizeRightColumn;
        bool _selectAllCheckBox;
        bool _selectAllChecked;

        public SmartListView()
        {
            View = View.Details;
            FullRowSelect = true;
            this.ListViewItemSorter = new SmartListSorter(this);
            _groups = new SortedList<SmartGroup, ListViewGroup>(new SmartGroupSorter(this));
            Sorting = SortOrder.Ascending;
            base.UseCompatibleStateImageBehavior = false;
        }

        [DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool UseCompatibleStateImageBehavior
        {
            get { return base.UseCompatibleStateImageBehavior; }
            set { base.UseCompatibleStateImageBehavior = value; }
        }

        [DefaultValue(false)]
        public bool ShowSelectAllCheckBox
        {
            get { return _selectAllCheckBox; }
            set
            {
                if (value == _selectAllCheckBox)
                    return;

                _selectAllCheckBox = value;
                UpdateSortGlyphs();
            }
        }

        [DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual bool SelectAllChecked
        {
            get { return _selectAllChecked; }
            set
            {
                if (value == _selectAllChecked)
                    return;

                _selectAllChecked = value;
                if (IsHandleCreated && ShowSelectAllCheckBox)
                    UpdateSortGlyphs();
            }
        }

        /// <summary>
        /// Gets or sets how items are displayed in the control.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Windows.Forms.View"/> values. The default is <see cref="F:System.Windows.Forms.View.LargeIcon"/>.</returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value specified is not one of the <see cref="T:System.Windows.Forms.View"/> values. </exception>
        [DefaultValue(View.Details)]
        public new View View
        {
            get { return base.View; }
            set { base.View = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether clicking an item selects all its subitems.
        /// </summary>
        /// <value></value>
        /// <returns>true if clicking an item selects the item and all its subitems; false if clicking an item selects only the item itself. The default is false.</returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(true)]
        public new bool FullRowSelect
        {
            get { return base.FullRowSelect; }
            set { base.FullRowSelect = value; }
        }

        /// <summary>
        /// Gets or sets the sort order for items in the control.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Windows.Forms.SortOrder"/> values. The default is <see cref="F:System.Windows.Forms.SortOrder.None"/>.</returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The value specified is not one of the <see cref="T:System.Windows.Forms.SortOrder"/> values. </exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(SortOrder.Ascending)]
        public new SortOrder Sorting
        {
            get { return base.Sorting; }
            set { base.Sorting = value; }
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
            ListViewHitTestInfo hi = HitTest(p);

            // Use indexes to be compatible with the virtual mode users of this class!

            bool onItem = hi.Item != null && hi.Location != ListViewHitTestLocations.None;

            if (rightClick)
            {
                // We try to replicate the right click behavior of the Windows Explorer in this method

                if (onItem)
                {
                    if (!hi.Item.Selected)
                    {
                        // If the clicked item is not selected, make the item the only selected item
                        SelectedIndices.Clear();
                        hi.Item.Selected = true;
                    }

                    // Always set focus to the clicked item
                    hi.Item.Focused = true;
                }
                else if ((ModifierKeys & (Keys.Shift | Keys.Control | Keys.Alt)) == 0)
                {
                    // Only clear the selection if no modifier key is pressed
                    if (SelectedIndices.Count > 0)
                        SelectedIndices.Clear();
                }
            }
            //else
            //    throw new NotImplementedException();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Collection<SmartColumn> GroupColumns
        {
            get { return _groupColumns; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Collection<SmartColumn> AllColumns
        {
            get { return _allColumns; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Collection<SmartColumn> SortColumns
        {
            get { return _sortColumns; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ISmartValueComparer TopSortColumn
        {
            get { return _topSorter; }
            set { _topSorter = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ISmartValueComparer FinalSortColumn
        {
            get { return _finalSorter; }
            set { _finalSorter = value; }
        }

        #region SortIcons
        static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct HDITEM
            {
                public Int32 mask;
                public Int32 cxy;
                [MarshalAs(UnmanagedType.LPTStr)]
                public String pszText;
                public IntPtr hbm;
                public Int32 cchTextMax;
                public Int32 fmt;
                public IntPtr lParam;
                public Int32 iImage;
                public Int32 iOrder;
                public Int32 type;
                public IntPtr pvFilter;
            };

            [StructLayout(LayoutKind.Sequential)]
            public struct LVITEM
            {
                public int mask;
                public int iItem;
                public int iSubItem;
                public int state;
                public int stateMask;
                public string pszText;
                public int cchTextMax;
                public int iImage;
                public IntPtr lParam;
                public int iIndent;
                public int iGroupId;
                public int cColumns;
                public IntPtr puColumns;
            }

            public const Int32 GWL_STYLE = -16;

            // Parameters for ListView-Headers
            public const Int32 HDI_FORMAT = 0x0004;
            public const Int32 HDF_LEFT = 0x0000;
            public const Int32 HDF_STRING = 0x4000;
            public const Int32 HDF_SORTUP = 0x0400;
            public const Int32 HDF_SORTDOWN = 0x0200;
            public const Int32 HDF_CHECKBOX = 0x0040;
            public const Int32 HDF_CHECKED = 0x0080;

            public const Int32 HDS_CHECKBOXES = 0x0400;
            public const Int32 HDS_NOSIZING = 0x0800;
            public const Int32 HDS_OVERFLOW = 0x1000;

            
            public const Int32 LVM_SETBKCOLOR = 0x1000 + 1;     // LVM_FIRST + 1
            public const Int32 LVM_GETHEADER = 0x1000 + 31;     // LVM_FIRST + 31
            public const Int32 LVM_SETTEXTCOLOR = 0x1000 + 36;  // LVM_FIRST + 36
            public const Int32 LVM_SETITEMSTATE = 0x1000 + 43;  // LVM_FIRST + 43
            public const Int32 HDM_GETITEM = 0x1200 + 11;  // HDM_FIRST + 11
            public const Int32 HDM_SETITEM = 0x1200 + 12;  // HDM_FIRST + 12

            public const int WM_CONTEXTMENU = 0x007B;
            public const int WM_PAINT = 0x000F;
            public const int WM_NOTIFY = 0x004E;
            public const int OCM_NOTIFY = 0x204E;
            public const int WM_LBUTTONDBLCLK = 0x0203;

            public const int NM_CLICK = -2;
            public const int NM_DBLCLK = -3;
            public const int NM_RETURN = -4;
            public const int NM_CUSTOMDRAW = -12;

            public const uint CDDS_PREPAINT = 0x00000001;
            public const uint CDDS_ITEMPREPAINT = 0x00010001;
            public const int CDRF_NEWFONT = 0x00000002;
            public const int CDRF_NOTIFYITEMDRAW = 0x00000020;
            public const uint CDIS_SELECTED = 0x0001;
            public const uint CDIS_HOT = 0x0040;

            public const int HDN_ITEMSTATEICONCLICK = -316;

            public const int WM_HSCROLL = 0x114;
            public const int WM_VSCROLL = 0x115;

            public const int LVIF_STATE = 8;
            public const int LVIS_SELECTED = 0x02;

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr Handle, Int32 msg, IntPtr wParam, ref HDITEM lParam);

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr Handle, Int32 msg, IntPtr wParam, ref LVITEM lParam);

            [StructLayout(LayoutKind.Sequential)]
            public struct NMCUSTOMDRAW
            {
                public NMHDR hdr;
                public uint dwDrawStage;
                public IntPtr hdc;
                public RECT rc;
                public IntPtr dwItemSpec;
                public uint uItemState;
                public IntPtr lItemlParam;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct NMLVCUSTOMDRAW
            {
                public NMCUSTOMDRAW nmcd;
                public int clrText;
                public int clrTextBk;
                public int iSubItem;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left, top, right, bottom;
            }

            [DllImport("user32.dll")]
            public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern int SetWindowLongW(IntPtr hWnd, int nIndex, int dwNewLong);

            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern int GetWindowLongW(IntPtr hWnd, int nIndex);

            [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
            public static extern int SetWindowTheme(IntPtr hWnd, String pszSubAppName, String pszSubIdList);

            [StructLayout(LayoutKind.Sequential)]
            public struct SCROLLINFO
            {
                public uint cbSize;
                public uint fMask;
                public int nMin;
                public int nMax;
                public uint nPage;
                public int nPos;
                public int nTrackPos;
            }

            public enum ScrollBarDirection
            {
                SB_HORZ = 0,
                SB_VERT = 1,
                SB_CTL = 2,
                SB_BOTH = 3
            }

            public enum ScrollInfoMask
            {
                SIF_RANGE = 0x1,
                SIF_PAGE = 0x2,
                SIF_POS = 0x4,
                SIF_DISABLENOSCROLL = 0x8,
                SIF_TRACKPOS = 0x10,
                SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
            }
        }

        public void SetSortIcon(int column, SortIcon mode)
        {
            if (DesignMode || !IsHandleCreated || View != View.Details)
                return;

            if (column < 0 || column > Columns.Count)
                throw new ArgumentOutOfRangeException("column", column, "Invalid column number");

            IntPtr hHeader = NativeMethods.SendMessage(Handle, NativeMethods.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            IntPtr col = new IntPtr(column);
            NativeMethods.HDITEM hdItem = new NativeMethods.HDITEM();
            IntPtr rtn;

            // Only update the previous item if it existed and if it was a different one.
            hdItem.mask = NativeMethods.HDI_FORMAT;
            rtn = NativeMethods.SendMessage(hHeader, NativeMethods.HDM_GETITEM, col, ref hdItem);

            hdItem.mask = NativeMethods.HDI_FORMAT;
            hdItem.fmt &= ~(NativeMethods.HDF_SORTDOWN | NativeMethods.HDF_SORTUP | NativeMethods.HDF_CHECKBOX | NativeMethods.HDF_CHECKED);
            switch (mode)
            {
                case SortIcon.Ascending:
                    hdItem.fmt |= NativeMethods.HDF_SORTUP;
                    break;
                case SortIcon.Descending:
                    hdItem.fmt |= NativeMethods.HDF_SORTDOWN;
                    break;
            }

            if (column == 0 && CheckBoxes && ShowSelectAllCheckBox)
            {
                if (!_setHeaderStyle)
                {
                    _setHeaderStyle = true;
                    int style = NativeMethods.GetWindowLongW(hHeader, NativeMethods.GWL_STYLE);

                    NativeMethods.SetWindowLongW(hHeader, NativeMethods.GWL_STYLE, style | NativeMethods.HDS_CHECKBOXES);
                }

                hdItem.fmt |= NativeMethods.HDF_CHECKBOX;

                if (SelectAllChecked)
                    hdItem.fmt |= NativeMethods.HDF_CHECKED;
            }

            rtn = NativeMethods.SendMessage(hHeader, NativeMethods.HDM_SETITEM, col, ref hdItem);

            GC.KeepAlive(HeaderHeight);
        }

        int GetHeaderHeight()
        {
            if (View != View.Details || !IsHandleCreated)
                return -1;

            IntPtr hHeader = NativeMethods.SendMessage(Handle, NativeMethods.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            if (hHeader == IntPtr.Zero)
                return -1;

            NativeMethods.RECT r = new NativeMethods.RECT();

            if (NativeMethods.GetWindowRect(hHeader, ref r))
            {
                return r.bottom - r.top + 1;
            }

            return -1;
        }

        int _headerHeight;
        [Browsable(false)]
        public int HeaderHeight
        {
            get { return Math.Max((_headerHeight > 0) ? _headerHeight - 1 : (_headerHeight = GetHeaderHeight()) - 1, 0); }
        }

        int _itemHeight;
        [Browsable(false)]
        public int ItemHeight
        {
            get
            {
                if (_itemHeight == 0)
                {
                    if (Items.Count > 0)
                    {
                        Rectangle r = GetItemRect(TopItem.Index);
                        _itemHeight = r.Height;
                    }
                }
                return _itemHeight;
            }
        }

        int _itemsPerPage;
        [Browsable(false)]
        public int ItemsVisible
        {
            get
            {
                if (View != View.Details)
                    throw new NotImplementedException();
                if (_itemsPerPage == 0 && ItemHeight > 0)
                {
                    _itemsPerPage = (Height + ItemHeight - 1) / ItemHeight;
                }
                return _itemsPerPage;
            }
        }

        [DefaultValue(false), Description("Automatically resizes the last column to fit")]
        public bool AutoSizeLastColumn
        {
            get { return _autoSizeRightColumn; }
            set { _autoSizeRightColumn = value; }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            _headerHeight = 0;
            _itemHeight = 0;
            _itemsPerPage = 0;
            base.OnSizeChanged(e);

            if (_autoSizeRightColumn && !DesignMode && Columns.Count > 0)
                ResizeColumnsToFit(Columns[Columns.Count - 1]);
        }

        #endregion

        string _groupSeparator = ", ";
        [DefaultValue(", ")]
        public string GroupSeparator
        {
            get { return _groupSeparator; }
            set { _groupSeparator = value; }
        }

        private const int LVM_FIRST = 0x1000;
        private const int LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
        private const int LVS_EX_DOUBLEBUFFER = 0x00010000;
        private bool _setHeaderStyle;

        bool _isThemed;
        bool _useDarkNativeTheme;
        bool _allowDarkNativeTheme = true;
        bool _ownerDrawPaletteHeader;
        Color _headerBackColor;
        Color _headerForeColor;
        Color _headerBorderColor;
        Color _selectionBackColor;
        Color _selectionForeColor;
        Color _hoverBackColor;
        bool _usePaletteSelectionColors;
        bool _preserveItemForeColorWhenSelected;
        bool _preserveItemForeColorWhenHot;

        [DefaultValue(false)]
        public bool PreserveItemForeColorWhenHot
        {
            get { return _preserveItemForeColorWhenHot; }
            set
            {
                if (_preserveItemForeColorWhenHot == value)
                    return;

                _preserveItemForeColorWhenHot = value;
                Invalidate();
            }
        }

        [DefaultValue(false)]
        public bool PreserveItemForeColorWhenSelected
        {
            get { return _preserveItemForeColorWhenSelected; }
            set
            {
                if (_preserveItemForeColorWhenSelected == value)
                    return;

                _preserveItemForeColorWhenSelected = value;
                Invalidate();
            }
        }

        [DefaultValue(true)]
        public bool AllowDarkNativeTheme
        {
            get { return _allowDarkNativeTheme; }
            set
            {
                if (_allowDarkNativeTheme == value)
                    return;

                _allowDarkNativeTheme = value;

                if (IsHandleCreated)
                    RecreateHandle();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            _setHeaderStyle = false;

            UpdateSortGlyphs();

            if (_useDarkNativeTheme)
            {
                if (AllowDarkNativeTheme)
                    NativeMethods.SetWindowTheme(Handle, "DarkMode_Explorer", null);
                else
                    NativeMethods.SetWindowTheme(Handle, "", "");
            }
            else if (!OwnerDraw && !_isThemed)
                NativeMethods.SetWindowTheme(Handle, "Explorer", null);

            NativeMethods.SendMessage(
                Handle,
                LVM_SETEXTENDEDLISTVIEWSTYLE,
                (IntPtr)LVS_EX_DOUBLEBUFFER,
                (IntPtr)LVS_EX_DOUBLEBUFFER);

            // SetWindowTheme() can reset the native ListView colors even when
            // the managed BackColor/ForeColor still contain the VS palette.
            // Reapply them after every handle creation so selected rows do not
            // fall back to black text on dark Visual Studio surfaces.
            RestoreNativeColors(this);
        }

        internal static void RestoreNativeColors(ListView listView)
        {
            if (listView == null)
                throw new ArgumentNullException("listView");

            if (!listView.IsHandleCreated)
                return;

            NativeMethods.SendMessage(
                listView.Handle,
                NativeMethods.LVM_SETBKCOLOR,
                IntPtr.Zero,
                (IntPtr)ColorTranslator.ToWin32(listView.BackColor));
            NativeMethods.SendMessage(
                listView.Handle,
                NativeMethods.LVM_SETTEXTCOLOR,
                IntPtr.Zero,
                (IntPtr)ColorTranslator.ToWin32(listView.ForeColor));
            listView.Invalidate();
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            if (!_ownerDrawPaletteHeader)
            {
                e.DrawDefault = true;
                return;
            }

            Color headerBack = _headerBackColor.IsEmpty
                ? BackColor
                : _headerBackColor;
            Color headerFore = _headerForeColor.IsEmpty
                ? ForeColor
                : _headerForeColor;
            Color border = _headerBorderColor.IsEmpty
                ? AnkhThemePalette.Blend(ForeColor, BackColor, 0.25)
                : _headerBorderColor;

            using (SolidBrush background = new SolidBrush(headerBack))
                e.Graphics.FillRectangle(background, e.Bounds);

            using (Pen pen = new Pen(border))
            {
                e.Graphics.DrawLine(
                    pen,
                    e.Bounds.Left,
                    e.Bounds.Bottom - 1,
                    e.Bounds.Right,
                    e.Bounds.Bottom - 1);

                e.Graphics.DrawLine(
                    pen,
                    e.Bounds.Right - 1,
                    e.Bounds.Top,
                    e.Bounds.Right - 1,
                    e.Bounds.Bottom);
            }

            SmartColumn smartColumn = e.Header as SmartColumn;
            bool sorted = smartColumn != null && SortColumns.Contains(smartColumn);
            bool descending = sorted && smartColumn.ReverseSort;

            int padding = Math.Max(6, Font.Height / 3);
            int glyphSize = Math.Max(6, Font.Height / 2);
            Rectangle textBounds = new Rectangle(
                e.Bounds.Left + padding,
                e.Bounds.Top,
                Math.Max(0, e.Bounds.Width - (padding * 2)),
                e.Bounds.Height);

            if (sorted)
                textBounds.Width = Math.Max(0, textBounds.Width - glyphSize - padding);

            TextFormatFlags flags =
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.SingleLine |
                TextFormatFlags.NoPrefix;

            switch (e.Header.TextAlign)
            {
                case HorizontalAlignment.Center:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                case HorizontalAlignment.Right:
                    flags |= TextFormatFlags.Right;
                    break;
                default:
                    flags |= TextFormatFlags.Left;
                    break;
            }

            TextRenderer.DrawText(
                e.Graphics,
                e.Header.Text,
                Font,
                textBounds,
                headerFore,
                flags);

            if (sorted)
                DrawDarkSortGlyph(e.Graphics, e.Bounds, headerFore, glyphSize, descending);
        }

        static void DrawDarkSortGlyph(
            Graphics graphics,
            Rectangle bounds,
            Color color,
            int size,
            bool descending)
        {
            int centerX = bounds.Right - Math.Max(8, size);
            int centerY = bounds.Top + bounds.Height / 2;
            int half = Math.Max(3, size / 2);

            Point[] points;
            if (descending)
            {
                points = new[]
                {
                    new Point(centerX - half, centerY - half / 2),
                    new Point(centerX + half, centerY - half / 2),
                    new Point(centerX, centerY + half / 2 + 1)
                };
            }
            else
            {
                points = new[]
                {
                    new Point(centerX - half, centerY + half / 2),
                    new Point(centerX + half, centerY + half / 2),
                    new Point(centerX, centerY - half / 2 - 1)
                };
            }

            using (SolidBrush brush = new SolidBrush(color))
                graphics.FillPolygon(brush, points);
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (_ownerDrawPaletteHeader)
                e.DrawDefault = true;

            base.OnDrawItem(e);
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            if (_ownerDrawPaletteHeader)
                e.DrawDefault = true;

            base.OnDrawSubItem(e);
        }

        /// <summary>
        /// Gets a value indicating whether the listview supports grouping.
        /// </summary>
        /// <value><c>true</c> if [supports grouping]; otherwise, <c>false</c>.</value>
        public static bool SupportsGrouping
        {
            get { return IsXPPlus; }
        }

        public static bool SupportsSortGlypgs
        {
            get { return IsXPPlus; }
        }

        internal void UpdateSortGlyphs()
        {
            if (!IsHandleCreated || !SupportsSortGlypgs || DesignMode || View != View.Details)
                return;

            foreach (ColumnHeader ch in Columns)
            {
                SmartColumn sc = ch as SmartColumn;

                if (sc != null)
                {
                    if (SortColumns.Contains(sc))
                    {
                        SetSortIcon(sc.Index, sc.ReverseSort ? SortIcon.Descending : SortIcon.Ascending);
                    }
                    else
                        SetSortIcon(sc.Index, SortIcon.None);
                }

            }
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            if (!DesignMode && View == View.Details && !VirtualMode)
            {
                ColumnHeader column = Columns[e.Column];

                SmartColumn sc = column as SmartColumn;
                if (sc != null && sc.Sortable)
                {
                    bool extend = (Control.ModifierKeys & Keys.Control) != 0;

                    if (!extend)
                    {
                        if (SortColumns.Count == 1 && SortColumns[0] == sc)
                            sc.ReverseSort = !sc.ReverseSort;
                        else
                        {
                            SortColumns.Clear();
                            SortColumns.Add(sc);
                            sc.ReverseSort = false;
                        }
                    }
                    else
                    {
                        if (SortColumns.Contains(sc))
                            sc.ReverseSort = !sc.ReverseSort;
                        else
                        {
                            sc.ReverseSort = false;
                            SortColumns.Add(sc);
                        }
                    }
                    Sort();
                    UpdateSortGlyphs();
                }
            }
            base.OnColumnClick(e);
        }

        protected internal virtual void UpdateGroup(SmartListViewItem item, string[] values)
        {
            if (VirtualMode)
                return; // Not valid for virtual

            StringBuilder sb = new StringBuilder();

            foreach (SmartColumn col in GroupColumns)
            {
                int c = col.AllColumnsIndex;
                if (c < values.Length)
                {
                    if (sb.Length > 0)
                        sb.Append(GroupSeparator);

                    sb.Append(values[c]);
                }
            }

            string g = sb.ToString();

            if (item.Group != null && item.Group.Name == g)
                return; // Nothing to do

            item.Group = null;

            SmartGroup group = (SmartGroup)Groups[g];

            if (group == null)
            {
                string txt = string.IsNullOrEmpty(g) ? "<Rest>" : g;
                group = new SmartGroup(this, g, txt);
                _groups.Add(group, group);
                Groups.Clear();
                Groups.AddRange(new List<ListViewGroup>(_groups.Values).ToArray());
            }

            group.Items.Add(item);

            RefreshGroupsAvailable();
        }

        public void ClearItems()
        {
            Items.Clear();
            Groups.Clear();
            _groups.Clear();
        }

        public void RefreshGroups()
        {
            if (DesignMode || VirtualMode || View != View.Details || !SupportsGrouping)
                return;

            bool inGroups = _inRefreshGroupsAvailable;
            try
            {
                _inRefreshGroupsAvailable = true;

                if (GroupColumns.Count == 0)
                {
                    ShowGroups = false;
                    foreach (ListViewGroup grp in Groups)
                    {
                        grp.Items.Clear();
                    }
                    Groups.Clear();
                    _groups.Clear();
                }
                else
                {
                    foreach (ListViewItem i in Items)
                    {
                        i.Group = null;

                        SmartListViewItem si = i as SmartListViewItem;

                        if (si != null)
                            si.UpdateGroup();
                    }

                    FlushGroups();
                }
            }
            finally
            {
                _inRefreshGroupsAvailable = inGroups;
            }
        }

        void FlushGroups()
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                ListViewGroup group = Groups[i];

                if (group.Items.Count == 0)
                {
                    _groups.Remove((SmartGroup)group);
                    Groups.RemoveAt(i--);
                }
            }

            bool shouldShow = (Groups.Count > 1);

            if (ShowGroups != shouldShow)
            {
                ShowGroups = shouldShow;

                if (shouldShow)
                {
                    // Recreate the groups
                    Groups.Clear();
                    Groups.AddRange(new List<ListViewGroup>(_groups.Values).ToArray());
                }
            }
        }

        bool _inRefreshGroupsAvailable;
        public void RefreshGroupsAvailable()
        {
            if (_inRefreshGroupsAvailable || !SupportsGrouping)
                return;

            try
            {
                _inRefreshGroupsAvailable = true;

                FlushGroups();
            }
            finally
            {
                _inRefreshGroupsAvailable = false;
            }
        }

        bool _noStrictCheckboxesClick;
        [DefaultValue(true)]
        public bool StrictCheckboxesClick
        {
            get { return !_noStrictCheckboxesClick; }
            set { _noStrictCheckboxesClick = !value; }
        }


        internal static bool ShouldSuppressNativeCheckboxDoubleClick(
            bool checkBoxes,
            bool strictCheckboxesClick,
            bool hasItem,
            ListViewHitTestLocations location)
        {
            return checkBoxes
                && strictCheckboxesClick
                && hasItem
                && location != ListViewHitTestLocations.None
                && location != ListViewHitTestLocations.StateImage;
        }

        public event MouseEventHandler ShowContextMenu;
        public virtual void OnShowContextMenu(MouseEventArgs e)
        {
            if (ShowContextMenu != null)
                ShowContextMenu(this, e);
        }

        bool InUpdateBatch
        {
            get { return _nInUpdates > 0; }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)"/>.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
        protected override void WndProc(ref Message m)
        {
            if (!DesignMode && m.Msg == NativeMethods.WM_PAINT)
            {
                base.WndProc(ref m);

                if (PreserveItemForeColorWhenSelected)
                {
                    RedrawSelectedItemText();
                    RedrawHotItemText();
                }

                return;
            }

            if (!DesignMode)
            {
                switch (m.Msg)
                {
                    case NativeMethods.WM_CONTEXTMENU:
                        {
                            uint pos = unchecked((uint)(ulong)m.LParam);

                            Select();

                            OnShowContextMenu(new MouseEventArgs(Control.MouseButtons, 1,
                                unchecked((short)(ushort)(pos & 0xFFFF)),
                                unchecked((short)(ushort)(pos >> 16)), 0));

                            return;
                        }

                    case NativeMethods.WM_LBUTTONDBLCLK:
                        // Modern native ListView controls toggle a checkbox when
                        // a checked row is double-clicked, even when the pointer
                        // is nowhere near the state image. Intercept the raw
                        // message before native processing so a row double-click
                        // remains an open/diff gesture. Double-clicking the actual
                        // checkbox is still left to the native control.
                        if (CheckBoxes && StrictCheckboxesClick)
                        {
                            Point mp = PointToClient(MousePosition);
                            ListViewHitTestInfo hi = HitTest(mp);

                            if (hi != null
                                && ShouldSuppressNativeCheckboxDoubleClick(
                                    CheckBoxes,
                                    StrictCheckboxesClick,
                                    hi.Item != null,
                                    hi.Location))
                            {
                                MouseEventArgs me = new MouseEventArgs(
                                    MouseButtons.Left, 2, mp.X, mp.Y, 0);
                                OnDoubleClick(me);
                                OnMouseDoubleClick(me);
                                return;
                            }
                        }
                        break;

                    case NativeMethods.OCM_NOTIFY:
                        // Receives ListView notifications
                        NMHDR notifyHdr = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));

                        if (notifyHdr.code == NativeMethods.NM_CUSTOMDRAW
                            && _usePaletteSelectionColors)
                        {
                            NativeMethods.NMLVCUSTOMDRAW draw =
                                (NativeMethods.NMLVCUSTOMDRAW)Marshal.PtrToStructure(
                                    m.LParam, typeof(NativeMethods.NMLVCUSTOMDRAW));

                            if (draw.nmcd.dwDrawStage == NativeMethods.CDDS_PREPAINT)
                            {
                                m.Result = (IntPtr)NativeMethods.CDRF_NOTIFYITEMDRAW;
                                return;
                            }

                            if (draw.nmcd.dwDrawStage == NativeMethods.CDDS_ITEMPREPAINT)
                            {
                                bool selected =
                                    (draw.nmcd.uItemState & NativeMethods.CDIS_SELECTED) != 0;
                                bool hot =
                                    (draw.nmcd.uItemState & NativeMethods.CDIS_HOT) != 0;

                                if (selected || hot)
                                {
                                    int itemIndex = unchecked((int)draw.nmcd.dwItemSpec.ToInt64());
                                    Color itemForeColor = ForeColor;

                                    if (itemIndex >= 0 && itemIndex < Items.Count)
                                    {
                                        itemForeColor =
                                            SmartListViewThemeLogic.ResolveSelectedItemForeground(
                                                Items[itemIndex].ForeColor,
                                                ForeColor);
                                    }

                                    // Hovering or selecting a row must not replace the
                                    // item's theme/status foreground. Let the native
                                    // control keep drawing its interaction background,
                                    // but preserve the resolved item text color.
                                    draw.clrText = ColorTranslator.ToWin32(itemForeColor);

                                    if (selected)
                                        draw.clrTextBk =
                                            ColorTranslator.ToWin32(_selectionBackColor);

                                    Marshal.StructureToPtr(draw, m.LParam, false);
                                    m.Result = (IntPtr)NativeMethods.CDRF_NEWFONT;
                                    return;
                                }
                            }
                        }

                        if (CheckBoxes && StrictCheckboxesClick)
                        {

                            if (notifyHdr.code == NativeMethods.NM_DBLCLK)
                            {
                                Point mp = PointToClient(MousePosition);
                                ListViewHitTestInfo hi = HitTest(mp);

                                if (hi != null && hi.Location != ListViewHitTestLocations.StateImage)
                                {
                                    MouseEventArgs me = new MouseEventArgs(MouseButtons.Left, 2, mp.X, mp.Y, 0);
                                    OnDoubleClick(me);
                                    OnMouseDoubleClick(me);
                                    return;
                                }
                            }
                        }
                        break;
                    case NativeMethods.WM_NOTIFY:
                        // Receives child control notifications (like that of the header control)
                        if (CheckBoxes && ShowSelectAllCheckBox)
                        {
                            NMHDR hdr = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));

                            if (hdr.code == NativeMethods.HDN_ITEMSTATEICONCLICK)
                            {
                                CancelEventArgs ce = new CancelEventArgs();

                                PerformSelectAllCheckedChange(ce);

                                if (ce.Cancel)
                                    return;
                            }
                        }
                        break;

                    case NativeMethods.WM_HSCROLL:
                        WmHScroll(ref m);
                        break;
                    case NativeMethods.WM_VSCROLL:
                        WmVScroll(ref m);
                        break;
                }
            }
            base.WndProc(ref m);
        }

        void RedrawHotItemText()
        {
            if (View != View.Details)
                return;

            Point mouse = PointToClient(MousePosition);
            if (!ClientRectangle.Contains(mouse))
                return;

            ListViewHitTestInfo hit = HitTest(mouse);
            if (hit == null || hit.Item == null)
                return;

            // A selected item is already repaired by RedrawSelectedItemText(),
            // including its selection background. Repaint only an unselected
            // hot row here so the native hover background remains untouched.
            if (hit.Item.Selected)
                return;

            using (Graphics graphics = CreateGraphics())
                RedrawItemText(graphics, hit.Item, Color.Empty);
        }

        void RedrawItemText(
            Graphics graphics,
            ListViewItem item,
            Color background)
        {
            Color itemForeColor = SmartListViewThemeLogic.ResolveSelectedItemForeground(
                item.ForeColor,
                ForeColor);

            int count = Math.Min(item.SubItems.Count, Columns.Count);
            for (int i = 0; i < count; i++)
            {
                Rectangle bounds = i == 0
                    ? item.GetBounds(ItemBoundsPortion.Label)
                    : item.SubItems[i].Bounds;

                bounds.Intersect(ClientRectangle);
                if (bounds.Width <= 0 || bounds.Height <= 0)
                    continue;

                if (i > 0)
                {
                    bounds.X += 4;
                    bounds.Width = Math.Max(0, bounds.Width - 8);
                }

                TextFormatFlags flags =
                    TextFormatFlags.VerticalCenter
                    | TextFormatFlags.SingleLine
                    | TextFormatFlags.EndEllipsis
                    | TextFormatFlags.NoPrefix
                    | TextFormatFlags.PreserveGraphicsClipping;

                HorizontalAlignment alignment = Columns[i].TextAlign;
                if (alignment == HorizontalAlignment.Center)
                    flags |= TextFormatFlags.HorizontalCenter;
                else if (alignment == HorizontalAlignment.Right)
                    flags |= TextFormatFlags.Right;

                if (background.IsEmpty)
                {
                    TextRenderer.DrawText(
                        graphics,
                        item.SubItems[i].Text,
                        item.Font ?? Font,
                        bounds,
                        itemForeColor,
                        flags);
                }
                else
                {
                    TextRenderer.DrawText(
                        graphics,
                        item.SubItems[i].Text,
                        item.Font ?? Font,
                        bounds,
                        itemForeColor,
                        background,
                        flags);
                }
            }
        }

        void RedrawSelectedItemText()
        {
            if (View != View.Details
                || SelectedItems.Count == 0
                || (!Focused && HideSelection)
                || _selectionBackColor.IsEmpty)
            {
                return;
            }

            using (Graphics graphics = CreateGraphics())
            {
                foreach (ListViewItem item in SelectedItems)
                    RedrawItemText(graphics, item, _selectionBackColor);
            }
        }

        public IDictionary<string, int> GetColumnWidths()
        {
            IDictionary<string, int> widths = new Dictionary<string, int>(_allColumns.Count);

            foreach (SmartColumn item in _allColumns)
            {
                if (!string.IsNullOrEmpty(item.Name))
                {
                    widths.Add(item.Name, item.DefaultWidth == item.Width ? -1 : item.Width);
                }
            }

            return widths;
        }

        public void SetColumnWidths(IDictionary<string, int> widths)
        {
            if (widths == null || widths.Count == 0)
                return;

            foreach (SmartColumn item in _allColumns)
            {
                if (!string.IsNullOrEmpty(item.Name) && widths.ContainsKey(item.Name))
                {
                    item.Width = widths[item.Name];
                }
            }
        }

        private void PerformSelectAllCheckedChange(CancelEventArgs ce)
        {
            OnSelectAllCheckedChanging(ce);

            if (ce.Cancel)
                return;

            bool check = !SelectAllChecked;

            if (Items.Count == 0)
                check = false;

            SelectAllChecked = check;
            UpdateSortGlyphs();

            foreach (ListViewItem i in Items)
            {
                if (IsPartOfSelectAll(i))
                {
                    i.Checked = check;
                }
                else
                    i.Checked = false;
            }

            OnSelectAllCheckedChanged(EventArgs.Empty);

            ce.Cancel = true; // Kill default behavior
        }

        protected virtual bool IsPartOfSelectAll(ListViewItem i)
        {
            return true;
        }

        int _nInUpdates;
        bool _updateAllBox; /* = false */
        public new void BeginUpdate()
        {
            try
            {
                base.BeginUpdate();

                if (_nInUpdates == 0)
                    OnBeginUpdate(EventArgs.Empty);
            }
            finally
            {
                _nInUpdates++;
            }
        }

        public new void EndUpdate()
        {
            try
            {
                --_nInUpdates;

                if (_nInUpdates == 0)
                {
                    OnEndUpdate(EventArgs.Empty);                    
                }
            }
            finally
            {
                base.EndUpdate();
            }
        }

        protected virtual void OnBeginUpdate(EventArgs eventArgs)
        {
        }

        protected virtual void OnEndUpdate(EventArgs e)
        {
            if (_updateAllBox && CheckBoxes && ShowSelectAllCheckBox)
            {
                _updateAllBox = false;

                bool allChecked = true;
                foreach (ListViewItem i in Items)
                {
                    if (!i.Checked && IsPartOfSelectAll(i))
                    {
                        allChecked = false;
                        break;
                    }
                }

                SelectAllChecked = allChecked;
                UpdateSortGlyphs();
            }
        }

        protected override void OnItemChecked(ItemCheckedEventArgs e)
        {
            base.OnItemChecked(e);

            if (_nInUpdates > 0)
            {
                _updateAllBox = true;
                return;
            }

            if (CheckBoxes && ShowSelectAllCheckBox)
            {
                bool check = e.Item.Checked;
                if (SelectAllChecked && !check && IsPartOfSelectAll(e.Item))
                {
                    SelectAllChecked = false;
                    UpdateSortGlyphs();
                }
                else if (!SelectAllChecked && check)
                {
                    bool allChecked = true;
                    foreach (ListViewItem i in Items)
                    {
                        if (!i.Checked && IsPartOfSelectAll(i))
                        {
                            allChecked = false;
                            break;
                        }
                    }

                    if (allChecked)
                    {
                        SelectAllChecked = true;
                        UpdateSortGlyphs();
                    }
                }
            }
        }

        protected virtual void OnSelectAllCheckedChanging(CancelEventArgs ce)
        {

        }

        protected virtual void OnSelectAllCheckedChanged(EventArgs eventArgs)
        {

        }

        private void WmVScroll(ref Message m)
        {
            OnVScroll(new ScrollEventArgs((ScrollEventType)((int)m.WParam & 0xFFFF), -1));
        }

        /// <summary>
        /// 
        /// </summary>
        public EventHandler VScroll;
        protected virtual void OnVScroll(EventArgs e)
        {
            if (VScroll != null)
                VScroll(this, e);
        }

        private void WmHScroll(ref Message m)
        {
            OnHScroll(new ScrollEventArgs((ScrollEventType)((int)m.WParam & 0xFFFF), -1));
        }

        /// <summary>
        /// 
        /// </summary>
        public EventHandler HScroll;
        protected virtual void OnHScroll(EventArgs e)
        {
            if (HScroll != null)
                HScroll(this, e);
        }

        sealed class SmartListSorter : System.Collections.IComparer, IComparer<ListViewItem>
        {
            SmartListView _view;

            public SmartListSorter(SmartListView view)
            {
                if (view == null)
                    throw new ArgumentNullException("view");

                _view = view;
            }

            int System.Collections.IComparer.Compare(object x, object y)
            {
                return Compare((ListViewItem)x, (ListViewItem)y);
            }

            public int Compare(ListViewItem x, ListViewItem y)
            {
                if (_view.TopSortColumn != null)
                {
                    int n = _view.TopSortColumn.Compare(x, y, false);
                    if (n != 0)
                        return n;
                }

                foreach (SmartColumn col in _view.SortColumns)
                {
                    int n = col.Compare(x, y, true);

                    if (n != 0)
                        return n;
                }

                if (_view.FinalSortColumn != null)
                    return _view.FinalSortColumn.Compare(x, y, false);

                return 0;
            }
        }

        sealed class SmartGroupSorter : IComparer<SmartGroup>, System.Collections.IComparer
        {
            SmartListView _view;

            public SmartGroupSorter(SmartListView view)
            {
                if (view == null)
                    throw new ArgumentNullException("view");

                _view = view;
            }

            #region IComparer<SmartGroup> Members

            public int Compare(SmartGroup x, SmartGroup y)
            {
                // TODO: Replace with better comparer
                return StringComparer.OrdinalIgnoreCase.Compare(x.Header, y.Header);
            }

            #endregion

            int System.Collections.IComparer.Compare(object x, object y)
            {
                return Compare((SmartGroup)x, (SmartGroup)y);
            }
        }

        #region XPPlus
        static readonly object _lck = new object();
        static int _osLevel;

        internal static int OSLevel
        {
            get
            {
                lock (_lck)
                {
                    if (_osLevel == 0)
                    {
                        Version osVersion = Environment.OSVersion.Version;

                        if (osVersion.Major >= 6)
                            _osLevel = 600;
                        else if (osVersion.Major == 5)
                        {
                            if (osVersion.Minor >= 2)
                                _osLevel = 520;
                            else if (osVersion.Minor == 1)
                                _osLevel = 510;
                            else
                                _osLevel = 500;
                        }
                        else
                            _osLevel = 1;
                    }

                    return _osLevel;
                }
            }
        }

        internal static bool IsXPPlus
        {
            get { return OSLevel >= 510; }
        }
        #endregion

        [Browsable(false)]
        public int HScrollPos
        {
            get
            {
                NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO();
                si.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(si);
                si.fMask = (int)(NativeMethods.ScrollInfoMask.SIF_TRACKPOS | NativeMethods.ScrollInfoMask.SIF_POS);

                if (NativeMethods.GetScrollInfo(Handle, (int)NativeMethods.ScrollBarDirection.SB_HORZ, ref si))
                    return si.nPos;
                return -1;
            }
        }

        [Browsable(false)]
        public int HScrollMax
        {
            get
            {
                NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO();
                si.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(si);
                si.fMask = (int)(NativeMethods.ScrollInfoMask.SIF_RANGE);

                if (NativeMethods.GetScrollInfo(Handle, (int)NativeMethods.ScrollBarDirection.SB_HORZ, ref si))
                    return si.nMax;
                return -1;
            }
        }

        [Browsable(false)]
        public int VScrollPos
        {
            get
            {
                if (!IsHandleCreated)
                    return -1;
                NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO();
                si.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(si);
                si.fMask = (int)(NativeMethods.ScrollInfoMask.SIF_TRACKPOS | NativeMethods.ScrollInfoMask.SIF_POS);

                if (NativeMethods.GetScrollInfo(Handle, (int)NativeMethods.ScrollBarDirection.SB_VERT, ref si))
                    return si.nPos;
                return -1;
            }
        }

        [Browsable(false)]
        public int VScrollMax
        {
            get
            {
                if (!IsHandleCreated)
                    return -1;
                NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO();
                si.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(si);
                si.fMask = (int)(NativeMethods.ScrollInfoMask.SIF_RANGE);

                if (NativeMethods.GetScrollInfo(Handle, (int)NativeMethods.ScrollBarDirection.SB_VERT, ref si))
                    return si.nMax;
                return -1;
            }
        }

        /// <summary>
        /// Resizes the specified columns to optimally fill the usable width
        /// </summary>
        /// <param name="resizeColumns">The resize columns.</param>
        public void ResizeColumnsToFit(params ColumnHeader[] resizeColumns)
        {
            if (DesignMode || resizeColumns == null)
                return;

            int currentWidth = 0;
            foreach (ColumnHeader ch in resizeColumns)
            {
                if (ch == null)
                    throw new ArgumentException("Null column", "resizeColumns");

                currentWidth += ch.Width + 1;
            }

            int otherWidth = 0;

            foreach (ColumnHeader ch in Columns)
            {
                if (Array.IndexOf(resizeColumns, ch) >= 0)
                    continue;

                if (ch.DisplayIndex >= 0)
                    otherWidth += ch.Width + 1; // 1 = separator space
            }

            int restWidth = Width - otherWidth - SystemInformation.VerticalScrollBarWidth - 2;
            int rest = restWidth - currentWidth;

            if (restWidth > 0)
            {
                foreach (ColumnHeader ch in resizeColumns)
                {
                    ch.Width = Math.Max(0, ch.Width + rest / resizeColumns.Length);
                }
            }
        }

        internal void SetSelected(ListViewItem i, bool selected)
        {
            if (i == null)
                throw new ArgumentNullException("i");
            else if (VirtualMode || !IsHandleCreated)
            {
                i.Selected = true;
                return;
            }

            NativeMethods.LVITEM lvItem = new NativeMethods.LVITEM();
            lvItem.mask = NativeMethods.LVIF_STATE;
            lvItem.state = selected ? NativeMethods.LVIS_SELECTED : 0;
            lvItem.stateMask = NativeMethods.LVIS_SELECTED;

            NativeMethods.SendMessage(Handle, NativeMethods.LVM_SETITEMSTATE, (IntPtr)i.Index, ref lvItem);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ListView.ColumnHeaderCollection Columns
        {
            get { return base.Columns; }
        }

        public virtual void OnThemeChange(IAnkhServiceProvider sender, CancelEventArgs e)
        {
            ShowSelectAllCheckBox = false; // Not supported by VS theming. Disable to avoid problems and unnecessary work :(

            IWinFormsThemingService themer = sender.GetService<IWinFormsThemingService>();
            AnkhThemePalette palette = themer != null ? themer.ThemePalette : null;
            if (palette != null)
            {
                BackColor = palette.SurfaceBackground;
                ForeColor = palette.SurfaceForeground;
                _headerBackColor = AnkhThemePalette.Blend(palette.SurfaceForeground, palette.SurfaceBackground, 0.08);
                _headerForeColor = palette.SurfaceForeground;
                _headerBorderColor = palette.Border;
                _selectionBackColor = palette.SelectionBackground;
                _selectionForeColor = palette.SelectionForeground;
                _hoverBackColor = palette.HoverBackground;
                _usePaletteSelectionColors = !SystemInformation.HighContrast;
            }
            else
            {
                _headerBackColor = Color.Empty;
                _headerForeColor = Color.Empty;
                _headerBorderColor = Color.Empty;
                _selectionBackColor = Color.Empty;
                _selectionForeColor = Color.Empty;
                _hoverBackColor = Color.Empty;
                _usePaletteSelectionColors = false;
            }

            _ownerDrawPaletteHeader = SmartListViewThemeLogic.ShouldOwnerDrawHeader(
                e.Cancel,
                palette != null,
                SystemInformation.HighContrast);

            OwnerDraw = _ownerDrawPaletteHeader;
            _isThemed = !e.Cancel;
            _useDarkNativeTheme = SmartListViewThemeLogic.ShouldUseDarkNativeTheme(
                _isThemed,
                palette != null
                    ? palette.IsDarkSurface
                    : AnkhThemePalette.IsDark(BackColor),
                SystemInformation.HighContrast);

            try
            {
                base.OnParentChanged(EventArgs.Empty); // Recreate handle, keeping state
            }
            catch(Exception ee)
            {
                IAnkhErrorHandler handler = sender.GetService<IAnkhErrorHandler>();

                if (handler != null && handler.IsEnabled(ee))
                    handler.OnError(ee);
                else
                    throw;
            }
        }

        public bool HasCheckedItems
        {
            get
            {
                foreach (ListViewItem lvi in this.Items)
                {
                    try
                    {
                        if (lvi.Checked)
                            return true;
                    }
                    catch
                    { }
                }
                return false;
            }
        }
    }
}
