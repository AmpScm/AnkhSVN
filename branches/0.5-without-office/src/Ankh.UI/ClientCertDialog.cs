using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace Ankh.UI
{
	/// <summary>
	/// A dialog that prompts the user for a client certificate.
	/// </summary>
	public class ClientCertDialog : System.Windows.Forms.Form
	{
        public ClientCertDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

        /// <summary>
        /// The path to the certificate file.
        /// </summary>
        public string CertificateFile
        {
            get{ return this.clientCertTextBox.Text; }
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
        /// The realm for which this cert applies.
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.clientCertTextBox = new System.Windows.Forms.TextBox();
            this.pathWarningLabel = new System.Windows.Forms.Label();
            this.saveCredentialsCheckBox = new System.Windows.Forms.CheckBox();
            this.realmLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(192, 144);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(280, 144);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 23);
            this.label1.TabIndex = 2;
            this.label1.Text = "Supply a client certificate:";
            // 
            // browseButton
            // 
            this.browseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.browseButton.Location = new System.Drawing.Point(336, 64);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(22, 23);
            this.browseButton.TabIndex = 3;
            this.browseButton.Text = "...";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // clientCertTextBox
            // 
            this.clientCertTextBox.Location = new System.Drawing.Point(8, 88);
            this.clientCertTextBox.Name = "clientCertTextBox";
            this.clientCertTextBox.Size = new System.Drawing.Size(320, 20);
            this.clientCertTextBox.TabIndex = 4;
            this.clientCertTextBox.Text = "";
            this.clientCertTextBox.TextChanged += new System.EventHandler(this.clientCertTextBox_TextChanged);
            // 
            // pathWarningLabel
            // 
            this.pathWarningLabel.ForeColor = System.Drawing.Color.Red;
            this.pathWarningLabel.Location = new System.Drawing.Point(16, 144);
            this.pathWarningLabel.Name = "pathWarningLabel";
            this.pathWarningLabel.TabIndex = 5;
            this.pathWarningLabel.Text = "Invalid path";
            // 
            // saveCredentialsCheckBox
            // 
            this.saveCredentialsCheckBox.Location = new System.Drawing.Point(8, 112);
            this.saveCredentialsCheckBox.Name = "saveCredentialsCheckBox";
            this.saveCredentialsCheckBox.Size = new System.Drawing.Size(136, 24);
            this.saveCredentialsCheckBox.TabIndex = 6;
            this.saveCredentialsCheckBox.Text = "Store credentials?";
            // 
            // realmLabel
            // 
            this.realmLabel.Location = new System.Drawing.Point(72, 32);
            this.realmLabel.Name = "realmLabel";
            this.realmLabel.Size = new System.Drawing.Size(256, 32);
            this.realmLabel.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 23);
            this.label2.TabIndex = 10;
            this.label2.Text = "Realm:";
            // 
            // ClientCertDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(370, 175);
            this.Controls.Add(this.realmLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.saveCredentialsCheckBox);
            this.Controls.Add(this.pathWarningLabel);
            this.Controls.Add(this.clientCertTextBox);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClientCertDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client certificate required";
            this.ResumeLayout(false);

        }
		#endregion

        private void clientCertTextBox_TextChanged(object sender, System.EventArgs e)
        {
            this.pathWarningLabel.Visible = !(this.okButton.Enabled = 
                File.Exists( this.clientCertTextBox.Text ));
        }

        private void browseButton_Click(object sender, System.EventArgs e)
        {
            using( OpenFileDialog dialog = new OpenFileDialog() )
            {
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;
                dialog.Filter = "All files(*.*)|*.*";
                if ( dialog.ShowDialog(this) == DialogResult.OK )
                    this.clientCertTextBox.Text = dialog.FileName;
            }        
        }

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox clientCertTextBox;
        private System.Windows.Forms.Label pathWarningLabel;
        private System.Windows.Forms.CheckBox saveCredentialsCheckBox;
        private System.Windows.Forms.Label realmLabel;
        private System.Windows.Forms.Label label2;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public static void Main()
        {
            new ClientCertDialog().ShowDialog();
        }

        
	}
}
