#region Copyright And Revision History

/*---------------------------------------------------------------------------

	DiffControl.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
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

namespace Ankh.Diff.DiffUtils.Controls
{
	public class DiffControl : System.Windows.Forms.UserControl, IDisposable
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

		public void SetData(IList StringListA, IList StringListB, EditScript Script)
		{
			SetData(StringListA, StringListB, Script, String.Empty, String.Empty);
		}

		public void SetData(IList StringListA, IList StringListB, EditScript Script, string strNameA, string strNameB)
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

		public void ViewFile()
		{
			if (!CanViewFile) return;

			string strFileName;
			if (ViewA.Focused || edtLeft.Focused)
			{
				strFileName = edtLeft.Text;
			}
			else
			{
				strFileName = edtRight.Text;
			}

			Utilities.ShellExecute(this, strFileName, "open");
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

		public event DifferenceEventHandler ShowTextDifferences;

		public event EventHandler LineDiffSizeChanged;

		public event EventHandler RecompareNeeded;

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Overview = new Ankh.Diff.DiffUtils.Controls.DiffOverview();
			this.ViewA = new Ankh.Diff.DiffUtils.Controls.DiffView();
			this.CtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsSep7 = new System.Windows.Forms.ToolStripSeparator();
			this.ViewB = new Ankh.Diff.DiffUtils.Controls.DiffView();
			this.pnlSeparator = new System.Windows.Forms.Panel();
			this.pnlMiddle = new System.Windows.Forms.Panel();
			this.pnlRight = new System.Windows.Forms.Panel();
			this.edtRight = new System.Windows.Forms.TextBox();
			this.MiddleSplitter = new System.Windows.Forms.Splitter();
			this.pnlLeft = new System.Windows.Forms.Panel();
			this.edtLeft = new System.Windows.Forms.TextBox();
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.ViewLineDiff = new Ankh.Diff.DiffUtils.Controls.DiffView();
			this.BottomSplitter = new System.Windows.Forms.Splitter();
			this.ToolBar = new System.Windows.Forms.ToolStrip();
			this.tsSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsSep2 = new System.Windows.Forms.ToolStripSeparator();
			this.tsSep3 = new System.Windows.Forms.ToolStripSeparator();
			this.tsSep4 = new System.Windows.Forms.ToolStripSeparator();
			this.tsSep5 = new System.Windows.Forms.ToolStripSeparator();
			this.tsSep6 = new System.Windows.Forms.ToolStripSeparator();
			this.lblPosition = new System.Windows.Forms.ToolStripLabel();
			this.mnuViewFile = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuTextDiff = new System.Windows.Forms.ToolStripMenuItem();
			this.btnViewFile = new System.Windows.Forms.ToolStripButton();
			this.btnCopy = new System.Windows.Forms.ToolStripButton();
			this.btnTextDiff = new System.Windows.Forms.ToolStripButton();
			this.btnFind = new System.Windows.Forms.ToolStripButton();
			this.btnFindNext = new System.Windows.Forms.ToolStripButton();
			this.btnFindPrevious = new System.Windows.Forms.ToolStripButton();
			this.btnFirstDiff = new System.Windows.Forms.ToolStripButton();
			this.btnPrevDiff = new System.Windows.Forms.ToolStripButton();
			this.btnNextDiff = new System.Windows.Forms.ToolStripButton();
			this.btnLastDiff = new System.Windows.Forms.ToolStripButton();
			this.btnGotoLine = new System.Windows.Forms.ToolStripButton();
			this.btnRecompare = new System.Windows.Forms.ToolStripButton();
			this.lblDelete = new System.Windows.Forms.ToolStripLabel();
			this.lblChange = new System.Windows.Forms.ToolStripLabel();
			this.lblInsert = new System.Windows.Forms.ToolStripLabel();
			this.CtxMenu.SuspendLayout();
			this.pnlMiddle.SuspendLayout();
			this.pnlRight.SuspendLayout();
			this.pnlLeft.SuspendLayout();
			this.pnlBottom.SuspendLayout();
			this.ToolBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// Overview
			// 
			this.Overview.BackColor = System.Drawing.SystemColors.Window;
			this.Overview.DiffView = null;
			this.Overview.Dock = System.Windows.Forms.DockStyle.Left;
			this.Overview.Location = new System.Drawing.Point(0, 0);
			this.Overview.Name = "Overview";
			this.Overview.Size = new System.Drawing.Size(32, 138);
			this.Overview.TabIndex = 0;
			this.Overview.TabStop = false;
			this.Overview.Text = "diffOverview1";
			this.Overview.LineClick += new Ankh.Diff.DiffUtils.Controls.DiffLineClickEventHandler(this.Overview_LineClick);
			// 
			// ViewA
			// 
			this.ViewA.BackColor = System.Drawing.SystemColors.Window;
			this.ViewA.CenterVisibleLine = 3;
			this.ViewA.ContextMenuStrip = this.CtxMenu;
			this.ViewA.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.ViewA.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ViewA.FirstVisibleLine = 0;
			this.ViewA.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ViewA.HScrollPos = 0;
			this.ViewA.Location = new System.Drawing.Point(0, 20);
			this.ViewA.Name = "ViewA";
			this.ViewA.Size = new System.Drawing.Size(213, 118);
			this.ViewA.TabIndex = 2;
			this.ViewA.Text = "diffView1";
			this.ViewA.VScrollPos = 0;
			this.ViewA.HScrollPosChanged += new System.EventHandler(this.ViewA_HScrollPosChanged);
			this.ViewA.SelectionChanged += new System.EventHandler(this.View_PositionChanged);
			this.ViewA.VScrollPosChanged += new System.EventHandler(this.ViewA_VScrollPosChanged);
			this.ViewA.PositionChanged += new System.EventHandler(this.View_PositionChanged);
			// 
			// CtxMenu
			// 
			this.CtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewFile,
            this.tsSep7,
            this.mnuCopy,
            this.mnuTextDiff});
			this.CtxMenu.Name = "CtxMenu";
			this.CtxMenu.Size = new System.Drawing.Size(155, 76);
			// 
			// tsSep7
			// 
			this.tsSep7.Name = "tsSep7";
			this.tsSep7.Size = new System.Drawing.Size(151, 6);
			// 
			// ViewB
			// 
			this.ViewB.BackColor = System.Drawing.SystemColors.Window;
			this.ViewB.CenterVisibleLine = 3;
			this.ViewB.ContextMenuStrip = this.CtxMenu;
			this.ViewB.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.ViewB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ViewB.FirstVisibleLine = 0;
			this.ViewB.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ViewB.HScrollPos = 0;
			this.ViewB.Location = new System.Drawing.Point(0, 20);
			this.ViewB.Name = "ViewB";
			this.ViewB.Size = new System.Drawing.Size(233, 118);
			this.ViewB.TabIndex = 4;
			this.ViewB.Text = "diffView2";
			this.ViewB.VScrollPos = 0;
			this.ViewB.HScrollPosChanged += new System.EventHandler(this.ViewB_HScrollPosChanged);
			this.ViewB.SelectionChanged += new System.EventHandler(this.View_PositionChanged);
			this.ViewB.VScrollPosChanged += new System.EventHandler(this.ViewB_VScrollPosChanged);
			this.ViewB.PositionChanged += new System.EventHandler(this.View_PositionChanged);
			// 
			// pnlSeparator
			// 
			this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlSeparator.Location = new System.Drawing.Point(32, 0);
			this.pnlSeparator.Name = "pnlSeparator";
			this.pnlSeparator.Size = new System.Drawing.Size(3, 138);
			this.pnlSeparator.TabIndex = 1;
			// 
			// pnlMiddle
			// 
			this.pnlMiddle.Controls.Add(this.pnlRight);
			this.pnlMiddle.Controls.Add(this.MiddleSplitter);
			this.pnlMiddle.Controls.Add(this.pnlLeft);
			this.pnlMiddle.Controls.Add(this.pnlSeparator);
			this.pnlMiddle.Controls.Add(this.Overview);
			this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlMiddle.Location = new System.Drawing.Point(0, 25);
			this.pnlMiddle.Name = "pnlMiddle";
			this.pnlMiddle.Size = new System.Drawing.Size(484, 138);
			this.pnlMiddle.TabIndex = 1;
			// 
			// pnlRight
			// 
			this.pnlRight.Controls.Add(this.ViewB);
			this.pnlRight.Controls.Add(this.edtRight);
			this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlRight.Location = new System.Drawing.Point(251, 0);
			this.pnlRight.Name = "pnlRight";
			this.pnlRight.Size = new System.Drawing.Size(233, 138);
			this.pnlRight.TabIndex = 5;
			// 
			// edtRight
			// 
			this.edtRight.Dock = System.Windows.Forms.DockStyle.Top;
			this.edtRight.Location = new System.Drawing.Point(0, 0);
			this.edtRight.Name = "edtRight";
			this.edtRight.ReadOnly = true;
			this.edtRight.Size = new System.Drawing.Size(233, 20);
			this.edtRight.TabIndex = 5;
			// 
			// MiddleSplitter
			// 
			this.MiddleSplitter.Location = new System.Drawing.Point(248, 0);
			this.MiddleSplitter.Name = "MiddleSplitter";
			this.MiddleSplitter.Size = new System.Drawing.Size(3, 138);
			this.MiddleSplitter.TabIndex = 3;
			this.MiddleSplitter.TabStop = false;
			// 
			// pnlLeft
			// 
			this.pnlLeft.Controls.Add(this.ViewA);
			this.pnlLeft.Controls.Add(this.edtLeft);
			this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlLeft.Location = new System.Drawing.Point(35, 0);
			this.pnlLeft.Name = "pnlLeft";
			this.pnlLeft.Size = new System.Drawing.Size(213, 138);
			this.pnlLeft.TabIndex = 4;
			// 
			// edtLeft
			// 
			this.edtLeft.Dock = System.Windows.Forms.DockStyle.Top;
			this.edtLeft.Location = new System.Drawing.Point(0, 0);
			this.edtLeft.Name = "edtLeft";
			this.edtLeft.ReadOnly = true;
			this.edtLeft.Size = new System.Drawing.Size(213, 20);
			this.edtLeft.TabIndex = 3;
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.ViewLineDiff);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 166);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(484, 38);
			this.pnlBottom.TabIndex = 5;
			// 
			// ViewLineDiff
			// 
			this.ViewLineDiff.BackColor = System.Drawing.SystemColors.Window;
			this.ViewLineDiff.CenterVisibleLine = 1;
			this.ViewLineDiff.ContextMenuStrip = this.CtxMenu;
			this.ViewLineDiff.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.ViewLineDiff.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ViewLineDiff.FirstVisibleLine = 0;
			this.ViewLineDiff.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ViewLineDiff.HScrollPos = 0;
			this.ViewLineDiff.Location = new System.Drawing.Point(0, 0);
			this.ViewLineDiff.Name = "ViewLineDiff";
			this.ViewLineDiff.ShowWhitespace = true;
			this.ViewLineDiff.Size = new System.Drawing.Size(484, 38);
			this.ViewLineDiff.TabIndex = 3;
			this.ViewLineDiff.Text = "diffView1";
			this.ViewLineDiff.VScrollPos = 0;
			this.ViewLineDiff.SelectionChanged += new System.EventHandler(this.View_PositionChanged);
			this.ViewLineDiff.PositionChanged += new System.EventHandler(this.View_PositionChanged);
			this.ViewLineDiff.SizeChanged += new System.EventHandler(this.ViewLineDiff_SizeChanged);
			// 
			// BottomSplitter
			// 
			this.BottomSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.BottomSplitter.Location = new System.Drawing.Point(0, 163);
			this.BottomSplitter.Name = "BottomSplitter";
			this.BottomSplitter.Size = new System.Drawing.Size(484, 3);
			this.BottomSplitter.TabIndex = 6;
			this.BottomSplitter.TabStop = false;
			// 
			// ToolBar
			// 
			this.ToolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnViewFile,
            this.tsSep1,
            this.btnCopy,
            this.btnTextDiff,
            this.tsSep2,
            this.btnFind,
            this.btnFindNext,
            this.btnFindPrevious,
            this.tsSep3,
            this.btnFirstDiff,
            this.btnPrevDiff,
            this.btnNextDiff,
            this.btnLastDiff,
            this.tsSep4,
            this.btnGotoLine,
            this.btnRecompare,
            this.tsSep5,
            this.lblDelete,
            this.lblChange,
            this.lblInsert,
            this.tsSep6,
            this.lblPosition});
			this.ToolBar.Location = new System.Drawing.Point(0, 0);
			this.ToolBar.Name = "ToolBar";
			this.ToolBar.Size = new System.Drawing.Size(484, 25);
			this.ToolBar.TabIndex = 7;
			this.ToolBar.Text = "toolStrip1";
			// 
			// tsSep1
			// 
			this.tsSep1.Name = "tsSep1";
			this.tsSep1.Size = new System.Drawing.Size(6, 25);
			// 
			// tsSep2
			// 
			this.tsSep2.Name = "tsSep2";
			this.tsSep2.Size = new System.Drawing.Size(6, 25);
			// 
			// tsSep3
			// 
			this.tsSep3.Name = "tsSep3";
			this.tsSep3.Size = new System.Drawing.Size(6, 25);
			// 
			// tsSep4
			// 
			this.tsSep4.Name = "tsSep4";
			this.tsSep4.Size = new System.Drawing.Size(6, 25);
			// 
			// tsSep5
			// 
			this.tsSep5.Name = "tsSep5";
			this.tsSep5.Size = new System.Drawing.Size(6, 25);
			// 
			// tsSep6
			// 
			this.tsSep6.Name = "tsSep6";
			this.tsSep6.Size = new System.Drawing.Size(6, 25);
			// 
			// lblPosition
			// 
			this.lblPosition.Name = "lblPosition";
			this.lblPosition.Size = new System.Drawing.Size(58, 22);
			this.lblPosition.Text = "Ln 1, Col 1";
			// 
			// mnuViewFile
			// 
			this.mnuViewFile.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.View;
			this.mnuViewFile.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuViewFile.Name = "mnuViewFile";
			this.mnuViewFile.Size = new System.Drawing.Size(154, 22);
			this.mnuViewFile.Text = "&View File";
			this.mnuViewFile.Click += new System.EventHandler(this.btnViewFile_Click);
			// 
			// mnuCopy
			// 
			this.mnuCopy.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.Copy;
			this.mnuCopy.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuCopy.Name = "mnuCopy";
			this.mnuCopy.Size = new System.Drawing.Size(154, 22);
			this.mnuCopy.Text = "&Copy";
			this.mnuCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// mnuTextDiff
			// 
			this.mnuTextDiff.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.ShowDifferences;
			this.mnuTextDiff.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuTextDiff.Name = "mnuTextDiff";
			this.mnuTextDiff.Size = new System.Drawing.Size(154, 22);
			this.mnuTextDiff.Text = "Compare &Text...";
			this.mnuTextDiff.Click += new System.EventHandler(this.mnuTextDiff_Click);
			// 
			// btnViewFile
			// 
			this.btnViewFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnViewFile.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.View;
			this.btnViewFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnViewFile.Name = "btnViewFile";
			this.btnViewFile.Size = new System.Drawing.Size(23, 22);
			this.btnViewFile.Text = "View File";
			this.btnViewFile.Click += new System.EventHandler(this.btnViewFile_Click);
			// 
			// btnCopy
			// 
			this.btnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnCopy.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.Copy;
			this.btnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Size = new System.Drawing.Size(23, 22);
			this.btnCopy.Text = "Copy";
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// btnTextDiff
			// 
			this.btnTextDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnTextDiff.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.ShowDifferences;
			this.btnTextDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnTextDiff.Name = "btnTextDiff";
			this.btnTextDiff.Size = new System.Drawing.Size(23, 22);
			this.btnTextDiff.Text = "Compare Text";
			this.btnTextDiff.Click += new System.EventHandler(this.mnuTextDiff_Click);
			// 
			// btnFind
			// 
			this.btnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFind.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.Find;
			this.btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFind.Name = "btnFind";
			this.btnFind.Size = new System.Drawing.Size(23, 22);
			this.btnFind.Text = "Find";
			this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
			// 
			// btnFindNext
			// 
			this.btnFindNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFindNext.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.FindNext;
			this.btnFindNext.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFindNext.Name = "btnFindNext";
			this.btnFindNext.Size = new System.Drawing.Size(23, 22);
			this.btnFindNext.Text = "Find Next";
			this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
			// 
			// btnFindPrevious
			// 
			this.btnFindPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFindPrevious.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.FindPrev;
			this.btnFindPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFindPrevious.Name = "btnFindPrevious";
			this.btnFindPrevious.Size = new System.Drawing.Size(23, 22);
			this.btnFindPrevious.Text = "Find Previous";
			this.btnFindPrevious.Click += new System.EventHandler(this.btnFindPrevious_Click);
			// 
			// btnFirstDiff
			// 
			this.btnFirstDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnFirstDiff.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.FirstDiff;
			this.btnFirstDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnFirstDiff.Name = "btnFirstDiff";
			this.btnFirstDiff.Size = new System.Drawing.Size(23, 22);
			this.btnFirstDiff.Text = "First Difference";
			this.btnFirstDiff.ToolTipText = "First Difference";
			this.btnFirstDiff.Click += new System.EventHandler(this.btnFirstDiff_Click);
			// 
			// btnPrevDiff
			// 
			this.btnPrevDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnPrevDiff.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.PrevDiff;
			this.btnPrevDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnPrevDiff.Name = "btnPrevDiff";
			this.btnPrevDiff.Size = new System.Drawing.Size(23, 22);
			this.btnPrevDiff.Text = "Previous Difference";
			this.btnPrevDiff.Click += new System.EventHandler(this.btnPrevDiff_Click);
			// 
			// btnNextDiff
			// 
			this.btnNextDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnNextDiff.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.NextDiff;
			this.btnNextDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnNextDiff.Name = "btnNextDiff";
			this.btnNextDiff.Size = new System.Drawing.Size(23, 22);
			this.btnNextDiff.Text = "Next Difference";
			this.btnNextDiff.Click += new System.EventHandler(this.btnNextDiff_Click);
			// 
			// btnLastDiff
			// 
			this.btnLastDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnLastDiff.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.LastDiff;
			this.btnLastDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnLastDiff.Name = "btnLastDiff";
			this.btnLastDiff.Size = new System.Drawing.Size(23, 22);
			this.btnLastDiff.Text = "Last Difference";
			this.btnLastDiff.Click += new System.EventHandler(this.btnLastDiff_Click);
			// 
			// btnGotoLine
			// 
			this.btnGotoLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnGotoLine.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.GotoLine;
			this.btnGotoLine.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnGotoLine.Name = "btnGotoLine";
			this.btnGotoLine.Size = new System.Drawing.Size(23, 22);
			this.btnGotoLine.Text = "Go To Line";
			this.btnGotoLine.Click += new System.EventHandler(this.btnGotoLine_Click);
			// 
			// btnRecompare
			// 
			this.btnRecompare.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnRecompare.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.Recompare;
			this.btnRecompare.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnRecompare.Name = "btnRecompare";
			this.btnRecompare.Size = new System.Drawing.Size(23, 22);
			this.btnRecompare.Text = "Recompare";
			this.btnRecompare.Click += new System.EventHandler(this.btnRecompare_Click);
			// 
			// lblDelete
			// 
			this.lblDelete.AutoSize = false;
			this.lblDelete.BackColor = System.Drawing.Color.Pink;
			this.lblDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lblDelete.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.Deleted;
			this.lblDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.lblDelete.Name = "lblDelete";
			this.lblDelete.Size = new System.Drawing.Size(22, 22);
			this.lblDelete.Text = "Deleted";
			this.lblDelete.ToolTipText = "Deleted";
			this.lblDelete.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLegend_Paint);
			// 
			// lblChange
			// 
			this.lblChange.AutoSize = false;
			this.lblChange.BackColor = System.Drawing.Color.PaleGreen;
			this.lblChange.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lblChange.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.Changed;
			this.lblChange.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.lblChange.Name = "lblChange";
			this.lblChange.Size = new System.Drawing.Size(22, 22);
			this.lblChange.Text = "Changed";
			this.lblChange.ToolTipText = "Changed";
			this.lblChange.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLegend_Paint);
			// 
			// lblInsert
			// 
			this.lblInsert.AutoSize = false;
			this.lblInsert.BackColor = System.Drawing.Color.PaleTurquoise;
			this.lblInsert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.lblInsert.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.Inserted;
			this.lblInsert.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.lblInsert.Name = "lblInsert";
			this.lblInsert.Size = new System.Drawing.Size(22, 22);
			this.lblInsert.Text = "Inserted";
			this.lblInsert.ToolTipText = "Inserted";
			this.lblInsert.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLegend_Paint);
			// 
			// DiffControl
			// 
			this.Controls.Add(this.pnlMiddle);
			this.Controls.Add(this.BottomSplitter);
			this.Controls.Add(this.pnlBottom);
			this.Controls.Add(this.ToolBar);
			this.Name = "DiffControl";
			this.Size = new System.Drawing.Size(484, 204);
			this.SizeChanged += new System.EventHandler(this.DiffControl_SizeChanged);
			this.CtxMenu.ResumeLayout(false);
			this.pnlMiddle.ResumeLayout(false);
			this.pnlRight.ResumeLayout(false);
			this.pnlRight.PerformLayout();
			this.pnlLeft.ResumeLayout(false);
			this.pnlLeft.PerformLayout();
			this.pnlBottom.ResumeLayout(false);
			this.ToolBar.ResumeLayout(false);
			this.ToolBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Protected Members

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				DiffOptions.OptionsChanged -= m_OptionsChangedHandler;
			}
			base.Dispose( disposing );
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
				Rectangle ImageRect = new Rectangle(R.X + (R.Width - I.Width)/2, R.Y + (R.Height - I.Height)/2, I.Width, I.Height);
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
			lblPosition.Text = String.Format("Ln {0}, Col {1}", Pos.Line+1, Pos.Column+1);
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
			btnViewFile.Enabled = CanViewFile;
			mnuViewFile.Enabled = btnViewFile.Enabled;

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

		private void btnViewFile_Click(object sender, System.EventArgs e)
		{
			ViewFile();
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
		private ToolStripButton btnViewFile;
		private ToolStripSeparator tsSep1;
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
		private ToolStripMenuItem mnuViewFile;
		private ToolStripSeparator tsSep7;
		private ToolStripMenuItem mnuCopy;
		private ToolStripMenuItem mnuTextDiff;
		private ToolStripButton btnRecompare;
		private ToolStripButton btnFirstDiff;
		private ToolStripButton btnLastDiff;
		private EventHandler m_OptionsChangedHandler;

		#endregion
	}
}
