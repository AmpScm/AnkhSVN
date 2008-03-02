using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class ViewLogDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.showRevisionCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.singleRevisionCheckBox = new System.Windows.Forms.CheckBox();
            this.logRichTextBox = new System.Windows.Forms.RichTextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.getLogButton = new System.Windows.Forms.Button();
            this.showDateCheckBox = new System.Windows.Forms.CheckBox();
            this.showAuthorCheckBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.showMessageCheckBox = new System.Windows.Forms.CheckBox();
            this.showModifiedFilesCheckBox = new System.Windows.Forms.CheckBox();
            this.selectRevisionLabel = new System.Windows.Forms.Label();
            this.fromRevision = new Ankh.UI.RevisionPicker();
            this.toRevision = new Ankh.UI.RevisionPicker();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(88, -16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Revision";
            // 
            // showRevisionCheckBox
            // 
            this.showRevisionCheckBox.Checked = true;
            this.showRevisionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showRevisionCheckBox.Location = new System.Drawing.Point(0, 122);
            this.showRevisionCheckBox.Name = "showRevisionCheckBox";
            this.showRevisionCheckBox.Size = new System.Drawing.Size(88, 24);
            this.showRevisionCheckBox.TabIndex = 4;
            this.showRevisionCheckBox.Text = "Revision nr.";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "From";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 23);
            this.label3.TabIndex = 8;
            this.label3.Text = "To";
            // 
            // singleRevisionCheckBox
            // 
            this.singleRevisionCheckBox.Checked = true;
            this.singleRevisionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.singleRevisionCheckBox.Location = new System.Drawing.Point(424, 64);
            this.singleRevisionCheckBox.Name = "singleRevisionCheckBox";
            this.singleRevisionCheckBox.Size = new System.Drawing.Size(112, 24);
            this.singleRevisionCheckBox.TabIndex = 2;
            this.singleRevisionCheckBox.Text = "Disable";
            this.singleRevisionCheckBox.CheckedChanged += new System.EventHandler(this.singleRevisionCheckBoxChecked);
            // 
            // logRichTextBox
            // 
            this.logRichTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.logRichTextBox.Enabled = false;
            this.logRichTextBox.Location = new System.Drawing.Point(0, 160);
            this.logRichTextBox.Name = "logRichTextBox";
            this.logRichTextBox.ReadOnly = true;
            this.logRichTextBox.Size = new System.Drawing.Size(592, 296);
            this.logRichTextBox.TabIndex = 11;
            this.logRichTextBox.Text = "";
            // 
            // closeButton
            // 
            this.closeButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Location = new System.Drawing.Point(488, 472);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(94, 23);
            this.closeButton.TabIndex = 10;
            this.closeButton.Text = "Close";
            // 
            // getLogButton
            // 
            this.getLogButton.Location = new System.Drawing.Point(472, 122);
            this.getLogButton.Name = "getLogButton";
            this.getLogButton.Size = new System.Drawing.Size(99, 23);
            this.getLogButton.TabIndex = 9;
            this.getLogButton.Text = "Get log";
            this.getLogButton.Click += new System.EventHandler(this.getLogButton_Click);
            // 
            // showDateCheckBox
            // 
            this.showDateCheckBox.Checked = true;
            this.showDateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showDateCheckBox.Location = new System.Drawing.Point(96, 122);
            this.showDateCheckBox.Name = "showDateCheckBox";
            this.showDateCheckBox.Size = new System.Drawing.Size(48, 24);
            this.showDateCheckBox.TabIndex = 5;
            this.showDateCheckBox.Text = "Date";
            // 
            // showAuthorCheckBox
            // 
            this.showAuthorCheckBox.Checked = true;
            this.showAuthorCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showAuthorCheckBox.Location = new System.Drawing.Point(152, 122);
            this.showAuthorCheckBox.Name = "showAuthorCheckBox";
            this.showAuthorCheckBox.Size = new System.Drawing.Size(64, 24);
            this.showAuthorCheckBox.TabIndex = 6;
            this.showAuthorCheckBox.Text = "Author";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "Show:";
            // 
            // showMessageCheckBox
            // 
            this.showMessageCheckBox.Checked = true;
            this.showMessageCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showMessageCheckBox.Location = new System.Drawing.Point(224, 122);
            this.showMessageCheckBox.Name = "showMessageCheckBox";
            this.showMessageCheckBox.TabIndex = 7;
            this.showMessageCheckBox.Text = "Message";
            // 
            // showModifiedFilesCheckBox
            // 
            this.showModifiedFilesCheckBox.Location = new System.Drawing.Point(328, 122);
            this.showModifiedFilesCheckBox.Name = "showModifiedFilesCheckBox";
            this.showModifiedFilesCheckBox.TabIndex = 8;
            this.showModifiedFilesCheckBox.Text = "Modified files";
            // 
            // selectRevisionLabel
            // 
            this.selectRevisionLabel.Name = "selectRevisionLabel";
            this.selectRevisionLabel.TabIndex = 22;
            this.selectRevisionLabel.Text = "Select:";
            // 
            // fromRevision
            // 
            this.fromRevision.Location = new System.Drawing.Point(64, 32);
            this.fromRevision.Name = "fromRevision";
            this.fromRevision.Size = new System.Drawing.Size(344, 24);
            this.fromRevision.TabIndex = 23;
            this.fromRevision.Changed += new System.EventHandler(this.fromRevision_Changed);
            // 
            // toRevision
            // 
            this.toRevision.Enabled = false;
            this.toRevision.Location = new System.Drawing.Point(64, 64);
            this.toRevision.Name = "toRevision";
            this.toRevision.Size = new System.Drawing.Size(344, 24);
            this.toRevision.TabIndex = 24;
            // 
            // ViewLogDialog
            // 
            this.AcceptButton = this.getLogButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(592, 501);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.toRevision,
                                                                          this.fromRevision,
                                                                          this.selectRevisionLabel,
                                                                          this.showModifiedFilesCheckBox,
                                                                          this.showMessageCheckBox,
                                                                          this.label4,
                                                                          this.showAuthorCheckBox,
                                                                          this.showDateCheckBox,
                                                                          this.getLogButton,
                                                                          this.closeButton,
                                                                          this.logRichTextBox,
                                                                          this.singleRevisionCheckBox,
                                                                          this.label3,
                                                                          this.label2,
                                                                          this.showRevisionCheckBox,
                                                                          this.label1});
            this.Name = "ViewLogDialog";
            this.Text = "View Log";
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button getLogButton;
        private System.Windows.Forms.CheckBox showRevisionCheckBox;
        private System.Windows.Forms.RichTextBox logRichTextBox;
        private System.Windows.Forms.CheckBox showDateCheckBox;
        private System.Windows.Forms.CheckBox showAuthorCheckBox;
        private System.Windows.Forms.CheckBox showMessageCheckBox;
        private System.Windows.Forms.CheckBox showModifiedFilesCheckBox;
        private System.Windows.Forms.CheckBox singleRevisionCheckBox;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label selectRevisionLabel;
        private Ankh.UI.RevisionPicker fromRevision;
        private Ankh.UI.RevisionPicker toRevision;

    }
}
