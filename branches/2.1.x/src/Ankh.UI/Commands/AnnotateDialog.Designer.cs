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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnnotateDialog));
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
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// targetBox
			// 
			resources.ApplyResources(this.targetBox, "targetBox");
			this.targetBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.targetBox.FormatString = "f";
			this.targetBox.FormattingEnabled = true;
			this.targetBox.Name = "targetBox";
			this.targetBox.SelectedValueChanged += new System.EventHandler(this.targetBox_SelectedValueChanged);
			// 
			// browseButton
			// 
			resources.ApplyResources(this.browseButton, "browseButton");
			this.browseButton.Name = "browseButton";
			this.browseButton.UseVisualStyleBackColor = true;
			this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.startRevision);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// startRevision
			// 
			resources.ApplyResources(this.startRevision, "startRevision");
			this.startRevision.Name = "startRevision";
			this.startRevision.SvnOrigin = null;
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this.toRevision);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// toRevision
			// 
			resources.ApplyResources(this.toRevision, "toRevision");
			this.toRevision.Name = "toRevision";
			this.toRevision.SvnOrigin = null;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// whitespaceBox
			// 
			this.whitespaceBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.whitespaceBox, "whitespaceBox");
			this.whitespaceBox.Name = "whitespaceBox";
			// 
			// ignoreEols
			// 
			resources.ApplyResources(this.ignoreEols, "ignoreEols");
			this.ignoreEols.Checked = true;
			this.ignoreEols.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ignoreEols.Name = "ignoreEols";
			this.ignoreEols.UseVisualStyleBackColor = true;
			// 
			// includeMergeInfo
			// 
			resources.ApplyResources(this.includeMergeInfo, "includeMergeInfo");
			this.includeMergeInfo.Name = "includeMergeInfo";
			this.includeMergeInfo.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// AnnotateDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
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
