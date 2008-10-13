using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Ankh.UI.Blame
{
    class BlameMarginControl :Control
    {
        List<BlameSection> _sections;
        BlameToolWindowControl _control;
        int _firstLine;
        int _lastLine;

        internal void Init(BlameToolWindowControl control, List<BlameSection> sections)
        {
            _control = control;
            _sections = sections;
            DoubleBuffered = true;
        }

        bool _setLineHeight;
        int _lineHeight;
        int LineHeight
        {
            get
            {
                if (!_setLineHeight)
                {
                    _setLineHeight = true;
                    _lineHeight = _control.GetLineHeight();
                }
                return _lineHeight;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bool changed = false;
            foreach (BlameSection section in _sections.ToArray())
            {
                int top = (section.StartLine - _firstLine) * LineHeight;
                int height = (section.EndLine - section.StartLine + 1) * LineHeight;


                Rectangle rect = new Rectangle(0, top, Width, height);
                bool hovered = rect.Contains(e.Location);
                if (section.Hovered != hovered)
                {
                    section.Hovered = hovered;
                    changed = true;
                }

            }
            if (changed)
            {
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            bool changed = false;

            foreach (BlameSection section in _sections)
            {
                if (section.Hovered == true)
                    changed = true;

                section.Hovered = false;
            }
            if(changed)
                Invalidate();
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
            using (Brush black = new SolidBrush(Color.Black))
            using (Brush grayBg = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), BackColor, Color.LightGray))
            using (Brush blueBg = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), BackColor, Color.LightBlue))
            {
                foreach (BlameSection section in _sections.ToArray())
                {
                    if (section.EndLine < _firstLine)
                        continue;
                    if (section.StartLine > _lastLine)
                        continue;

                    int top = (section.StartLine - _firstLine) * LineHeight;
                    int height = (section.EndLine - section.StartLine + 1) * LineHeight;

                    Rectangle rect = new Rectangle(0, top, Width, height);
                    if (!e.ClipRectangle.IntersectsWith(rect))
                        continue;

                    if(section.Hovered)
                        e.Graphics.FillRectangle(blueBg, rect);
                    else
                        e.Graphics.FillRectangle(grayBg, rect);
                    e.Graphics.DrawRectangle(border, rect);


                    e.Graphics.DrawString(section.Revision.ToString(), f, black, new RectangleF(3, top + 2, 30, LineHeight), sfr);
                    e.Graphics.DrawString(section.Author, f, black, new RectangleF(35, top + 2, 40, LineHeight), sfl);
                    e.Graphics.DrawString(section.Time.ToShortDateString(), f, black, new RectangleF(Width - 60,top + 2, 58,LineHeight), sfr);

                }
            }
        }

        internal void NotifyScroll(int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
        {
            int dy = (_firstLine - iFirstVisibleUnit) * LineHeight;
            _firstLine = iFirstVisibleUnit;
            _lastLine = iVisibleUnits + iFirstVisibleUnit;

            NativeMethods.ScrollWindowEx(Handle, 0, dy, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, NativeMethods.SW_INVALIDATE);
            //Invalidate();
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
