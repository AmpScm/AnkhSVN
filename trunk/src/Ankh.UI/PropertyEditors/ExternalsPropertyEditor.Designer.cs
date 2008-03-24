using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class ExternalsPropertyEditor
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.externalsTextBox = new System.Windows.Forms.TextBox();
            this.externalsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // externalsTextBox
            // 
            this.externalsTextBox.AcceptsReturn = true;
            this.externalsTextBox.AcceptsTab = true;
            this.externalsTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.externalsTextBox.Location = new System.Drawing.Point(0, 22);
            this.externalsTextBox.Multiline = true;
            this.externalsTextBox.Name = "externalsTextBox";
            this.externalsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.externalsTextBox.Size = new System.Drawing.Size(400, 128);
            this.externalsTextBox.TabIndex = 2;
            this.externalsTextBox.Text = "";
            this.externalsTextBox.TextChanged += new System.EventHandler(this.externalsTextBox_TextChanged);
            // 
            // externalsLabel
            // 
            this.externalsLabel.Location = new System.Drawing.Point(0, 1);
            this.externalsLabel.Name = "externalsLabel";
            this.externalsLabel.Size = new System.Drawing.Size(408, 16);
            this.externalsLabel.TabIndex = 3;
            this.externalsLabel.Text = "Write path and URL:";
            // 
            // ExternalsPropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.externalsLabel,
                                                                          this.externalsTextBox});
            this.Name = "ExternalsPropertyEditor";
            this.Size = new System.Drawing.Size(400, 150);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.TextBox externalsTextBox;
        private System.Windows.Forms.Label externalsLabel;
    }
}
