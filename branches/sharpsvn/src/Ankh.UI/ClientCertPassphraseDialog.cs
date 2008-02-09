using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Summary description for ClientCertPassphraseDialog.
	/// </summary>
	public class ClientCertPassphraseDialog : System.Windows.Forms.Form
	{
		

		public ClientCertPassphraseDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

        public string Passphrase
        {
            get{ return this.passphraseTextBox.Text; }
        }

        /// <summary>
        /// Whether the user is allowed to save the credentials.
        /// </summary>
        public bool MaySave
        {
            get{ return this.saveCredentialsCheckBox.Enabled; }
            set{ this.saveCredentialsCheckBox.Enabled = value; }
        }

        /// <summary>
        /// Whether the user wants to save the credentials entered.
        /// </summary>
        public bool ShallSave
        {
            get{ return this.saveCredentialsCheckBox.Checked && this.MaySave; }
            set{ this.saveCredentialsCheckBox.Checked = value; }
        }

        /// <summary>
        /// The realm for which this passphrase applies.
        /// </summary>
        public string Realm
        {
            get{ return this.realmLabel.Text; }
            set{ this.realmLabel.Text = value; }
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.passphraseTextBox = new System.Windows.Forms.TextBox();
            this.realmLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.saveCredentialsCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.phraseLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.groupBox1.Controls.Add( this.phraseLabel );
            this.groupBox1.Controls.Add( this.passphraseTextBox );
            this.groupBox1.Controls.Add( this.realmLabel );
            this.groupBox1.Controls.Add( this.label1 );
            this.groupBox1.Location = new System.Drawing.Point( 16, 16 );
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size( 280, 125 );
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Passphrase";
            // 
            // passphraseTextBox
            // 
            this.passphraseTextBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.passphraseTextBox.Location = new System.Drawing.Point( 59, 93 );
            this.passphraseTextBox.Name = "passphraseTextBox";
            this.passphraseTextBox.Size = new System.Drawing.Size( 205, 20 );
            this.passphraseTextBox.TabIndex = 2;
            // 
            // realmLabel
            // 
            this.realmLabel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.realmLabel.Location = new System.Drawing.Point( 56, 21 );
            this.realmLabel.Name = "realmLabel";
            this.realmLabel.Size = new System.Drawing.Size( 208, 53 );
            this.realmLabel.TabIndex = 1;
            this.realmLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.label1.Location = new System.Drawing.Point( 8, 32 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 40, 29 );
            this.label1.TabIndex = 0;
            this.label1.Text = "Realm:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveCredentialsCheckBox
            // 
            this.saveCredentialsCheckBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.saveCredentialsCheckBox.Location = new System.Drawing.Point( 13, 150 );
            this.saveCredentialsCheckBox.Name = "saveCredentialsCheckBox";
            this.saveCredentialsCheckBox.Size = new System.Drawing.Size( 120, 24 );
            this.saveCredentialsCheckBox.TabIndex = 1;
            this.saveCredentialsCheckBox.Text = "&Store credentials";
            // 
            // okButton
            // 
            this.okButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point( 133, 150 );
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size( 75, 23 );
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point( 221, 150 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            // 
            // phraseLabel
            // 
            this.phraseLabel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.phraseLabel.AutoSize = true;
            this.phraseLabel.Location = new System.Drawing.Point( 8, 96 );
            this.phraseLabel.Name = "phraseLabel";
            this.phraseLabel.Size = new System.Drawing.Size( 40, 13 );
            this.phraseLabel.TabIndex = 4;
            this.phraseLabel.Text = "&Phrase";
            // 
            // ClientCertPassphraseDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size( 312, 182 );
            this.ControlBox = false;
            this.Controls.Add( this.cancelButton );
            this.Controls.Add( this.okButton );
            this.Controls.Add( this.saveCredentialsCheckBox );
            this.Controls.Add( this.groupBox1 );
            this.MinimumSize = new System.Drawing.Size( 320, 216 );
            this.Name = "ClientCertPassphraseDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Client certificate passphrase";
            this.groupBox1.ResumeLayout( false );
            this.groupBox1.PerformLayout();
            this.ResumeLayout( false );

        }
		#endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label realmLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox passphraseTextBox;
        private System.Windows.Forms.CheckBox saveCredentialsCheckBox;
        private Label phraseLabel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
	}
}
