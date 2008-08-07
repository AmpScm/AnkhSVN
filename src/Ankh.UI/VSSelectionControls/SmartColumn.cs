using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Ankh.UI.VSSelectionControls
{
    public class SmartColumn : ColumnHeader, IComparer<ListViewItem>
    {
        IComparer<ListViewItem> _sorter;
        readonly SmartListView _list;
        string _menuText;
        bool _notSortable;
        bool _notHideable;
        bool _notMoveable;
        bool _groupable;
        bool _notDragable;
        bool _reverseSort;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartColumn"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="text">The text.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="width">The width.</param>
        public SmartColumn(SmartListView list, string text, string menuText, int width)
            : this(list,text, menuText, width, HorizontalAlignment.Left)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartColumn"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="text">The text.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="width">The width.</param>
        /// <param name="align">The align.</param>
        public SmartColumn(SmartListView list, string text, string menuText, int width, HorizontalAlignment align)
        {
            if(list == null)
                throw new ArgumentNullException("list");

            _list = list;
            Text = text;
            MenuText = menuText;
            Width = width;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartColumn"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="width">The width.</param>
        public SmartColumn(SmartListView list, string menuText, int width)
            : this(list, menuText, width, HorizontalAlignment.Left)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartColumn"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="width">The width.</param>
        /// <param name="align">The align.</param>
        public SmartColumn(SmartListView list, string menuText, int width, HorizontalAlignment align)
            : this(list, menuText.Replace("&", ""), menuText, width)
        {
        }

        public string MenuText
        {
            get { return _menuText ?? Text; }
            set { _menuText = value ?? ""; }
        }

        [DefaultValue(true)]
        public bool Sortable
        {
            get { return !_notSortable; }
            set { _notSortable = !value; }
        }

        [DefaultValue(true)]
        public bool Hideable
        {
            get { return !_notHideable; }
            set { _notHideable = !value; }
        }

        [DefaultValue(true)]
        public bool Moveable
        {
            get { return !_notMoveable; }
            set { _notMoveable = !value; }
        }

        [DefaultValue(true)]
        public bool Dragable
        {
            get { return !_notDragable; }
            set { _notDragable = !value; }
        }

        [DefaultValue(false)]
        public bool Groupable
        {
            get { return _groupable; }
            set { _groupable = value; }
        }
       
        [DefaultValue(null)]
        public IComparer<ListViewItem> Sorter
        {
            get { return _sorter; }
            set { _sorter = value; }
        }

        [DefaultValue(false)]
        public bool ReverseSort
        {
            get { return _reverseSort; }
            set { _reverseSort = value; }
        }

        int _allColumnsIndex = -1;

        public int AllColumnsIndex
        {
            get { return (_allColumnsIndex >= 0) ? _allColumnsIndex : (_allColumnsIndex = _list.AllColumns.IndexOf(this)); }
        }

        protected virtual int Compare(ListViewItem x, ListViewItem y)
        {
            int n;
            if (x == null)
                n = -1;
            else if (y == null)
                n = 1;
            else if (_sorter != null)
                n = _sorter.Compare(x, y);
            else
            {
                SmartListViewItem sX = x as SmartListViewItem;
                SmartListViewItem sY = y as SmartListViewItem;
                string vX, vY;

                if (sX != null)
                    vX = sX.GetValue(AllColumnsIndex);
                else if (Index >= 0 && Index < x.SubItems.Count)
                    vX = x.SubItems[Index].Text;
                else
                    vX = null;

                if (sY != null)
                    vY = sY.GetValue(AllColumnsIndex);
                else if (Index >= 0 && Index < y.SubItems.Count)
                    vY = y.SubItems[Index].Text;
                else
                    vY = null;

                n = StringComparer.OrdinalIgnoreCase.Compare(vX, vY);
            }

            return n;
        }

        int IComparer<ListViewItem>.Compare(ListViewItem x, ListViewItem y)
        {
            return Compare(x, y);
        }

        /// <summary>
        /// Compares the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="reverseSort">if set to <c>true</c> [reverse sort].</param>
        /// <returns></returns>
        public int Compare(ListViewItem x, ListViewItem y, bool reverseSort)
        {
            int n = Compare(x, y);

            if(reverseSort && ReverseSort)
                return -n;
            else
                return n;
        }
    }
}
