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
            this.splitter = new System.Windows.Forms.Splitter();
            this.logMessagePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.keepLocksCheckBox = new System.Windows.Forms.CheckBox();
            this.logMessageBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.commitButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.commitItemsTree = new Ankh.UI.PathSelectionTreeView();
            this.logMessagePanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter
            // 
            this.splitter.BackColor = System.Drawing.SystemColors.Control;
            this.splitter.Location = new System.Drawing.Point(305, 3);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(3, 343);
            this.splitter.TabIndex = 3;
            this.splitter.TabStop = false;
            // 
            // logMessagePanel
            // 
            this.logMessagePanel.Controls.Add(this.label1);
            this.logMessagePanel.Controls.Add(this.keepLocksCheckBox);
            this.logMessagePanel.Controls.Add(this.logMessageBox);
            this.logMessagePanel.Controls.Add(this.cancelButton);
            this.logMessagePanel.Controls.Add(this.commitButton);
            this.logMessagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessagePanel.Location = new System.Drawing.Point(308, 3);
            this.logMessagePanel.Name = "logMessagePanel";
            this.logMessagePanel.Size = new System.Drawing.Size(386, 343);
            this.logMessagePanel.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 321);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Press Ctrl-Enter to commit";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // keepLocksCheckBox
            // 
            this.keepLocksCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.keepLocksCheckBox.Location = new System.Drawing.Point(6, 318);
            this.keepLocksCheckBox.Name = "keepLocksCheckBox";
            this.keepLocksCheckBox.Size = new System.Drawing.Size(85, 21);
            this.keepLocksCheckBox.TabIndex = 4;
            this.keepLocksCheckBox.Text = "Keep locks";
            // 
            // logMessageBox
            // 
            this.logMessageBox.AcceptsReturn = true;
            this.logMessageBox.AcceptsTab = true;
            this.logMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logMessageBox.Location = new System.Drawing.Point(0, 0);
            this.logMessageBox.Multiline = true;
            this.logMessageBox.Name = "logMessageBox";
            this.logMessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logMessageBox.Size = new System.Drawing.Size(386, 310);
            this.logMessageBox.TabIndex = 3;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(308, 316);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.RaiseProceed);
            // 
            // commitButton
            // 
            this.commitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.commitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.commitButton.Location = new System.Drawing.Point(227, 316);
            this.commitButton.Name = "commitButton";
            this.commitButton.Size = new System.Drawing.Size(75, 23);
            this.commitButton.TabIndex = 0;
            this.commitButton.Text = "Commit";
            this.commitButton.Click += new System.EventHandler(this.RaiseProceed);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.logMessagePanel);
            this.mainPanel.Controls.Add(this.splitter);
            this.mainPanel.Controls.Add(this.commitItemsTree);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(697, 349);
            this.mainPanel.TabIndex = 0;
            // 
            // commitItemsTree
            // 
            this.commitItemsTree.CheckBoxes = true;
            this.commitItemsTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.commitItemsTree.Items = new object[0];
            this.commitItemsTree.Location = new System.Drawing.Point(3, 3);
            this.commitItemsTree.Name = "commitItemsTree";
            this.commitItemsTree.Recursive = false;
            this.commitItemsTree.SingleCheck = false;
            this.commitItemsTree.Size = new System.Drawing.Size(302, 343);
            this.commitItemsTree.TabIndex = 2;
            this.commitItemsTree.UrlPaths = false;
            // 
            // CommitDialog
            // 
            this.Controls.Add(this.mainPanel);
            this.Name = "CommitDialog";
            this.Size = new System.Drawing.Size(697, 349);
            this.logMessagePanel.ResumeLayout(false);
            this.logMessagePanel.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private Ankh.UI.PathSelectionTreeView commitItemsTree;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox logMessageBox;
        private System.Windows.Forms.Splitter splitter;
        private System.Windows.Forms.Panel logMessagePanel;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button commitButton;
        private CommitDialogResult dialogResult = CommitDialogResult.Cancel;
        private System.Windows.Forms.CheckBox keepLocksCheckBox;
        private Label label1;
        private System.ComponentModel.Container components = null;
    }
}
