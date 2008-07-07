#region Copyright And Revision History

/*---------------------------------------------------------------------------

	DirDiffControl.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	11.16.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using Ankh.Diff.DiffUtils;

#endregion

namespace Ankh.Diff.DiffUtils.Controls
{
	/// <summary>
	/// Summary description for DirDiffControl.
	/// </summary>
	public class DirDiffControl : System.Windows.Forms.UserControl
	{
		#region Public Members

		public DirDiffControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			m_ActiveTree = TreeA;

			TreeA.ImageList = Images;
			TreeB.ImageList = Images;

			UpdateButtons();
			UpdateColors();

			m_OptionsChangedHandler = new EventHandler(DiffOptionsChanged);
			DiffOptions.OptionsChanged += m_OptionsChangedHandler;
		}

		public void SetData(DirectoryDiffResults Results)
		{
			edtA.Text = Results.A.FullName;
			edtB.Text = Results.B.FullName;

			TreeA.SetData(Results, true);
			TreeB.SetData(Results, false);

			//Set a filter description
			if (Results.Filter == null)
			{
				lblFilter.Text = "All Files";
			}
			else
			{
				DirectoryDiffFileFilter Filter = Results.Filter;
				lblFilter.Text = String.Format("{0}: {1}", Filter.Include ? "Includes" : "Excludes", Filter.FilterString);
			}

			UpdateButtons();

			if (TreeA.Nodes.Count > 0)
			{
				TreeA.SelectedNode = TreeA.Nodes[0];
			}
		}

		[DefaultValue(true)]
		public bool FullRowSelect
		{
			get
			{
				return !TreeA.ShowLines;
			}
			set
			{
				//The TreeViews will always have FullRowSelect set to
				//true, but it doesn't affect anything unless ShowLines
				//is false.  So we'll just toggle the ShowLines property.
				if (FullRowSelect != value)
				{
					TreeA.ShowLines = !value;
					TreeB.ShowLines = !value;
				}
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
					tsSep2.Visible = value;
				}
			}
		}

		[Browsable(false)]
		public bool CanView
		{
			get
			{
				return m_ActiveTree.CanView;
			}
		}

		[Browsable(false)]
		public bool CanShowDifferences
		{
			get
			{
				DirectoryDiffEntry Entry = SelectedEntry;
				return (Entry != null && Entry.IsFile && Entry.InA && Entry.InB && ShowFileDifferences != null);
			}
		}

		public void View()
		{
			if (!CanView) return;
			m_ActiveTree.View();
		}

		public void ShowDifferences()
		{
			if (!CanShowDifferences) return;

			DirectoryDiffEntry Entry = SelectedEntry;

			string strFileA = TreeA.GetFullNameForNode(Entry.NodeA);
			string strFileB = TreeB.GetFullNameForNode(Entry.NodeB);

			ShowFileDifferences(this, new DifferenceEventArgs(strFileA, strFileB));
		}

		public bool CanRecompare
		{
			get
			{
				return RecompareNeeded != null;
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

		public event DifferenceEventHandler ShowFileDifferences;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DirDiffControl));
			this.Images = new System.Windows.Forms.ImageList(this.components);
			this.pnlBottom = new System.Windows.Forms.Panel();
			this.pnlRight = new System.Windows.Forms.Panel();
            this.TreeB = new Ankh.Diff.DiffUtils.Controls.DirDiffTreeView();
			this.CtxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
			this.mnuShowDifferences = new System.Windows.Forms.ToolStripMenuItem();
			this.edtB = new System.Windows.Forms.TextBox();
			this.Splitter = new System.Windows.Forms.Splitter();
			this.pnlLeft = new System.Windows.Forms.Panel();
            this.TreeA = new Ankh.Diff.DiffUtils.Controls.DirDiffTreeView();
			this.edtA = new System.Windows.Forms.TextBox();
			this.ToolBar = new System.Windows.Forms.ToolStrip();
			this.btnView = new System.Windows.Forms.ToolStripButton();
			this.btnShowDifferences = new System.Windows.Forms.ToolStripButton();
			this.tsSep1 = new System.Windows.Forms.ToolStripSeparator();
			this.lblDelete = new System.Windows.Forms.ToolStripLabel();
			this.lblChange = new System.Windows.Forms.ToolStripLabel();
			this.lblInsert = new System.Windows.Forms.ToolStripLabel();
			this.tsSep2 = new System.Windows.Forms.ToolStripSeparator();
			this.lblFilter = new System.Windows.Forms.ToolStripLabel();
			this.btnRecompare = new System.Windows.Forms.ToolStripButton();
			this.tsSep3 = new System.Windows.Forms.ToolStripSeparator();
			this.pnlBottom.SuspendLayout();
			this.pnlRight.SuspendLayout();
			this.CtxMenu.SuspendLayout();
			this.pnlLeft.SuspendLayout();
			this.ToolBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// Images
			// 
			this.Images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("Images.ImageStream")));
			this.Images.TransparentColor = System.Drawing.Color.Magenta;
			this.Images.Images.SetKeyName(0, "FolderClosed.bmp");
			this.Images.Images.SetKeyName(1, "FolderOpen.bmp");
			this.Images.Images.SetKeyName(2, "MissingFolder.bmp");
			this.Images.Images.SetKeyName(3, "File.bmp");
			this.Images.Images.SetKeyName(4, "MissingFile.bmp");
			this.Images.Images.SetKeyName(5, "FileError.bmp");
			// 
			// pnlBottom
			// 
			this.pnlBottom.Controls.Add(this.pnlRight);
			this.pnlBottom.Controls.Add(this.Splitter);
			this.pnlBottom.Controls.Add(this.pnlLeft);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlBottom.Location = new System.Drawing.Point(0, 25);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(394, 135);
			this.pnlBottom.TabIndex = 1;
			// 
			// pnlRight
			// 
			this.pnlRight.Controls.Add(this.TreeB);
			this.pnlRight.Controls.Add(this.edtB);
			this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlRight.Location = new System.Drawing.Point(199, 0);
			this.pnlRight.Name = "pnlRight";
			this.pnlRight.Size = new System.Drawing.Size(195, 135);
			this.pnlRight.TabIndex = 2;
			// 
			// TreeB
			// 
			this.TreeB.ContextMenuStrip = this.CtxMenu;
			this.TreeB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TreeB.FullRowSelect = true;
			this.TreeB.HideSelection = false;
			this.TreeB.Location = new System.Drawing.Point(0, 20);
			this.TreeB.Name = "TreeB";
			this.TreeB.ShowLines = false;
			this.TreeB.Size = new System.Drawing.Size(195, 115);
			this.TreeB.TabIndex = 0;
			this.TreeB.MouseWheelMsg += new Ankh.Diff.DiffUtils.Controls.Win32MessageEventHandler(this.TreeB_MouseWheelMsg);
			this.TreeB.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.TreeNode_StateChange);
			this.TreeB.Enter += new System.EventHandler(this.TreeView_Enter);
			this.TreeB.DoubleClick += new System.EventHandler(this.TreeView_DoubleClick);
			this.TreeB.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeNode_SelectChanged);
			this.TreeB.VScroll += new Ankh.Diff.DiffUtils.Controls.Win32MessageEventHandler(this.TreeB_VScroll);
			this.TreeB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeB_KeyDown);
			this.TreeB.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.TreeNode_StateChange);
			// 
			// CtxMenu
			// 
			this.CtxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuView,
            this.mnuShowDifferences});
			this.CtxMenu.Name = "CtxMenu";
			this.CtxMenu.Size = new System.Drawing.Size(159, 48);
			// 
			// mnuView
			// 
			this.mnuView.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.View;
			this.mnuView.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuView.Name = "mnuView";
			this.mnuView.Size = new System.Drawing.Size(158, 22);
			this.mnuView.Text = "&View";
			this.mnuView.Click += new System.EventHandler(this.btnView_Click);
			// 
			// mnuShowDifferences
			// 
			this.mnuShowDifferences.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.ShowDifferences;
			this.mnuShowDifferences.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			this.mnuShowDifferences.Name = "mnuShowDifferences";
			this.mnuShowDifferences.Size = new System.Drawing.Size(158, 22);
			this.mnuShowDifferences.Text = "&Show Differences";
			this.mnuShowDifferences.Click += new System.EventHandler(this.btnShowDifferences_Click);
			// 
			// edtB
			// 
			this.edtB.Dock = System.Windows.Forms.DockStyle.Top;
			this.edtB.Location = new System.Drawing.Point(0, 0);
			this.edtB.Name = "edtB";
			this.edtB.ReadOnly = true;
			this.edtB.Size = new System.Drawing.Size(195, 20);
			this.edtB.TabIndex = 1;
			// 
			// Splitter
			// 
			this.Splitter.Location = new System.Drawing.Point(196, 0);
			this.Splitter.Name = "Splitter";
			this.Splitter.Size = new System.Drawing.Size(3, 135);
			this.Splitter.TabIndex = 1;
			this.Splitter.TabStop = false;
			// 
			// pnlLeft
			// 
			this.pnlLeft.Controls.Add(this.TreeA);
			this.pnlLeft.Controls.Add(this.edtA);
			this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.pnlLeft.Location = new System.Drawing.Point(0, 0);
			this.pnlLeft.Name = "pnlLeft";
			this.pnlLeft.Size = new System.Drawing.Size(196, 135);
			this.pnlLeft.TabIndex = 0;
			// 
			// TreeA
			// 
			this.TreeA.ContextMenuStrip = this.CtxMenu;
			this.TreeA.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TreeA.FullRowSelect = true;
			this.TreeA.HideSelection = false;
			this.TreeA.Location = new System.Drawing.Point(0, 20);
			this.TreeA.Name = "TreeA";
			this.TreeA.ShowLines = false;
			this.TreeA.Size = new System.Drawing.Size(196, 115);
			this.TreeA.TabIndex = 0;
			this.TreeA.MouseWheelMsg += new Ankh.Diff.DiffUtils.Controls.Win32MessageEventHandler(this.TreeA_MouseWheelMsg);
			this.TreeA.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.TreeNode_StateChange);
			this.TreeA.Enter += new System.EventHandler(this.TreeView_Enter);
			this.TreeA.DoubleClick += new System.EventHandler(this.TreeView_DoubleClick);
			this.TreeA.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeNode_SelectChanged);
			this.TreeA.VScroll += new Ankh.Diff.DiffUtils.Controls.Win32MessageEventHandler(this.TreeA_VScroll);
			this.TreeA.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeA_KeyDown);
			this.TreeA.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.TreeNode_StateChange);
			// 
			// edtA
			// 
			this.edtA.Dock = System.Windows.Forms.DockStyle.Top;
			this.edtA.Location = new System.Drawing.Point(0, 0);
			this.edtA.Name = "edtA";
			this.edtA.ReadOnly = true;
			this.edtA.Size = new System.Drawing.Size(196, 20);
			this.edtA.TabIndex = 1;
			// 
			// ToolBar
			// 
			this.ToolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnView,
            this.btnShowDifferences,
            this.tsSep3,
            this.btnRecompare,
            this.tsSep1,
            this.lblDelete,
            this.lblChange,
            this.lblInsert,
            this.tsSep2,
            this.lblFilter});
			this.ToolBar.Location = new System.Drawing.Point(0, 0);
			this.ToolBar.Name = "ToolBar";
			this.ToolBar.Size = new System.Drawing.Size(394, 25);
			this.ToolBar.TabIndex = 2;
			this.ToolBar.Text = "toolStrip1";
			// 
			// btnView
			// 
			this.btnView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnView.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.View;
			this.btnView.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnView.Name = "btnView";
			this.btnView.Size = new System.Drawing.Size(23, 22);
			this.btnView.Text = "View";
			this.btnView.Click += new System.EventHandler(this.btnView_Click);
			// 
			// btnShowDifferences
			// 
			this.btnShowDifferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnShowDifferences.Image = global::Ankh.Diff.DiffUtils.Properties.Resources.ShowDifferences;
			this.btnShowDifferences.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnShowDifferences.Name = "btnShowDifferences";
			this.btnShowDifferences.Size = new System.Drawing.Size(23, 22);
			this.btnShowDifferences.Text = "Show Differences";
			this.btnShowDifferences.Click += new System.EventHandler(this.btnShowDifferences_Click);
			// 
			// tsSep1
			// 
			this.tsSep1.Name = "tsSep1";
			this.tsSep1.Size = new System.Drawing.Size(6, 25);
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
			this.lblInsert.ToolTipText = "Inserted";
			this.lblInsert.Paint += new System.Windows.Forms.PaintEventHandler(this.ColorLegend_Paint);
			// 
			// tsSep2
			// 
			this.tsSep2.Name = "tsSep2";
			this.tsSep2.Size = new System.Drawing.Size(6, 25);
			// 
			// lblFilter
			// 
			this.lblFilter.Name = "lblFilter";
			this.lblFilter.Size = new System.Drawing.Size(42, 22);
			this.lblFilter.Text = "All Files";
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
			// tsSep3
			// 
			this.tsSep3.Name = "tsSep3";
			this.tsSep3.Size = new System.Drawing.Size(6, 25);
			// 
			// DirDiffControl
			// 
			this.Controls.Add(this.pnlBottom);
			this.Controls.Add(this.ToolBar);
			this.Name = "DirDiffControl";
			this.Size = new System.Drawing.Size(394, 160);
			this.SizeChanged += new System.EventHandler(this.DirDiffControl_SizeChanged);
			this.pnlBottom.ResumeLayout(false);
			this.pnlRight.ResumeLayout(false);
			this.pnlRight.PerformLayout();
			this.CtxMenu.ResumeLayout(false);
			this.pnlLeft.ResumeLayout(false);
			this.pnlLeft.PerformLayout();
			this.ToolBar.ResumeLayout(false);
			this.ToolBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Protected Members

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}

				DiffOptions.OptionsChanged -= m_OptionsChangedHandler;
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Private Methods

		private DirectoryDiffEntry SelectedEntry
		{
			get
			{
				if (TreeB.Focused)
				{
					return TreeB.GetEntryForNode(TreeB.SelectedNode);
				}
				else
				{
					return TreeA.GetEntryForNode(TreeA.SelectedNode);
				}
			}
		}

		private void DirDiffControl_SizeChanged(object sender, System.EventArgs e)
		{
			pnlLeft.Width = (Width - Splitter.Width) / 2;
		}

		private void UpdateButtons()
		{
			btnView.Enabled = CanView;
			mnuView.Enabled = btnView.Enabled;
			btnShowDifferences.Enabled = CanShowDifferences;
			mnuShowDifferences.Enabled = btnShowDifferences.Enabled;
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

		private void TreeNode_StateChange(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			DirDiffTreeView Tree = (DirDiffTreeView)sender;
			DirectoryDiffEntry Entry = Tree.GetEntryForNode(e.Node);
			if (Entry != null)
			{
				TreeNode NodeA = Entry.NodeA;
				TreeNode NodeB = Entry.NodeB;
				if (NodeA != null && NodeB != null)
				{
					if (e.Action == TreeViewAction.Collapse)
					{
						//Although the Docs say that Collapse() isn't recursive,
						//it actually is.  Since clicking the +/- isn't recursive
						//we have to simulate that by calling Collapse even on the
						//node that fired the event.
						if (NodeA.IsExpanded || e.Node == NodeA) NodeA.Collapse();
						if (NodeB.IsExpanded || e.Node == NodeB) NodeB.Collapse();
					}
					else if (e.Action == TreeViewAction.Expand)
					{
						if (!NodeA.IsExpanded || e.Node == NodeA) NodeA.Expand();
						if (!NodeB.IsExpanded || e.Node == NodeB) NodeB.Expand();
					}
				}
			}
		}

		private void TreeNode_SelectChanged(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			DirDiffTreeView Tree = (DirDiffTreeView)sender;
			DirectoryDiffEntry Entry = Tree.GetEntryForNode(e.Node);
			if (Entry != null)
			{
				TreeNode NodeA = Entry.NodeA;
				TreeNode NodeB = Entry.NodeB;
				if (NodeA != null && NodeB != null)
				{
					TreeA.SelectedNode = NodeA;
					TreeB.SelectedNode = NodeB;
				}
			}
		
			UpdateButtons();
		}

        private void TreeA_VScroll(object sender, Ankh.Diff.DiffUtils.Controls.Win32MessageEventArgs e)
		{
			TreeB.SendVScroll(e.WParam);
		}

        private void TreeB_VScroll(object sender, Ankh.Diff.DiffUtils.Controls.Win32MessageEventArgs e)
		{
			TreeA.SendVScroll(e.WParam);
		}

		private void TreeA_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//We only need to send scrolling key strokes (e.g. Ctrl+Home)
			//to the other tree.  If we send all keystrokes then weird
			//things happen.  For example, we already have the AfterSelect
			//events tied together, so when a user changes the selection
			//with a keystroke, we need AfterSelect to update the other
			//tree.  If we sent all keystrokes we'd end up with a double
			//move of the selection.
			if (ScrollingKey(e))
			{
				TreeB.SendKeyDown(e.KeyCode, e.KeyData);
			}
		}

		private void TreeB_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//See note in TreeA_KeyDown.
			if (ScrollingKey(e))
			{
				TreeA.SendKeyDown(e.KeyCode, e.KeyData);
			}
		}

		private bool ScrollingKey(KeyEventArgs e)
		{
			if (e.Modifiers == Keys.Control)
			{
				switch(e.KeyCode)
				{
					case Keys.Home:
					case Keys.End:
					case Keys.PageUp:
					case Keys.PageDown:
					case Keys.Up:
					case Keys.Down:
						return true;
				}
			}

			return false;
		}

		private void btnView_Click(object sender, System.EventArgs e)
		{
			View();
		}

		private void btnShowDifferences_Click(object sender, System.EventArgs e)
		{
			ShowDifferences();
		}

		private void TreeView_Enter(object sender, System.EventArgs e)
		{
			m_ActiveTree = (DirDiffTreeView)sender;
			UpdateButtons();
		}

		private void TreeView_DoubleClick(object sender, System.EventArgs e)
		{
			ShowDifferences();
		}

        private void TreeA_MouseWheelMsg(object sender, Ankh.Diff.DiffUtils.Controls.Win32MessageEventArgs e)
		{
			TreeB.SendMouseWheel(e.WParam, e.LParam);
		}

        private void TreeB_MouseWheelMsg(object sender, Ankh.Diff.DiffUtils.Controls.Win32MessageEventArgs e)
		{
			TreeA.SendMouseWheel(e.WParam, e.LParam);
		}

		private void ColorLegend_Paint(object sender, PaintEventArgs e)
		{
			DiffControl.PaintColorLegendItem(sender as ToolStripItem, e);
		}

		private void btnRecompare_Click(object sender, EventArgs e)
		{
			Recompare();
		}

		#endregion

		#region Private Data Members

		private System.Windows.Forms.Panel pnlBottom;
		private System.Windows.Forms.Panel pnlLeft;
		private System.Windows.Forms.Panel pnlRight;
		private System.Windows.Forms.TextBox edtA;
		private System.Windows.Forms.TextBox edtB;
		private System.Windows.Forms.ImageList Images;
        private Ankh.Diff.DiffUtils.Controls.DirDiffTreeView TreeB;
		private System.Windows.Forms.Splitter Splitter;
        private Ankh.Diff.DiffUtils.Controls.DirDiffTreeView TreeA;
		private System.ComponentModel.IContainer components;
		private bool m_bShowToolbar = true;
		private bool m_bShowColorLegend = true;
		private DirDiffTreeView m_ActiveTree;
		private ToolStrip ToolBar;
		private ToolStripButton btnView;
		private ToolStripButton btnShowDifferences;
		private ToolStripSeparator tsSep1;
		private ToolStripLabel lblDelete;
		private ToolStripLabel lblChange;
		private ToolStripLabel lblInsert;
		private ToolStripSeparator tsSep2;
		private ToolStripLabel lblFilter;
		private ContextMenuStrip CtxMenu;
		private ToolStripMenuItem mnuView;
		private ToolStripMenuItem mnuShowDifferences;
		private ToolStripSeparator tsSep3;
		private ToolStripButton btnRecompare;
		private EventHandler m_OptionsChangedHandler;

		#endregion
	}
}
