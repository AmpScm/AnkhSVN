namespace Ankh.UI
{
    partial class AnkhSettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbDiffMergeManual = new System.Windows.Forms.CheckBox();
            this.txtDiffExePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDiffExePath = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMergeExePath = new System.Windows.Forms.TextBox();
            this.btnMergeExePath = new System.Windows.Forms.Button();
            this.interactiveMergeOnConflict = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbDiffMergeManual
            // 
            this.cbDiffMergeManual.AutoSize = true;
            this.cbDiffMergeManual.Location = new System.Drawing.Point(3, 86);
            this.cbDiffMergeManual.Name = "cbDiffMergeManual";
            this.cbDiffMergeManual.Size = new System.Drawing.Size(269, 17);
            this.cbDiffMergeManual.TabIndex = 4;
            this.cbDiffMergeManual.Text = "Manually choose between Internal and External Diff";
            this.cbDiffMergeManual.UseVisualStyleBackColor = true;
            // 
            // txtDiffExePath
            // 
            this.txtDiffExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDiffExePath.Location = new System.Drawing.Point(0, 15);
            this.txtDiffExePath.Name = "txtDiffExePath";
            this.txtDiffExePath.Size = new System.Drawing.Size(355, 20);
            this.txtDiffExePath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "External &Diff Path:";
            // 
            // btnDiffExePath
            // 
            this.btnDiffExePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDiffExePath.Location = new System.Drawing.Point(366, 13);
            this.btnDiffExePath.Name = "btnDiffExePath";
            this.btnDiffExePath.Size = new System.Drawing.Size(26, 23);
            this.btnDiffExePath.TabIndex = 1;
            this.btnDiffExePath.Text = "...";
            this.btnDiffExePath.UseVisualStyleBackColor = true;
            this.btnDiffExePath.Click += new System.EventHandler(this.btnDiffExePath_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "External &Merge Path:";
            // 
            // txtMergeExePath
            // 
            this.txtMergeExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMergeExePath.Location = new System.Drawing.Point(0, 58);
            this.txtMergeExePath.Name = "txtMergeExePath";
            this.txtMergeExePath.Size = new System.Drawing.Size(355, 20);
            this.txtMergeExePath.TabIndex = 2;
            // 
            // btnMergeExePath
            // 
            this.btnMergeExePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMergeExePath.Location = new System.Drawing.Point(366, 55);
            this.btnMergeExePath.Name = "btnMergeExePath";
            this.btnMergeExePath.Size = new System.Drawing.Size(26, 23);
            this.btnMergeExePath.TabIndex = 3;
            this.btnMergeExePath.Text = "...";
            this.btnMergeExePath.UseVisualStyleBackColor = true;
            this.btnMergeExePath.Click += new System.EventHandler(this.btnMergePath_Click);
            // 
            // interactiveMergeOnConflict
            // 
            this.interactiveMergeOnConflict.AutoSize = true;
            this.interactiveMergeOnConflict.Location = new System.Drawing.Point(3, 109);
            this.interactiveMergeOnConflict.Name = "interactiveMergeOnConflict";
            this.interactiveMergeOnConflict.Size = new System.Drawing.Size(218, 17);
            this.interactiveMergeOnConflict.TabIndex = 6;
            this.interactiveMergeOnConflict.Text = "Start Interactive Merge on &Conflict (Beta)";
            this.interactiveMergeOnConflict.UseVisualStyleBackColor = true;
            // 
            // AnkhSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.interactiveMergeOnConflict);
            this.Controls.Add(this.btnMergeExePath);
            this.Controls.Add(this.txtMergeExePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnDiffExePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDiffExePath);
            this.Controls.Add(this.cbDiffMergeManual);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "AnkhSettingsControl";
            this.Size = new System.Drawing.Size(400, 271);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.CheckBox cbDiffMergeManual;
		private System.Windows.Forms.TextBox txtDiffExePath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnDiffExePath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtMergeExePath;
		private System.Windows.Forms.Button btnMergeExePath;
        private System.Windows.Forms.CheckBox interactiveMergeOnConflict;
    }
}
