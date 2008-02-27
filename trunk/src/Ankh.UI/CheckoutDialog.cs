using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Utils;
using Utils.Win32;
using SharpSvn;

namespace Ankh.UI
{
	/// <summary>
	/// A dialog for performing checkouts.
	/// </summary>
	public class CheckoutDialog : System.Windows.Forms.Form
	{
		public CheckoutDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();	

            Win32.SHAutoComplete( this.urlTextBox.Handle, 
                Shacf.UrlAll );
            Win32.SHAutoComplete( this.localDirTextBox.Handle, 
                Shacf.Filesystem );
	
            this.ControlsChanged( this, EventArgs.Empty );
		}

        /// <summary>
        /// The URL of the repository.
        /// </summary>
        public string Url
        {
            get{ return this.urlTextBox.Text; }
            set{ this.urlTextBox.Text = value; }
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision Revision
        {
            get{ return this.revisionPicker.Revision; }
        }

        /// <summary>
        /// Whether to perform a non-recursive checkout.
        /// </summary>
        public bool Recursive
        {
            get{ return !this.nonRecursiveCheckBox.Checked; }
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
            this.urlGroupBox = new System.Windows.Forms.GroupBox();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.localDirGroupBox = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.localDirTextBox = new System.Windows.Forms.TextBox();
            this.nonRecursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.revisionPicker = new Ankh.UI.RevisionPicker();
            this.revisionGroupBox.SuspendLayout();
            this.urlGroupBox.SuspendLayout();
            this.localDirGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // revisionGroupBox
            // 
            this.revisionGroupBox.Controls.Add( this.revisionPicker );
            this.revisionGroupBox.Location = new System.Drawing.Point( 8, 16 );
            this.revisionGroupBox.Name = "revisionGroupBox";
            this.revisionGroupBox.Size = new System.Drawing.Size( 416, 64 );
            this.revisionGroupBox.TabIndex = 0;
            this.revisionGroupBox.TabStop = false;
            this.revisionGroupBox.Text = "&Revision";
            // 
            // urlGroupBox
            // 
            this.urlGroupBox.Controls.Add( this.urlTextBox );
            this.urlGroupBox.Location = new System.Drawing.Point( 8, 88 );
            this.urlGroupBox.Name = "urlGroupBox";
            this.urlGroupBox.Size = new System.Drawing.Size( 416, 64 );
            this.urlGroupBox.TabIndex = 1;
            this.urlGroupBox.TabStop = false;
            this.urlGroupBox.Text = "&URL";
            this.urlGroupBox.TextChanged += new System.EventHandler( this.ControlsChanged );
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.urlTextBox.Location = new System.Drawing.Point( 16, 24 );
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size( 384, 20 );
            this.urlTextBox.TabIndex = 0;
            // 
            // localDirGroupBox
            // 
            this.localDirGroupBox.Controls.Add( this.button1 );
            this.localDirGroupBox.Controls.Add( this.localDirTextBox );
            this.localDirGroupBox.Location = new System.Drawing.Point( 8, 160 );
            this.localDirGroupBox.Name = "localDirGroupBox";
            this.localDirGroupBox.Size = new System.Drawing.Size( 416, 80 );
            this.localDirGroupBox.TabIndex = 2;
            this.localDirGroupBox.TabStop = false;
            this.localDirGroupBox.Text = "Local &Directory";
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button1.Location = new System.Drawing.Point( 384, 32 );
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size( 24, 23 );
            this.button1.TabIndex = 1;
            this.button1.Text = "&...";
            this.button1.Click += new System.EventHandler( this.BrowseClicked );
            // 
            // localDirTextBox
            // 
            this.localDirTextBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.localDirTextBox.Location = new System.Drawing.Point( 16, 32 );
            this.localDirTextBox.Name = "localDirTextBox";
            this.localDirTextBox.Size = new System.Drawing.Size( 360, 20 );
            this.localDirTextBox.TabIndex = 0;
            this.localDirTextBox.TextChanged += new System.EventHandler( this.ControlsChanged );
            // 
            // nonRecursiveCheckBox
            // 
            this.nonRecursiveCheckBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.nonRecursiveCheckBox.Location = new System.Drawing.Point( 8, 256 );
            this.nonRecursiveCheckBox.Name = "nonRecursiveCheckBox";
            this.nonRecursiveCheckBox.Size = new System.Drawing.Size( 104, 24 );
            this.nonRecursiveCheckBox.TabIndex = 3;
            this.nonRecursiveCheckBox.Text = "&Non-recursive";
            // 
            // okButton
            // 
            this.okButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point( 256, 280 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 4;
            this.okButton.Text = "Chec&kout";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point( 344, 280 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "&Cancel";
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.revisionPicker.Location = new System.Drawing.Point( 16, 28 );
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size( 392, 20 );
            this.revisionPicker.TabIndex = 0;
            this.revisionPicker.Changed += new System.EventHandler( this.ControlsChanged );
            // 
            // CheckoutDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size( 432, 317 );
            this.ControlBox = false;
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.okButton );
            this.Controls.Add( this.nonRecursiveCheckBox );
            this.Controls.Add( this.localDirGroupBox );
            this.Controls.Add( this.urlGroupBox );
            this.Controls.Add( this.revisionGroupBox );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CheckoutDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Checkout";
            this.revisionGroupBox.ResumeLayout( false );
            this.urlGroupBox.ResumeLayout( false );
            this.urlGroupBox.PerformLayout();
            this.localDirGroupBox.ResumeLayout( false );
            this.localDirGroupBox.PerformLayout();
            this.ResumeLayout( false );

        }
		#endregion

        private System.Windows.Forms.GroupBox revisionGroupBox;
        private Ankh.UI.RevisionPicker revisionPicker;
        private System.Windows.Forms.GroupBox urlGroupBox;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.GroupBox localDirGroupBox;
        private System.Windows.Forms.TextBox localDirTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox nonRecursiveCheckBox;
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
            this.okButton.Enabled = this.revisionPicker.Valid &&
                UriUtils.ValidUrl.IsMatch( this.urlTextBox.Text ) &&
                this.localDirTextBox.Text.Length > 0;
        }

        /// <summary>
        /// Let the user browse for a directory.
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
	}
}
