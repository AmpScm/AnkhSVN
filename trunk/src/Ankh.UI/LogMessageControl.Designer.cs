using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class LogMessageControl
    {

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
