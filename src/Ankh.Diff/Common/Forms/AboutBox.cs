#region Copyright And Revision History

/*---------------------------------------------------------------------------

	AboutBox.cs
	Copyright © 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	4.18.2003	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.Diff
{
	internal sealed class AboutBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox picIcon;
		private System.Windows.Forms.PictureBox picTop;
		private System.Windows.Forms.Label lblProductName;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel lnkWeb;
		private System.Windows.Forms.LinkLabel lnkEmail;
		private System.Windows.Forms.GroupBox grpSeparator;
		private GroupBox grpSeparatorTop;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			VisualStyles.EnableControls(this);
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
			this.btnOK = new System.Windows.Forms.Button();
			this.picIcon = new System.Windows.Forms.PictureBox();
			this.picTop = new System.Windows.Forms.PictureBox();
			this.lblProductName = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.lnkWeb = new System.Windows.Forms.LinkLabel();
			this.grpSeparator = new System.Windows.Forms.GroupBox();
			this.lnkEmail = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.grpSeparatorTop = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTop)).BeginInit();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(332, 240);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			// 
			// picIcon
			// 
			this.picIcon.Location = new System.Drawing.Point(40, 80);
			this.picIcon.Name = "picIcon";
			this.picIcon.Size = new System.Drawing.Size(32, 32);
			this.picIcon.TabIndex = 1;
			this.picIcon.TabStop = false;
			// 
			// picTop
			// 
			this.picTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.picTop.Image = global::Ankh.Diff.Properties.Resources.Menees;
			this.picTop.Location = new System.Drawing.Point(0, 0);
			this.picTop.Name = "picTop";
			this.picTop.Size = new System.Drawing.Size(418, 60);
			this.picTop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picTop.TabIndex = 2;
			this.picTop.TabStop = false;
			// 
			// lblProductName
			// 
			this.lblProductName.AutoSize = true;
			this.lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblProductName.Location = new System.Drawing.Point(108, 72);
			this.lblProductName.Name = "lblProductName";
			this.lblProductName.Size = new System.Drawing.Size(200, 28);
			this.lblProductName.TabIndex = 1;
			this.lblProductName.Text = "Product Name";
			// 
			// lblVersion
			// 
			this.lblVersion.AutoSize = true;
			this.lblVersion.Location = new System.Drawing.Point(108, 104);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(60, 13);
			this.lblVersion.TabIndex = 2;
			this.lblVersion.Text = "Version 1.0";
			// 
			// lblCopyright
			// 
			this.lblCopyright.AutoSize = true;
			this.lblCopyright.Location = new System.Drawing.Point(108, 128);
			this.lblCopyright.Name = "lblCopyright";
			this.lblCopyright.Size = new System.Drawing.Size(174, 13);
			this.lblCopyright.TabIndex = 3;
			this.lblCopyright.Text = "Copyright © 2002-2003 Bill Menees";
			// 
			// lnkWeb
			// 
			this.lnkWeb.AutoSize = true;
			this.lnkWeb.Location = new System.Drawing.Point(108, 152);
			this.lnkWeb.Name = "lnkWeb";
			this.lnkWeb.Size = new System.Drawing.Size(94, 13);
			this.lnkWeb.TabIndex = 4;
			this.lnkWeb.TabStop = true;
			this.lnkWeb.Text = "www.menees.com";
			this.lnkWeb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWeb_LinkClicked);
			// 
			// grpSeparator
			// 
			this.grpSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.grpSeparator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grpSeparator.Location = new System.Drawing.Point(-8, 224);
			this.grpSeparator.Name = "grpSeparator";
			this.grpSeparator.Size = new System.Drawing.Size(428, 4);
			this.grpSeparator.TabIndex = 7;
			this.grpSeparator.TabStop = false;
			// 
			// lnkEmail
			// 
			this.lnkEmail.AutoSize = true;
			this.lnkEmail.Location = new System.Drawing.Point(216, 152);
			this.lnkEmail.Name = "lnkEmail";
			this.lnkEmail.Size = new System.Drawing.Size(90, 13);
			this.lnkEmail.TabIndex = 5;
			this.lnkEmail.TabStop = true;
			this.lnkEmail.Text = "bill@menees.com";
			this.lnkEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkEmail_LinkClicked);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(108, 180);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(284, 36);
			this.label1.TabIndex = 6;
			this.label1.Text = "This software is CharityWare.  If you use it, please donate at least $5 (US) to t" +
				"he charity of your choice.";
			// 
			// grpSeparatorTop
			// 
			this.grpSeparatorTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.grpSeparatorTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grpSeparatorTop.Location = new System.Drawing.Point(-8, 58);
			this.grpSeparatorTop.Name = "grpSeparatorTop";
			this.grpSeparatorTop.Size = new System.Drawing.Size(428, 4);
			this.grpSeparatorTop.TabIndex = 8;
			this.grpSeparatorTop.TabStop = false;
			// 
			// AboutBox
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(418, 275);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lnkEmail);
			this.Controls.Add(this.grpSeparator);
			this.Controls.Add(this.lnkWeb);
			this.Controls.Add(this.lblCopyright);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.lblProductName);
			this.Controls.Add(this.picTop);
			this.Controls.Add(this.picIcon);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.grpSeparatorTop);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About";
			((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTop)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		public void Execute(Form frmOwner, string strName, string strVersion)
		{
			Text = "About " + strName;
			lblProductName.Text = strName;
			lblVersion.Text = "Version " + strVersion + (IntPtr.Size == 8 ? " – 64-bit" : " – 32-bit");
			lblCopyright.Text = String.Format("Copyright © 2002-{0} Bill Menees", DateTime.Now.Year);
			picIcon.Image = frmOwner.Icon.ToBitmap();

			//This is important for tray icon apps where the main form may be hidden.
			if (!frmOwner.Visible)
			{
				StartPosition = FormStartPosition.CenterScreen;
			}

			ShowDialog(frmOwner);
		}

		private void lnkWeb_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			Utilities.ShellExecute(this, "http://www.menees.com", "");
			lnkWeb.Links[0].Visited = true;
		}

		private void lnkEmail_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			string strLink = String.Format("mailto:Bill@Menees.com?subject={0} {1}", lblProductName.Text, lblVersion.Text);
			strLink = strLink.Replace(" ", "%20");
			Utilities.ShellExecute(this, strLink, "");
			lnkEmail.Links[0].Visited = true;
		}
	}
}
