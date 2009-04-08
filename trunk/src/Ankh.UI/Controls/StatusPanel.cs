using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Ankh.UI.Controls
{
	[Designer(typeof(StatusPanelDesigner))]
	class StatusPanel : Panel
	{
		StatusContainer _owner;
		string _title;
		string _summary;
		bool _collapsed;

		public StatusPanel()
		{
		}

		public StatusPanel(IContainer container)
		{
			if(container != null)
				container.Add(this);

			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AnchorStyles Anchor
		{
			get { return base.Anchor; }
			set { base.Anchor = value; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoSize
		{
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AutoSizeMode AutoSizeMode
		{
			get { return base.AutoSizeMode; }
			set { base.AutoSizeMode = value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new BorderStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}

		[DefaultValue(false)]
		public bool Collapsed
		{
			get { return _collapsed; }
			set 
			{
				if (_owner != null)
					_collapsed = _owner.SetCollapsed(this, value);
				else
					_collapsed = value;
			}
		}

		protected override Padding DefaultMargin
		{
			get
			{
				return new Padding(0, 0, 0, 0);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new DockStyle Dock
		{
			get { return base.Dock; }
			set { base.Dock = value; }
		}

		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new ScrollableControl.DockPaddingEdges DockPadding
		{
			get
			{
				return base.DockPadding;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new int Height
		{
			get
			{
				if (this.Collapsed)
				{
					return 0;
				}
				return base.Height;
			}
			set
			{
				base.Height = value;
			}
		}

		public bool ShouldSerializeHeight()
		{
			return (Height != DefaultSize.Height) && !Collapsed;
		}

		internal int HeightInternal
		{
			get
			{
				return base.Height;
			}
			set
			{
				base.Height = value;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(128, 32);
			}
		}

		Color _left = Color.Red;
		Color _right = SystemColors.Window;

		public Color HeaderLeft
		{
			get { return _left; }
			set { _left = value; }
		}

		public Color HeaderRight
		{
			get { return _right; }
			set { _right = value; }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Point PanelLocation
		{
			get { return base.Location - new Size(6, HeaderHeight); }
			set { base.Location = value + new Size(6, HeaderHeight); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public Size PanelSize
		{
			get { return Size + new Size(12, HeaderHeight + FooterHeight); }
			set { Size = value - new Size(12, HeaderHeight + FooterHeight); }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new Point Location
		{
			get
			{
				return base.Location;
			}
			set
			{
				base.Location = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int HeaderHeight
		{
			get { return 34; }
			set { }
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int FooterHeight
		{
			get { return 4; }
			set { }
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new Size MaximumSize
		{
			get
			{
				return base.MaximumSize;
			}
			set
			{
				base.MaximumSize = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new Size MinimumSize
		{
			get
			{
				return base.MinimumSize;
			}
			set
			{
				base.MinimumSize = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new Padding Margin
		{
			get
			{
				return base.Margin;
			}
			set
			{
				base.Margin = value;
			}
		}


		[Localizable(true), DefaultValue(""), Category("Panel")]
		public string Title
		{
			get { return _title ?? ""; }
			set { _title = value; }
		}

		[Localizable(true), DefaultValue(""), Category("Panel")]
		public string Summary
		{
			get { return _summary ?? ""; }
			set { _summary = value; }
		}

		protected internal StatusContainer Owner
		{
			get { return _owner; }
			internal set { _owner = value; }
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (Owner != null)
				Owner.ApplySizes(this);
		}
	}
}
