using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Summary description for ConflictDialog.
	/// </summary>
	public class ConflictDialog : System.Windows.Forms.Form
	{
        
		public ConflictDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.mineFileRadioButton.Checked = true;
		}

        public string Filename
        {
            get{ return this.filename; }
            set
            { 
                this.filename = value;
                this.mineFileRadioButton.Text = this.filename + " with conflict markers";
                this.fileRadioButton.Text = this.filename + " from revision ";
                this.fileRadioButton.Text = this.filename + " from revision ";
                this.fileRadioButton.Text = this.filename;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.mineFileRadioButton = new System.Windows.Forms.RadioButton();
            this.lastRevRadioButton = new System.Windows.Forms.RadioButton();
            this.thisRevRadioButton = new System.Windows.Forms.RadioButton();
            this.fileRadioButton = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.viewButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mineFileRadioButton
            // 
            this.mineFileRadioButton.Location = new System.Drawing.Point(16, 8);
            this.mineFileRadioButton.Name = "mineFileRadioButton";
            this.mineFileRadioButton.TabIndex = 0;
            this.mineFileRadioButton.Text = "test.mine";
            // 
            // lastRevRadioButton
            // 
            this.lastRevRadioButton.Location = new System.Drawing.Point(16, 32);
            this.lastRevRadioButton.Name = "lastRevRadioButton";
            this.lastRevRadioButton.TabIndex = 1;
            this.lastRevRadioButton.Text = "test.r1";
            // 
            // thisRevRadioButton
            // 
            this.thisRevRadioButton.Location = new System.Drawing.Point(16, 56);
            this.thisRevRadioButton.Name = "thisRevRadioButton";
            this.thisRevRadioButton.TabIndex = 2;
            this.thisRevRadioButton.Text = "test.r2";
            // 
            // fileRadioButton
            // 
            this.fileRadioButton.Location = new System.Drawing.Point(16, 80);
            this.fileRadioButton.Name = "fileRadioButton";
            this.fileRadioButton.TabIndex = 3;
            this.fileRadioButton.Text = "test.txt";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(215, 122);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            // 
            // viewButton
            // 
            this.viewButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.viewButton.Location = new System.Drawing.Point(55, 122);
            this.viewButton.Name = "viewButton";
            this.viewButton.TabIndex = 6;
            this.viewButton.Text = "View";
            this.viewButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(135, 122);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 7;
            this.okButton.Text = "Ok";
            // 
            // ConflictDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 149);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.okButton,
                                                                          this.viewButton,
                                                                          this.cancelButton,
                                                                          this.fileRadioButton,
                                                                          this.thisRevRadioButton,
                                                                          this.lastRevRadioButton,
                                                                          this.mineFileRadioButton});
            this.Name = "ConflictDialog";
            this.Text = "Conflict";
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.RadioButton mineFileRadioButton;
        private System.Windows.Forms.RadioButton lastRevRadioButton;
        private System.Windows.Forms.RadioButton thisRevRadioButton;
        private System.Windows.Forms.RadioButton fileRadioButton;

        private string filename;
        private System.Windows.Forms.Button viewButton;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        

	}

}
