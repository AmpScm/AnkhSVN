using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

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
            foreach (BlameSection section in _sections.ToArray())
            {
                int top = (section.StartLine - _firstLine) * LineHeight;
                int height = (section.EndLine - section.StartLine + 1) * LineHeight;


                Rectangle rect = new Rectangle(0, top, Width, height);

                section.Hovered = rect.Contains(e.Location);

            }
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            foreach (BlameSection section in _sections)
                section.Hovered = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
                return;

            using (Font f = new Font(Font.FontFamily, 7F))
            using (Pen border = new Pen(Color.Gray))
            using (Brush black = new SolidBrush(Color.Black))
            using (Brush grayBg = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), BackColor, Color.DarkGray))
            using (Brush blueBg = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), BackColor, SystemColors.Highlight))
            {
                foreach (BlameSection section in _sections.ToArray())
                {
                    if (section.EndLine < _firstLine)
                        continue;
                    if (section.StartLine > _lastLine)
                        continue;

                    int top = (section.StartLine - _firstLine) * LineHeight;
                    int height = (section.EndLine - section.StartLine + 1) * LineHeight;

                    if(section.Hovered)
                        e.Graphics.FillRectangle(blueBg, new Rectangle(0, top, Width, height));
                    else
                        e.Graphics.FillRectangle(grayBg, new Rectangle(0, top, Width, height));
                    e.Graphics.DrawRectangle(border, new Rectangle(0, top, Width, height));

                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Far;

                    e.Graphics.DrawString(section.Revision.ToString(), f, black, new RectangleF(3, top + 2, 30, LineHeight), sf);
                    e.Graphics.DrawString(section.Author, f, black, new RectangleF(35, top + 2, 40, LineHeight));
                    e.Graphics.DrawString(section.Time.ToShortDateString(), f, black, new RectangleF(Width - 60,top + 2, 58,LineHeight), sf);

                }
            }
        }

        internal void NotifyScroll(int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
        {
            _firstLine = iFirstVisibleUnit;
            _lastLine = iVisibleUnits + iFirstVisibleUnit;

            Invalidate();
        }
    }
}
