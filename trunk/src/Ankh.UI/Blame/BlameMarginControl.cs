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

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
                return;

            using (Font f = new Font(Font.FontFamily, 7F))
            using (Pen border = new Pen(Color.Gray))
            using (Brush black = new SolidBrush(Color.Black))
            using (Brush bg = new LinearGradientBrush(new Point(0, 0), new Point(Width, 0), Color.White, Color.LightGray))
            {
                foreach (BlameSection section in _sections.ToArray())
                {
                    if (section.EndLine < _firstLine)
                        continue;
                    if (section.StartLine > _lastLine)
                        continue;

                    int top = (section.StartLine - _firstLine) * LineHeight;
                    int height = (section.EndLine - section.StartLine + 1) * LineHeight;


                    e.Graphics.FillRectangle(bg, new Rectangle(1, top + 1, Width - 2, height - 2));
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
