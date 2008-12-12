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

	DiffView.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

	BMenees	03.13.2003	Find always returned false.  Add "return" before
						FindNext and FindPrevious.

	BMenees	05.14.2004	Updated to initialize Find text from the selection.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using Ankh.Diff.DiffUtils;
using System.Diagnostics;
using System.Collections.Generic;

/*
 * Chad discovered that with lines over about 25000 characters long, that GDI+
 * will blow up and paint the window with red lines going from corner to corner.
 * I'm not sure how to fix it, and since it occurs so rarely in practice, I don't
 * want to spend much time on it...
 */

namespace Ankh.Diff.DiffUtils.Controls
{
    /// <summary>
    /// Single pane like SourceSafe has on the left or right.
    /// </summary>
    public class DiffView : Control
    {
        #region Public Members

        public DiffView()
        {
            //Set some important control styles
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.StandardClick, true);
            SetStyle(ControlStyles.StandardDoubleClick, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            m_Position = new DiffViewPosition(0, 0);

            UpdateTextMetrics(true);

            m_AutoScrollTimer = new Timer();
            m_AutoScrollTimer.Enabled = false;
            m_AutoScrollTimer.Interval = 100;
            m_AutoScrollTimer.Tick += new EventHandler(AutoScrollTimer_Tick);

            m_OptionsChangedHandler = new EventHandler(DiffOptionsChanged);
            DiffOptions.OptionsChanged += m_OptionsChangedHandler;

            Cursor = Cursors.IBeam;
        }

        public void SetData(IList<string> StringList, EditScript Script, bool bUseA)
        {
            m_StringList = StringList;
            m_Lines = new DiffViewLines(m_StringList, Script, bUseA);
            UpdateAfterSetData();
        }

        public void SetData(DiffViewLine LineOne, DiffViewLine LineTwo)
        {
            m_StringList = null;
            m_Lines = new DiffViewLines(LineOne, LineTwo);
            UpdateAfterSetData();
        }

        [Browsable(false)]
        public int FirstVisibleLine
        {
            get
            {
                return VScrollPos;
            }
            set
            {
                VScrollPos = value;
            }
        }

        [Browsable(false)]
        public int VisibleLineCount
        {
            get
            {
                return ClientSize.Height / LineHeight;
            }
        }

        /// <summary>
        /// Stores each line's text, color, and original number.
        /// </summary>
        [Browsable(false)]
        public DiffViewLines Lines
        {
            get
            {
                return m_Lines;
            }
        }

        [Browsable(false)]
        public int VScrollPos
        {
            get
            {
                return NativeMethods.GetScrollPos(this, false);
            }
            set
            {
                ScrollVertically(value, VScrollPos);
            }
        }

        [Browsable(false)]
        public int HScrollPos
        {
            get
            {
                return NativeMethods.GetScrollPos(this, true);
            }
            set
            {
                ScrollHorizontally(value, HScrollPos);
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
                }
            }
        }

        [Browsable(false)]
        public int LineCount
        {
            get
            {
                return m_Lines != null ? m_Lines.Count : 0;
            }
        }

        public bool Find(FindData Data)
        {
            FindDlg Dlg = new FindDlg();

            //If text is selected on a single line, then use that for the new Find text.
            string strOriginalFindText = Data.Text;
            string strSelectedText;
            if (GetSingleLineSelectedText(out strSelectedText))
            {
                Data.Text = strSelectedText;
            }

            if (Dlg.Execute(this, Data))
            {
                if (Data.SearchUp)
                    return FindPrevious(Data);
                else
                    return FindNext(Data);
            }
            else
            {
                //Reset the Find text if the user cancelled.
                Data.Text = strOriginalFindText;
            }

            return false;
        }

        public bool FindNext(FindData Data)
        {
            if (Data.Text.Length == 0)
            {
                Data.SearchUp = false;
                return Find(Data);
            }
            else
            {
                int iNumLines = LineCount;
                if (iNumLines > 0)
                {
                    string strText = Data.Text;
                    if (!Data.MatchCase)
                    {
                        strText = strText.ToUpper();
                    }

                    int iStartLine = m_Position.Line;
                    int iStartColumn = m_Position.Column;
                    for (int i = 0; i <= iNumLines; i++) // <= so we check the start line again from the beginning
                    {
                        //Use % so we wrap around at the end.
                        int iLine = (iStartLine + i) % iNumLines;
                        string strLine = GetLineText(iLine);

                        if (!Data.MatchCase)
                        {
                            strLine = strLine.ToUpper();
                        }

                        int iIndex;
                        if (i == iNumLines) //We're rechecking the start line from the beginning.
                            iIndex = strLine.IndexOf(strText, 0, m_Position.Column);
                        else
                            iIndex = strLine.IndexOf(strText, iStartColumn);

                        if (iIndex >= 0)
                        {
                            SetPosition(iLine, iIndex);
                            ExtendSelection(0, strText.Length);
                            return true;
                        }

                        //On all lines but the first, we need to start at 0
                        iStartColumn = 0;
                    }
                }

                string strMsg = String.Format("'{0}' was not found.", Data.Text);
                MessageBox.Show(this, strMsg, "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return false;
            }
        }

        public bool FindPrevious(FindData Data)
        {
            if (Data.Text.Length == 0)
            {
                Data.SearchUp = true;
                return Find(Data);
            }
            else
            {
                int iNumLines = LineCount;
                if (iNumLines > 0)
                {
                    string strText = Data.Text;
                    if (!Data.MatchCase)
                    {
                        strText = strText.ToUpper();
                    }

                    int iStartLine = m_Position.Line;
                    int iStartColumn = m_Position.Column;
                    for (int i = 0; i <= iNumLines; i++) // <= so we check the start line again from the end
                    {
                        //Use % so we wrap around at the end.
                        int iLine = (iStartLine - i + iNumLines) % iNumLines;
                        string strLine = GetLineText(iLine);

                        if (!Data.MatchCase)
                        {
                            strLine = strLine.ToUpper();
                        }

                        if (iStartColumn == -1)
                        {
                            iStartColumn = Math.Max(0, strLine.Length - 1);
                        }

                        int iIndex;
                        if (i == iNumLines) //We're rechecking the start line from the end.
                            iIndex = strLine.LastIndexOf(strText, iStartColumn, iStartColumn - m_Position.Column);
                        else
                            iIndex = strLine.LastIndexOf(strText, iStartColumn);
                        if (iIndex >= 0)
                        {
                            int iLength = strText.Length;
                            SetPosition(iLine, iIndex + iLength);
                            ExtendSelection(0, -iLength);
                            return true;
                        }

                        //On all lines but the first, we need to start at the end
                        iStartColumn = -1;
                    }
                }

                string strMsg = String.Format("'{0}' was not found.", Data.Text);
                MessageBox.Show(this, strMsg, "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return false;
            }
        }

        [Browsable(false)]
        public bool CanGoToFirstDiff
        {
            get
            {
                if (m_Lines != null)
                {
                    int[] arStarts = m_Lines.DiffStartLines;
                    int[] arEnds = m_Lines.DiffEndLines;
                    return arStarts.Length > 0 && arEnds.Length > 0 && (m_Position.Line < arStarts[0] || m_Position.Line > arEnds[0]);
                }
                else
                    return false;
            }
        }

        public bool GoToFirstDiff()
        {
            if (!CanGoToFirstDiff)
                return false;

            int iStartLine = m_Lines.DiffStartLines[0];
            CenterVisibleLine = iStartLine;
            SetPosition(iStartLine, m_Position.Column);
            return true;
        }

        [Browsable(false)]
        public bool CanGoToNextDiff
        {
            get
            {
                if (m_Lines != null)
                {
                    int[] arStarts = m_Lines.DiffStartLines;
                    return arStarts.Length > 0 && m_Position.Line < arStarts[arStarts.Length - 1];
                }
                else
                    return false;
            }
        }

        public bool GoToNextDiff()
        {
            if (!CanGoToNextDiff)
                return false;

            int[] arStarts = m_Lines.DiffStartLines;
            int iStarts = arStarts.Length;
            for (int i = 0; i < iStarts; i++)
            {
                if (m_Position.Line < arStarts[i])
                {
                    CenterVisibleLine = arStarts[i];
                    SetPosition(arStarts[i], m_Position.Column);
                    return true;
                }
            }

            //We should never get here.
            Debug.Assert(false, "CanGoToNextDiff was wrong.");
            return false;
        }

        [Browsable(false)]
        public bool CanGoToPreviousDiff
        {
            get
            {
                if (m_Lines != null)
                {
                    int[] arEnds = m_Lines.DiffEndLines;
                    return arEnds.Length > 0 && m_Position.Line > arEnds[0];
                }
                else
                    return false;
            }
        }

        public bool GoToPreviousDiff()
        {
            if (!CanGoToPreviousDiff)
                return false;

            int[] arEnds = m_Lines.DiffEndLines;
            int iEnds = arEnds.Length;
            for (int i = iEnds - 1; i >= 0; i--)
            {
                if (m_Position.Line > arEnds[i])
                {
                    //I'm intentionally setting the line to
                    //Starts[i] here instead of arEnds[i].
                    int iStartLine = m_Lines.DiffStartLines[i];
                    CenterVisibleLine = iStartLine;
                    SetPosition(iStartLine, m_Position.Column);
                    return true;
                }
            }

            //We should never get here.
            Debug.Assert(false, "CanGoToPreviousDiff was wrong.");
            return false;
        }

        [Browsable(false)]
        public bool CanGoToLastDiff
        {
            get
            {
                if (m_Lines != null)
                {
                    int[] arStarts = m_Lines.DiffStartLines;
                    int[] arEnds = m_Lines.DiffEndLines;
                    return arStarts.Length > 0 && arEnds.Length > 0 && (m_Position.Line < arStarts[arStarts.Length - 1] || m_Position.Line > arEnds[arEnds.Length - 1]);
                }
                else
                    return false;
            }
        }

        public bool GoToLastDiff()
        {
            if (!CanGoToLastDiff)
                return false;

            int[] arStarts = m_Lines.DiffStartLines;
            int iStartLine = arStarts[arStarts.Length - 1];
            CenterVisibleLine = iStartLine;
            SetPosition(iStartLine, m_Position.Column);
            return true;
        }

        public bool GoToLine()
        {
            if (m_StringList != null)
            {
                int iMaxLineNumber = m_StringList.Count;
                if (iMaxLineNumber > 0)
                {
                    GoToDlg Dlg = new GoToDlg();
                    int iLine;
                    if (Dlg.Execute(this, iMaxLineNumber, out iLine))
                    {
                        //Subtract 1 because the Dlg returns a 1-based number
                        return GoToLine(iLine - 1);
                    }
                }
            }

            return false;
        }

        public bool GoToLine(int iLine)
        {
            //We know the original line number will be in a 
            //DiffViewLine at a position >= iLine.
            if (iLine >= 0 && m_Lines != null && iLine < m_Lines.Count && iLine < m_StringList.Count)
            {
                for (int i = iLine; i < m_Lines.Count; i++)
                {
                    DiffViewLine L = m_Lines[i];
                    if (L.Number == iLine)
                    {
                        CenterVisibleLine = i;
                        SetPosition(i, 0);
                        return true;
                    }
                }
            }

            return false;
        }

        public DiffViewPosition GetPosFromPoint(int X, int Y)
        {
            int iLine = Y / LineHeight + VScrollPos;

            //Because we're not guaranteed to have a monospaced font,
            //this gets tricky.  We have to make an initial guess at
            //the column, and then we'll converge to the best one.
            string strText = GetLineText(iLine);

            //Make a starting guess.  Because of tabs and variable width 
            //fonts, this may be nowhere near the right place...
            int iColumn = (int)((X - GutterWidth + (HScrollPos * CharWidth)) / CharWidthF);

            Graphics G = Graphics.FromHwnd(Handle);

            int iStrLength = strText.Length;
            int iColumnGreater = -1;
            int iColumnLess = -1;

            int iX = GetXForColumn(G, strText, iColumn);
            if (iX != X)
            {
                if (iX > X)
                {
                    iColumnGreater = iColumn;
                    iColumnLess = 0;
                    for (iColumn = iColumnGreater - 1; iColumn >= 0; iColumn--)
                    {
                        iX = GetXForColumn(G, strText, iColumn);
                        if (iX > X)
                        {
                            iColumnGreater = iColumn;
                        }
                        else
                        {
                            iColumnLess = iColumn;
                            break;
                        }
                    }
                }
                else //iX < X
                {
                    iColumnLess = iColumn;
                    iColumnGreater = iStrLength;
                    for (iColumn = iColumnLess + 1; iColumn <= iStrLength; iColumn++)
                    {
                        iX = GetXForColumn(G, strText, iColumn);
                        if (iX < X)
                        {
                            iColumnLess = iColumn;
                        }
                        else
                        {
                            iColumnGreater = iColumn;
                            break;
                        }
                    }
                }

                iColumnGreater = Math.Max(Math.Min(iColumnGreater, iStrLength), 0);
                iColumnLess = Math.Min(Math.Max(iColumnLess, 0), iStrLength);

                int iGreaterX = GetXForColumn(G, strText, iColumnGreater);
                int iLessX = GetXForColumn(G, strText, iColumnLess);

                if (Math.Abs(iGreaterX - X) < Math.Abs(iLessX - X))
                    iColumn = iColumnGreater;
                else
                    iColumn = iColumnLess;
            }

            G.Dispose();

            return new DiffViewPosition(iLine, iColumn);
        }

        public Point GetPointFromPos(int iLine, int iColumn)
        {
            int iY = (iLine - VScrollPos) * LineHeight;

            //Because we're not guaranteed to have a monospaced font,
            //this gets tricky.  We have to measure the substring to
            //get the correct X.
            string strText = GetLineText(iLine);
            Graphics G = Graphics.FromHwnd(Handle);
            int iX = GetXForColumn(G, strText, iColumn);
            G.Dispose();
            return new Point(iX, iY);
        }

        [Browsable(false)]
        public DiffViewPosition Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                if (!m_Position.Equals(value))
                {
                    SetPosition(value.Line, value.Column, true, false);
                }
            }
        }

        public void ScrollToCaret()
        {
            if (m_Caret != null)
            {
                //Assume the caret is always at m_Position.
                //It would be nice if we had:
                //Debug.Assert(m_Position.Line == CaretPos.Line && m_Position.Column == CaretPos.Column);
                //but that fails on occasion because of rounding problems
                //between calling GetPointFromPos and then GetPosFromPoint.
                DiffViewPosition CaretPos = m_Position;
                Point CaretPoint = GetPointFromPos(CaretPos.Line, CaretPos.Column);

                //Make sure that position is on the screen by
                //scrolling the minimal number of lines and characters.
                int iFirstVisibleLine = FirstVisibleLine;
                int iLastVisibleLine = iFirstVisibleLine + VisibleLineCount - 1;

                if (CaretPos.Line < iFirstVisibleLine)
                {
                    VScrollPos -= (iFirstVisibleLine - CaretPos.Line);
                }
                else if (CaretPos.Line > iLastVisibleLine)
                {
                    VScrollPos += (CaretPos.Line - iLastVisibleLine);
                }

                //This is tricky because we might not have a monospaced font.
                //We have to figure out the number of pixels we need to scroll
                //and then translate that into characters (i.e. CharWidths).
                int iFirstVisibleX = (GutterWidth - c_iGutterSeparator);
                int iLastVisibleX = ClientSize.Width - m_Caret.Size.Width;
                if (CaretPoint.X < iFirstVisibleX)
                {
                    int iScrollPixels = CaretPoint.X - iFirstVisibleX;
                    HScrollPos += (int)Math.Floor(iScrollPixels / (double)CharWidth);
                }
                else if (CaretPoint.X > iLastVisibleX)
                {
                    int iScrollPixels = CaretPoint.X - iLastVisibleX;
                    HScrollPos += (int)Math.Ceiling(iScrollPixels / (double)CharWidth);
                }
            }
        }

        public bool HasSelection
        {
            get
            {
                return m_SelectionStart != DiffViewPosition.Empty;
            }
        }

        public string SelectedText
        {
            get
            {
                if (!HasSelection || m_Lines == null)
                {
                    return "";
                }
                else
                {
                    DiffViewPosition StartSel, EndSel;
                    GetForwardOrderSelection(out StartSel, out EndSel);

                    int iNumLines = EndSel.Line - StartSel.Line + 1;
                    StringBuilder B = new StringBuilder(iNumLines * 50);

                    for (int i = StartSel.Line; i <= EndSel.Line; i++)
                    {
                        //Leave out lines that are only in the display for alignment
                        //purposes.  This makes SelectedText useful for "Compare Text",
                        //and typically much more useful for "Copy".
                        DiffViewLine Line = m_Lines[i];
                        if (Line.Number >= 0)
                        {
                            string strText = Line.Text;
                            int iLineLength = strText.Length;
                            int iSelStartColumn = (i == StartSel.Line) ? StartSel.Column : 0;
                            int iSelEndColumn = (i == EndSel.Line) ? Math.Min(EndSel.Column, iLineLength) : iLineLength;

                            bool bLineFullySelected = (i > StartSel.Line && i < EndSel.Line) || (iSelStartColumn == 0 && iSelEndColumn == iLineLength);
                            if (!bLineFullySelected)
                            {
                                strText = strText.Substring(iSelStartColumn, iSelEndColumn - iSelStartColumn);
                            }

                            B.Append(strText);
                            if (i != EndSel.Line)
                            {
                                B.Append("\r\n");
                            }
                        }
                    }

                    return B.ToString();
                }
            }
        }

        [DefaultValue(false)]
        public bool ShowWhitespace
        {
            get
            {
                return m_bShowWhitespace;
            }
            set
            {
                if (m_bShowWhitespace != value)
                {
                    m_bShowWhitespace = value;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public int CenterVisibleLine
        {
            get
            {
                return FirstVisibleLine + VisibleLineCount / 2;
            }
            set
            {
                //Make this line the center of the view.
                int iFirstLine = value - VisibleLineCount / 2;
                FirstVisibleLine = iFirstLine;
            }
        }

        #endregion

        #region Public Events

        public event EventHandler VScrollPosChanged;
        public event EventHandler HScrollPosChanged;
        public event EventHandler LinesChanged;
        public event EventHandler PositionChanged;
        public event EventHandler SelectionChanged;

        #endregion

        #region Protected Overrides

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams P = base.CreateParams;
                P.Style = P.Style | NativeMethods.WS_VSCROLL | NativeMethods.WS_HSCROLL;
                NativeMethods.SetBorderStyle(P, m_BorderStyle);
                return P;
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == NativeMethods.WM_VSCROLL || m.Msg == NativeMethods.WM_HSCROLL)
            {
                bool bHorz = (m.Msg == NativeMethods.WM_HSCROLL);

                NativeMethods.ScrollInfo Info = NativeMethods.GetScrollInfo(this, bHorz);
                int iNewPos = Info.nPos;
                int iOriginalPos = iNewPos;

                //The SB_THUMBTRACK code is only in the lower word.
                ushort usSBCode = (ushort)((int)m.WParam & 0xFFFF);
                switch (usSBCode)
                {
                    case NativeMethods.SB_TOP: //SB_LEFT
                        iNewPos = Info.nMin;
                        break;
                    case NativeMethods.SB_BOTTOM: //SB_RIGHT
                        iNewPos = Info.nMax;
                        break;
                    case NativeMethods.SB_LINEUP: //SB_LINELEFT;
                        iNewPos--;
                        break;
                    case NativeMethods.SB_LINEDOWN: //SB_LINERIGHT
                        iNewPos++;
                        break;
                    case NativeMethods.SB_PAGEUP: //SB_PAGELEFT
                        iNewPos -= (int)Info.nPage;
                        break;
                    case NativeMethods.SB_PAGEDOWN: //SB_PAGERIGHT
                        iNewPos += (int)Info.nPage;
                        break;
                    case NativeMethods.SB_THUMBTRACK:
                        iNewPos = Info.nTrackPos;
                        break;
                }

                if (bHorz)
                    ScrollHorizontally(iNewPos, iOriginalPos);
                else
                    ScrollVertically(iNewPos, iOriginalPos);
            }
            else
            {
                base.WndProc(ref m);
            }

            if (m.Msg == NativeMethods.WM_GETDLGCODE)
            {
                //I've since learned that the .NET way to do this is to
                //override IsInputKey and return true for the arrow keys.
                //But this works, and I don't feel like recoding and testing.
                m.Result = (IntPtr)((int)m.Result + NativeMethods.DLGC_WANTARROWS);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            bool bCtrl = e.Modifiers == Keys.Control;
            bool bShift = e.Modifiers == Keys.Shift;
            bool bNormal = e.Modifiers == 0;

            switch (e.KeyCode)
            {
                case Keys.C:
                    if (bCtrl)
                    {
                        if (HasSelection)
                        {
                            Clipboard.SetDataObject(SelectedText, true);
                        }
                    }
                    break;
                case Keys.Up:
                    if (bCtrl)
                    {
                        VScrollPos--;
                        OffsetPosition(-1, 0);
                    }
                    else if (bShift)
                    {
                        ExtendSelection(-1, 0);
                    }
                    else if (bNormal)
                    {
                        OffsetPosition(-1, 0);
                    }
                    break;
                case Keys.Down:
                    if (bCtrl)
                    {
                        VScrollPos++;
                        OffsetPosition(1, 0);
                    }
                    else if (bShift)
                    {
                        ExtendSelection(1, 0);
                    }
                    else if (bNormal)
                    {
                        OffsetPosition(1, 0);
                    }
                    break;
                case Keys.Left:
                    if (bShift)
                    {
                        ExtendSelection(0, -1);
                    }
                    else if (bNormal)
                    {
                        OffsetPosition(0, -1);
                    }
                    break;
                case Keys.Right:
                    if (bShift)
                    {
                        ExtendSelection(0, 1);
                    }
                    else if (bNormal)
                    {
                        OffsetPosition(0, 1);
                    }
                    break;
                case Keys.PageUp:
                    {
                        int iPage = NativeMethods.GetScrollPage(this, false);
                        if (bShift)
                        {
                            ExtendSelection(-iPage, 0);
                        }
                        else if (bNormal)
                        {
                            VScrollPos -= iPage;
                            OffsetPosition(-iPage, 0);
                        }
                    }
                    break;
                case Keys.PageDown:
                    {
                        int iPage = NativeMethods.GetScrollPage(this, false);
                        if (bShift)
                        {
                            ExtendSelection(iPage, 0);
                        }
                        else if (bNormal)
                        {
                            VScrollPos += iPage;
                            OffsetPosition(iPage, 0);
                        }
                    }
                    break;
                case Keys.Home:
                    if (bCtrl)
                    {
                        SetPosition(0, 0);
                    }
                    else if (bShift)
                    {
                        ExtendSelection(0, -Position.Column);
                    }
                    else if (bNormal)
                    {
                        SetPosition(Position.Line, 0);
                    }
                    break;
                case Keys.End:
                    if (bCtrl)
                    {
                        int iLine = LineCount;
                        SetPosition(iLine, GetLineLength(iLine));
                    }
                    else if (bShift)
                    {
                        ExtendSelection(0, GetLineLength(Position.Line) - Position.Column);
                    }
                    else if (bNormal)
                    {
                        int iLine = Position.Line;
                        SetPosition(iLine, GetLineLength(iLine));
                    }
                    break;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            //We must setup the scroll bars before calling the base handler.
            //Attached event handlers like DiffOverview need to be able to pull
            //the correct FirstVisibleLine and VisibleLineCount properties.
            SetupScrollBars();

            //Now, call the base handler and let it notify registered delegates.
            base.OnSizeChanged(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            //Do these first so event handlers can pull correct values.
            UpdateTextMetrics(true);
            SetupScrollBars();

            //Now, call the base handler and let it notify registered delegates.
            base.OnFontChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //Get scroll positions
            int iYPos = VScrollPos;
            int iXPos = HScrollPos;

            //Find painting limits
            int iNumLines = m_Lines != null ? m_Lines.Count : 0;
            int iFirstLine = Math.Max(0, iYPos + e.ClipRectangle.Top / LineHeight);
            int iLastCalcLine = iYPos + e.ClipRectangle.Bottom / LineHeight;
            int iLastLine = Math.Min(iNumLines - 1, iLastCalcLine);

            //Create some graphics objects
            Graphics G = e.Graphics;
            SolidBrush FontBrush = new SolidBrush(Enabled ? ForeColor : SystemColors.GrayText);
            SolidBrush BackBrush = new SolidBrush(BackColor);
            Brush GutterBrush = SystemBrushes.Control;

            //See what we need to paint.  For horz scrolling,
            //the gutter won't need it.  For focus changes,
            //the lines won't need it.
            bool bPaintGutter = e.ClipRectangle.X < GutterWidth;
            bool bPaintLine = e.ClipRectangle.X + e.ClipRectangle.Width >= GutterWidth;
            bool bHasFocus = Focused;

            //Indent the text horizontally a little bit
            int iLineNumIndent = CharWidth / 2; //This centers it since it has 1 extra char width

            //Determine the selection positions in forward order
            bool bHasSelection = HasSelection;
            DiffViewPosition StartSel = m_SelectionStart;
            DiffViewPosition EndSel = m_Position;
            if (bHasSelection)
            {
                GetForwardOrderSelection(out StartSel, out EndSel);
            }

            //Paint each line
            for (int i = iFirstLine; i <= iLastLine; i++)
            {
                //If we get inside this loop there must be at least one line.
                Debug.Assert(m_Lines != null && m_Lines.Count > 0);

                int x = CharWidth * (-iXPos) + GutterWidth;
                int y = LineHeight * (i - iYPos);

                DiffViewLine L = m_Lines[i];

                if (bPaintLine)
                {
                    //If any portion of the line is selected, we have to paint that too.
                    int iSelStartColumn = 0, iSelEndColumn = 0, iSelStartX = 0, iSelEndX = 0;
                    bool bLineHasSelection = false;
                    bool bLineFullySelected = false;
                    if (bHasSelection && i >= StartSel.Line && i <= EndSel.Line)
                    {
                        int iLineLength = L.Text.Length;
                        iSelStartColumn = (i == StartSel.Line) ? StartSel.Column : 0;
                        iSelEndColumn = (i == EndSel.Line) ? Math.Min(EndSel.Column, iLineLength) : iLineLength;

                        bLineHasSelection = true;
                        bLineFullySelected = (i > StartSel.Line && i < EndSel.Line) || (iSelStartColumn == 0 && iSelEndColumn == iLineLength);

                        iSelStartX = GetXForColumn(G, L.Text, iSelStartColumn);
                        iSelEndX = GetXForColumn(G, L.Text, iSelEndColumn);
                    }

                    //Draw the background.  Even if the line is completely selected,
                    //we want to do this because after the last char, we don't paint
                    //with the selection color.  So it needs to be the normal back color.
                    BackBrush.Color = L.Edited ? DiffOptions.GetColorForEditType(L.EditType) : BackColor;
                    DrawBackground(G, BackBrush, y);

                    //Draw the line text if any portion of it is unselected.
                    if (!bLineFullySelected)
                    {
                        DrawStringWithWS(G, L.Text, FontBrush, x, y);
                    }

                    //Draw the selection
                    if (bLineHasSelection)
                    {
                        //Draw the background
                        RectangleF R = new RectangleF(iSelStartX, y, iSelEndX - iSelStartX, LineHeight);
                        Brush B = bHasFocus ? SystemBrushes.Highlight : SystemBrushes.Control;
                        G.FillRectangle(B, R);

                        //Draw the selected text.  Because of GDI+'s device independence,
                        //it is virtually impossible to draw adjacent text correctly.  Minor
                        //character shifting always occurs when DrawString starts at a
                        //different X because of how GDI+ does grid fitting.  So I'm working
                        //around that by drawing the string from the original X, but I'm
                        //changing the clipping region so that only the portion inside my
                        //highlighted rectangle will show up.
                        Region OriginalClipRegion = G.Clip;
                        G.Clip = new Region(R);
                        B = bHasFocus ? SystemBrushes.HighlightText : SystemBrushes.WindowText;
                        DrawStringWithWS(G, L.Text, B, x, y);
                        G.Clip = OriginalClipRegion;
                    }
                }

                if (bPaintGutter)
                {
                    //Draw the gutter background
                    BackBrush.Color = BackColor;

                    Brush LineGutterBrush = GutterBrush;
                    Brush LineFontBrush = FontBrush;

                    if (i == m_Position.Line && bHasFocus)
                    {
                        LineGutterBrush = SystemBrushes.Highlight;
                        LineFontBrush = SystemBrushes.HighlightText;
                    }

                    DrawGutterBackground(G, LineGutterBrush, BackBrush, y);

                    //Draw the line number (as 1-based)
                    if (L.Number >= 0)
                    {
                        //We don't need DrawStringWithWS here because there will never be whitespace.
                        G.DrawString(String.Format(GutterFormat, L.Number + 1), Font, LineFontBrush, iLineNumIndent, y);
                    }
                }
            }

            //Draw the background and an empty gutter for any 
            //blank lines past the end of the actual lines.
            BackBrush.Color = BackColor;
            for (int i = iLastLine + 1; i <= iLastCalcLine; i++)
            {
                int y = LineHeight * (i - iYPos);

                DrawBackground(G, BackBrush, y);

                if (bPaintGutter)
                {
                    DrawGutterBackground(G, GutterBrush, BackBrush, y);
                }
            }

            //We can't free GutterBrush since it is a system brush.
            BackBrush.Dispose();
            FontBrush.Dispose();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            Debug.Assert(m_Caret == null);
            m_Caret = new Caret(this, LineHeight);
            UpdateCaret();

            InvalidateSelection();
            InvalidateCaretGutter();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            //Sometimes in the debugger, the IDE seems to
            //steal focus, and it seems like this event fires
            //twice without an intermediate OnGotFocus.  So
            //we have to protect against that here and check
            //to see if we still have the caret.
            if (m_Caret != null)
            {
                m_Caret.Dispose();
                m_Caret = null;
            }

            InvalidateSelection();
            InvalidateCaretGutter();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Focused && CanFocus)
            {
                Focus();
            }

            if (e.X >= GutterWidth)
            {
                DiffViewPosition Pos = GetPosFromPoint(e.X, e.Y);

                //Only change pos if non-right-click or right-click not in selection
                if (e.Button != MouseButtons.Right || !InSelection(Pos))
                {
                    SetPosition(Pos.Line, Pos.Column);
                }

                if (e.Button == MouseButtons.Left)
                {
                    Capture = true;
                    m_bCapturedMouse = true;
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //Update the mouse cursor to be an IBeam if we're not in the gutter.
            Cursor = (e.X < GutterWidth) ? Cursors.Default : Cursors.IBeam;

            if (!m_bCapturedMouse) return;

            if (e.Button == MouseButtons.Left)
            {
                //Determine if we're at or above the first visible line
                //or at or below the last visible line.  If so, then 
                //auto-scroll.  Similarly, if we're on the first or last
                //character or beyond, then auto-scroll.
                Rectangle R = new Rectangle(GutterWidth, 0, ClientSize.Width, ClientSize.Height);
                R.Inflate(-CharWidth, -LineHeight);
                if (!R.Contains(e.X, e.Y))
                {
                    m_iVAutoScrollAmount = 0;
                    if (e.Y < R.Y)
                        m_iVAutoScrollAmount = -1;
                    else if (e.Y > R.Bottom)
                        m_iVAutoScrollAmount = 1;

                    m_iHAutoScrollAmount = 0;
                    if (e.X < R.X)
                        m_iHAutoScrollAmount = -1;
                    else if (e.X > R.Right)
                        m_iHAutoScrollAmount = 1;

                    m_AutoScrollTimer.Enabled = true;
                }
                else
                {
                    m_AutoScrollTimer.Enabled = false;
                }

                //Set the selection end to the current mouse position
                //if the new position is different from the caret position.
                DiffViewPosition Pos = GetPosFromPoint(e.X, e.Y);
                if (Pos != m_Position)
                {
                    SetSelectionEnd(Pos.Line, Pos.Column);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!m_bCapturedMouse) return;

            if (e.Button == MouseButtons.Left)
            {
                Capture = false;
                m_bCapturedMouse = false;
                m_AutoScrollTimer.Enabled = false;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_Caret != null)
                {
                    m_Caret.Dispose();
                }

                DiffOptions.OptionsChanged -= m_OptionsChangedHandler;
            }
            base.Dispose(disposing);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            m_iWheelDelta += e.Delta;
            if (Math.Abs(m_iWheelDelta) >= 120)
            {
                //I'm using "-=" here because Delta is reversed from what seems normal to me.
                //(e.g. wheel scrolling towards the user returns a negative value).
                VScrollPos -= SystemInformation.MouseWheelScrollLines * (m_iWheelDelta / 120);
                m_iWheelDelta = 0;
            }
        }

        #endregion

        #region Private Properties

        private int LineHeight
        {
            get
            {
                return m_iLineHeight;
            }
        }

        private int CharWidth
        {
            get
            {
                return m_iCharWidth;
            }
        }

        private float CharWidthF
        {
            get
            {
                return m_fCharWidth;
            }
        }

        private int GutterWidth
        {
            get
            {
                return m_iGutterWidth;
            }
        }

        private string GutterFormat
        {
            get
            {
                return m_strGutterFormat;
            }
        }

        #endregion

        #region Private Methods

        private void DrawStringWithWS(Graphics G, string strText, Brush B, int x, int y)
        {
            if (m_bShowWhitespace)
            {
                strText = String.Format("{0}", strText.Replace(' ', ''));
            }

            G.DrawString(strText, Font, B, x, y, m_StringFormat);
        }

        private int GetXForColumn(Graphics G, string strText, int iColumn)
        {
            int iLength = Math.Max(0, Math.Min(strText.Length, iColumn));
            float fX = MeasureStringDisplayWidth(G, strText, iLength, Font, m_StringFormat);
            return (int)Math.Ceiling(fX) - HScrollPos * CharWidth + GutterWidth;
        }

        private void DrawBackground(Graphics G, Brush B, int y)
        {
            G.FillRectangle(B, GutterWidth, y, ClientSize.Width, LineHeight);
        }

        private void DrawGutterBackground(Graphics G, Brush GutterBrush, Brush BackBrush, int y)
        {
            int iDarkWidth = GutterWidth - c_iGutterSeparator;
            G.FillRectangle(GutterBrush, 0, y, iDarkWidth, LineHeight);
            G.FillRectangle(BackBrush, iDarkWidth, y, c_iGutterSeparator, LineHeight);
            G.DrawLine(SystemPens.ControlDark, iDarkWidth - 1, y, iDarkWidth - 1, y + LineHeight);
        }

        private void SetupScrollBars()
        {
            //Vertical
            int iPage = ClientSize.Height / LineHeight;
            int iMax = m_Lines != null ? m_Lines.Count - 1 : 0;
            NativeMethods.SetScrollPageAndRange(this, false, 0, iMax, iPage);

            //Horizontal
            iPage = ClientSize.Width / CharWidth;
            //Don't subtract 1 here because it's necessary to see 
            //everything when the client size is small.
            iMax = m_Lines != null ? m_Lines.LongestStringLength : 0;
            NativeMethods.SetScrollPageAndRange(this, true, 0, iMax, iPage);
        }

        private int UpdateScrollPos(int iNewPos, bool bHorz)
        {
            //Set the position and then retrieve it.  Due to adjustments by Windows 
            //(e.g. if Pos is > Max) it may not be the same as the value set.
            NativeMethods.SetScrollPos(this, bHorz, iNewPos);
            return NativeMethods.GetScrollPos(this, bHorz);
        }

        private void ScrollVertically(int iNewPos, int iOriginalPos)
        {
            int iPos = UpdateScrollPos(iNewPos, false);
            if (iPos != iOriginalPos)
            {
                int iNumPixels = LineHeight * (iOriginalPos - iPos);
                if (iNumPixels < ClientSize.Height)
                {
                    NativeMethods.ScrollWindow(this, 0, iNumPixels);
                }
                else
                {
                    Invalidate();
                    //We have to manually update the caret
                    //if we don't call ScrollWindow.
                    UpdateCaret();
                }

                if (VScrollPosChanged != null)
                {
                    VScrollPosChanged(this, EventArgs.Empty);
                }
            }
        }

        private void ScrollHorizontally(int iNewPos, int iOriginalPos)
        {
            int iPos = UpdateScrollPos(iNewPos, true);
            if (iPos != iOriginalPos)
            {
                //Don't scroll the line number gutter
                Rectangle R = ClientRectangle;
                R.Offset(GutterWidth, 0);
                //This really seems like it should be necessary,
                //but if I do it then a GutterWidth sized band
                //is skipped on the scrolling/invalidated end...
                //R.Width -= GutterWidth;

                int iNumPixels = CharWidth * (iOriginalPos - iPos);
                if (iNumPixels < ClientSize.Width)
                {
                    //Scroll the subset of the window in the clipping region.
                    //
                    //Note: We must scroll by the integral CharWidth.  Otherwise,
                    //round off causes pixel columns to occasionally get dropped
                    //or duplicated.  This makes for ugly text until the next full
                    //repaint.  By always using the same integral CharWidth, the
                    //text scrolls smoothly and correctly.
                    //
                    //To make this smooth and correct, we also set the scroll bar
                    //Page size and calculate X in OnPaint using the integral
                    //CharWidth.
                    NativeMethods.ScrollWindow(this, iNumPixels, 0, ref R, ref R);
                }
                else
                {
                    Invalidate(R);
                }

                //ScrollWindow is supposed to update the caret position too, 
                //but it doesn't when a rect and clipping rect are specified.
                //So we have to update it manually.  This is also necessary
                //because we don't ever want the caret to display in the gutter.
                UpdateCaret();

                if (HScrollPosChanged != null)
                {
                    HScrollPosChanged(this, EventArgs.Empty);
                }
            }
        }

        private void UpdateTextMetrics(bool bFontOrTabsChanged)
        {
            if (bFontOrTabsChanged)
            {
                //Get the pixel width that a space should be.
                Graphics G = Graphics.FromHwnd(Handle);

                //See KBase article Q125681 for what I'm doing here to get the average character width.
                m_fCharWidth = MeasureStringDisplayWidth(G, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 52, Font, m_StringFormat) / 52;
                m_iCharWidth = (int)Math.Ceiling(m_fCharWidth);

                //Get the average pixels per inch
                float fDpi = (G.DpiX + G.DpiY) / 2;

                G.Dispose();

                //Get the line height in pixels
                FontFamily FF = Font.FontFamily;
                int iLineSpacingDesignUnits = FF.GetLineSpacing(Font.Style);
                int iFontHeightDesignUnits = FF.GetEmHeight(Font.Style);
                float fFontPoints = Font.Size;
                float fFontPixels = fFontPoints * fDpi / 72;
                m_iLineHeight = (int)Math.Ceiling((fFontPixels * iLineSpacingDesignUnits) / iFontHeightDesignUnits);
                //This height still isn't "enough" (i.e. it still doesn't match 
                //what the GetTextMetrics API would return as TEXTMETRICS.Height 
                //+ TEXTMETRICS.ExternalLeading.  It seems to be one pixel too
                //short, so I'll just add it back.
                m_iLineHeight++;

                //Update the string format's Tab stops
                float[] arTabStops = new float[100];
                float fTabWidth = m_fCharWidth * DiffOptions.SpacesPerTab;
                //Without Floor() then sometimes we end up with tabs that are
                //only a few pixels wide, when they should be fTabWidth wide.
                //It happens when a cumulative tab stop has enough fractional
                //portion to be more than a pixel larger than the current
                //non-whitespace string length.  This seems to work fine for
                //monospaced fonts, and tabs and spaces never line up for
                //proportional fonts anyway.
                fTabWidth = (float)Math.Floor(fTabWidth);
                for (int i = 0; i < arTabStops.Length; i++)
                {
                    arTabStops[i] = fTabWidth;
                }
                m_StringFormat.SetTabStops(0, arTabStops);
            }

            //Set the gutter width to the CharWidth times the
            //number of characters we'll need to display.  Then
            //add another character for padding, another
            //pixel so we can have a separator line, and then
            //a small separator window-colored area.
            int iMaxLineNumChars = 1;
            if (m_Lines != null && m_Lines.Count > 0)
            {
                //Get the largest number.  Add 1 to it because we will
                //when we display it.  This is important when the number
                //is 9, but will be displayed as 10, etc.
                //
                //Also, we want to take the max of MaxLineNumber and 1 to
                //ensure that we never take the Log of 1 or less.  Negatives
                //and 0 don't have Logs, and Log(1) returns 0.  We always
                //want to end up with at least one for iMaxLineNumChars.
                int iMaxLineNumber = Math.Max(m_Lines.MaxLineNumber, 1) + 1;
                //If the number of lines is NNNN (e.g. 1234), we need to get 4.
                //Add 1 and take the floor so that 10, 100, 1000, etc. will work
                //correctly.
                iMaxLineNumChars = (int)Math.Floor(Math.Log10(iMaxLineNumber) + 1);
            }
            m_iGutterWidth = m_iCharWidth * (iMaxLineNumChars + 1) + 1 + c_iGutterSeparator;

            //Build the gutter format string
            StringBuilder B = new StringBuilder(20);
            B.Append("{0:");
            for (int i = 0; i < iMaxLineNumChars; i++)
            {
                B.Append("0");
            }
            B.Append("}");
            m_strGutterFormat = B.ToString();

            //Update the caret position (Gutter width or Font changes affect it)
            UpdateCaret();
        }

        private void UpdateCaret()
        {
            if (m_Lines != null && m_Caret != null)
            {
                Point NewPt = GetPointFromPos(m_Position.Line, m_Position.Column);
                m_Caret.Visible = (NewPt.X >= (GutterWidth - c_iGutterSeparator));
                m_Caret.Position = NewPt;
            }
        }

        private void ExtendSelection(int iLines, int iColumns)
        {
            int iLine = m_Position.Line + iLines;
            int iColumn = m_Position.Column + iColumns;
            SetSelectionEnd(iLine, iColumn);
        }

        private void SetSelectionEnd(int iLine, int iColumn)
        {
            bool bSelectionChanged = false;
            if (!HasSelection)
            {
                m_SelectionStart = m_Position;
                bSelectionChanged = true;
            }

            //Move the Position but keep the selection start
            int iOriginalLine = m_Position.Line;
            SetPosition(iLine, iColumn, false, true);
            int iNumLines = Math.Abs(iLine - iOriginalLine);

            //Invalidate new selection
            int iFirstLine = Math.Min(iOriginalLine, iLine);
            Point Pt = GetPointFromPos(iFirstLine, 0);
            Rectangle R = new Rectangle(GutterWidth, Pt.Y, ClientSize.Width, (iNumLines + 1) * LineHeight);
            Invalidate(R);

            if (bSelectionChanged)
            {
                FireSelectionChanged();
            }
        }

        private void OffsetPosition(int iLines, int iColumns)
        {
            int iLine = m_Position.Line + iLines;
            int iColumn = m_Position.Column + iColumns;
            SetPosition(iLine, iColumn);
        }

        private void SetPosition(int iLine, int iColumn)
        {
            SetPosition(iLine, iColumn, true, true);
        }

        private void SetPosition(int iLine, int iColumn, bool bClearSelection, bool bScrollToCaret)
        {
            if (iLine >= LineCount) iLine = LineCount - 1;
            if (iLine < 0) iLine = 0;

            int iLength = GetLineLength(iLine);
            if (iColumn > iLength) iColumn = iLength;
            if (iColumn < 0) iColumn = 0;

            if (bClearSelection)
            {
                ClearSelection();
            }

            bool bLineNumberChanged = m_Position.Line != iLine;
            bool bColumnNumberChanged = m_Position.Column != iColumn;

            if (bLineNumberChanged || bColumnNumberChanged)
            {
                //Invalidate the old gutter line.
                if (bLineNumberChanged) InvalidateCaretGutter();

                m_Position = new DiffViewPosition(iLine, iColumn);

                //Invalidate the new gutter line.
                if (bLineNumberChanged) InvalidateCaretGutter();

                UpdateCaret();

                //If the selection range is now empty, then clear the selection.
                if (m_Position == m_SelectionStart)
                {
                    ClearSelection();
                    //Set the flag so we don't refire the SelectionChanged event below.
                    bClearSelection = true;
                }

                if (m_Lines != null)
                {
                    if (PositionChanged != null)
                    {
                        PositionChanged(this, EventArgs.Empty);
                    }

                    //If we cleared the selection earlier, then that
                    //fire a SelectionChanged event.  If not, then we
                    //need to fire it now because we've changed the
                    //selection end point.
                    if (!bClearSelection)
                    {
                        FireSelectionChanged();
                    }
                }
            }

            if (bScrollToCaret)
            {
                ScrollToCaret();
            }
        }

        private void InvalidateCaretGutter()
        {
            //Invalidate the gutter portion for the line with the caret.
            Point Pt = GetPointFromPos(m_Position.Line, 0);
            Rectangle R = new Rectangle(0, Pt.Y, GutterWidth, LineHeight);
            Invalidate(R);
        }

        private void ClearSelection()
        {
            if (HasSelection)
            {
                InvalidateSelection();
                m_SelectionStart = DiffViewPosition.Empty;
                FireSelectionChanged();
            }
        }

        private void InvalidateSelection()
        {
            if (HasSelection)
            {
                int iFirstLine = Math.Min(m_SelectionStart.Line, m_Position.Line);
                Point Pt = GetPointFromPos(iFirstLine, 0);
                int iNumLines = Math.Abs(m_SelectionStart.Line - m_Position.Line) + 1;
                Rectangle R = new Rectangle(GutterWidth, Pt.Y, ClientSize.Width, iNumLines * LineHeight);
                Invalidate(R);
            }
        }

        private int GetLineLength(int iLine)
        {
            return GetLineText(iLine).Length;
        }

        private string GetLineText(int iLine)
        {
            string strResult = "";

            if (m_Lines != null && iLine >= 0 && iLine < m_Lines.Count)
            {
                strResult = m_Lines[iLine].Text;
            }

            return strResult;
        }

        /// <summary>
        /// This method is to work around the fact that GDI+'s MeasureString function
        /// tries to work in a device independent manner, so it returns too large a
        /// result for small DPIs like 96.  For more information see:
        ///		http://www.codeproject.com/cs/media/measurestring.asp
        ///		http://www.gotdotnet.com/team/windowsforms/gdiptext.aspx
        ///		http://www.syncfusion.com/FAQ/WinForms/FAQ_c39c.asp
        /// </summary>
        private static float MeasureStringDisplayWidth(Graphics G, string strText, int iLength, Font Font, StringFormat Fmt)
        {
            if (iLength > 0)
            {
                StringFormat NewFmt = new StringFormat(Fmt);
                NewFmt.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

                RectangleF R = new RectangleF(0, 0, 1000000, 1000000);
                CharacterRange[] arRanges = { new CharacterRange(0, iLength) };
                NewFmt.SetMeasurableCharacterRanges(arRanges);

                Region[] arRegions = G.MeasureCharacterRanges(strText, Font, R, NewFmt);
                R = arRegions[0].GetBounds(G);

                return R.Right;
            }
            else
            {
                //This little fudge factor is used to make things line up better.
                //Without this, then the caret is too far left on empty lines and
                //at column 0.
                return c_iEmptyStringWidth;
            }
        }

        private void AutoScrollTimer_Tick(object sender, EventArgs e)
        {
            VScrollPos += m_iVAutoScrollAmount;
            HScrollPos += m_iHAutoScrollAmount;

            //Set the selection end
            Point Pt = PointToClient(Control.MousePosition);
            DiffViewPosition Pos = GetPosFromPoint(Pt.X, Pt.Y);
            SetSelectionEnd(Pos.Line, Pos.Column);
        }

        private void DiffOptionsChanged(object sender, EventArgs e)
        {
            //The colors and/or tab width changed.
            UpdateTextMetrics(true);

            //If the tab width changed, we have to recalculate the 
            //scroll boundaries based on string lengths
            if (m_Lines != null)
            {
                m_Lines.RecheckLongestStringLength();
            }
            SetupScrollBars();

            //Invalidating the whole window will take
            //care of the color change.
            Invalidate();
        }

        private void UpdateAfterSetData()
        {
            //Reset the position before we start calculating things
            m_Position = new DiffViewPosition(0, 0);
            m_SelectionStart = DiffViewPosition.Empty;

            //We have to call this to recalc the gutter width
            UpdateTextMetrics(false);

            //We have to call this to setup the scroll bars
            SetupScrollBars();

            //Reset the scroll position
            VScrollPos = 0;
            HScrollPos = 0;

            //Update the caret
            UpdateCaret();

            //Force a repaint
            Invalidate();

            //Fire the LinesChanged event
            if (LinesChanged != null)
            {
                LinesChanged(this, EventArgs.Empty);
            }

            //Fire the position changed event
            if (PositionChanged != null)
            {
                PositionChanged(this, EventArgs.Empty);
            }

            FireSelectionChanged();
        }

        private void GetForwardOrderSelection(out DiffViewPosition StartSel, out DiffViewPosition EndSel)
        {
            //Determine the selection positions in forward order.
            //Get them in order in case we have a reverse selection.
            StartSel = m_SelectionStart;
            EndSel = m_Position;
            if (StartSel.Line > EndSel.Line || (StartSel.Line == EndSel.Line && StartSel.Column > EndSel.Column))
            {
                StartSel = m_Position;
                EndSel = m_SelectionStart;
            }
        }

        private bool InSelection(DiffViewPosition Pos)
        {
            bool bResult = false;

            if (HasSelection)
            {
                DiffViewPosition StartSel, EndSel;
                GetForwardOrderSelection(out StartSel, out EndSel);
                bResult = (Pos >= StartSel && Pos <= EndSel);
            }

            return bResult;
        }

        private bool GetSingleLineSelectedText(out string strText)
        {
            if (HasSelection)
            {
                DiffViewPosition StartSel, EndSel;
                GetForwardOrderSelection(out StartSel, out EndSel);
                if (StartSel.Line == EndSel.Line && EndSel.Column > StartSel.Column)
                {
                    strText = m_Lines[StartSel.Line].Text.Substring(StartSel.Column, EndSel.Column - StartSel.Column);
                    return true;
                }
            }

            strText = null;
            return false;
        }

        private void FireSelectionChanged()
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Private Data Members

        private BorderStyle m_BorderStyle = BorderStyle.Fixed3D;
        private DiffViewLines m_Lines;
        private IList<string> m_StringList;
        private int m_iLineHeight = 1; //Make these non-zero because we divide by them.
        private int m_iCharWidth = 1;
        private float m_fCharWidth = 1;
        private int m_iGutterWidth = 1;
        private const int c_iGutterSeparator = 2;
        private string m_strGutterFormat = "{0}";
        private Caret m_Caret;
        private DiffViewPosition m_Position;
        private StringFormat m_StringFormat = new StringFormat();
        private DiffViewPosition m_SelectionStart = DiffViewPosition.Empty;
        private bool m_bCapturedMouse = false;
        private Timer m_AutoScrollTimer;
        private int m_iVAutoScrollAmount = 0;
        private int m_iHAutoScrollAmount = 0;
        private bool m_bShowWhitespace = false;
        private EventHandler m_OptionsChangedHandler;
        private int m_iWheelDelta = 0;

        private const int c_iEmptyStringWidth = 2;

        #endregion
    }
}
