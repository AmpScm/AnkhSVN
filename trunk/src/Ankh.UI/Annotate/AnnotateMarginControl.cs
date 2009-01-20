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
using System.Diagnostics;

namespace Ankh.UI.Annotate
{
    sealed class AnnotateMarginControl : Control
    {
        List<AnnotateRegion> _regions;
        AnnotateEditorControl _control;
        int _firstLine;
        int _lastLine;
        IAnkhServiceProvider _context;
        ToolTip _toolTip;

        AnnotateRegion _hoverSection;
        AnnotateRegion _tipSection;

        public AnnotateMarginControl()
        {
            _regions = new List<AnnotateRegion>();

            _toolTip = new ToolTip();
            _toolTip.ShowAlways = true;
        }

        internal void Init(IAnkhServiceProvider context, AnnotateEditorControl control, List<AnnotateRegion> sections)
        {
            _context = context;
            _control = control;
            _regions = sections;
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (DesignMode)
                return;

            DoMove(e);
            
            _in = true;
            if (_tipSection == null && _hoverSection != null)
                ShowTip(_hoverSection);
        }

        void ShowTip(AnnotateRegion region)
        {            
            Point mp = PointToClient(MousePosition);
            Rectangle rect = GetRectangle(region);

            if (rect.Contains(mp))
            {
                AnnotateSource src = region.Source;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Revision: {0}", src.Revision);
                sb.AppendLine();
                sb.AppendFormat("Author: {0}", src.Author);
                sb.AppendLine();
                sb.AppendFormat("Time: {0}", src.Time.ToLocalTime());
                sb.AppendLine();

                string msg = src.LogMessage;

                if (msg == null)
                    sb.Append("<loading logmessages>");
                else
                    sb.AppendLine(msg);

                _toolTip.Show(sb.ToString(), this, mp);
                _tipSection = region;
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (!DesignMode)
                Invalidate();
        }

        class FindLocation : IComparer<AnnotateRegion>
        {
            readonly Point _location;
            readonly AnnotateMarginControl _ctrl;

            public FindLocation(AnnotateMarginControl ctrl, Point location)
            {
                _ctrl = ctrl;
                _location = location;
            }


            #region IComparer<AnnotateRegion> Members

            public int Compare(AnnotateRegion x, AnnotateRegion y)
            {
                bool xn = (x == null);
                bool yn = (y == null);
                if(xn)
                {
                    return yn ? 0 : - Compare(y,x);
                }
                else if (!yn)
                    return Comparer<AnnotateRegion>.Default.Compare(x, y);

                Rectangle r = _ctrl.GetRectangle(x);

                if (_location.Y < r.Top)
                    return 1;
                else if (_location.Y >= r.Bottom)
                    return -1;

                return 0;
            }

            #endregion
        }

        AnnotateRegion GetSection(Point location)
        {
            // Find the current region via binary search
            int n = _regions.BinarySearch(null,
                new FindLocation(this, location));

            if (n >= 0 && n < _regions.Count)
                return _regions[n];

            return null;
        }

        bool _in;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (DesignMode)
                return;

            DoMove(e);

            if (_in && _tipSection == null && _hoverSection != null)
                ShowTip(_hoverSection);
        }

        void DoMove(EventArgs e)
        {
            AnnotateRegion region = GetSection(PointToClient(MousePosition));

            if(_hoverSection != null && _hoverSection != region)
            {
                _hoverSection.Hovered = false;
                Invalidate(GetRectangle(_hoverSection));
                _hoverSection = null;                
            }

            if(region != _hoverSection)
            {
                _hoverSection = region;
                region.Hovered = true;
                Invalidate(GetRectangle(region));
            }
            
            if (_tipSection != null && (!_in || _tipSection != region))
            {
                _tipSection = null;
                _toolTip.Hide(this);

                if (_in && region != null)
                {
                    ShowTip(region);
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (DesignMode)
                return;

            _in = false;
            DoMove(e);

            if (_hoverSection != null)
            {
                _hoverSection.Hovered = false;
                Invalidate(GetRectangle(_hoverSection));
                _hoverSection = null;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            DoMove(e);
            Focus();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            DoMove(e);

            AnnotateRegion section = GetSection(e.Location);

            if (section == null)
                _control.SetSelection(null);
            else
                _control.SetSelection(section.Source);

            Invalidate();            

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
            {
                using (Brush grayBg = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), BackColor, Color.LightGray))
                {
                    pevent.Graphics.FillRectangle(grayBg, ClientRectangle);
                }
            }
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
                foreach (AnnotateRegion region in _regions)
                {
                    if (region.EndLine < _firstLine)
                        continue;
                    if (region.StartLine > _lastLine)
                        break;

                    Rectangle rect = GetRectangle(region);
                    if (!e.ClipRectangle.IntersectsWith(rect))
                        continue;

                    if (IsSelected(region))
                        e.Graphics.FillRectangle(selectedBg, rect);
                    else
                    {
                        if (region.Hovered)
                            e.Graphics.FillRectangle(blueBg, rect);
                        else
                            e.Graphics.FillRectangle(grayBg, rect);
                    }
                    e.Graphics.DrawRectangle(border, rect);

                    AnnotateSource src = region.Source;

                    Brush color = IsSelected(region) ? selectedTextColor : textColor;
                    e.Graphics.DrawString(src.Revision.ToString(), f, color, new RectangleF(3, rect.Top + 2, 30, LineHeight), sfr);
                    e.Graphics.DrawString(src.Author, f, color, new RectangleF(35, rect.Top + 2, 40, LineHeight), sfl);
                    e.Graphics.DrawString(src.Time.ToShortDateString(), f, color, new RectangleF(Width - 60, rect.Top + 2, 58, LineHeight), sfr);
                }

                Rectangle clip = e.ClipRectangle;
                if (_regions.Count > 0)
                {
                    Rectangle rect = GetRectangle(_regions[_regions.Count - 1]);

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
            if (scrollInfo.Orientation != ScrollOrientation.VerticalScroll)
                return;

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
        }
    }
}
