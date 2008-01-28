// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NSvn.Core;
using Utils.Win32;
using Utils;

namespace Ankh.UI
{
    /// <summary>
    /// Summary description for AddRepositoryDialog.
    /// </summary>
    public class AddRepositoryRootDialog : System.Windows.Forms.Form
    {
        public AddRepositoryRootDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //Set revision choices in combobox
            this.revisionPicker.WorkingEnabled = false;
            this.revisionPicker.BaseEnabled = false;
            this.revisionPicker.CommittedEnabled = false;
            this.revisionPicker.PreviousEnabled = false;

            Win32.SHAutoComplete( this.urlTextBox.Handle, 
                Shacf.UrlAll );

            this.ValidateAdd();
        }

        /// <summary>
        /// The revision selected by the user.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Revision Revision
        {
            get{ return this.revisionPicker.Revision; }
        }

        /// <summary>
        /// The URL entered in the text box.
        /// </summary>
        public string Url
        {
            get
            {
                return this.urlTextBox.Text;
            }
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

        private void revisionPicker_Changed(object sender, System.EventArgs e)
        {
            this.ValidateAdd();            
        }

        private void urlTextBox_TextChanged(object sender, System.EventArgs e)
        {
            this.ValidateAdd();
        }

        private void browseButton_Click(object sender, System.EventArgs e)
        {
            // Browse to a local repository

            FolderBrowser browser = new FolderBrowser();

            //convert the returned directory path to a URL - for a local path URL no need for encoding
            if ( browser.ShowDialog() == DialogResult.OK) 
                urlTextBox.Text ="file:///" +  browser.DirectoryPath.Replace( '\\', '/');

        }

        private void ValidateAdd()
        {
            this.okButton.Enabled = this.revisionPicker.Valid && 
                UriUtils.ValidUrl.IsMatch( this.urlTextBox.Text );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.revisionPicker = new Ankh.UI.RevisionPicker();
            this.urlLabel = new System.Windows.Forms.Label();
            this.revisionLabel = new System.Windows.Forms.Label();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.browseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPicker.Location = new System.Drawing.Point(8, 72);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(344, 20);
            this.revisionPicker.TabIndex = 1;
            this.revisionPicker.Changed += new System.EventHandler(this.revisionPicker_Changed);
            // 
            // urlLabel
            // 
            this.urlLabel.Location = new System.Drawing.Point(8, 8);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(24, 16);
            this.urlLabel.TabIndex = 15;
            this.urlLabel.Text = "Url:";
            // 
            // revisionLabel
            // 
            this.revisionLabel.Location = new System.Drawing.Point(8, 56);
            this.revisionLabel.Name = "revisionLabel";
            this.revisionLabel.Size = new System.Drawing.Size(292, 18);
            this.revisionLabel.TabIndex = 13;
            this.revisionLabel.Text = "Select a revision or manually type the revision number:";
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.urlTextBox.Location = new System.Drawing.Point(8, 24);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(312, 20);
            this.urlTextBox.TabIndex = 0;
            this.urlTextBox.Text = "";
            this.urlTextBox.TextChanged += new System.EventHandler(this.urlTextBox_TextChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(194, 106);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(274, 106);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(328, 24);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(24, 23);
            this.browseButton.TabIndex = 16;
            this.browseButton.Text = "...";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // AddRepositoryRootDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(354, 135);
            this.ControlBox = false;
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.revisionPicker);
            this.Controls.Add(this.urlLabel);
            this.Controls.Add(this.revisionLabel);
            this.Controls.Add(this.urlTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AddRepositoryRootDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add repository URL";
            this.ResumeLayout(false);

        }
        #endregion

        private Ankh.UI.RevisionPicker revisionPicker;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.Label revisionLabel;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button browseButton;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

       
    }
}
