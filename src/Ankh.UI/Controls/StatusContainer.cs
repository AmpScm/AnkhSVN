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
				int height = panel.HeaderHeight + panel.Height + panel.FooterHeight;
				panel.PanelSize = new Size(clSize.Width - 2, height);

				i += height + 4;
			}
		}

		internal void DoAddPanel(Control value)
		{
			//throw new NotImplementedException();
		}

		internal bool InsertingItem()
		{
			return false;
		}




		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			int width = ClientSize.Width;
			foreach (StatusPanel panel in Controls)
			{
				if (!panel.Visible)
					continue;

				Point pl = panel.PanelLocation;
				Size pz = panel.PanelSize;
				int hh = panel.HeaderHeight;

				e.Graphics.DrawRectangle(SystemPens.ControlDark, new Rectangle(pl, pz));
				e.Graphics.DrawLine(SystemPens.ControlDark, pl.X+1, pl.Y+hh-1, pl.X + pz.Width, pl.Y+hh-1);

				using (Brush brush = new LinearGradientBrush(pl, pl+new Size(width,0), panel.HeaderLeft, panel.HeaderRight))
					e.Graphics.FillRectangle(brush, new Rectangle(pl.X+1,pl.Y+1, pz.Width-2, panel.HeaderHeight-2));
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
