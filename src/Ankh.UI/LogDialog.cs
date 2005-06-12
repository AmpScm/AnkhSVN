using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ankh.UI
{
	public class LogDialog : Ankh.UI.PathSelector
	{
        private System.Windows.Forms.CheckBox stopOnCopyCheckBox;
		private System.ComponentModel.IContainer components = null;

		public LogDialog()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

        /// <summary>
        /// Whether to stop on a copy.
        /// </summary>
        public bool StopOnCopy
        {
            get{ return this.stopOnCopyCheckBox.Checked; }
            set{ this.stopOnCopyCheckBox.Checked = value; }
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.stopOnCopyCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // stopOnCopyCheckBox
            // 
            this.stopOnCopyCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.stopOnCopyCheckBox.Location = new System.Drawing.Point(88, 388);
            this.stopOnCopyCheckBox.Name = "stopOnCopyCheckBox";
            this.stopOnCopyCheckBox.Size = new System.Drawing.Size(96, 24);
            this.stopOnCopyCheckBox.TabIndex = 7;
            this.stopOnCopyCheckBox.Text = "Stop on copy";
            // 
            // LogDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.Caption = "Log";
            this.ClientSize = new System.Drawing.Size(368, 420);
            this.Controls.Add(this.stopOnCopyCheckBox);
            this.Name = "LogDialog";
            this.Text = "Log";
            this.Controls.SetChildIndex(this.stopOnCopyCheckBox, 0);
            this.ResumeLayout(false);

        }
		#endregion
	}
}

