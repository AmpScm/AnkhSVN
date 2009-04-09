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
        Image _panelImage;
        string _title;
        string _summary;
        bool _collapsed;
        int _headerHeight = 30;

        public StatusPanel()
        {
        }

        public StatusPanel(IContainer container)
        {
            if (container != null)
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
                return new Padding(3, 3, 3, 3);
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = value; }
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

        Color _gradientLeft = SystemColors.Window;
        Color _gradientRight = SystemColors.Window;

        [Category("Panel")]
        public Color GradientLeft
        {
            get { return _gradientLeft; }
            set
            {
                if (_gradientLeft != value)
                {
                    _mode = StatusPanelMode.None;
                    _gradientLeft = value;

                    if (Owner != null && Owner.IsHandleCreated)
                        Owner.Invalidate();
                }
            }
        }

        bool ShouldSerializeGradientLeft()
        {
            return (_mode == StatusPanelMode.None) && GradientLeft != SystemColors.Window;
        }

        [Category("Panel")]
        public Color GradientRight
        {
            get { return _gradientRight; }
            set
            {
                if (_gradientRight != value)
                {
                    _mode = StatusPanelMode.None;
                    _gradientRight = value;

                    if (Owner != null && Owner.IsHandleCreated)
                        Owner.Invalidate();
                }
            }
        }

        bool ShouldSerializeGradientRight()
        {
            return (_mode == StatusPanelMode.None) && GradientRight != SystemColors.Window;
        }


        [Category("Panel")]
        public Image HeaderImage
        {
            get { return _panelImage; }
            set
            {
                if (_panelImage != value)
                {
                    _mode = StatusPanelMode.None;
                    _panelImage = value;
                    if (Owner != null && Owner.IsHandleCreated)
                        Owner.Invalidate();
                }
            }
        }

        bool ShouldSerializeHeaderImage()
        {
            return _mode == StatusPanelMode.None && HeaderImage != null;
        }

        bool ShouldSerializeHeaderRight()
        {
            return (_mode == StatusPanelMode.None) && GradientRight != SystemColors.Window;
        }

        StatusPanelMode _mode;
        [DefaultValue(StatusPanelMode.None), Localizable(false), Category("Panel")]
        public StatusPanelMode PanelMode
        {
            get { return _mode; }
            set
            {
                switch (value)
                {
                    case StatusPanelMode.None:
                        return;
                    default:
                        throw new InvalidOperationException();
                    case StatusPanelMode.Error:
                        _gradientLeft = Color.FromArgb(0xB0, 0xFF, 0, 0);
                        _panelImage = StatusPanelResources.Status_Error;
                        break;
                    case StatusPanelMode.Warning:
                        _gradientLeft = Color.FromArgb(0xB0, 0xFF, 0xFF, 0);
                        _panelImage = StatusPanelResources.Status_Warning;
                        break;
                    case StatusPanelMode.Ok:
                        _gradientLeft = Color.PaleGreen;
                        _panelImage = StatusPanelResources.Status_Ok;
                        break;
                    case StatusPanelMode.Suggestion:
                        _gradientLeft = (Owner ?? (Control)this).BackColor;
                        _panelImage = StatusPanelResources.Status_Suggestion;
                        break;
                }

                _mode = value;
                _gradientRight = (Owner ?? (Control)this).BackColor;

                if (Owner != null && Owner.IsHandleCreated)
                    Owner.Invalidate();
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            switch (_mode)
            {
                case StatusPanelMode.Error:
                case StatusPanelMode.Warning:
                case StatusPanelMode.Ok:
                case StatusPanelMode.Suggestion:
                    _gradientRight = (Owner ?? (Control)this).BackColor;

                    if (Owner != null && Owner.IsHandleCreated)
                        Owner.Invalidate();
                    break;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Point PanelLocation
        {
            get { return base.Location - new Size(Margin.Left + 2, HeaderHeight + Margin.Top + 3); }
            set { base.Location = value + new Size(Margin.Left + 2, HeaderHeight + Margin.Top + 3); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Size PanelSize
        {
            get { return Size + new Size(Margin.Horizontal + 3, HeaderHeight + Margin.Vertical + 4); }
            set { Size = value - new Size(Margin.Horizontal + 3, HeaderHeight + Margin.Vertical + 4); }
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), DefaultValue(30)]
        public int HeaderHeight
        {
            get { return _headerHeight; }
            set { _headerHeight = value; if (Owner != null) Owner.ApplySizes(this); }
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

        [Localizable(true), DefaultValue(""), Category("Panel")]
        public string Title
        {
            get { return _title ?? ""; }
            set
            {
                if (_title != value)
                {
                    _title = value;

                    if (Owner != null)
                        Owner.Invalidate();
                }
            }
        }

        [Localizable(true), DefaultValue(""), Category("Panel")]
        public string Summary
        {
            get { return _summary ?? ""; }
            set
            {
                if (_summary != value)
                {
                    _summary = value;

                    if (Owner != null)
                        Owner.Invalidate();
                }
            }
        }

        protected internal StatusContainer Owner
        {
            get { return _owner ?? (_owner = Parent as StatusContainer); }
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
