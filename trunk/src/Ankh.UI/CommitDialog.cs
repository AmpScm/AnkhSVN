// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Summary description for CommitDialog.
	/// </summary>
	public class CommitDialog : System.Windows.Forms.Form
	{
        
		public CommitDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			
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
            this.fileTreeView = new System.Windows.Forms.TreeView();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.logMessageControl1 = new Ankh.UI.LogMessageControl();
            this.logLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // fileTreeView
            // 
            this.fileTreeView.Dock = System.Windows.Forms.DockStyle.Top;
            this.fileTreeView.ImageIndex = -1;
            this.fileTreeView.Name = "fileTreeView";
            this.fileTreeView.SelectedImageIndex = -1;
            this.fileTreeView.Size = new System.Drawing.Size(512, 152);
            this.fileTreeView.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(436, 316);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(356, 316);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Ok";
            // 
            // logMessageControl1
            // 
            this.logMessageControl1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.logMessageControl1.Location = new System.Drawing.Point(0, 184);
            this.logMessageControl1.Name = "logMessageControl1";
            this.logMessageControl1.Size = new System.Drawing.Size(508, 128);
            this.logMessageControl1.TabIndex = 3;
            // 
            // logLabel
            // 
            this.logLabel.Location = new System.Drawing.Point(0, 160);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(296, 23);
            this.logLabel.TabIndex = 4;
            this.logLabel.Text = "Write log message:";
            // 
            // CommitDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(512, 341);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.logLabel,
                                                                          this.logMessageControl1,
                                                                          this.okButton,
                                                                          this.cancelButton,
                                                                          this.fileTreeView});
            this.MinimumSize = new System.Drawing.Size(390, 350);
            this.Name = "CommitDialog";
            this.Text = "Commit";
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Ankh.UI.LogMessageControl logMessageControl1;
        private System.Windows.Forms.Label logLabel;
        private System.Windows.Forms.TreeView fileTreeView;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

	}
}


