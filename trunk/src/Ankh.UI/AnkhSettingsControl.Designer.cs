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
			this.SuspendLayout();
			// 
			// cbDiffMergeManual
			// 
			this.cbDiffMergeManual.AutoSize = true;
			this.cbDiffMergeManual.Location = new System.Drawing.Point(3, 162);
			this.cbDiffMergeManual.Name = "cbDiffMergeManual";
			this.cbDiffMergeManual.Size = new System.Drawing.Size(256, 17);
			this.cbDiffMergeManual.TabIndex = 1;
			this.cbDiffMergeManual.Text = "Manually choose between internal or external diff";
			this.cbDiffMergeManual.UseVisualStyleBackColor = true;
			// 
			// txtDiffExePath
			// 
			this.txtDiffExePath.Location = new System.Drawing.Point(0, 16);
			this.txtDiffExePath.Name = "txtDiffExePath";
			this.txtDiffExePath.ReadOnly = true;
			this.txtDiffExePath.Size = new System.Drawing.Size(361, 20);
			this.txtDiffExePath.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "External diff path:";
			// 
			// btnDiffExePath
			// 
			this.btnDiffExePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDiffExePath.Location = new System.Drawing.Point(367, 14);
			this.btnDiffExePath.Name = "btnDiffExePath";
			this.btnDiffExePath.Size = new System.Drawing.Size(30, 23);
			this.btnDiffExePath.TabIndex = 4;
			this.btnDiffExePath.Text = "...";
			this.btnDiffExePath.UseVisualStyleBackColor = true;
			this.btnDiffExePath.Click += new System.EventHandler(this.btnDiffExePath_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "External merge path:";
			// 
			// txtMergeExePath
			// 
			this.txtMergeExePath.Location = new System.Drawing.Point(0, 55);
			this.txtMergeExePath.Name = "txtMergeExePath";
			this.txtMergeExePath.ReadOnly = true;
			this.txtMergeExePath.Size = new System.Drawing.Size(361, 20);
			this.txtMergeExePath.TabIndex = 6;
			// 
			// btnMergeExePath
			// 
			this.btnMergeExePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMergeExePath.Location = new System.Drawing.Point(367, 52);
			this.btnMergeExePath.Name = "btnMergeExePath";
			this.btnMergeExePath.Size = new System.Drawing.Size(30, 23);
			this.btnMergeExePath.TabIndex = 7;
			this.btnMergeExePath.Text = "...";
			this.btnMergeExePath.UseVisualStyleBackColor = true;
			this.btnMergeExePath.Click += new System.EventHandler(this.btnMergePath_Click);
			// 
			// AnkhSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnMergeExePath);
			this.Controls.Add(this.txtMergeExePath);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnDiffExePath);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtDiffExePath);
			this.Controls.Add(this.cbDiffMergeManual);
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
    }
}
