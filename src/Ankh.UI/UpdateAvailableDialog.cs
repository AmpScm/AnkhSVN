using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Summary description for UpdateAvailableDialog.
	/// </summary>
	public class UpdateAvailableDialog : System.Windows.Forms.Form
	{
		public System.Windows.Forms.Label headLabel;
		public System.Windows.Forms.Label bodyLabel;
		public System.Windows.Forms.LinkLabel linkLabel;
		public System.Windows.Forms.CheckBox sameCheck;
		public System.Windows.Forms.Panel versionPanel;
		public System.Windows.Forms.Label newVerLabel;
		public System.Windows.Forms.Label curVerLabel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public UpdateAvailableDialog()
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
			this.headLabel = new System.Windows.Forms.Label();
			this.bodyLabel = new System.Windows.Forms.Label();
			this.linkLabel = new System.Windows.Forms.LinkLabel();
			this.sameCheck = new System.Windows.Forms.CheckBox();
			this.versionPanel = new System.Windows.Forms.Panel();
			this.newVerLabel = new System.Windows.Forms.Label();
			this.curVerLabel = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.versionPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// headLabel
			// 
			this.headLabel.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.headLabel.BackColor = System.Drawing.SystemColors.Window;
			this.headLabel.Location = new System.Drawing.Point(12, 9);
			this.headLabel.Name = "headLabel";
			this.headLabel.Size = new System.Drawing.Size(387, 30);
			this.headLabel.TabIndex = 0;
			this.headLabel.Text = "headLabel";
			this.headLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// bodyLabel
			// 
			this.bodyLabel.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.bodyLabel.BackColor = System.Drawing.SystemColors.Window;
			this.bodyLabel.Location = new System.Drawing.Point(12, 39);
			this.bodyLabel.Name = "bodyLabel";
			this.bodyLabel.Size = new System.Drawing.Size(396, 76);
			this.bodyLabel.TabIndex = 1;
			this.bodyLabel.Text = "bodyLabel";
			// 
			// linkLabel
			// 
			this.linkLabel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.linkLabel.BackColor = System.Drawing.SystemColors.Window;
			this.linkLabel.Location = new System.Drawing.Point(12, 166);
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.Size = new System.Drawing.Size(387, 23);
			this.linkLabel.TabIndex = 2;
			this.linkLabel.TabStop = true;
			this.linkLabel.Text = "linkLabel";
			// 
			// sameCheck
			// 
			this.sameCheck.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.sameCheck.Location = new System.Drawing.Point(12, 214);
			this.sameCheck.Name = "sameCheck";
			this.sameCheck.Size = new System.Drawing.Size(180, 17);
			this.sameCheck.TabIndex = 3;
			this.sameCheck.Text = "&Don\'t show this update again";
			// 
			// versionPanel
			// 
			this.versionPanel.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.versionPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.label2,
																					   this.label1,
																					   this.newVerLabel,
																					   this.curVerLabel});
			this.versionPanel.Location = new System.Drawing.Point(15, 118);
			this.versionPanel.Name = "versionPanel";
			this.versionPanel.Size = new System.Drawing.Size(384, 45);
			this.versionPanel.TabIndex = 4;
			// 
			// newVerLabel
			// 
			this.newVerLabel.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.newVerLabel.Location = new System.Drawing.Point(109, 23);
			this.newVerLabel.Name = "newVerLabel";
			this.newVerLabel.Size = new System.Drawing.Size(275, 23);
			this.newVerLabel.TabIndex = 5;
			this.newVerLabel.Text = "newVerLabel";
			// 
			// curVerLabel
			// 
			this.curVerLabel.Location = new System.Drawing.Point(109, 0);
			this.curVerLabel.Name = "curVerLabel";
			this.curVerLabel.Size = new System.Drawing.Size(275, 23);
			this.curVerLabel.TabIndex = 6;
			this.curVerLabel.Text = "curVerLabel";
			// 
			// panel1
			// 
			this.panel1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.versionPanel});
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(411, 199);
			this.panel1.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Current version:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(-3, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Updated version:";
			// 
			// button1
			// 
			this.button1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(324, 210);
			this.button1.Name = "button1";
			this.button1.TabIndex = 9;
			this.button1.Text = "OK";
			// 
			// UpdateAvailableDialog
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(411, 245);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.sameCheck,
																		  this.linkLabel,
																		  this.bodyLabel,
																		  this.headLabel,
																		  this.panel1,
																		  this.button1});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdateAvailableDialog";
			this.ShowInTaskbar = false;
			this.Text = "AnkhSVN - {0}";
			this.versionPanel.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
