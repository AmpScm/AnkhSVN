using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class IgnorePropertyEditor
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ignoreTextBox = new System.Windows.Forms.TextBox();
            this.ignoreLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ignoreTextBox
            // 
            this.ignoreTextBox.AcceptsReturn = true;
            this.ignoreTextBox.AcceptsTab = true;
            this.ignoreTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ignoreTextBox.Location = new System.Drawing.Point(0, 30);
            this.ignoreTextBox.Multiline = true;
            this.ignoreTextBox.Name = "ignoreTextBox";
            this.ignoreTextBox.Size = new System.Drawing.Size(250, 120);
            this.ignoreTextBox.TabIndex = 2;
            this.ignoreTextBox.Text = "";
            this.ignoreTextBox.TextChanged += new System.EventHandler(this.ignoreTextBox_TextChanged);
            // 
            // ignoreLabel
            // 
            this.ignoreLabel.Name = "ignoreLabel";
            this.ignoreLabel.Size = new System.Drawing.Size(256, 24);
            this.ignoreLabel.TabIndex = 3;
            this.ignoreLabel.Text = "Ignore the following files or patterns:";
            // 
            // IgnorePropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.ignoreLabel,
                                                                          this.ignoreTextBox});
            this.Name = "IgnorePropertyEditor";
            this.Size = new System.Drawing.Size(250, 150);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.TextBox ignoreTextBox;
        private System.Windows.Forms.Label ignoreLabel;
        
    }
}
