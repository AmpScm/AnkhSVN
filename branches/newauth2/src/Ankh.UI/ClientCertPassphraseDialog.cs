using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// This is a dialog that prompts the user for a passphrase for a client certificate.
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ClientCertPassphraseDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.passphraseTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter the passphrase for the client certificate:";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(96, 88);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(184, 88);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            // 
            // passphraseTextBox
            // 
            this.passphraseTextBox.Location = new System.Drawing.Point(8, 48);
            this.passphraseTextBox.Name = "passphraseTextBox";
            this.passphraseTextBox.Size = new System.Drawing.Size(240, 20);
            this.passphraseTextBox.TabIndex = 3;
            this.passphraseTextBox.Text = "";
            // 
            // ClientCertPassphraseDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(272, 117);
            this.Controls.Add(this.passphraseTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClientCertPassphraseDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Passphrase";
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox passphraseTextBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
    }
}
