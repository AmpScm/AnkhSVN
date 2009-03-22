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
using System.ComponentModel;
using System.Windows.Forms;

using Ankh.UI.VSSelectionControls;
using System.Drawing;

namespace Ankh.UI.SvnLog
{
    class LogRevisionView : ListViewWithSelection<LogRevisionItem>
    {
        readonly List<LogRevisionItem> _items = new List<LogRevisionItem>();
        public LogRevisionView()
        {
            Sorting = SortOrder.None;
            OwnerDraw = true;
            Init();
        }
        public LogRevisionView(IContainer container)
            : this()
        {
            container.Add(this);
        }

        LogDataSource _dataSource;
        public LogDataSource LogSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        SmartColumn _revisionColumn;
        SmartColumn _messageColumn;
        void Init()
        {
            _revisionColumn = new SmartColumn(this, "&Revision", 64, HorizontalAlignment.Right);
            SmartColumn author = new SmartColumn(this, "&Author", 73);
            SmartColumn date = new SmartColumn(this, "&Date", 118);
            SmartColumn issue = new SmartColumn(this, "&Issue", 60);
            _messageColumn = new SmartColumn(this, "&Message", 300);

            _revisionColumn.Sortable = author.Sortable = date.Sortable = issue.Sortable = _messageColumn.Sortable = false;


            AllColumns.Add(_revisionColumn);
            AllColumns.Add(author);

            AllColumns.Add(date);
            AllColumns.Add(issue);
            AllColumns.Add(_messageColumn);

            // The listview can't align the first column right. We switch their display position
            // to work around this            
            Columns.AddRange(
                new ColumnHeader[]
                {
                    _revisionColumn,
                    author,
                    date,
                    _messageColumn
                });
        }

        protected override void OnRetrieveSelection(RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new LogItem(e.Item, LogSource.RepositoryRoot);
            base.OnRetrieveSelection(e);
        }

        protected override void OnResolveItem(ResolveItemEventArgs e)
        {
            e.Item = ((LogItem)e.SelectionItem).ListViewItem;
            base.OnResolveItem(e);
        }

        public List<LogRevisionItem> VirtualItems
        {
            get { return _items; }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            OnScrolled(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x115) // WM_VSCROLL
                OnScrolled(EventArgs.Empty);
        }

        public event EventHandler Scrolled;
        private void OnScrolled(EventArgs e)
        {
            if (Scrolled != null)
                Scrolled(this, e);
        }   

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (!DesignMode && _messageColumn != null)
                ResizeColumnsToFit(_messageColumn);
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            bool isSelected = SelectedIndices.Contains(e.ItemIndex);

            if (!isSelected)
                e.DrawBackground();
            else
            {
                Rectangle b = e.Bounds;
                b.X += 4;
                b.Width -= 4;
                if (Focused)
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, b);
                else
                    e.Graphics.FillRectangle(SystemBrushes.Menu, b);
            }


            
            if ((e.State & ListViewItemStates.Focused) != 0)
                e.DrawFocusRectangle();
            //    ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);

            // Draw the item text for views other than the Details view.
            if (View != View.Details)
            {
                e.DrawText();
            }
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            // Okay, bugs in .Net 2.0 ahead:
            // * Using e.DrawText(flags) gives us a margin of one " " on left and right. (Confirmed via reflector)
            // * We can't trust the selected flag in e.ItemState (???)
            bool isSelected = SelectedIndices.Contains(e.ItemIndex);

            string text = (e.ItemIndex == -1) ? e.Item.Text : e.SubItem.Text;

            if (e.ColumnIndex == 0)
            {
                // TODO: Update bounds for indent levels
                text = ((LogRevisionItem)e.Item).RevisionText;
            }

            var fnt = e.Item.Font;
            Color clr;

            if (isSelected)
                clr = Focused ? SystemColors.HighlightText : SystemColors.MenuText;
            else
                clr = e.Item.ForeColor;

            HorizontalAlignment textAlign = e.Header.TextAlign;
            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix |
                ((textAlign == HorizontalAlignment.Left) ? TextFormatFlags.GlyphOverhangPadding : ((textAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Right));
            flags |= TextFormatFlags.WordEllipsis;

            TextRenderer.DrawText(e.Graphics, text, fnt, e.Bounds, clr, flags);
        }
    }
}
