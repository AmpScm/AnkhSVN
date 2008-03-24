﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    partial class ReverseMergeDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.revisionPicker = new Ankh.UI.RevisionPicker();
            this.treeView = new Ankh.UI.PathSelectionTreeView();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.recursiveCheckbox = new System.Windows.Forms.CheckBox();
            this.dryRunCheckBox = new System.Windows.Forms.CheckBox();
            this.revisionGroupBox = new System.Windows.Forms.GroupBox();
            this.pathsLabel = new System.Windows.Forms.Label();
            this.revisionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // revisionPicker
            // 
            this.revisionPicker.Location = new System.Drawing.Point(9, 24);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(304, 20);
            this.revisionPicker.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.CheckBoxes = true;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeView.Items = new object[0];
            this.treeView.Location = new System.Drawing.Point(0, 19);
            this.treeView.Name = "treeView";
            this.treeView.Recursive = false;
            this.treeView.SingleCheck = false;
            this.treeView.Size = new System.Drawing.Size(338, 220);
            this.treeView.TabIndex = 1;
            this.treeView.UrlPaths = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(168, 346);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "&Merge";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(256, 346);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "&Cancel";
            // 
            // recursiveCheckbox
            // 
            this.recursiveCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.recursiveCheckbox.Location = new System.Drawing.Point(8, 322);
            this.recursiveCheckbox.Name = "recursiveCheckbox";
            this.recursiveCheckbox.Size = new System.Drawing.Size(104, 24);
            this.recursiveCheckbox.TabIndex = 3;
            this.recursiveCheckbox.Text = "Recursi&ve";
            this.recursiveCheckbox.CheckedChanged += new System.EventHandler(this.recursiveCheckbox_CheckedChanged);
            // 
            // dryRunCheckBox
            // 
            this.dryRunCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dryRunCheckBox.Location = new System.Drawing.Point(8, 346);
            this.dryRunCheckBox.Name = "dryRunCheckBox";
            this.dryRunCheckBox.Size = new System.Drawing.Size(104, 24);
            this.dryRunCheckBox.TabIndex = 4;
            this.dryRunCheckBox.Text = "&Dry run";
            // 
            // revisionGroupBox
            // 
            this.revisionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionGroupBox.Controls.Add(this.revisionPicker);
            this.revisionGroupBox.Location = new System.Drawing.Point(8, 252);
            this.revisionGroupBox.Name = "revisionGroupBox";
            this.revisionGroupBox.Size = new System.Drawing.Size(319, 55);
            this.revisionGroupBox.TabIndex = 2;
            this.revisionGroupBox.TabStop = false;
            this.revisionGroupBox.Text = "&Revision";
            // 
            // pathsLabel
            // 
            this.pathsLabel.AutoSize = true;
            this.pathsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pathsLabel.Location = new System.Drawing.Point(0, 0);
            this.pathsLabel.Name = "pathsLabel";
            this.pathsLabel.Size = new System.Drawing.Size(34, 19);
            this.pathsLabel.TabIndex = 0;
            this.pathsLabel.Text = "&Paths";
            // 
            // ReverseMergeDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(338, 383);
            this.ControlBox = false;
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.pathsLabel);
            this.Controls.Add(this.revisionGroupBox);
            this.Controls.Add(this.dryRunCheckBox);
            this.Controls.Add(this.recursiveCheckbox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ReverseMergeDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Reverse merge";
            this.revisionGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private Ankh.UI.RevisionPicker revisionPicker;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Ankh.UI.PathSelectionTreeView treeView;
        private System.Windows.Forms.CheckBox recursiveCheckbox;
        private System.Windows.Forms.CheckBox dryRunCheckBox;
        private GroupBox revisionGroupBox;
        private Label pathsLabel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
    }
}
