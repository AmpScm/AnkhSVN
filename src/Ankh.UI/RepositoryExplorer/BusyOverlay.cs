using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using Ankh.UI.Properties;

namespace Ankh.UI.RepositoryExplorer
{
    public class BusyOverlay : Component
    {
        Control _parent;
        PictureBox _pb;
        AnchorStyles _anchor;
        int _show;

        public BusyOverlay()
        {
            _anchor = AnchorStyles.None;
        }

        public BusyOverlay(Control parent, AnchorStyles anchor)
            : this()
        {
            Parent = parent;
            Anchor = anchor;
        }

        [DefaultValue(DockStyle.None)]
        public AnchorStyles Anchor
        {
            get { return _anchor; }
            set { _anchor = value; UpdatePosition(); }
        }

        public Control Parent
        {
            get { return _parent; }
            set 
            {
                if (_parent == value)
                    return;

                if (_parent != null)
                {
                    _parent.SizeChanged -= new EventHandler(OnParentChanged);
                    _parent.VisibleChanged -= new EventHandler(OnParentChanged);
                    _parent = value;
                }

                _parent = value;
                if(_pb != null)
                    _pb.Parent = value;
                    
                UpdatePosition();

                if (_parent != null)
                {
                    _parent.VisibleChanged += new EventHandler(OnParentChanged);
                    _parent.SizeChanged += new EventHandler(OnParentChanged);
                }
            }
        }

        void OnParentChanged(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (Parent == null || _pb == null)
                return;

            Size parentSize = Parent.Size;
            Point p = new Point();

            if ((parentSize.Width < _pb.Width) || (parentSize.Height < _pb.Height))
                p = new Point(1, 1);
            else
            {
                Size ps = Parent.ClientSize;

                switch (Anchor & (AnchorStyles.Left | AnchorStyles.Right))
                {
                    case AnchorStyles.Left:
                        p.X = 1;
                        break;
                    case AnchorStyles.Right:
                        p.X = ps.Width - _pb.Width - 2;
                        break;
                    default:
                        p.X = (ps.Width - _pb.Width) / 2;
                        break;
                }

                switch (Anchor & (AnchorStyles.Top | AnchorStyles.Bottom))
                {
                    case AnchorStyles.Top:
                        p.Y = 1;
                        break;
                    case AnchorStyles.Bottom:
                        p.Y = ps.Height - _pb.Height - 2;
                        break;
                    default:
                        p.X = (ps.Height - _pb.Height) / 2;
                        break;
                }
            }

            _pb.Location = p;
        }

        public void Show()
        {
            if (_show == 0)
            {
                if (_pb == null)
                {
                    Bitmap img = (Bitmap)Resources.Busy;

                    Size sz = img.Size;

                    _pb = new PictureBox();
                    _pb.Size = img.Size;
                    _pb.BackColor = Parent.BackColor;
                    _pb.Image = img;
                    _parent.Controls.Add(_pb);                    
                }
                UpdatePosition();
                _pb.Show();
            }
            _show++;
        }

        public void Hide()
        {
            if(_show > 0)
            {
                _show--;

                if(_show == 0 && _pb != null)
                {
                    _pb.Dispose();
                    _pb = null;
                }                    
            }
        }
    }
}
