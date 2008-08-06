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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.itemsLabel = new System.Windows.Forms.Label();
            this.messageLabel = new System.Windows.Forms.Label();
            this.toolTipController = new System.Windows.Forms.ToolTip(this.components);
            this.keepChangeListsCheckBox = new System.Windows.Forms.CheckBox();
            this.keepLocksCheckBox = new System.Windows.Forms.CheckBox();
            this.commitButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.commitItemsTree = new Ankh.UI.PathSelectionTreeView();
            this.logMessageBox = new Ankh.UI.PendingChanges.LogMessageEditor();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
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
            this.splitContainer1.Size = new System.Drawing.Size(586, 298);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 0;
            // 
            // itemsLabel
            // 
            this.itemsLabel.AutoSize = true;
            this.itemsLabel.Location = new System.Drawing.Point(0, 0);
            this.itemsLabel.Name = "itemsLabel";
            this.itemsLabel.Size = new System.Drawing.Size(31, 13);
            this.itemsLabel.TabIndex = 0;
            this.itemsLabel.Text = "&Files:";
            // 
            // messageLabel
            // 
            this.messageLabel.AutoSize = true;
            this.messageLabel.Location = new System.Drawing.Point(3, 0);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(74, 13);
            this.messageLabel.TabIndex = 0;
            this.messageLabel.Text = "Log &Message:";
            // 
            // keepChangeListsCheckBox
            // 
            this.keepChangeListsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.keepChangeListsCheckBox.AutoSize = true;
            this.keepChangeListsCheckBox.Location = new System.Drawing.Point(106, 329);
            this.keepChangeListsCheckBox.Name = "keepChangeListsCheckBox";
            this.keepChangeListsCheckBox.Size = new System.Drawing.Size(107, 17);
            this.keepChangeListsCheckBox.TabIndex = 2;
            this.keepChangeListsCheckBox.Text = "Keep changelists";
            this.toolTipController.SetToolTip(this.keepChangeListsCheckBox, "Do not remove committed files from changelists");
            this.keepChangeListsCheckBox.UseVisualStyleBackColor = true;
            // 
            // keepLocksCheckBox
            // 
            this.keepLocksCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.keepLocksCheckBox.Location = new System.Drawing.Point(12, 327);
            this.keepLocksCheckBox.Name = "keepLocksCheckBox";
            this.keepLocksCheckBox.Size = new System.Drawing.Size(85, 21);
            this.keepLocksCheckBox.TabIndex = 1;
            this.keepLocksCheckBox.Text = "Keep locks";
            this.toolTipController.SetToolTip(this.keepLocksCheckBox, "Do not unlock committed files");
            // 
            // commitButton
            // 
            this.commitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.commitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.commitButton.Location = new System.Drawing.Point(439, 325);
            this.commitButton.Name = "commitButton";
            this.commitButton.Size = new System.Drawing.Size(75, 23);
            this.commitButton.TabIndex = 4;
            this.commitButton.Text = "Commit";
            this.toolTipController.SetToolTip(this.commitButton, "Perform the commit.");
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(520, 325);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.toolTipController.SetToolTip(this.cancelButton, "Cancel the commit.");
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(232, 330);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Press Ctrl-Enter to commit";
            // 
            // commitItemsTree
            // 
            this.commitItemsTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commitItemsTree.CheckBoxes = true;
            this.commitItemsTree.Context = null;
            this.commitItemsTree.Location = new System.Drawing.Point(3, 16);
            this.commitItemsTree.Name = "commitItemsTree";
            this.commitItemsTree.Recursive = false;
            this.commitItemsTree.SingleCheck = false;
            this.commitItemsTree.Size = new System.Drawing.Size(233, 279);
            this.commitItemsTree.TabIndex = 0;
            this.toolTipController.SetToolTip(this.commitItemsTree, "Files you attempt to publish/commit.");
            // 
            // logMessageBox
            // 
            this.logMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logMessageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logMessageBox.Location = new System.Drawing.Point(6, 16);
            this.logMessageBox.Name = "logMessageBox";
            this.logMessageBox.Size = new System.Drawing.Size(334, 279);
            this.logMessageBox.TabIndex = 1;
            this.logMessageBox.Text = null;
            this.toolTipController.SetToolTip(this.logMessageBox, "Write changes you have performed since last revision or update");
            // 
            // CommitDialog
            // 
            this.AcceptButton = this.commitButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(610, 360);
            this.Controls.Add(this.keepChangeListsCheckBox);
            this.Controls.Add(this.keepLocksCheckBox);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.commitButton);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(530, 200);
            this.Name = "CommitDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Commit";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private Ankh.UI.PathSelectionTreeView commitItemsTree;
        private Ankh.UI.PendingChanges.LogMessageEditor logMessageBox;
        private SplitContainer splitContainer1;
        private ToolTip toolTipController;
        private System.ComponentModel.IContainer components;
        private CheckBox keepChangeListsCheckBox;
        private CheckBox keepLocksCheckBox;
        private Label label1;
        private Button commitButton;
        private Button cancelButton;
        private Label itemsLabel;
        private Label messageLabel;
    }
}
