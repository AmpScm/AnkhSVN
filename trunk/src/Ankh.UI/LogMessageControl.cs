// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Ankh.UI
{
	/// <summary>
	/// Summary description for LogMessageControl.
	/// </summary>
	public class LogMessageControl : System.Windows.Forms.UserControl
	{
      
		public LogMessageControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}


        public string LogMessage
        {
       
            get
            {
                return this.logTextBox.Text;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.logTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // logTextBox
            // 
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.Size = new System.Drawing.Size(368, 312);
            this.logTextBox.TabIndex = 0;
            this.logTextBox.Text = "";
            // 
            // LogMessageControl
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.logTextBox});
            this.Name = "LogMessageControl";
            this.Size = new System.Drawing.Size(368, 312);
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.RichTextBox logTextBox;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

	}
}


