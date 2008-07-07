#region Copyright And Revision History

/*---------------------------------------------------------------------------

	GoToDlg.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.27.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.Diff.DiffUtils.Controls
{
	/// <summary>
	/// Summary description for GoToDlg.
	/// </summary>
	public class GoToDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblLineNumber;
		private Ankh.Diff.NumericTextBox edtLineNumber;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GoToDlg()
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.lblLineNumber = new System.Windows.Forms.Label();
            this.edtLineNumber = new Ankh.Diff.NumericTextBox();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(76, 60);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(160, 60);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			// 
			// lblLineNumber
			// 
			this.lblLineNumber.AutoSize = true;
			this.lblLineNumber.Location = new System.Drawing.Point(8, 8);
			this.lblLineNumber.Name = "lblLineNumber";
			this.lblLineNumber.Size = new System.Drawing.Size(101, 13);
			this.lblLineNumber.TabIndex = 0;
			this.lblLineNumber.Text = "&Line Number (1-N):";
			// 
			// edtLineNumber
			// 
			this.edtLineNumber.AllowFloat = false;
			this.edtLineNumber.AllowNegative = false;
			this.edtLineNumber.Location = new System.Drawing.Point(8, 28);
			this.edtLineNumber.MaxValue = 100;
			this.edtLineNumber.MinValue = 1;
			this.edtLineNumber.Name = "edtLineNumber";
			this.edtLineNumber.Size = new System.Drawing.Size(228, 20);
			this.edtLineNumber.TabIndex = 1;
			this.edtLineNumber.Value = 1;
			// 
			// GoToDlg
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(246, 93);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.edtLineNumber,
																		  this.lblLineNumber,
																		  this.btnCancel,
																		  this.btnOK});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GoToDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Go To Line";
			this.ResumeLayout(false);

		}
		#endregion

		public bool Execute(IWin32Window owner, int iMaxLineNumber, out int iLine)
		{
			lblLineNumber.Text = String.Format("&Line Number (1-{0}):", iMaxLineNumber);
			edtLineNumber.MaxValue = iMaxLineNumber;
			if (ShowDialog(owner) == DialogResult.OK)
			{
				iLine = edtLineNumber.IntValue;
				return true;
			}

			iLine = 0;
			return false;
		}
	}
}
