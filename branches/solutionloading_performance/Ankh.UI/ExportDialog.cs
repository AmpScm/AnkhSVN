using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NSvn.Core;
using Utils;
using Utils.Win32;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for performing exports.
    /// </summary>
    public class ExportDialog : System.Windows.Forms.Form
    {
        public ExportDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();	

            Win32.SHAutoComplete( this.urlTextBox.Handle, 
                Shacf.UrlAll );
            Win32.SHAutoComplete( this.localDirTextBox.Handle, 
                Shacf.Filesystem );
            Win32.SHAutoComplete( this.exportFromDirTextBox.Handle, 
                Shacf.Filesystem );
	
            this.ControlsChanged( this, EventArgs.Empty );
        }

        /// <summary>
        /// The URL of the repository.
        /// </summary>
        public string Source
        {
            get
            {
                if(this.radioButtonFromURL.Checked) 
                    return this.urlTextBox.Text; 
                else
                    return this.exportFromDirTextBox.Text; 
            }
        }


        /// <summary>
        /// The local path to check out to.
        /// </summary>
        public string LocalPath
        {
            get{ return this.localDirTextBox.Text; }
            set{ this.localDirTextBox.Text = value; }
        }

        /// <summary>
        /// The revision to check out.
        /// </summary>
        public Revision Revision
        {
            get{ return this.revisionPicker.Revision; }
        }

        /// <summary>
        /// Whether to perform a non-recursive export.
        /// </summary>
        public bool NonRecursive
        {
            get{ return this.nonRecursiveCheckBox.Checked; }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

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
            this.revisionGroupBox.Text = "Revision";
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
            this.urlGroupBox.Text = "URL";
            this.urlGroupBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.urlTextBox.Location = new System.Drawing.Point(16, 20);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(376, 20);
            this.urlTextBox.TabIndex = 0;
            this.urlTextBox.Text = "";
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
            this.localDirGroupBox.Text = "Local Directory";
            // 
            // toDirBrowseButton
            // 
            this.toDirBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.toDirBrowseButton.Location = new System.Drawing.Point(376, 20);
            this.toDirBrowseButton.Name = "toDirBrowseButton";
            this.toDirBrowseButton.Size = new System.Drawing.Size(24, 23);
            this.toDirBrowseButton.TabIndex = 1;
            this.toDirBrowseButton.Text = "...";
            this.toDirBrowseButton.Click += new System.EventHandler(this.BrowseClicked);
            // 
            // localDirTextBox
            // 
            this.localDirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.localDirTextBox.Location = new System.Drawing.Point(16, 20);
            this.localDirTextBox.Name = "localDirTextBox";
            this.localDirTextBox.Size = new System.Drawing.Size(352, 20);
            this.localDirTextBox.TabIndex = 0;
            this.localDirTextBox.Text = "";
            this.localDirTextBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            // 
            // nonRecursiveCheckBox
            // 
            this.nonRecursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nonRecursiveCheckBox.Location = new System.Drawing.Point(16, 304);
            this.nonRecursiveCheckBox.Name = "nonRecursiveCheckBox";
            this.nonRecursiveCheckBox.TabIndex = 3;
            this.nonRecursiveCheckBox.Text = "Non-recursive";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(256, 304);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(344, 304);
            this.cancelButton.Name = "cancelButton";
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
            this.radioButtonFromDir.Text = "Export From Local Directory";
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
            this.radioButtonFromURL.Text = "Export From URL";
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
            this.exportFromDirGroupbox.Text = "Export from Local Directory";
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
            this.exportFromDirTextBox.Location = new System.Drawing.Point(16, 20);
            this.exportFromDirTextBox.Name = "exportFromDirTextBox";
            this.exportFromDirTextBox.Size = new System.Drawing.Size(352, 20);
            this.exportFromDirTextBox.TabIndex = 0;
            this.exportFromDirTextBox.Text = "";
            this.exportFromDirTextBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            // 
            // ExportDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
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
            this.localDirGroupBox.ResumeLayout(false);
            this.radioButtonGroupbox.ResumeLayout(false);
            this.exportFromDirGroupbox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Validate the input here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlsChanged(object sender, System.EventArgs e)
        {
            if( this.revisionPicker.Valid && this.localDirTextBox.Text.Length > 0)
            {
                if(this.radioButtonFromURL.Checked)
                    this.okButton.Enabled = UriUtils.ValidUrl.IsMatch( this.urlTextBox.Text ) ;
                else 
                    this.okButton.Enabled=SvnUtils.IsWorkingCopyPath(this.exportFromDirTextBox.Text);
            }
        }

        /// <summary>
        ///   User clicked radio button to export from a dir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonFromDir_CheckedChanged(object sender, System.EventArgs e)
        {
            this.urlGroupBox.Visible = false; 
            this.exportFromDirGroupbox.Visible = true; 
           
        }// <summary>
        ///   User clicked radio button to export from a URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonFromURL_CheckedChanged(object sender, System.EventArgs e)
        {
        
            this.urlGroupBox.Visible = true; 
            this.exportFromDirGroupbox.Visible = false; 
        }

        /// <summary>
        /// Let the user browse for a directory to export from
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportFromDirButton_Click(object sender, System.EventArgs e)
        {
            using( FolderBrowser browser = new FolderBrowser() )
            {
                if ( browser.ShowDialog(this) == DialogResult.OK )
                {
                    this.exportFromDirTextBox.Text = browser.DirectoryPath;
                }
            }
        
        }
                
        /// <summary>
        /// Let the user browse for a directory to export To.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseClicked(object sender, System.EventArgs e)
        {
            using( FolderBrowser browser = new FolderBrowser() )
            {
                if ( browser.ShowDialog(this) == DialogResult.OK )
                {
                    this.localDirTextBox.Text = browser.DirectoryPath;
                }
            }
        }

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
