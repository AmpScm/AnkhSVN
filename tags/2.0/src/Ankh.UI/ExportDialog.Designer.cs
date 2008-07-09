using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class ExportDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.revisionGroupBox = new System.Windows.Forms.GroupBox();
			this.revisionPicker = new Ankh.UI.RevisionPicker();
			this.urlGroupBox = new System.Windows.Forms.GroupBox();
			this.urlTextBox = new System.Windows.Forms.TextBox();
			this.localDirGroupBox = new System.Windows.Forms.GroupBox();
			this.toDirBrowseButton = new System.Windows.Forms.Button();
			this.localDirTextBox = new System.Windows.Forms.TextBox();
			this.nonRecursiveCheckBox = new System.Windows.Forms.CheckBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.radioButtonGroupbox = new System.Windows.Forms.GroupBox();
			this.radioButtonFromDir = new System.Windows.Forms.RadioButton();
			this.radioButtonFromURL = new System.Windows.Forms.RadioButton();
			this.exportFromDirGroupbox = new System.Windows.Forms.GroupBox();
			this.exportFromDirButton = new System.Windows.Forms.Button();
			this.exportFromDirTextBox = new System.Windows.Forms.TextBox();
			this.revisionGroupBox.SuspendLayout();
			this.urlGroupBox.SuspendLayout();
			this.localDirGroupBox.SuspendLayout();
			this.radioButtonGroupbox.SuspendLayout();
			this.exportFromDirGroupbox.SuspendLayout();
			this.SuspendLayout();
			// 
			// revisionGroupBox
			// 
			this.revisionGroupBox.Controls.Add(this.revisionPicker);
			this.revisionGroupBox.Location = new System.Drawing.Point(8, 82);
			this.revisionGroupBox.Name = "revisionGroupBox";
			this.revisionGroupBox.Size = new System.Drawing.Size(408, 56);
			this.revisionGroupBox.TabIndex = 0;
			this.revisionGroupBox.TabStop = false;
			this.revisionGroupBox.Text = "&Revision";
			// 
			// revisionPicker
			// 
			this.revisionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.revisionPicker.Location = new System.Drawing.Point(16, 24);
			this.revisionPicker.Name = "revisionPicker";
			this.revisionPicker.Size = new System.Drawing.Size(384, 20);
			this.revisionPicker.TabIndex = 0;
			this.revisionPicker.Changed += new System.EventHandler(this.ControlsChanged);
			// 
			// urlGroupBox
			// 
			this.urlGroupBox.Controls.Add(this.urlTextBox);
			this.urlGroupBox.Location = new System.Drawing.Point(8, 156);
			this.urlGroupBox.Name = "urlGroupBox";
			this.urlGroupBox.Size = new System.Drawing.Size(408, 56);
			this.urlGroupBox.TabIndex = 1;
			this.urlGroupBox.TabStop = false;
			this.urlGroupBox.Text = "Export &from URL";
			this.urlGroupBox.TextChanged += new System.EventHandler(this.ControlsChanged);
			// 
			// urlTextBox
			// 
			this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.urlTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.urlTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.urlTextBox.Location = new System.Drawing.Point(16, 20);
			this.urlTextBox.Name = "urlTextBox";
			this.urlTextBox.Size = new System.Drawing.Size(376, 20);
			this.urlTextBox.TabIndex = 0;
			// 
			// localDirGroupBox
			// 
			this.localDirGroupBox.Controls.Add(this.toDirBrowseButton);
			this.localDirGroupBox.Controls.Add(this.localDirTextBox);
			this.localDirGroupBox.Location = new System.Drawing.Point(8, 230);
			this.localDirGroupBox.Name = "localDirGroupBox";
			this.localDirGroupBox.Size = new System.Drawing.Size(408, 56);
			this.localDirGroupBox.TabIndex = 2;
			this.localDirGroupBox.TabStop = false;
			this.localDirGroupBox.Text = "Local &Directory";
			// 
			// toDirBrowseButton
			// 
			this.toDirBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.toDirBrowseButton.Location = new System.Drawing.Point(376, 20);
			this.toDirBrowseButton.Name = "toDirBrowseButton";
			this.toDirBrowseButton.Size = new System.Drawing.Size(24, 23);
			this.toDirBrowseButton.TabIndex = 1;
			this.toDirBrowseButton.Text = "&...";
			this.toDirBrowseButton.Click += new System.EventHandler(this.BrowseClicked);
			// 
			// localDirTextBox
			// 
			this.localDirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.localDirTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.localDirTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this.localDirTextBox.Location = new System.Drawing.Point(16, 20);
			this.localDirTextBox.Name = "localDirTextBox";
			this.localDirTextBox.Size = new System.Drawing.Size(352, 20);
			this.localDirTextBox.TabIndex = 0;
			this.localDirTextBox.TextChanged += new System.EventHandler(this.ControlsChanged);
			// 
			// nonRecursiveCheckBox
			// 
			this.nonRecursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.nonRecursiveCheckBox.Location = new System.Drawing.Point(16, 304);
			this.nonRecursiveCheckBox.Name = "nonRecursiveCheckBox";
			this.nonRecursiveCheckBox.Size = new System.Drawing.Size(104, 24);
			this.nonRecursiveCheckBox.TabIndex = 3;
			this.nonRecursiveCheckBox.Text = "&Non-recursive";
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Enabled = false;
			this.okButton.Location = new System.Drawing.Point(256, 304);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 4;
			this.okButton.Text = "&Export";
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(344, 304);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			// 
			// radioButtonGroupbox
			// 
			this.radioButtonGroupbox.Controls.Add(this.radioButtonFromDir);
			this.radioButtonGroupbox.Controls.Add(this.radioButtonFromURL);
			this.radioButtonGroupbox.Location = new System.Drawing.Point(8, 8);
			this.radioButtonGroupbox.Name = "radioButtonGroupbox";
			this.radioButtonGroupbox.Size = new System.Drawing.Size(408, 56);
			this.radioButtonGroupbox.TabIndex = 6;
			this.radioButtonGroupbox.TabStop = false;
			this.radioButtonGroupbox.Text = "Export Source";
			// 
			// radioButtonFromDir
			// 
			this.radioButtonFromDir.Location = new System.Drawing.Point(216, 24);
			this.radioButtonFromDir.Name = "radioButtonFromDir";
			this.radioButtonFromDir.Size = new System.Drawing.Size(176, 24);
			this.radioButtonFromDir.TabIndex = 1;
			this.radioButtonFromDir.Text = "Export From &Local Directory";
			this.radioButtonFromDir.CheckedChanged += new System.EventHandler(this.radioButtonFromDir_CheckedChanged);
			// 
			// radioButtonFromURL
			// 
			this.radioButtonFromURL.Checked = true;
			this.radioButtonFromURL.Location = new System.Drawing.Point(32, 24);
			this.radioButtonFromURL.Name = "radioButtonFromURL";
			this.radioButtonFromURL.Size = new System.Drawing.Size(128, 24);
			this.radioButtonFromURL.TabIndex = 0;
			this.radioButtonFromURL.TabStop = true;
			this.radioButtonFromURL.Text = "Export From &URL";
			this.radioButtonFromURL.CheckedChanged += new System.EventHandler(this.radioButtonFromURL_CheckedChanged);
			// 
			// exportFromDirGroupbox
			// 
			this.exportFromDirGroupbox.Controls.Add(this.exportFromDirButton);
			this.exportFromDirGroupbox.Controls.Add(this.exportFromDirTextBox);
			this.exportFromDirGroupbox.Location = new System.Drawing.Point(8, 156);
			this.exportFromDirGroupbox.Name = "exportFromDirGroupbox";
			this.exportFromDirGroupbox.Size = new System.Drawing.Size(408, 56);
			this.exportFromDirGroupbox.TabIndex = 7;
			this.exportFromDirGroupbox.TabStop = false;
			this.exportFromDirGroupbox.Text = "Export &from Local Directory";
			this.exportFromDirGroupbox.Visible = false;
			// 
			// exportFromDirButton
			// 
			this.exportFromDirButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.exportFromDirButton.Location = new System.Drawing.Point(376, 20);
			this.exportFromDirButton.Name = "exportFromDirButton";
			this.exportFromDirButton.Size = new System.Drawing.Size(24, 23);
			this.exportFromDirButton.TabIndex = 1;
			this.exportFromDirButton.Text = "...";
			this.exportFromDirButton.Click += new System.EventHandler(this.exportFromDirButton_Click);
			// 
			// exportFromDirTextBox
			// 
			this.exportFromDirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.exportFromDirTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.exportFromDirTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this.exportFromDirTextBox.Location = new System.Drawing.Point(16, 20);
			this.exportFromDirTextBox.Name = "exportFromDirTextBox";
			this.exportFromDirTextBox.Size = new System.Drawing.Size(352, 20);
			this.exportFromDirTextBox.TabIndex = 0;
			this.exportFromDirTextBox.TextChanged += new System.EventHandler(this.ControlsChanged);
			// 
			// ExportDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(426, 343);
			this.ControlBox = false;
			this.Controls.Add(this.radioButtonGroupbox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.nonRecursiveCheckBox);
			this.Controls.Add(this.localDirGroupBox);
			this.Controls.Add(this.revisionGroupBox);
			this.Controls.Add(this.exportFromDirGroupbox);
			this.Controls.Add(this.urlGroupBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ExportDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Export";
			this.revisionGroupBox.ResumeLayout(false);
			this.urlGroupBox.ResumeLayout(false);
			this.urlGroupBox.PerformLayout();
			this.localDirGroupBox.ResumeLayout(false);
			this.localDirGroupBox.PerformLayout();
			this.radioButtonGroupbox.ResumeLayout(false);
			this.exportFromDirGroupbox.ResumeLayout(false);
			this.exportFromDirGroupbox.PerformLayout();
			this.ResumeLayout(false);

        }
        #endregion



        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.GroupBox revisionGroupBox;
        private Ankh.UI.RevisionPicker revisionPicker;
        private System.Windows.Forms.GroupBox urlGroupBox;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.GroupBox localDirGroupBox;
        private System.Windows.Forms.TextBox localDirTextBox;
        private System.Windows.Forms.Button toDirBrowseButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox nonRecursiveCheckBox;
        private System.Windows.Forms.GroupBox radioButtonGroupbox;
        private System.Windows.Forms.RadioButton radioButtonFromURL;
        private System.Windows.Forms.RadioButton radioButtonFromDir;
        private System.Windows.Forms.Button exportFromDirButton;
        private System.Windows.Forms.TextBox exportFromDirTextBox;
        private System.Windows.Forms.GroupBox exportFromDirGroupbox;
    }
}
