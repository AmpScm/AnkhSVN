namespace Ankh.UI.Commands
{
	partial class AnnotateDialog
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
			this.label1 = new System.Windows.Forms.Label();
			this.targetBox = new System.Windows.Forms.ComboBox();
			this.browseButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.startRevision = new Ankh.UI.PathSelector.VersionSelector();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.toRevision = new Ankh.UI.PathSelector.VersionSelector();
			this.label2 = new System.Windows.Forms.Label();
			this.whitespaceBox = new System.Windows.Forms.ComboBox();
			this.ignoreEols = new System.Windows.Forms.CheckBox();
			this.includeMergeInfo = new System.Windows.Forms.CheckBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Target:";
			// 
			// targetBox
			// 
			this.targetBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.targetBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.targetBox.FormatString = "f";
			this.targetBox.FormattingEnabled = true;
			this.targetBox.Location = new System.Drawing.Point(15, 25);
			this.targetBox.Name = "targetBox";
			this.targetBox.Size = new System.Drawing.Size(400, 21);
			this.targetBox.TabIndex = 1;
			this.targetBox.SelectedValueChanged += new System.EventHandler(this.targetBox_SelectedValueChanged);
			// 
			// browseButton
			// 
			this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browseButton.Location = new System.Drawing.Point(421, 23);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new System.Drawing.Size(27, 23);
			this.browseButton.TabIndex = 2;
			this.browseButton.Text = "...";
			this.browseButton.UseVisualStyleBackColor = true;
			this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.startRevision);
			this.groupBox1.Location = new System.Drawing.Point(15, 57);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(433, 55);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "&From Revision:";
			// 
			// startRevision
			// 
			this.startRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.startRevision.Location = new System.Drawing.Point(6, 19);
			this.startRevision.Name = "startRevision";
			this.startRevision.Size = new System.Drawing.Size(379, 29);
			this.startRevision.SvnOrigin = null;
			this.startRevision.TabIndex = 2;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.toRevision);
			this.groupBox2.Location = new System.Drawing.Point(15, 118);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(433, 55);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "&To Revision:";
			// 
			// toRevision
			// 
			this.toRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.toRevision.Location = new System.Drawing.Point(6, 19);
			this.toRevision.Name = "toRevision";
			this.toRevision.Size = new System.Drawing.Size(379, 29);
			this.toRevision.SvnOrigin = null;
			this.toRevision.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 188);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "&Whitespace:";
			// 
			// whitespaceBox
			// 
			this.whitespaceBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.whitespaceBox.Location = new System.Drawing.Point(91, 185);
			this.whitespaceBox.Name = "whitespaceBox";
			this.whitespaceBox.Size = new System.Drawing.Size(187, 21);
			this.whitespaceBox.TabIndex = 8;
			// 
			// ignoreEols
			// 
			this.ignoreEols.AutoSize = true;
			this.ignoreEols.Checked = true;
			this.ignoreEols.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ignoreEols.Location = new System.Drawing.Point(21, 216);
			this.ignoreEols.Name = "ignoreEols";
			this.ignoreEols.Size = new System.Drawing.Size(120, 17);
			this.ignoreEols.TabIndex = 9;
			this.ignoreEols.Text = "&Ignore Line Endings";
			this.ignoreEols.UseVisualStyleBackColor = true;
			// 
			// includeMergeInfo
			// 
			this.includeMergeInfo.AutoSize = true;
			this.includeMergeInfo.Location = new System.Drawing.Point(21, 239);
			this.includeMergeInfo.Name = "includeMergeInfo";
			this.includeMergeInfo.Size = new System.Drawing.Size(151, 17);
			this.includeMergeInfo.TabIndex = 10;
			this.includeMergeInfo.Text = "Retrieve &Mergeinfo (Slow!)";
			this.includeMergeInfo.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(286, 243);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 11;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(367, 243);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 12;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// AnnotateDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(460, 278);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.includeMergeInfo);
			this.Controls.Add(this.ignoreEols);
			this.Controls.Add(this.whitespaceBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.browseButton);
			this.Controls.Add(this.targetBox);
			this.Controls.Add(this.label1);
			this.Name = "AnnotateDialog";
			this.Text = "Annotate";
			this.Load += new System.EventHandler(this.AnnotateDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox targetBox;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private Ankh.UI.PathSelector.VersionSelector startRevision;
		private System.Windows.Forms.GroupBox groupBox2;
		private Ankh.UI.PathSelector.VersionSelector toRevision;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox whitespaceBox;
		private System.Windows.Forms.CheckBox ignoreEols;
		private System.Windows.Forms.CheckBox includeMergeInfo;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
	}
}