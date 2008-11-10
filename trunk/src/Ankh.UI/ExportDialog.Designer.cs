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
            this.localDirGroupBox = new System.Windows.Forms.GroupBox();
            this.toDirBrowseButton = new System.Windows.Forms.Button();
            this.localDirTextBox = new System.Windows.Forms.TextBox();
            this.nonRecursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.radioButtonGroupbox = new System.Windows.Forms.GroupBox();
            this.radioButtonFromDir = new System.Windows.Forms.RadioButton();
            this.radioButtonFromURL = new System.Windows.Forms.RadioButton();
            this.wcBrowseBtn = new System.Windows.Forms.Button();
            this.exportFromDirTextBox = new System.Windows.Forms.TextBox();
            this.versionLabel = new System.Windows.Forms.Label();
            this.urlBrowseBtn = new System.Windows.Forms.Button();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.revisionPicker = new Ankh.UI.PathSelector.VersionSelector();
            this.localDirGroupBox.SuspendLayout();
            this.radioButtonGroupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // localDirGroupBox
            // 
            this.localDirGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.localDirGroupBox.Controls.Add(this.toDirBrowseButton);
            this.localDirGroupBox.Controls.Add(this.localDirTextBox);
            this.localDirGroupBox.Location = new System.Drawing.Point(8, 189);
            this.localDirGroupBox.Name = "localDirGroupBox";
            this.localDirGroupBox.Size = new System.Drawing.Size(443, 56);
            this.localDirGroupBox.TabIndex = 1;
            this.localDirGroupBox.TabStop = false;
            this.localDirGroupBox.Text = "Export &To:";
            // 
            // toDirBrowseButton
            // 
            this.toDirBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.toDirBrowseButton.Location = new System.Drawing.Point(409, 18);
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
            this.localDirTextBox.Location = new System.Drawing.Point(26, 20);
            this.localDirTextBox.Name = "localDirTextBox";
            this.localDirTextBox.Size = new System.Drawing.Size(377, 20);
            this.localDirTextBox.TabIndex = 0;
            this.localDirTextBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            // 
            // nonRecursiveCheckBox
            // 
            this.nonRecursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nonRecursiveCheckBox.Location = new System.Drawing.Point(24, 268);
            this.nonRecursiveCheckBox.Name = "nonRecursiveCheckBox";
            this.nonRecursiveCheckBox.Size = new System.Drawing.Size(104, 24);
            this.nonRecursiveCheckBox.TabIndex = 2;
            this.nonRecursiveCheckBox.Text = "&Non-recursive";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(295, 269);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(376, 269);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            // 
            // radioButtonGroupbox
            // 
            this.radioButtonGroupbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonGroupbox.Controls.Add(this.versionLabel);
            this.radioButtonGroupbox.Controls.Add(this.revisionPicker);
            this.radioButtonGroupbox.Controls.Add(this.urlBrowseBtn);
            this.radioButtonGroupbox.Controls.Add(this.wcBrowseBtn);
            this.radioButtonGroupbox.Controls.Add(this.urlTextBox);
            this.radioButtonGroupbox.Controls.Add(this.exportFromDirTextBox);
            this.radioButtonGroupbox.Controls.Add(this.radioButtonFromDir);
            this.radioButtonGroupbox.Controls.Add(this.radioButtonFromURL);
            this.radioButtonGroupbox.Location = new System.Drawing.Point(8, 12);
            this.radioButtonGroupbox.Name = "radioButtonGroupbox";
            this.radioButtonGroupbox.Size = new System.Drawing.Size(443, 171);
            this.radioButtonGroupbox.TabIndex = 0;
            this.radioButtonGroupbox.TabStop = false;
            this.radioButtonGroupbox.Text = "&Export From:";
            // 
            // radioButtonFromDir
            // 
            this.radioButtonFromDir.Location = new System.Drawing.Point(8, 64);
            this.radioButtonFromDir.Name = "radioButtonFromDir";
            this.radioButtonFromDir.Size = new System.Drawing.Size(176, 24);
            this.radioButtonFromDir.TabIndex = 1;
            this.radioButtonFromDir.TabStop = true;
            this.radioButtonFromDir.Text = "Export From &Working Copy:";
            this.radioButtonFromDir.CheckedChanged += new System.EventHandler(this.radioButtonFromDir_CheckedChanged);
            // 
            // radioButtonFromURL
            // 
            this.radioButtonFromURL.Checked = true;
            this.radioButtonFromURL.Location = new System.Drawing.Point(8, 19);
            this.radioButtonFromURL.Name = "radioButtonFromURL";
            this.radioButtonFromURL.Size = new System.Drawing.Size(128, 24);
            this.radioButtonFromURL.TabIndex = 0;
            this.radioButtonFromURL.TabStop = true;
            this.radioButtonFromURL.Text = "Export From &Url:";
            this.radioButtonFromURL.CheckedChanged += new System.EventHandler(this.radioButtonFromURL_CheckedChanged);
            // 
            // wcBrowseBtn
            // 
            this.wcBrowseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.wcBrowseBtn.Enabled = false;
            this.wcBrowseBtn.Location = new System.Drawing.Point(409, 87);
            this.wcBrowseBtn.Name = "wcBrowseBtn";
            this.wcBrowseBtn.Size = new System.Drawing.Size(24, 23);
            this.wcBrowseBtn.TabIndex = 5;
            this.wcBrowseBtn.Text = "...";
            this.wcBrowseBtn.Click += new System.EventHandler(this.exportFromDirButton_Click);
            // 
            // exportFromDirTextBox
            // 
            this.exportFromDirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.exportFromDirTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.exportFromDirTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.exportFromDirTextBox.Enabled = false;
            this.exportFromDirTextBox.Location = new System.Drawing.Point(26, 89);
            this.exportFromDirTextBox.Name = "exportFromDirTextBox";
            this.exportFromDirTextBox.Size = new System.Drawing.Size(377, 20);
            this.exportFromDirTextBox.TabIndex = 4;
            this.exportFromDirTextBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(23, 120);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(45, 13);
            this.versionLabel.TabIndex = 6;
            this.versionLabel.Text = "Version:";
            // 
            // urlBrowseBtn
            // 
            this.urlBrowseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.urlBrowseBtn.Location = new System.Drawing.Point(409, 42);
            this.urlBrowseBtn.Name = "urlBrowseBtn";
            this.urlBrowseBtn.Size = new System.Drawing.Size(24, 23);
            this.urlBrowseBtn.TabIndex = 3;
            this.urlBrowseBtn.Text = "...";
            this.urlBrowseBtn.Click += new System.EventHandler(this.urlBrowseBtn_Click);
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.urlTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.urlTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.urlTextBox.Location = new System.Drawing.Point(26, 44);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(377, 20);
            this.urlTextBox.TabIndex = 2;
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPicker.Location = new System.Drawing.Point(26, 136);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(377, 20);
            this.revisionPicker.TabIndex = 7;
            this.revisionPicker.Changed += new System.EventHandler(this.ControlsChanged);
            // 
            // ExportDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(461, 304);
            this.Controls.Add(this.radioButtonGroupbox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.nonRecursiveCheckBox);
            this.Controls.Add(this.localDirGroupBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export";
            this.localDirGroupBox.ResumeLayout(false);
            this.localDirGroupBox.PerformLayout();
            this.radioButtonGroupbox.ResumeLayout(false);
            this.radioButtonGroupbox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion



        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private Ankh.UI.PathSelector.VersionSelector revisionPicker;
        private System.Windows.Forms.GroupBox localDirGroupBox;
        private System.Windows.Forms.TextBox localDirTextBox;
        private System.Windows.Forms.Button toDirBrowseButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox nonRecursiveCheckBox;
        private System.Windows.Forms.GroupBox radioButtonGroupbox;
        private System.Windows.Forms.RadioButton radioButtonFromURL;
        private System.Windows.Forms.RadioButton radioButtonFromDir;
        private System.Windows.Forms.Button wcBrowseBtn;
        private System.Windows.Forms.TextBox exportFromDirTextBox;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Button urlBrowseBtn;
        private System.Windows.Forms.TextBox urlTextBox;
    }
}
