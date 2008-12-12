// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.ComponentModel;

namespace Ankh.UI.VSSelectionControls
{
    public class SmartColumn : ColumnHeader, IComparer<ListViewItem>
    {
        IComparer<ListViewItem> _sorter;
        readonly SmartListView _view;
        string _menuText;
        bool _notSortable;
        bool _notHideable;
        bool _notMoveable;
        bool _groupable;
        bool _notDragable;
        bool _reverseSort;

        static SmartListView FindView(IContainer container)
        {
            foreach(Component c in container.Components)
            {
                SmartListView slv = c as SmartListView;

                if(slv != null)
                    return slv;
            }
            return null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartColumn"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public SmartColumn(IContainer container)
            : this(FindView(container))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartColumn"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public SmartColumn(SmartListView view)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            _view = view;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartColumn"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="text">The text.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="width">The width.</param>
        public SmartColumn(SmartListView view, string text, string menuText, int width)
            : this(view, text, menuText, width, HorizontalAlignment.Left)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartColumn"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="text">The text.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="width">The width.</param>
        /// <param name="align">The align.</param>
        public SmartColumn(SmartListView view, string text, string menuText, int width, HorizontalAlignment align)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            _view = view;
            Text = text;
            MenuText = menuText;
            Width = width;
            TextAlign = align;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartColumn"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="width">The width.</param>
        public SmartColumn(SmartListView view, string menuText, int width)
            : this(view, menuText, width, HorizontalAlignment.Left)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartColumn"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="menuText">The menu text.</param>
        /// <param name="width">The width.</param>
        /// <param name="align">The align.</param>
        public SmartColumn(SmartListView view, string menuText, int width, HorizontalAlignment align)
            : this(view, menuText.Replace("&", ""), menuText, width, align)
        {
        }

        /// <summary>
        /// Gets or sets the menu text.
        /// </summary>
        /// <value>The menu text.</value>
        public string MenuText
        {
            get { return _menuText ?? Text; }
            set { _menuText = value ?? ""; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SmartColumn"/> is sortable.
        /// </summary>
        /// <value><c>true</c> if sortable; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Sortable
        {
            get { return !_notSortable; }
            set { _notSortable = !value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SmartColumn"/> is hideable.
        /// </summary>
        /// <value><c>true</c> if hideable; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Hideable
        {
            get { return !_notHideable; }
            set { _notHideable = !value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SmartColumn"/> is moveable.
        /// </summary>
        /// <value><c>true</c> if moveable; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Moveable
        {
            get { return !_notMoveable; }
            set { _notMoveable = !value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SmartColumn"/> is dragable.
        /// </summary>
        /// <value><c>true</c> if dragable; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool Dragable
        {
            get { return !_notDragable; }
            set { _notDragable = !value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SmartColumn"/> is groupable.
        /// </summary>
        /// <value><c>true</c> if groupable; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool Groupable
        {
            get { return _groupable; }
            set { _groupable = value; }
        }

        /// <summary>
        /// Gets or sets the sorter.
        /// </summary>
        /// <value>The sorter.</value>
        [DefaultValue(null)]
        public IComparer<ListViewItem> Sorter
        {
            get { return _sorter; }
            set { _sorter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [reverse sort].
        /// </summary>
        /// <value><c>true</c> if [reverse sort]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool ReverseSort
        {
            get { return _reverseSort; }
            set { _reverseSort = value; }
        }

        int _allColumnsIndex = -1;

        /// <summary>
        /// Gets the index of all columns.
        /// </summary>
        /// <value>The index of all columns.</value>
        public int AllColumnsIndex
        {
            get { return (_allColumnsIndex >= 0) ? _allColumnsIndex : (_allColumnsIndex = _view.AllColumns.IndexOf(this)); }
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
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

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
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
