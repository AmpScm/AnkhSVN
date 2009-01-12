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
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Ankh.Selection;
using Ankh.UI.VSSelectionControls;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.UI.PendingChanges;

namespace Ankh.UI.Annotate
{
    sealed class AnnotateMarginControl : Control
    {
        List<AnnotateRegion> _sections;
        AnnotateEditorControl _control;
        int _firstLine;
        int _lastLine;
        IAnkhServiceProvider _context;
        ToolTip _toolTip;

        AnnotateRegion _hoverSection;
        AnnotateRegion _tipSection;

        public AnnotateMarginControl()
        {
            _sections = new List<AnnotateRegion>();

            _toolTip = new ToolTip();
            _toolTip.ShowAlways = true;
        }

        internal void Init(IAnkhServiceProvider context, AnnotateEditorControl control, List<AnnotateRegion> sections)
        {
            _context = context;
            _control = control;
            _sections = sections;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            Point mp = PointToClient(MousePosition);

            if (_tipSection != null)
            {
                if (GetRectangle(_tipSection).Contains(mp))
                    return;
                else
                {
                    _tipSection = null;
                    _toolTip.Hide(this);

                }
            }

            AnnotateRegion section = _hoverSection;
            if (section == null)
                return;

            Rectangle rect = GetRectangle(section);

            if (rect.Contains(mp))
            {
                AnnotateSource src = section.Source;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Revision: {0}", src.Revision);
                sb.AppendLine();
                sb.AppendFormat("Author: {0}", src.Author);
                sb.AppendLine();
                sb.AppendFormat("Time: {0}", src.Time.ToLocalTime());

                _toolTip.Show(sb.ToString(), this, mp);
                _tipSection = section;
                return;
            }
        }

        bool _setLineHeight;
        int _lineHeight;
        int LineHeight
        {
            get
            {
                if (!_setLineHeight && _control != null)
                {
                    _setLineHeight = true;
                    _lineHeight = _control.GetLineHeight();
                }
                return _lineHeight;
            }
        }

        Rectangle GetRectangle(AnnotateRegion section)
        {
            int top = (section.StartLine - _firstLine) * LineHeight;
            int height = (section.EndLine - section.StartLine + 1) * LineHeight;

            return new Rectangle(0, top, Width, height);
        }

        AnnotateRegion GetSection(Point location)
        {
            foreach (AnnotateRegion section in _sections)
            {
                Rectangle rect = GetRectangle(section);
                if (rect.Contains(location))
                    return section;
            }
            return null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            foreach (AnnotateRegion section in _sections)
            {
                Rectangle rect = GetRectangle(section);
                bool hovered = rect.Contains(e.Location);
                if (section.Hovered != hovered)
                {
                    section.Hovered = hovered;

                    Invalidate(rect);

                    if (hovered)
                        _hoverSection = section;
                }
            }

            if (_tipSection != null && _tipSection != _hoverSection)
            {
                _tipSection = null;
                _toolTip.Hide(this);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            bool changed = false;

            foreach (AnnotateRegion section in _sections)
            {
                if (section.Hovered == true)
                {
                    changed = true;

                    section.Hovered = false;
                }
            }
            if (changed)
                Invalidate();

            _hoverSection = null;

            if (_tipSection != null)
            {
                _tipSection = null;
                _toolTip.Hide(this);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            AnnotateRegion section = GetSection(e.Location);
            _control.SetSelection(section.Source);
            Invalidate();

            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Right)
                ShowContextMenu();
        }

        private void ShowContextMenu()
        {
            Point p = MousePosition;

            if (_context != null)
            {
                IAnkhCommandService cs = _context.GetService<IAnkhCommandService>();
                cs.ShowContextMenu(AnkhCommandMenu.AnnotateContextMenu, p.X, p.Y);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (DesignMode)
                base.OnPaintBackground(pevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
                return;

            StringFormat sfr = new StringFormat();
            sfr.Alignment = StringAlignment.Far;

            StringFormat sfl = new StringFormat();
            sfl.Trimming = StringTrimming.EllipsisCharacter;
            sfl.FormatFlags = StringFormatFlags.NoWrap;

            using (Font f = new Font(Font.FontFamily, 7F))
            using (Pen border = new Pen(Color.Gray))
            using (Brush textColor = new SolidBrush(Color.Black))
            using (Brush selectedTextColor = new SolidBrush(SystemColors.HighlightText))
            using (Brush grayBg = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), BackColor, Color.LightGray))
            using (Brush blueBg = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), BackColor, Color.LightBlue))
            using (Brush selectedBg = new SolidBrush(SystemColors.Highlight))
            {
                foreach (AnnotateRegion section in _sections)
                {
                    if (section.EndLine < _firstLine)
                        continue;
                    if (section.StartLine > _lastLine)
                        break;

                    Rectangle rect = GetRectangle(section);
                    if (!e.ClipRectangle.IntersectsWith(rect))
                        continue;

                    if (IsSelected(section))
                        e.Graphics.FillRectangle(selectedBg, rect);
                    else
                    {
                        if (section.Hovered)
                            e.Graphics.FillRectangle(blueBg, rect);
                        else
                            e.Graphics.FillRectangle(grayBg, rect);
                    }
                    e.Graphics.DrawRectangle(border, rect);

                    AnnotateSource src = section.Source;

                    Brush color = IsSelected(section) ? selectedTextColor : textColor;
                    e.Graphics.DrawString(src.Revision.ToString(), f, color, new RectangleF(3, rect.Top + 2, 30, LineHeight), sfr);
                    e.Graphics.DrawString(src.Author, f, color, new RectangleF(35, rect.Top + 2, 40, LineHeight), sfl);
                    e.Graphics.DrawString(src.Time.ToShortDateString(), f, color, new RectangleF(Width - 60, rect.Top + 2, 58, LineHeight), sfr);
                }

                Rectangle clip = e.ClipRectangle;
                if (_sections.Count > 0)
                {
                    Rectangle rect = GetRectangle(_sections[_sections.Count - 1]);

                    if (e.ClipRectangle.Top <= rect.Bottom)
                        clip = new Rectangle(clip.Left, rect.Bottom + 1, clip.Width, clip.Bottom);
                }

                using (SolidBrush sb = new SolidBrush(BackColor))
                    e.Graphics.FillRectangle(sb, clip);
            }
        }

        bool IsSelected(AnnotateRegion region)
        {
            if (region == null)
                throw new ArgumentNullException("region");
            else if(_control == null)
                return false;

            AnnotateSource src = _control.Selected;

            if (src == null)
                return false;

            if (region.Source == src)
                return true;

            return false;            
        }

        internal void NotifyScroll(TextViewScrollEventArgs scrollInfo)
        {
            int dy = (_firstLine - scrollInfo.FirstVisibleUnit) * LineHeight;
            _firstLine = scrollInfo.FirstVisibleUnit;
            _lastLine = scrollInfo.VisibleUnits + scrollInfo.FirstVisibleUnit;

            NativeMethods.ScrollWindowEx(Handle, 0, dy, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, NativeMethods.SW_INVALIDATE);
        }

        static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern int ScrollWindowEx(IntPtr hwnd, int dx, int dy, IntPtr prcScroll, IntPtr prcClip, IntPtr hrgnUpdate, IntPtr prcUpdate, uint flags);

            public const uint SW_SCROLLCHILDREN = 0x0001;  /* Scroll children within *lprcScroll. */
            public const uint SW_INVALIDATE = 0x0002;  /* Invalidate after scrolling */
            public const uint SW_ERASE = 0x0004;  /* If SW_INVALIDATE, don't send WM_ERASEBACKGROUND */

            public const uint SW_SMOOTHSCROLL = 0x0010;
        }




    }
}
