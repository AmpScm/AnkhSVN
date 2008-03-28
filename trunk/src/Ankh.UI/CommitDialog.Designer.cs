using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    partial class CommitDialog
    {

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommitDialog));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.itemsLabel = new System.Windows.Forms.Label();
            this.commitItemsTree = new Ankh.UI.PathSelectionTreeView();
            this.messageLabel = new System.Windows.Forms.Label();
            this.logMessageBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.keepLocksCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.commitButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.keepChangeListsCheckBox = new System.Windows.Forms.CheckBox();
            this.toolTipController = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.itemsLabel);
            this.splitContainer1.Panel1.Controls.Add(this.commitItemsTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.messageLabel);
            this.splitContainer1.Panel2.Controls.Add(this.logMessageBox);
            this.splitContainer1.Size = new System.Drawing.Size(546, 313);
            this.splitContainer1.SplitterDistance = 224;
            this.splitContainer1.TabIndex = 6;
            // 
            // itemsLabel
            // 
            this.itemsLabel.AutoSize = true;
            this.itemsLabel.Location = new System.Drawing.Point(0, 9);
            this.itemsLabel.Name = "itemsLabel";
            this.itemsLabel.Size = new System.Drawing.Size(32, 13);
            this.itemsLabel.TabIndex = 3;
            this.itemsLabel.Text = "Items";
            // 
            // commitItemsTree
            // 
            this.commitItemsTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commitItemsTree.CheckBoxes = true;
            this.commitItemsTree.CheckedItems = ((System.Collections.IList)(resources.GetObject("commitItemsTree.CheckedItems")));
            this.commitItemsTree.Items = new object[0];
            this.commitItemsTree.Location = new System.Drawing.Point(3, 25);
            this.commitItemsTree.Name = "commitItemsTree";
            this.commitItemsTree.Recursive = false;
            this.commitItemsTree.SingleCheck = false;
            this.commitItemsTree.Size = new System.Drawing.Size(218, 241);
            this.commitItemsTree.TabIndex = 2;
            this.toolTipController.SetToolTip(this.commitItemsTree, "Files you attempt to publish/commit.");
            this.commitItemsTree.UrlPaths = false;
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Location = new System.Drawing.Point(3, 9);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(70, 13);
            this.messageLabel.TabIndex = 4;
            this.messageLabel.Text = "Log message";
            // 
            // logMessageBox
            // 
            this.logMessageBox.AcceptsReturn = true;
            this.logMessageBox.AcceptsTab = true;
            this.logMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logMessageBox.Location = new System.Drawing.Point(6, 25);
            this.logMessageBox.Multiline = true;
            this.logMessageBox.Name = "logMessageBox";
            this.logMessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logMessageBox.Size = new System.Drawing.Size(309, 241);
            this.logMessageBox.TabIndex = 3;
            this.toolTipController.SetToolTip(this.logMessageBox, "Write changes you have performed since last revision or update");
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(245, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Press Ctrl-Enter to commit";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // keepLocksCheckBox
            // 
            this.keepLocksCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.keepLocksCheckBox.Location = new System.Drawing.Point(12, 8);
            this.keepLocksCheckBox.Name = "keepLocksCheckBox";
            this.keepLocksCheckBox.Size = new System.Drawing.Size(85, 21);
            this.keepLocksCheckBox.TabIndex = 4;
            this.keepLocksCheckBox.Text = "Keep locks";
            this.toolTipController.SetToolTip(this.keepLocksCheckBox, "Do not unlock committed files");
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(459, 6);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.toolTipController.SetToolTip(this.cancelButton, "Cancel the commit.");
            this.cancelButton.Click += new System.EventHandler(this.RaiseProceed);
            // 
            // commitButton
            // 
            this.commitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.commitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.commitButton.Location = new System.Drawing.Point(378, 6);
            this.commitButton.Name = "commitButton";
            this.commitButton.Size = new System.Drawing.Size(75, 23);
            this.commitButton.TabIndex = 0;
            this.commitButton.Text = "Commit";
            this.toolTipController.SetToolTip(this.commitButton, "Perform the commit.");
            this.commitButton.Click += new System.EventHandler(this.RaiseProceed);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.panel1);
            this.mainPanel.Controls.Add(this.splitContainer1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(546, 313);
            this.mainPanel.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.keepChangeListsCheckBox);
            this.panel1.Controls.Add(this.keepLocksCheckBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.commitButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 272);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(546, 41);
            this.panel1.TabIndex = 7;
            // 
            // keepChangeListsCheckBox
            // 
            this.keepChangeListsCheckBox.AutoSize = true;
            this.keepChangeListsCheckBox.Location = new System.Drawing.Point(103, 10);
            this.keepChangeListsCheckBox.Name = "keepChangeListsCheckBox";
            this.keepChangeListsCheckBox.Size = new System.Drawing.Size(107, 17);
            this.keepChangeListsCheckBox.TabIndex = 6;
            this.keepChangeListsCheckBox.Text = "Keep changelists";
            this.toolTipController.SetToolTip(this.keepChangeListsCheckBox, "Do not remove committed files from changelists");
            this.keepChangeListsCheckBox.UseVisualStyleBackColor = true;
            // 
            // CommitDialog
            // 
            this.AcceptButton = this.commitButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(546, 313);
            this.Controls.Add(this.mainPanel);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(530, 200);
            this.Name = "CommitDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Commit";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.mainPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private Ankh.UI.PathSelectionTreeView commitItemsTree;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox logMessageBox;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button commitButton;
        private CommitDialogResult dialogResult = CommitDialogResult.Cancel;
        private System.Windows.Forms.CheckBox keepLocksCheckBox;
        private Label label1;
        private SplitContainer splitContainer1;
        private Panel panel1;
        private Label itemsLabel;
        private Label messageLabel;
        private CheckBox keepChangeListsCheckBox;
        private ToolTip toolTipController;
        private System.ComponentModel.IContainer components;
    }
}
