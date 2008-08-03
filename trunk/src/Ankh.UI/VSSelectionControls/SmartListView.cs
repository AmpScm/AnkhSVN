﻿using System;
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
        Up,
        Down
    }

    public class SmartListView : ListView
    {
        readonly Collection<SmartColumn> _groupColumns = new Collection<SmartColumn>();
        readonly Collection<SmartColumn> _sortColumns = new Collection<SmartColumn>();
        readonly Collection<SmartColumn> _allColumns = new Collection<SmartColumn>();

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
                case SortIcon.Up:
                    hdItem.fmt |= NativeMethods.HDF_SORTUP;
                    break;
                case SortIcon.Down:
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
    }
}
