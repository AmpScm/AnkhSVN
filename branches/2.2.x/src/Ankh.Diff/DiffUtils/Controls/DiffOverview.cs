// $Id$
//
// Copyright 2008 The AnkhSVN Project
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

#region Copyright And Revision History

/*---------------------------------------------------------------------------

	DiffOverview.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Ankh.Diff.DiffUtils;

namespace Ankh.Diff.DiffUtils.Controls
{
    /// <summary>
    /// This shows a "colored line" overview of the diffs (sort of like WinDiff has on the left).
    /// Instead of blue vertical bars like WinDiff, it uses a translucent selection view window.
    /// </summary>
    public class DiffOverview : Control
    {
        #region Public Members

        public DiffOverview()
        {
            //Set some important control styles
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.StandardClick, true);
            SetStyle(ControlStyles.StandardDoubleClick, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            base.BackColor = SystemColors.Window;

            m_OptionsChangedHandler = new EventHandler(DiffOptionsChanged);
            DiffOptions.OptionsChanged += m_OptionsChangedHandler;
        }

        public DiffView DiffView
        {
            get
            {
                return m_View;
            }
            set
            {
                if (m_View != value)
                {
                    //Detach from old view's events
                    if (m_View != null)
                    {
                        m_View.SizeChanged -= m_SizeChangedHandler;
                        m_View.VScrollPosChanged -= m_VScrollPosChangedHandler;
                        m_View.LinesChanged -= m_LinesChangedHandler;
                    }

                    m_View = value;

                    //Attach to new view's events
                    if (m_View != null)
                    {
                        if (m_SizeChangedHandler == null)
                        {
                            m_SizeChangedHandler = new EventHandler(DiffView_SizeChanged);
                        }
                        m_View.SizeChanged += m_SizeChangedHandler;

                        if (m_VScrollPosChangedHandler == null)
                        {
                            m_VScrollPosChangedHandler = new EventHandler(DiffView_VScrollPosChanged);
                        }
                        m_View.VScrollPosChanged += m_VScrollPosChangedHandler;

                        if (m_LinesChangedHandler == null)
                        {
                            m_LinesChangedHandler = new EventHandler(DiffView_LinesChanged);
                        }
                        m_View.LinesChanged += m_LinesChangedHandler;
                    }

                    UpdateAll();
                }
            }
        }

        [DefaultValue(true)]
        public bool UseTranslucentView
        {
            get
            {
                return m_bUseTranslucentView;
            }
            set
            {
                if (m_bUseTranslucentView != value)
                {
                    m_bUseTranslucentView = value;
                    InvalidateView();
                }
            }
        }

        /// <summary>
        /// Total number of lines
        /// </summary>
        [Browsable(false)]
        public int LineCount
        {
            get
            {
                return m_View != null ? m_View.LineCount : 0;
            }
        }

        /// <summary>
        /// Used to determine size of view window.
        /// </summary>
        [Browsable(false)]
        public int ViewLineCount
        {
            get
            {
                if (m_View != null)
                    return m_View.VisibleLineCount;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Used to determine position of view window
        /// </summary>
        [Browsable(false)]
        public int ViewTopLine
        {
            get
            {
                if (m_View != null)
                    return m_View.FirstVisibleLine;
                else
                    return 0;
            }
        }

        [Browsable(true), DefaultValue(BorderStyle.Fixed3D), Category("Appearance")]
        public BorderStyle BorderStyle
        {
            get
            {
                return m_BorderStyle;
            }
            set
            {
                if (m_BorderStyle != value)
                {
                    m_BorderStyle = value;
                    RecreateHandle();
                    UpdateAll();
                }
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Fired when the user clicks and/or drags to move the view.
        /// </summary>
        public event EventHandler<Ankh.Diff.DiffUtils.Controls.DiffLineClickEventArgs> LineClick;

        #endregion

        #region Protected Overrides

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams P = base.CreateParams;
                NativeMethods.SetBorderStyle(P, m_BorderStyle);
                return P;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (m_View == null) return;

            if (e.Button == MouseButtons.Left)
            {
                m_bDragging = true;
                Capture = true;
                FireLineClicked(GetLineFromPoint(e.X, e.Y));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (m_View == null || !m_bDragging) return;

            if (e.Button == MouseButtons.Left)
            {
                FireLineClicked(GetLineFromPoint(e.X, e.Y));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (m_View == null || !m_bDragging) return;

            if (e.Button == MouseButtons.Left)
            {
                Capture = false;
                m_bDragging = false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics G = e.Graphics;

            if (m_Image != null)
            {
                Rectangle R = e.ClipRectangle;
                G.DrawImage(m_Image, R.X, R.Y, R, GraphicsUnit.Pixel);

                //Repaint the view window if any of it is invalid
                if (R.IntersectsWith(m_ViewRect))
                {
                    bool bDisposePen = false;
                    Pen P = SystemPens.Highlight;
                    Rectangle rPen;

                    if (UseTranslucentView)
                    {
                        //Set the alpha blend to 20% (51/256);
                        SolidBrush B = new SolidBrush(Color.FromArgb(51, SystemColors.Highlight));
                        R.Intersect(m_ViewRect);
                        G.FillRectangle(B, R);
                        B.Dispose();

                        //Draw the pen border with view rect.
                        rPen = m_ViewRect;
                    }
                    else
                    {
                        //Create a two pixel wide highlight pen.
                        P = new Pen(SystemColors.Highlight, 2);
                        bDisposePen = true;

                        //Because the lines will go back up a pixel
                        //we have to shrink the bounds of the rect.
                        rPen = new Rectangle(m_ViewRect.X + 1, m_ViewRect.Y + 1, m_ViewRect.Width - 1, m_ViewRect.Height - 1);
                    }

                    //Draw a Highlight Pen border.  In some cases, it will
                    //draw a pixel too far (because we always round up), so
                    //we'll check for that case here.  If we're scrolled to
                    //the bottom, I don't want the last line cut off.
                    int iViewHeight = rPen.Height - 1;
                    int iUsableHeight = ClientSize.Height - rPen.Y - 1;
                    int iHeight = Math.Min(iViewHeight, iUsableHeight);
                    G.DrawRectangle(P, rPen.X, rPen.Y, rPen.Width - 1, iHeight);

                    if (bDisposePen)
                    {
                        P.Dispose();
                    }
                }
            }
            else
            {
                G.FillRectangle(SystemBrushes.Control, ClientRectangle);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            UpdateAll();
            base.OnSizeChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DiffOptions.OptionsChanged -= m_OptionsChangedHandler;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void UpdateAll()
        {
            RenderImage();
            CalculateViewRect();
            Invalidate();
        }

        private void CalculateViewRect()
        {
            if (ViewLineCount == 0)
            {
                m_ViewRect = new Rectangle(0, 0, 0, 0);
            }
            else
            {
                int iY = GetPixelLineHeight(ViewTopLine);
                m_ViewRect.Location = new Point(0, iY);

                int iHeight = GetPixelLineHeight(ViewLineCount);
                m_ViewRect.Size = new Size(ClientSize.Width, iHeight);
            }
        }

        private void InvalidateView()
        {
            Rectangle OldViewRect = m_ViewRect;
            CalculateViewRect();

            //If the old and new view rects are the
            //same, then there's nothing to do.
            if (OldViewRect == m_ViewRect) return;

            //If there is a current view rect, invalidate it first.
            if (!OldViewRect.IsEmpty)
            {
                Invalidate(OldViewRect);
            }

            //Invalidate the current view rectangle
            if (!m_ViewRect.IsEmpty)
            {
                Invalidate(m_ViewRect);
            }
        }

        private int GetPixelLineHeight(int iLines)
        {
            return (int)Math.Ceiling(GetPixelLineHeightF(iLines));
        }

        private float GetPixelLineHeightF(int iLines)
        {
            if (LineCount > 0)
                return (ClientSize.Height * (iLines / (float)LineCount));
            else
                return 0;
        }

        private void RenderImage()
        {
            if (m_Image != null)
            {
                m_Image.Dispose();
                m_Image = null;
            }

            int iWidth = ClientSize.Width;
            int iHeight = ClientSize.Height;

            if (iWidth > 0 && iHeight > 0 && m_View != null && m_View.Lines != null)
            {
                //Draw a bitmap in memory that we can render from
                m_Image = new Bitmap(iWidth, iHeight);
                Graphics G = Graphics.FromImage(m_Image);

                SolidBrush B = new SolidBrush(BackColor);
                G.FillRectangle(B, 0, 0, iWidth, iHeight);

                const float c_fGutter = 2.0F;
                //Make sure each line is at least 1 pixel high
                float fLineHeight = (float)Math.Max(1.0, GetPixelLineHeightF(1));
                DiffViewLines Lines = m_View.Lines;
                int iNumLines = Lines.Count;
                for (int i = 0; i < iNumLines; i++)
                {
                    DiffViewLine L = Lines[i];
                    if (L.Edited)
                    {
                        B.Color = DiffOptions.GetColorForEditType(L.EditType);
                        float fY = GetPixelLineHeightF(i);
                        G.FillRectangle(B, c_fGutter, fY, iWidth - 2 * c_fGutter, fLineHeight);
                    }
                }

                B.Dispose();
                G.Dispose();
            }
        }

        private void FireLineClicked(int iLine)
        {
            if (LineClick != null)
            {
                //Force it in bounds
                if (iLine < 0)
                    iLine = 0;
                else if (iLine >= LineCount)
                    iLine = LineCount - 1;

                LineClick(this, new DiffLineClickEventArgs(iLine));
            }
        }

        private int GetLineFromPoint(int X, int Y)
        {
            double dPercent = ((double)Y) / ClientSize.Height;
            return (int)(LineCount * dPercent);
        }

        private void DiffView_SizeChanged(object sender, EventArgs e)
        {
            //If the DiffView size changed, then our view window
            //may be longer or shorter, but the rendered image is
            //still valid.  So we just need to recalc the view rect
            //and invalidate the whole window.
            CalculateViewRect();
            Invalidate();
        }

        private void DiffView_LinesChanged(object sender, EventArgs e)
        {
            //If the Lines changed, we need to update everything.
            UpdateAll();
        }

        private void DiffView_VScrollPosChanged(object sender, EventArgs e)
        {
            //The DiffView's FirstVisibleLine has changed, so we
            //just need to invalidate our view.
            InvalidateView();
        }

        private void DiffOptionsChanged(object sender, EventArgs e)
        {
            //The diff colors changed, so we need to rerender the
            //image.  The current view rect should still be valid.
            RenderImage();
            Invalidate();
        }

        #endregion

        #region Private Data Members

        private Bitmap m_Image = null;
        private bool m_bDragging = false;
        private BorderStyle m_BorderStyle = BorderStyle.Fixed3D;
        private Rectangle m_ViewRect = new Rectangle();
        private DiffView m_View = null;
        private EventHandler m_SizeChangedHandler = null;
        private EventHandler m_VScrollPosChangedHandler = null;
        private EventHandler m_LinesChangedHandler = null;
        private bool m_bUseTranslucentView = true;
        private EventHandler m_OptionsChangedHandler;

        #endregion
    }

    #region Helper Types

    public sealed class DiffLineClickEventArgs : EventArgs
    {
        internal DiffLineClickEventArgs(int iLine)
        {
            m_iLine = iLine;
        }

        public int Line
        {
            get { return m_iLine; }
        }

        private int m_iLine;
    }

    #endregion
}
