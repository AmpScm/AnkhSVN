// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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

        Control _top;

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
                    _parent.HandleDestroyed -= new EventHandler(OnParentHandleDestroyed);
                    _parent = value;
                }

                if (_top != null)
                    _top.VisibleChanged -= new EventHandler(OnTopVisibleChanged);

                _parent = value;
                _top = null;
                if (_pb != null)
                {
                    _pb.Parent = value;
                }

                if(_parent != null)
                {
                    Control p = _parent;

                    while (p.Parent != null)
                        p = p.Parent;

                    if (p != value)
                        _top = p;
                }
                    
                UpdatePosition();

                if (_parent != null)
                {
                    _parent.HandleDestroyed += new EventHandler(OnParentHandleDestroyed);
                    _parent.VisibleChanged += new EventHandler(OnParentChanged);
                    _parent.SizeChanged += new EventHandler(OnParentChanged);
                }

                if (_top != null)
                    _top.VisibleChanged += new EventHandler(OnTopVisibleChanged);
            }
        }

        void OnTopVisibleChanged(object sender, EventArgs e)
        {
            if (_top != null && !_top.Visible)
            {
                if (_pb != null)
                {
                    _pb.Dispose();
                    _pb = null;
                }
            }
        }

        void OnParentHandleDestroyed(object sender, EventArgs e)
        {
            if (_pb != null)
            {
                _pb.Dispose();
                _pb = null;
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
            if (_show >= 0 && _pb == null && Parent != null && !Parent.IsDisposed && Parent.Visible)
            {
                Bitmap img = (Bitmap)Resources.Busy;

                Size sz = img.Size;

                _pb = new PictureBox();
                _pb.Size = img.Size;
                _pb.BackColor = Parent.BackColor;
                _pb.Image = img;
                _parent.Controls.Add(_pb);

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
