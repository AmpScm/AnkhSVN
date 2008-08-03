using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.ObjectModel;

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

        public SmartListView()
        {
            this.ListViewItemSorter = new SmartListSorter(this);
            //Sorting = SortOrder.Ascending;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {           
            if (e.Button == MouseButtons.Right)
            {
                ExtendSelection(e.Location, true);
            }

            base.OnMouseDown(e);
        }

        private void ExtendSelection(Point p, bool rightClick)
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

        public Collection<SmartColumn> GroupColumns
        {
            get { return _groupColumns; }
        }

        public Collection<SmartColumn> AllColumns
        {
            get { return _allColumns; }
        }

        public Collection<SmartColumn> SortColumns
        {
            get { return _sortColumns; }
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
        }

        public void SetSortIcon(int column, SortIcon mode)
        {
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
        }
        #endregion

        string _groupSeparator = ", ";
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
            if (!SupportsSortGlypgs)
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

            base.OnColumnClick(e);            
        }
        
        protected internal virtual void UpdateGroup(SmartListViewItem item, string[] values)
        {
            StringBuilder sb = new StringBuilder();

            foreach(SmartColumn col in GroupColumns)
            {
                int c = col.AllColumnsIndex;
                if(c < values.Length)
                {
                    if(sb.Length > 0)
                        sb.Append(GroupSeparator);

                    sb.Append(values[c]);
                }
            }

            string g = sb.ToString();
            bool restart;
            do
            {
                restart = false;
                foreach (ListViewGroup grp in Groups)
                {
                    if (grp.Name == g && !grp.Items.Contains(item))
                        grp.Items.Add(item);
                    else if (0 == grp.Items.Count)
                    {
                        Groups.Remove(grp);
                        restart = true;
                        break;
                    }
                }
            }
            while (restart);

            if (item.Group == null)
            {
                string txt = string.IsNullOrEmpty(g) ? "<Rest>" : g;
                Groups.Add(g, txt).Items.Add(item);
            }

            if ((Groups.Count > 1) != ShowGroups)
            {
                if (Groups.Count > 1)
                {
                    ShowGroups = true;
                    foreach (ListViewItem i in Items)
                    {
                        SmartListViewItem si = i as SmartListViewItem;

                        if (si != null)
                            si.UpdateGroup();
                    }
                }
                else
                    ShowGroups = false;
            }

        }

        sealed class SmartListSorter : System.Collections.IComparer
        {
            SmartListView _view;

            public SmartListSorter(SmartListView view)
            {
                if (view == null)
                    throw new ArgumentNullException("view");

                _view = view;
            }

            #region IComparer Members

            int System.Collections.IComparer.Compare(object x, object y)
            {
                return Compare((ListViewItem)x, (ListViewItem)y);
            }

            private int Compare(ListViewItem x, ListViewItem y)
            {
                foreach (SmartColumn col in _view.SortColumns)
                {
                    int n = col.Compare(x, y);

                    if (n != 0)
                        return n;
                }

                return 0;
            }

            #endregion
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

                        if(osVersion.Major >= 6)
                            _osLevel = 600;
                        else if(osVersion.Major == 5)
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
    }
}
