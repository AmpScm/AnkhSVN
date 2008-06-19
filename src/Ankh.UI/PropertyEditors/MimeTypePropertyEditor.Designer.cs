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
            this.mimeTextBox = new System.Windows.Forms.TextBox();
            this.mimeGroupBox = new System.Windows.Forms.GroupBox();
            this.mimeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // mimeTextBox
            // 
            this.mimeTextBox.Location = new System.Drawing.Point(6, 19);
            this.mimeTextBox.Name = "mimeTextBox";
            this.mimeTextBox.Size = new System.Drawing.Size(232, 20);
            this.mimeTextBox.TabIndex = 2;
            this.mimeTextBox.TextChanged += new System.EventHandler(this.mimeTextBox_TextChanged);
            // 
            // mimeGroupBox
            // 
            this.mimeGroupBox.Controls.Add(this.mimeTextBox);
            this.mimeGroupBox.Location = new System.Drawing.Point(3, 3);
            this.mimeGroupBox.Name = "mimeGroupBox";
            this.mimeGroupBox.Size = new System.Drawing.Size(244, 144);
            this.mimeGroupBox.TabIndex = 3;
            this.mimeGroupBox.TabStop = false;
            this.mimeGroupBox.Text = "Enter mime-type property";
            // 
            // MimeTypePropertyEditor
            // 
            this.Controls.Add(this.mimeGroupBox);
            this.Name = "MimeTypePropertyEditor";
            this.Size = new System.Drawing.Size(250, 150);
            this.mimeGroupBox.ResumeLayout(false);
            this.mimeGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TextBox mimeTextBox;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.GroupBox mimeGroupBox;

    }
}
