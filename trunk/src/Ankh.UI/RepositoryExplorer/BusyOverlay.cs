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
using Ankh.Scc.UI;

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
                    _parent.BackColorChanged -= new EventHandler(OnParentBackColorChanged);
                    _parent.SizeChanged -= new EventHandler(OnParentSizeChanged);
                    _parent.VisibleChanged -= new EventHandler(OnParentVisibleChanged);

                    _parent.HandleCreated -= new EventHandler(OnParentHandleCreated);
                    _parent.HandleDestroyed -= new EventHandler(OnParentHandleDestroyed);
                    _parent = value;
                }

                if (_top != null)
                {
                    IAnkhToolWindowControl tw = _top as IAnkhToolWindowControl;
                    if (tw != null)
                        tw.ToolWindowVisibileChanged -= new EventHandler(OnTopVisibleChanged);
                    else
                        _top.VisibleChanged -= new EventHandler(OnToolWindowVisibleChanged);
                }

                _parent = value;
                _top = null;
                if (_pb != null)
                {
                    _pb.Parent = value;
                }

                if (_parent != null)
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
                    _parent.HandleCreated += new EventHandler(OnParentHandleCreated);

                    _parent.VisibleChanged += new EventHandler(OnParentVisibleChanged);
                    _parent.SizeChanged += new EventHandler(OnParentSizeChanged);
                    _parent.BackColorChanged += new EventHandler(OnParentBackColorChanged);
                }

                if (_top != null)
                {
                    IAnkhToolWindowControl tw = _top as IAnkhToolWindowControl;
                    if (tw != null)
                        tw.ToolWindowVisibileChanged += new EventHandler(OnToolWindowVisibleChanged);
                    else
                        _top.VisibleChanged += new EventHandler(OnTopVisibleChanged);
                }
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

        void OnToolWindowVisibleChanged(object sender, EventArgs e)
        {
            IAnkhToolWindowControl tw = _top as IAnkhToolWindowControl;
            if (tw == null)
                return;

            if (_pb != null && !tw.ToolWindowVisible)
            {
                _pb.Dispose();
                _pb = null;
            }
        }

        void OnParentHandleCreated(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        void OnParentHandleDestroyed(object sender, EventArgs e)
        {
            if (Parent.RecreatingHandle)
                return;

            if (_pb != null)
            {
                _pb.Dispose();
                _pb = null;
            }
        }

        private void OnParentBackColorChanged(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        void OnParentSizeChanged(object sender, EventArgs e)
        {
            UpdatePosition();
        }

        void OnParentVisibleChanged(object sender, EventArgs e)
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
                System.Reflection.PropertyInfo pi = Parent.GetType().GetProperty("BorderStyle", typeof(BorderStyle));
                int borderWidth = 0;

                if (pi != null)
                    switch ((BorderStyle)pi.GetValue(Parent))
                    {
                        case BorderStyle.Fixed3D:
                            borderWidth = 2;
                            break;
                        case BorderStyle.FixedSingle:
                            borderWidth = 1;
                            break;
                    }

                switch (Anchor & (AnchorStyles.Left | AnchorStyles.Right))
                {
                    case AnchorStyles.Left:
                        p.X = borderWidth;
                        break;
                    case AnchorStyles.Right:
                        p.X = ps.Width - _pb.Width - borderWidth;
                        break;
                    default:
                        p.X = (ps.Width - _pb.Width) / 2;
                        break;
                }

                switch (Anchor & (AnchorStyles.Top | AnchorStyles.Bottom))
                {
                    case AnchorStyles.Top:
                        p.Y = borderWidth;
                        break;
                    case AnchorStyles.Bottom:
                        p.Y = ps.Height - _pb.Height - borderWidth;
                        break;
                    default:
                        p.X = (ps.Height - _pb.Height) / 2;
                        break;
                }
            }

            _pb.Location = p;
            if (_pb.BackColor != Parent.BackColor)
                _pb.BackColor = Parent.BackColor;
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
            if (_show > 0)
            {
                _show--;

                if (_show == 0 && _pb != null)
                {
                    _pb.Dispose();
                    _pb = null;
                }
            }
        }
    }
}
