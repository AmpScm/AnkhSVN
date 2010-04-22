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

	DirDiffControl.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
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
    public partial class DirDiffControl : System.Windows.Forms.UserControl
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

        public event EventHandler<DifferenceEventArgs> ShowFileDifferences;

        public event EventHandler RecompareNeeded;

        #endregion

        

        #region Protected Members

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                DiffOptions.OptionsChanged -= m_OptionsChangedHandler;
            }
            base.Dispose(disposing);
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
                switch (e.KeyCode)
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
