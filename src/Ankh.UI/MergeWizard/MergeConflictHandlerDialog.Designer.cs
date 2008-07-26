namespace Ankh.UI.MergeWizard
{
    partial class MergeConflictHandlerDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.preferencesGroupBox = new System.Windows.Forms.GroupBox();
            this.baseRadioButton = new System.Windows.Forms.RadioButton();
            this.theirsRadioButton = new System.Windows.Forms.RadioButton();
            this.mineRadioButton = new System.Windows.Forms.RadioButton();
            this.postponeRadioButton = new System.Windows.Forms.RadioButton();
            this.diffPanel = new System.Windows.Forms.Panel();
            this.diffControl = new Ankh.Diff.DiffUtils.Controls.DiffControl();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.applyToCheckBox = new System.Windows.Forms.CheckBox();
            this.preferencesGroupBox.SuspendLayout();
            this.diffPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // preferencesGroupBox
            // 
            this.preferencesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.preferencesGroupBox.Controls.Add(this.baseRadioButton);
            this.preferencesGroupBox.Controls.Add(this.theirsRadioButton);
            this.preferencesGroupBox.Controls.Add(this.mineRadioButton);
            this.preferencesGroupBox.Controls.Add(this.postponeRadioButton);
            this.preferencesGroupBox.Location = new System.Drawing.Point(12, 9);
            this.preferencesGroupBox.Name = "preferencesGroupBox";
            this.preferencesGroupBox.Size = new System.Drawing.Size(467, 119);
            this.preferencesGroupBox.TabIndex = 0;
            this.preferencesGroupBox.TabStop = false;
            this.preferencesGroupBox.Text = "How do you want to handle the conflict?";
            // 
            // baseRadioButton
            // 
            this.baseRadioButton.AutoSize = true;
            this.baseRadioButton.Location = new System.Drawing.Point(7, 90);
            this.baseRadioButton.Name = "baseRadioButton";
            this.baseRadioButton.Size = new System.Drawing.Size(318, 17);
            this.baseRadioButton.TabIndex = 3;
            this.baseRadioButton.TabStop = true;
            this.baseRadioButton.Text = "Resolve the conflict with the pre-merge &base version of the file";
            this.baseRadioButton.UseVisualStyleBackColor = true;
            this.baseRadioButton.CheckedChanged += new System.EventHandler(this.baseRadioButton_CheckedChanged);
            // 
            // theirsRadioButton
            // 
            this.theirsRadioButton.AutoSize = true;
            this.theirsRadioButton.Location = new System.Drawing.Point(7, 67);
            this.theirsRadioButton.Name = "theirsRadioButton";
            this.theirsRadioButton.Size = new System.Drawing.Size(296, 17);
            this.theirsRadioButton.TabIndex = 2;
            this.theirsRadioButton.TabStop = true;
            this.theirsRadioButton.Text = "R&esolve the conflict using the repository version of the file";
            this.theirsRadioButton.UseVisualStyleBackColor = true;
            this.theirsRadioButton.CheckedChanged += new System.EventHandler(this.theirsRadioButton_CheckedChanged);
            // 
            // mineRadioButton
            // 
            this.mineRadioButton.AutoSize = true;
            this.mineRadioButton.Location = new System.Drawing.Point(7, 44);
            this.mineRadioButton.Name = "mineRadioButton";
            this.mineRadioButton.Size = new System.Drawing.Size(246, 17);
            this.mineRadioButton.TabIndex = 1;
            this.mineRadioButton.TabStop = true;
            this.mineRadioButton.Text = "&Resolve the conflict using my version of the file";
            this.mineRadioButton.UseVisualStyleBackColor = true;
            this.mineRadioButton.CheckedChanged += new System.EventHandler(this.mineRadioButton_CheckedChanged);
            // 
            // postponeRadioButton
            // 
            this.postponeRadioButton.AutoSize = true;
            this.postponeRadioButton.Location = new System.Drawing.Point(7, 20);
            this.postponeRadioButton.Name = "postponeRadioButton";
            this.postponeRadioButton.Size = new System.Drawing.Size(216, 17);
            this.postponeRadioButton.TabIndex = 0;
            this.postponeRadioButton.TabStop = true;
            this.postponeRadioButton.Text = "&Mark the conflict and let me resolve later";
            this.postponeRadioButton.UseVisualStyleBackColor = true;
            this.postponeRadioButton.CheckedChanged += new System.EventHandler(this.postponeRadioButton_CheckedChanged);
            // 
            // diffPanel
            // 
            this.diffPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.diffPanel.Controls.Add(this.diffControl);
            this.diffPanel.Location = new System.Drawing.Point(12, 156);
            this.diffPanel.Name = "diffPanel";
            this.diffPanel.Size = new System.Drawing.Size(467, 313);
            this.diffPanel.TabIndex = 1;
            // 
            // diffControl
            // 
            this.diffControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.diffControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.diffControl.LineDiffHeight = 60;
            this.diffControl.Location = new System.Drawing.Point(0, 0);
            this.diffControl.Name = "diffControl";
            this.diffControl.ShowToolbar = false;
            this.diffControl.ShowWhitespaceInLineDiff = true;
            this.diffControl.Size = new System.Drawing.Size(467, 313);
            this.diffControl.TabIndex = 0;
            this.diffControl.ViewFont = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(323, 475);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(404, 475);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // applyToCheckBox
            // 
            this.applyToCheckBox.AutoSize = true;
            this.applyToCheckBox.Location = new System.Drawing.Point(12, 134);
            this.applyToCheckBox.Name = "applyToCheckBox";
            this.applyToCheckBox.Size = new System.Drawing.Size(119, 17);
            this.applyToCheckBox.TabIndex = 4;
            this.applyToCheckBox.Text = "&Apply to all conflicts";
            this.applyToCheckBox.UseVisualStyleBackColor = true;
            this.applyToCheckBox.CheckedChanged += new System.EventHandler(this.applyToCheckBox_CheckedChanged);
            // 
            // MergeConflictHandlerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(491, 510);
            this.Controls.Add(this.applyToCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.diffPanel);
            this.Controls.Add(this.preferencesGroupBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MergeConflictHandlerDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Handle Conflict";
            this.preferencesGroupBox.ResumeLayout(false);
            this.preferencesGroupBox.PerformLayout();
            this.diffPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox preferencesGroupBox;
        private System.Windows.Forms.RadioButton postponeRadioButton;
        private System.Windows.Forms.RadioButton mineRadioButton;
        private System.Windows.Forms.RadioButton theirsRadioButton;
        private System.Windows.Forms.RadioButton baseRadioButton;
        private System.Windows.Forms.Panel diffPanel;
        private Ankh.Diff.DiffUtils.Controls.DiffControl diffControl;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox applyToCheckBox;
    }
}