// $Id$
//
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

namespace Ankh.UI.VSSelectionControls
{
    public enum SortIcon
    {
        None,
        Ascending,
        Descending
    }

    public class SmartListView : ListView
    {
        readonly Collection<SmartColumn> _groupColumns = new Collection<SmartColumn>();
        readonly Collection<SmartColumn> _sortColumns = new Collection<SmartColumn>();
        readonly Collection<SmartColumn> _allColumns = new Collection<SmartColumn>();
        readonly SortedList<SmartGroup, ListViewGroup> _groups;
        ISmartValueComparer _topSorter;
        ISmartValueComparer _finalSorter;

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
            if (e.Button == MouseButtons.Right)
            {
                ExtendSelection(e.Location, true);
            }

            base.OnMouseDown(e);
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
            };

            // Parameters for ListView-Headers
            public const Int32 HDI_FORMAT = 0x0004;
            public const Int32 HDF_LEFT = 0x0000;
            public const Int32 HDF_STRING = 0x4000;
            public const Int32 HDF_SORTUP = 0x0400;
            public const Int32 HDF_SORTDOWN = 0x0200;
            public const Int32 LVM_GETHEADER = 0x1000 + 31;  // LVM_FIRST + 31
            public const Int32 HDM_GETITEM = 0x1200 + 11;  // HDM_FIRST + 11
            public const Int32 HDM_SETITEM = 0x1200 + 12;  // HDM_FIRST + 12

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr Handle, Int32 msg, IntPtr wParam, ref HDITEM lParam);

            public struct RECT
            {
                public int left, top, right, bottom;
            }

            [DllImport("user32.dll")]
            public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);


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
            hdItem.fmt &= ~(NativeMethods.HDF_SORTDOWN | NativeMethods.HDF_SORTUP);
            switch (mode)
            {
                case SortIcon.Ascending:
                    hdItem.fmt |= NativeMethods.HDF_SORTUP;
                    break;
                case SortIcon.Descending:
                    hdItem.fmt |= NativeMethods.HDF_SORTDOWN;
                    break;
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

        protected override void OnSizeChanged(EventArgs e)
        {
            _headerHeight = 0;
            _itemHeight = 0;
            _itemsPerPage = 0;
            base.OnSizeChanged(e);
        }

        #endregion

        string _groupSeparator = ", ";
        [DefaultValue(", ")]
        public string GroupSeparator
        {
            get { return _groupSeparator; }
            set { _groupSeparator = value; }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            UpdateSortGlyphs();
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
            if (!SupportsSortGlypgs || DesignMode || View != View.Details)
                return;
            //throw new NotImplementedException();
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
            if (!DesignMode && !VirtualMode)
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


        public event MouseEventHandler ShowContextMenu;
        public virtual void OnShowContextMenu(MouseEventArgs e)
        {
            if (ShowContextMenu != null)
                ShowContextMenu(this, e);
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)"/>.
        /// </summary>
        /// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
        protected override void WndProc(ref Message m)
        {
            if (!DesignMode)
            {
                switch (m.Msg)
                {
                    case 123: // WM_CONTEXTMENU
                        {
                            uint pos = unchecked((uint)m.LParam);

                            Select();

                            OnShowContextMenu(new MouseEventArgs(Control.MouseButtons, 1,
                                unchecked((short)(ushort)(pos & 0xFFFF)),
                                unchecked((short)(ushort)(pos >> 16)), 0));

                            return;
                        }

                    case 8270: // WM_REFLECTNOTIFY
                        if (CheckBoxes && StrictCheckboxesClick)
                        {
                            NMHDR hdr = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));

                            if (hdr.code == -3) // Double click
                            {
                                Point mp = PointToClient(MousePosition);
                                ListViewHitTestInfo hi = HitTest(mp);

                                if (hi != null && hi.Location != ListViewHitTestLocations.StateImage)
                                {
                                    OnMouseDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, mp.X, mp.Y, 0));
                                    return;
                                }
                            }
                        }
                        break;
                    case 0x114: // WM_HSCROLL
                        WmHScroll(ref m);
                        break;
                    case 0x115: // WM_VSCROLL
                        WmVScroll(ref m);
                        break;
                }
            }
            base.WndProc(ref m);
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
    }
}
