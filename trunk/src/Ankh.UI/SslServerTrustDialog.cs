using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NSvn.Core;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog that queries the user whether to trust a certain certificate.
    /// </summary>
    public class SslServerTrustDialog : System.Windows.Forms.Form
    {     

        public SslServerTrustDialog( )
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.invalidDateImage.Image = icons.Images[1];
            this.serverMismatchImage.Image = icons.Images[1];
            this.caUnknownImage.Image = icons.Images[1];

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Information about the certificate.
        /// </summary>
        public SslServerCertificateInfo CertificateInfo
        {
            get{ return this.info; }
            set
            { 
                this.info = value; 
                this.RefreshCertificateInfo();
            }
        }

        /// <summary>
        /// The problems with the certificate.
        /// </summary>
        public SslFailures Failures
        {
            get{ return this.failures; }
            set
            { 
                this.failures = value;
                this.RefreshFailures();
            }
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SslServerTrustDialog));
			this.caUnknownImage = new System.Windows.Forms.PictureBox();
			this.serverMismatchImage = new System.Windows.Forms.PictureBox();
			this.invalidDateImage = new System.Windows.Forms.PictureBox();
			this.caUnknownLabel = new System.Windows.Forms.Label();
			this.invalidDateLabel = new System.Windows.Forms.Label();
			this.serverMismatchLabel = new System.Windows.Forms.Label();
			this.acceptTemporarilyButton = new System.Windows.Forms.Button();
			this.rejectButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.issuerLabel = new System.Windows.Forms.Label();
			this.fingerprintLabel = new System.Windows.Forms.Label();
			this.validFromLabel = new System.Windows.Forms.Label();
			this.validToLabel = new System.Windows.Forms.Label();
			this.hostnameLabel = new System.Windows.Forms.Label();
			this.saveCredentialsCheckBox = new System.Windows.Forms.CheckBox();
			this.icons = new System.Windows.Forms.ImageList(this.components);
			this.certificateTextBox = new System.Windows.Forms.TextBox();
			this.errorsGroupBox = new System.Windows.Forms.GroupBox();
			this.errorsGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// caUnknownImage
			// 
			this.caUnknownImage.BackColor = System.Drawing.Color.Transparent;
			this.caUnknownImage.Location = new System.Drawing.Point(8, 24);
			this.caUnknownImage.Name = "caUnknownImage";
			this.caUnknownImage.Size = new System.Drawing.Size(20, 16);
			this.caUnknownImage.TabIndex = 1;
			this.caUnknownImage.TabStop = false;
			// 
			// serverMismatchImage
			// 
			this.serverMismatchImage.BackColor = System.Drawing.Color.Transparent;
			this.serverMismatchImage.Location = new System.Drawing.Point(8, 72);
			this.serverMismatchImage.Name = "serverMismatchImage";
			this.serverMismatchImage.Size = new System.Drawing.Size(20, 16);
			this.serverMismatchImage.TabIndex = 2;
			this.serverMismatchImage.TabStop = false;
			// 
			// invalidDateImage
			// 
			this.invalidDateImage.BackColor = System.Drawing.Color.Transparent;
			this.invalidDateImage.Location = new System.Drawing.Point(8, 48);
			this.invalidDateImage.Name = "invalidDateImage";
			this.invalidDateImage.Size = new System.Drawing.Size(20, 16);
			this.invalidDateImage.TabIndex = 3;
			this.invalidDateImage.TabStop = false;
			// 
			// caUnknownLabel
			// 
			this.caUnknownLabel.Location = new System.Drawing.Point(40, 24);
			this.caUnknownLabel.Name = "caUnknownLabel";
			this.caUnknownLabel.Size = new System.Drawing.Size(380, 23);
			this.caUnknownLabel.TabIndex = 6;
			this.caUnknownLabel.Text = "The Certificate Authority(CA) is trusted";
			// 
			// invalidDateLabel
			// 
			this.invalidDateLabel.Location = new System.Drawing.Point(40, 48);
			this.invalidDateLabel.Name = "invalidDateLabel";
			this.invalidDateLabel.Size = new System.Drawing.Size(380, 23);
			this.invalidDateLabel.TabIndex = 7;
			this.invalidDateLabel.Text = "The server certificate has a valid date";
			// 
			// serverMismatchLabel
			// 
			this.serverMismatchLabel.Location = new System.Drawing.Point(40, 72);
			this.serverMismatchLabel.Name = "serverMismatchLabel";
			this.serverMismatchLabel.Size = new System.Drawing.Size(380, 23);
			this.serverMismatchLabel.TabIndex = 8;
			this.serverMismatchLabel.Text = "The certificate\'s hostname matches the server\'s.";
			// 
			// acceptTemporarilyButton
			// 
			this.acceptTemporarilyButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.acceptTemporarilyButton.Location = new System.Drawing.Point(279, 304);
			this.acceptTemporarilyButton.Name = "acceptTemporarilyButton";
			this.acceptTemporarilyButton.TabIndex = 10;
			this.acceptTemporarilyButton.Text = "Accept";
			// 
			// rejectButton
			// 
			this.rejectButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.rejectButton.Location = new System.Drawing.Point(365, 304);
			this.rejectButton.Name = "rejectButton";
			this.rejectButton.TabIndex = 11;
			this.rejectButton.Text = "Reject";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 128);
			this.label3.Name = "label3";
			this.label3.TabIndex = 12;
			this.label3.Text = "Hostname:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 152);
			this.label2.Name = "label2";
			this.label2.TabIndex = 13;
			this.label2.Text = "Fingerprint:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 176);
			this.label1.Name = "label1";
			this.label1.TabIndex = 14;
			this.label1.Text = "Valid from:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 200);
			this.label4.Name = "label4";
			this.label4.TabIndex = 15;
			this.label4.Text = "Valid to:";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 224);
			this.label5.Name = "label5";
			this.label5.TabIndex = 16;
			this.label5.Text = "Issuer:";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 248);
			this.label6.Name = "label6";
			this.label6.TabIndex = 17;
			this.label6.Text = "Certificate:";
			// 
			// issuerLabel
			// 
			this.issuerLabel.Location = new System.Drawing.Point(72, 225);
			this.issuerLabel.Name = "issuerLabel";
			this.issuerLabel.Size = new System.Drawing.Size(368, 23);
			this.issuerLabel.TabIndex = 18;
			this.issuerLabel.Text = "label7";
			// 
			// fingerprintLabel
			// 
			this.fingerprintLabel.Location = new System.Drawing.Point(72, 152);
			this.fingerprintLabel.Name = "fingerprintLabel";
			this.fingerprintLabel.Size = new System.Drawing.Size(368, 23);
			this.fingerprintLabel.TabIndex = 19;
			this.fingerprintLabel.Text = "label8";
			// 
			// validFromLabel
			// 
			this.validFromLabel.Location = new System.Drawing.Point(72, 176);
			this.validFromLabel.Name = "validFromLabel";
			this.validFromLabel.Size = new System.Drawing.Size(368, 23);
			this.validFromLabel.TabIndex = 20;
			this.validFromLabel.Text = "label9";
			// 
			// validToLabel
			// 
			this.validToLabel.Location = new System.Drawing.Point(72, 200);
			this.validToLabel.Name = "validToLabel";
			this.validToLabel.Size = new System.Drawing.Size(368, 23);
			this.validToLabel.TabIndex = 21;
			this.validToLabel.Text = "label10";
			// 
			// hostnameLabel
			// 
			this.hostnameLabel.Location = new System.Drawing.Point(72, 128);
			this.hostnameLabel.Name = "hostnameLabel";
			this.hostnameLabel.Size = new System.Drawing.Size(368, 23);
			this.hostnameLabel.TabIndex = 22;
			this.hostnameLabel.Text = "label11";
			// 
			// saveCredentialsCheckBox
			// 
			this.saveCredentialsCheckBox.Location = new System.Drawing.Point(72, 304);
			this.saveCredentialsCheckBox.Name = "saveCredentialsCheckBox";
			this.saveCredentialsCheckBox.Size = new System.Drawing.Size(152, 24);
			this.saveCredentialsCheckBox.TabIndex = 24;
			this.saveCredentialsCheckBox.Text = "Store credentials?";
			// 
			// icons
			// 
			this.icons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.icons.ImageSize = new System.Drawing.Size(16, 16);
			this.icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("icons.ImageStream")));
			this.icons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// certificateTextBox
			// 
			this.certificateTextBox.BackColor = System.Drawing.SystemColors.Control;
			this.certificateTextBox.Location = new System.Drawing.Point(74, 248);
			this.certificateTextBox.Multiline = true;
			this.certificateTextBox.Name = "certificateTextBox";
			this.certificateTextBox.ReadOnly = true;
			this.certificateTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.certificateTextBox.Size = new System.Drawing.Size(366, 48);
			this.certificateTextBox.TabIndex = 25;
			this.certificateTextBox.Text = "certificateTextBox";
			// 
			// errorsGroupBox
			// 
			this.errorsGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.serverMismatchLabel,
																						 this.invalidDateImage,
																						 this.caUnknownLabel,
																						 this.invalidDateLabel,
																						 this.caUnknownImage,
																						 this.serverMismatchImage});
			this.errorsGroupBox.Location = new System.Drawing.Point(8, 16);
			this.errorsGroupBox.Name = "errorsGroupBox";
			this.errorsGroupBox.Size = new System.Drawing.Size(432, 100);
			this.errorsGroupBox.TabIndex = 26;
			this.errorsGroupBox.TabStop = false;
			this.errorsGroupBox.Text = "There are some problems with this server\'s certificate:";
			// 
			// SslServerTrustDialog
			// 
			this.AcceptButton = this.acceptTemporarilyButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.rejectButton;
			this.ClientSize = new System.Drawing.Size(450, 343);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.errorsGroupBox,
																		  this.certificateTextBox,
																		  this.saveCredentialsCheckBox,
																		  this.hostnameLabel,
																		  this.validToLabel,
																		  this.validFromLabel,
																		  this.fingerprintLabel,
																		  this.issuerLabel,
																		  this.label6,
																		  this.label5,
																		  this.label4,
																		  this.label1,
																		  this.label2,
																		  this.label3,
																		  this.rejectButton,
																		  this.acceptTemporarilyButton});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SslServerTrustDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Security alert";
			this.errorsGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion

        static void Main()
        {
            Application.Run( new SslServerTrustDialog() );
        }

        private void RefreshCertificateInfo()
        {
            this.hostnameLabel.Text = this.info.HostName;
            this.validFromLabel.Text = this.info.ValidFrom;
            this.validToLabel.Text = this.info.ValidUntil;
            this.certificateTextBox.Text = this.info.AsciiCertificate;
            this.fingerprintLabel.Text = this.info.FingerPrint;
            this.issuerLabel.Text = this.info.Issuer;
        }

        #region RefreshFailures
        private void RefreshFailures()
        {
            if ( (this.failures & SslFailures.CertificateAuthorityUnknown) != 0 )
            {
                this.caUnknownLabel.Text = "The issuing certificate authority(CA) is not trusted.";
                this.caUnknownImage.Image = this.icons.Images[FAILUREIMAGE];
            }
            else
            {
                this.caUnknownLabel.Text = 
                    "The issuing certificate authority(CA) is known and trusted.";
                this.caUnknownImage.Image = this.icons.Images[OKIMAGE];
            }

            if ( (this.failures & SslFailures.CertificateNameMismatch) != 0 )
            {
                this.serverMismatchLabel.Text = "The certificate's hostname does not match the " + 
                    "hostname of the server.";
                this.serverMismatchImage.Image = this.icons.Images[FAILUREIMAGE];
            }
            else
            {
                this.serverMismatchLabel.Text =
                    "The certificate's hostname matches the hostname of the server.";
                this.serverMismatchImage.Image = this.icons.Images[OKIMAGE];
            }

            if ( (this.failures & SslFailures.Expired) != 0 )
            {
                this.invalidDateLabel.Text = "The server certificate has expired.";
                this.invalidDateImage.Image = this.icons.Images[FAILUREIMAGE];
            }
            else if ( (this.failures & SslFailures.NotYetValid) != 0 )
            {
                this.invalidDateLabel.Text = "The server certificate is not yet valid.";
                this.invalidDateImage.Image = this.icons.Images[FAILUREIMAGE];
            }
            else
            {
                this.invalidDateLabel.Text = "The server certificate date is valid.";
                this.invalidDateImage.Image = this.icons.Images[OKIMAGE];
            }
        }
        #endregion
        

        #region private data

        private System.Windows.Forms.PictureBox caUnknownImage;
        private System.Windows.Forms.Label caUnknownLabel;
        private System.Windows.Forms.Button acceptTemporarilyButton;
        private System.Windows.Forms.Button rejectButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label issuerLabel;
        private System.Windows.Forms.Label fingerprintLabel;
        private System.Windows.Forms.Label validFromLabel;
        private System.Windows.Forms.Label validToLabel;
        private System.Windows.Forms.Label hostnameLabel;
        private System.ComponentModel.IContainer components;

        private SslServerCertificateInfo info;
        private SslFailures failures;

        private const int FAILUREIMAGE = 0;
        private System.Windows.Forms.Label invalidDateLabel;
        private System.Windows.Forms.Label serverMismatchLabel;
        private System.Windows.Forms.PictureBox serverMismatchImage;
        private System.Windows.Forms.PictureBox invalidDateImage;
        private System.Windows.Forms.CheckBox saveCredentialsCheckBox;
		private System.Windows.Forms.ImageList icons;
		private System.Windows.Forms.TextBox certificateTextBox;
		private System.Windows.Forms.GroupBox errorsGroupBox;
        private const int OKIMAGE = 1;
        #endregion
    }
}
