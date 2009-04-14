using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ankh.UI.Controls
{
    [Designer(typeof(StatusContainerDesigner))]
    public class StatusContainer : ContainerControl
    {
        public StatusContainer()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new StatusContainerTypedControlCollection(this);
        }

        sealed class StatusContainerTypedControlCollection : ControlCollection
        {
            StatusContainer _owner;

            public StatusContainerTypedControlCollection(StatusContainer owner)
                : base(owner)
            {
                _owner = owner;
            }
            public override void Add(Control value)
            {
                StatusPanel sp = value as StatusPanel;
                if (sp == null)
                    throw new ArgumentException("Not a status panel", "value");

                if (!_owner.InsertingItem())
                {
                    _owner.DoAddPanel(value);
                }

                base.Add(value);

                ISite site = _owner.Site;
                if (site != null && site.Container != null)
                {
                    site.Container.Add(value);
                }

                _owner.ApplySizes(sp);
                sp.Owner = _owner;
            }
        }

        internal bool SetCollapsed(StatusPanel statusPanel, bool value)
        {
            return value;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ApplySizes(null);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            ApplySizes(null);
        }

        protected override void AdjustFormScrollbars(bool displayScrollbars)
        {
            base.AdjustFormScrollbars(displayScrollbars);
            ApplySizes(null);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            ApplySizes(null);
        }

        internal void ApplySizes(StatusPanel first)
        {
            int i = 0;
            Size clSize = ClientSize;
            foreach (StatusPanel panel in Controls)
            {
                if (first != null && first != panel)
                    continue;
                else
                    first = null;

                if (!panel.Visible)
                    continue;

                panel.PanelLocation = new Point(0, i);
                int height = panel.PanelSize.Height;
                panel.PanelSize = new Size(clSize.Width - 2, height);

                i += height + 1 + PanelSpace;
            }

            this.AutoScrollMinSize = new Size(Width - SystemInformation.VerticalScrollBarWidth - 2, i + 1);
            Invalidate();
        }

        internal void DoAddPanel(Control value)
        {
            //throw new NotImplementedException();
        }

        internal bool InsertingItem()
        {
            return false;
        }

        int _panelSpace = 3;
        [DefaultValue(3), Localizable(false)]
        public int PanelSpace
        {
            get { return _panelSpace; }
            set { _panelSpace = Math.Max(0, value); ApplySizes(null); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            int width = ClientSize.Width;
            Font fnt = null;
            StringFormat fmt = null;
            try
            {
                foreach (StatusPanel panel in Controls)
                {
                    if (!panel.Visible)
                        continue;

                    Point pl = panel.PanelLocation;
                    Size pz = panel.PanelSize;
                    int hh = panel.HeaderHeight;

                    e.Graphics.DrawRectangle(SystemPens.ControlDark, new Rectangle(pl, pz));
                    e.Graphics.DrawLine(SystemPens.ControlDark, pl.X + 1, pl.Y + hh + 1, pl.X + pz.Width, pl.Y + hh + 1);

                    using (Brush brush = new LinearGradientBrush(pl, pl + new Size(width, 0), panel.GradientLeft, panel.GradientRight))
                        e.Graphics.FillRectangle(brush, new Rectangle(pl.X + 1, pl.Y + 1, pz.Width - 1, panel.HeaderHeight));

                    Image img = panel.HeaderImage;

                    if (img != null)
                        e.Graphics.DrawImageUnscaled(img, pl + new Size((hh - img.Width) / 2, (hh - img.Height) / 2));

                    if (!string.IsNullOrEmpty(panel.Title))
                    {
                        if (fnt == null)
                        {
                            Font f = Font;
                            fnt = new Font(SystemFonts.CaptionFont.FontFamily, f.SizeInPoints * 1.5f, FontStyle.Regular);
                            fmt = new StringFormat(StringFormatFlags.NoWrap);
                            fmt.LineAlignment = StringAlignment.Center;
                        }

                        int x = pl.X + hh;
                        e.Graphics.DrawString(panel.Title, fnt, SystemBrushes.ControlText, new RectangleF(x, pl.Y + 1, pz.Width - x, hh), fmt);
                    }
                }
            }
            finally
            {
                if (fnt != null)
                    fnt.Dispose();
            }
        }

        private static GraphicsPath RoundedRectPath(Rectangle rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);

            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);

            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }
    }
}
