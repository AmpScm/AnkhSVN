// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Utils.Win32;
using Utils;
using SharpSvn;

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
        public SvnRevision Revision
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
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.browseButton = new System.Windows.Forms.Button();
            this.revisionGroupBox = new System.Windows.Forms.GroupBox();
            this.urlGroupBox = new System.Windows.Forms.GroupBox();
            this.revisionGroupBox.SuspendLayout();
            this.urlGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.revisionPicker.Location = new System.Drawing.Point( 6, 28 );
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size( 380, 20 );
            this.revisionPicker.TabIndex = 0;
            this.revisionPicker.Changed += new System.EventHandler( this.revisionPicker_Changed );
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.urlTextBox.Location = new System.Drawing.Point( 6, 19 );
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size( 350, 20 );
            this.urlTextBox.TabIndex = 0;
            this.urlTextBox.TextChanged += new System.EventHandler( this.urlTextBox_TextChanged );
            // 
            // okButton
            // 
            this.okButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point( 252, 147 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 2;
            this.okButton.Text = "&Add";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point( 332, 147 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "&Cancel";
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.browseButton.Location = new System.Drawing.Point( 362, 17 );
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size( 24, 23 );
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "&...";
            this.browseButton.Click += new System.EventHandler( this.browseButton_Click );
            // 
            // revisionGroupBox
            // 
            this.revisionGroupBox.Controls.Add( this.revisionPicker );
            this.revisionGroupBox.Location = new System.Drawing.Point( 8, 77 );
            this.revisionGroupBox.Name = "revisionGroupBox";
            this.revisionGroupBox.Size = new System.Drawing.Size( 392, 60 );
            this.revisionGroupBox.TabIndex = 1;
            this.revisionGroupBox.TabStop = false;
            this.revisionGroupBox.Text = "&Revision";
            // 
            // urlGroupBox
            // 
            this.urlGroupBox.Controls.Add( this.urlTextBox );
            this.urlGroupBox.Controls.Add( this.browseButton );
            this.urlGroupBox.Location = new System.Drawing.Point( 8, 12 );
            this.urlGroupBox.Name = "urlGroupBox";
            this.urlGroupBox.Size = new System.Drawing.Size( 392, 59 );
            this.urlGroupBox.TabIndex = 0;
            this.urlGroupBox.TabStop = false;
            this.urlGroupBox.Text = "&Url";
            // 
            // AddRepositoryRootDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size( 412, 176 );
            this.ControlBox = false;
            this.Controls.Add( this.urlGroupBox );
            this.Controls.Add( this.revisionGroupBox );
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.okButton );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AddRepositoryRootDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add repository URL";
            this.revisionGroupBox.ResumeLayout( false );
            this.urlGroupBox.ResumeLayout( false );
            this.urlGroupBox.PerformLayout();
            this.ResumeLayout( false );

        }
        #endregion

        private Ankh.UI.RevisionPicker revisionPicker;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button browseButton;
        private GroupBox revisionGroupBox;
        private GroupBox urlGroupBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

       
    }
}
