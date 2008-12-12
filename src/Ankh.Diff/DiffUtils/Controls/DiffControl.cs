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

	DiffControl.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.26.2002	Created.

	BMenees 03.13.2003	Added ToolTips.  Removed lines that forced ViewA
						to get focus if no other DiffView had focus because
						it prevented you from selecting the text in the
						file name headers.
-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Ankh.Diff.DiffUtils;
using System.Diagnostics;
using System.Collections.Generic;

namespace Ankh.Diff.DiffUtils.Controls
{
    public partial class DiffControl : System.Windows.Forms.UserControl, IDisposable
    {
        #region Public Members

        public DiffControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            UpdateButtons();
            UpdateColors();

            m_OptionsChangedHandler = new EventHandler(DiffOptionsChanged);
            DiffOptions.OptionsChanged += m_OptionsChangedHandler;

            //We have to manually attach to the GotFocus event because it
            //isn't shown by the Event designer.  .NET wants us to use the
            //Enter event.  Unfortunately, the Focused property isn't set
            //yet when that event fires, so ActiveView returns the wrong
            //information.  We really do need the GotFocus event.
            EventHandler EH = new EventHandler(View_PositionChanged);
            ViewA.GotFocus += EH;
            ViewB.GotFocus += EH;
            ViewLineDiff.GotFocus += EH;
        }

        public void SetData(IList<string> StringListA, IList<string> StringListB, EditScript Script)
        {
            SetData(StringListA, StringListB, Script, "", "");
        }

        public void SetData(IList<string> StringListA, IList<string> StringListB, EditScript Script, string strNameA, string strNameB)
        {
            ViewA.SetData(StringListA, Script, true);
            ViewB.SetData(StringListB, Script, false);
            Overview.DiffView = ViewA;

            Debug.Assert(ViewA.LineCount == ViewB.LineCount, "Both DiffView's LineCounts must be the same");

            bool bShowNames = strNameA.Length > 0 || strNameB.Length > 0;
            edtLeft.Visible = bShowNames;
            edtRight.Visible = bShowNames;
            if (bShowNames)
            {
                edtLeft.Text = strNameA;
                edtRight.Text = strNameB;
            }

            UpdateButtons();
            m_iCurrentDiffLine = -1;
            UpdateLineDiff();

            ActiveControl = ViewA;
        }

        [DefaultValue(32)]
        public int OverviewWidth
        {
            get
            {
                return Overview.Width;
            }
            set
            {
                Overview.Width = value;
                DiffControl_SizeChanged(this, EventArgs.Empty);
            }
        }

        [DefaultValue(38)]
        public int LineDiffHeight
        {
            get
            {
                return pnlBottom.Height;
            }
            set
            {
                pnlBottom.Height = value;
            }
        }

        [DefaultValue(true)]
        public bool UseTranslucentOverview
        {
            get
            {
                return Overview.UseTranslucentView;
            }
            set
            {
                Overview.UseTranslucentView = value;
            }
        }

        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get
            {
                return m_bShowToolbar;
            }
            set
            {
                if (m_bShowToolbar != value)
                {
                    //Note: We have to store the state ourselves because
                    //Visible may return false even after we set it to true
                    //if any of its parents are visible.
                    m_bShowToolbar = value;
                    ToolBar.Visible = value;
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowColorLegend
        {
            get
            {
                return m_bShowColorLegend;
            }
            set
            {
                if (m_bShowColorLegend != value)
                {
                    m_bShowColorLegend = value;
                    lblDelete.Visible = value;
                    lblChange.Visible = value;
                    lblInsert.Visible = value;
                    tsSep6.Visible = value;
                }
            }
        }

        [DefaultValue(false)]
        public bool ShowWhitespaceInLineDiff
        {
            get
            {
                return ViewLineDiff.ShowWhitespace;
            }
            set
            {
                ViewLineDiff.ShowWhitespace = value;
            }
        }

        public bool Find()
        {
            bool bResult = ActiveView.Find(m_FindData);
            UpdateButtons();
            return bResult;
        }

        public bool FindNext()
        {
            bool bResult = ActiveView.FindNext(m_FindData);
            UpdateButtons();
            return bResult;
        }

        public bool FindPrevious()
        {
            bool bResult = ActiveView.FindPrevious(m_FindData);
            UpdateButtons();
            return bResult;
        }

        public bool GoToFirstDiff()
        {
            return ActiveView.GoToFirstDiff();
        }

        public bool GoToNextDiff()
        {
            return ActiveView.GoToNextDiff();
        }

        public bool GoToPreviousDiff()
        {
            return ActiveView.GoToPreviousDiff();
        }

        public bool GoToLastDiff()
        {
            return ActiveView.GoToLastDiff();
        }

        public bool GoToLine()
        {
            return ActiveView.GoToLine();
        }

        public Font ViewFont
        {
            get
            {
                return ViewA.Font;
            }
            set
            {
                ViewA.Font = value;
                ViewB.Font = value;
                ViewLineDiff.Font = value;
            }
        }

        public void Copy()
        {
            Clipboard.SetDataObject(ActiveView.SelectedText, true);
        }

        public bool CanCopy
        {
            get
            {
                return ActiveView.HasSelection;
            }
        }

        public bool CanFind
        {
            get
            {
                return HasText;
            }
        }

        public bool CanFindNext
        {
            get
            {
                return HasText && HasFindText;
            }
        }

        public bool CanFindPrevious
        {
            get
            {
                return HasText && HasFindText;
            }
        }

        public bool CanGoToFirstDiff
        {
            get
            {
                return HasText && ActiveView.CanGoToFirstDiff;
            }
        }

        public bool CanGoToNextDiff
        {
            get
            {
                return HasText && ActiveView.CanGoToNextDiff;
            }
        }

        public bool CanGoToPreviousDiff
        {
            get
            {
                return HasText && ActiveView.CanGoToPreviousDiff;
            }
        }

        public bool CanGoToLastDiff
        {
            get
            {
                return HasText && ActiveView.CanGoToLastDiff;
            }
        }

        public bool CanGoToLine
        {
            get
            {
                return HasText && ActiveView != ViewLineDiff;
            }
        }

        public bool CanViewFile
        {
            get
            {
                return (ViewA.Focused || ViewB.Focused || edtLeft.Focused || edtRight.Focused) && (edtLeft.TextLength > 0 && edtRight.TextLength > 0);
            }
        }

        public bool CanCompareSelectedText
        {
            get
            {
                return ShowTextDifferences != null && ViewA.HasSelection && ViewB.HasSelection;
            }
        }

        public bool CompareSelectedText()
        {
            if (CanCompareSelectedText)
            {
                string strA = ViewA.SelectedText;
                string strB = ViewB.SelectedText;

                DifferenceEventArgs DiffArgs = new DifferenceEventArgs(strA, strB);
                ShowTextDifferences(this, DiffArgs);
                return true;
            }

            return false;
        }

        public bool CanRecompare
        {
            get
            {
                //Only allow recompares on files, not text.
                return RecompareNeeded != null && edtLeft.Visible && edtLeft.TextLength > 0;
            }
        }

        public bool Recompare()
        {
            if (!CanRecompare)
                return false;

            RecompareNeeded(this, EventArgs.Empty);
            return true;
        }

        #endregion

        #region Public Events

        public event EventHandler<DifferenceEventArgs> ShowTextDifferences;

        public event EventHandler LineDiffSizeChanged;

        public event EventHandler RecompareNeeded;

        #endregion

        

        #region Protected Members

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DiffOptions.OptionsChanged -= m_OptionsChangedHandler;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Internal Methods

        internal static void PaintColorLegendItem(ToolStripItem Item, PaintEventArgs e)
        {
            if (Item != null)
            {
                //Make our outermost painting rect a little smaller.
                Rectangle R = e.ClipRectangle;
                R.Inflate(-1, -1);

                //Paint the background.
                Graphics G = e.Graphics;
                using (Brush B = new SolidBrush(Item.BackColor))
                {
                    G.FillRectangle(B, R);
                }

                //Draw a border.
                Rectangle BorderRect = new Rectangle(R.X, R.Y, R.Width - 1, R.Height - 1);
                ControlPaint.DrawVisualStyleBorder(G, BorderRect);

                //Draw the image centered.  (I should probably check the
                //item's ImageAlign property here, but I know I'm always
                //using MiddleCenter for all the passed-in items.)
                Image I = Item.Image;
                Rectangle ImageRect = new Rectangle(R.X + (R.Width - I.Width) / 2, R.Y + (R.Height - I.Height) / 2, I.Width, I.Height);
                G.DrawImage(I, ImageRect);
            }
        }

        #endregion

        #region Private Members

        private void ViewA_HScrollPosChanged(object sender, System.EventArgs e)
        {
            ViewB.HScrollPos = ViewA.HScrollPos;
        }

        private void ViewA_VScrollPosChanged(object sender, System.EventArgs e)
        {
            ViewB.VScrollPos = ViewA.VScrollPos;
        }

        private void ViewB_HScrollPosChanged(object sender, System.EventArgs e)
        {
            ViewA.HScrollPos = ViewB.HScrollPos;
        }

        private void ViewB_VScrollPosChanged(object sender, System.EventArgs e)
        {
            ViewA.VScrollPos = ViewB.VScrollPos;
        }

        private void Overview_LineClick(object sender, Ankh.Diff.DiffUtils.Controls.DiffLineClickEventArgs e)
        {
            ViewA.CenterVisibleLine = e.Line;
            ActiveView.Position = new DiffViewPosition(e.Line, 0);
        }

        private void DiffControl_SizeChanged(object sender, System.EventArgs e)
        {
            pnlLeft.Width = (Width - pnlLeft.Left - MiddleSplitter.Width) / 2;
        }

        private void btnFind_Click(object sender, System.EventArgs e)
        {
            Find();
        }

        private void btnFindNext_Click(object sender, System.EventArgs e)
        {
            FindNext();
        }

        private void btnFindPrevious_Click(object sender, System.EventArgs e)
        {
            FindPrevious();
        }

        private void btnNextDiff_Click(object sender, System.EventArgs e)
        {
            GoToNextDiff();
        }

        private void btnPrevDiff_Click(object sender, System.EventArgs e)
        {
            GoToPreviousDiff();
        }

        private void btnGotoLine_Click(object sender, System.EventArgs e)
        {
            GoToLine();
        }

        private DiffView ActiveView
        {
            get
            {
                if (ViewLineDiff.Focused)
                {
                    return ViewLineDiff;
                }
                else if (ViewB.Focused)
                {
                    return ViewB;
                }
                else
                {
                    return ViewA;
                }
            }
        }

        private void View_PositionChanged(object sender, System.EventArgs e)
        {
            DiffView View = ActiveView;
            DiffViewPosition Pos = View.Position;
            lblPosition.Text = String.Format("Ln {0}, Col {1}", Pos.Line + 1, Pos.Column + 1);
            UpdateButtons();

            if (View != ViewLineDiff)
            {
                UpdateLineDiff();
            }
        }

        private bool HasText
        {
            get
            {
                return ActiveView.LineCount > 0;
            }
        }

        private bool HasFindText
        {
            get
            {
                return m_FindData.Text.Length > 0;
            }
        }

        private void UpdateButtons()
        {

            btnCopy.Enabled = CanCopy;
            mnuCopy.Enabled = btnCopy.Enabled;

            bool bCanCompareText = CanCompareSelectedText;
            btnTextDiff.Enabled = bCanCompareText;
            mnuTextDiff.Enabled = bCanCompareText;

            btnFind.Enabled = CanFind;
            btnFindNext.Enabled = CanFindNext;
            btnFindPrevious.Enabled = CanFindPrevious;

            btnFirstDiff.Enabled = CanGoToFirstDiff;
            btnNextDiff.Enabled = CanGoToNextDiff;
            btnPrevDiff.Enabled = CanGoToPreviousDiff;
            btnLastDiff.Enabled = CanGoToLastDiff;

            btnGotoLine.Enabled = CanGoToLine;
            btnRecompare.Enabled = CanRecompare;
        }

        private void UpdateColors()
        {
            lblDelete.BackColor = DiffOptions.DeletedColor;
            lblChange.BackColor = DiffOptions.ChangedColor;
            lblInsert.BackColor = DiffOptions.InsertedColor;
        }

        private void DiffOptionsChanged(object sender, EventArgs e)
        {
            UpdateColors();
        }

        private void UpdateLineDiff()
        {
            int iLine = (ActiveView == ViewA) ? ViewA.Position.Line : ViewB.Position.Line;
            if (iLine == m_iCurrentDiffLine)
            {
                return;
            }

            m_iCurrentDiffLine = iLine;

            DiffViewLine LineOne = null;
            DiffViewLine LineTwo = null;
            if (iLine < ViewA.LineCount)
            {
                LineOne = ViewA.Lines[iLine];
            }
            //Normally, ViewA.LineCount == ViewB.LineCount, but during
            //SetData they'll be mismatched momentarily as each view
            //rebuilds its lines.
            if (iLine < ViewB.LineCount)
            {
                LineTwo = ViewB.Lines[iLine];
            }

            if (LineOne != null && LineTwo != null)
            {
                ViewLineDiff.SetData(LineOne, LineTwo);
            }
        }

        private void btnCopy_Click(object sender, System.EventArgs e)
        {
            Copy();
        }

        private void ViewLineDiff_SizeChanged(object sender, System.EventArgs e)
        {
            if (LineDiffSizeChanged != null)
            {
                LineDiffSizeChanged(this, e);
            }
        }

        private void mnuTextDiff_Click(object sender, System.EventArgs e)
        {
            CompareSelectedText();
        }

        private void ColorLegend_Paint(object sender, PaintEventArgs e)
        {
            PaintColorLegendItem(sender as ToolStripItem, e);
        }

        private void btnRecompare_Click(object sender, EventArgs e)
        {
            Recompare();
        }

        private void btnFirstDiff_Click(object sender, EventArgs e)
        {
            GoToFirstDiff();
        }

        private void btnLastDiff_Click(object sender, EventArgs e)
        {
            GoToLastDiff();
        }

        #endregion

        #region Private Data Members

        private Ankh.Diff.DiffUtils.Controls.DiffOverview Overview;
        private Ankh.Diff.DiffUtils.Controls.DiffView ViewA;
        private Ankh.Diff.DiffUtils.Controls.DiffView ViewB;
        private System.Windows.Forms.Panel pnlSeparator;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.TextBox edtLeft;
        private System.Windows.Forms.TextBox edtRight;
        private Ankh.Diff.DiffUtils.Controls.DiffView ViewLineDiff;
        private System.Windows.Forms.Splitter MiddleSplitter;
        private System.Windows.Forms.Splitter BottomSplitter;
        private FindData m_FindData = new FindData();
        private int m_iCurrentDiffLine = -1;
        private bool m_bShowToolbar = true;
        private bool m_bShowColorLegend = true;
        private ToolStrip ToolBar;
        private ToolStripButton btnCopy;
        private ToolStripButton btnTextDiff;
        private ToolStripSeparator tsSep2;
        private ToolStripButton btnFind;
        private ToolStripButton btnFindNext;
        private ToolStripButton btnFindPrevious;
        private ToolStripSeparator tsSep3;
        private ToolStripButton btnPrevDiff;
        private ToolStripButton btnNextDiff;
        private ToolStripSeparator tsSep4;
        private ToolStripButton btnGotoLine;
        private ToolStripSeparator tsSep5;
        private ToolStripLabel lblDelete;
        private ToolStripLabel lblChange;
        private ToolStripLabel lblInsert;
        private ToolStripSeparator tsSep6;
        private ToolStripLabel lblPosition;
        private ContextMenuStrip CtxMenu;
        private IContainer components;
        private ToolStripMenuItem mnuCopy;
        private ToolStripMenuItem mnuTextDiff;
        private ToolStripButton btnRecompare;
        private ToolStripButton btnFirstDiff;
        private ToolStripButton btnLastDiff;
        private EventHandler m_OptionsChangedHandler;

        #endregion
    }
}
