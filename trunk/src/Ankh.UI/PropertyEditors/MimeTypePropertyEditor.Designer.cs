using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class MimeTypePropertyEditor
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mimeLabel = new System.Windows.Forms.Label();
            this.mimeTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // mimeLabel
            // 
            this.mimeLabel.Name = "mimeLabel";
            this.mimeLabel.Size = new System.Drawing.Size(152, 16);
            this.mimeLabel.TabIndex = 1;
            this.mimeLabel.Text = "Enter mime-type property:";
            // 
            // mimeTextBox
            // 
            this.mimeTextBox.Location = new System.Drawing.Point(0, 21);
            this.mimeTextBox.Name = "mimeTextBox";
            this.mimeTextBox.Size = new System.Drawing.Size(152, 20);
            this.mimeTextBox.TabIndex = 2;
            this.mimeTextBox.Text = "";
            this.mimeTextBox.TextChanged += new System.EventHandler(this.mimeTextBox_TextChanged);
            // 
            // MimeTypePropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.mimeTextBox,
                                                                          this.mimeLabel});
            this.Name = "MimeTypePropertyEditor";
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Label mimeLabel;
        private System.Windows.Forms.TextBox mimeTextBox;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

    }
}
