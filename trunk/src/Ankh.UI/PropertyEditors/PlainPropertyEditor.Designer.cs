using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class PlainPropertyEditor
    {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.plainLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // valueTextBox
            // 
            this.valueTextBox.AcceptsReturn = true;
            this.valueTextBox.AcceptsTab = true;
            this.valueTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.valueTextBox.Location = new System.Drawing.Point(0, 22);
            this.valueTextBox.Multiline = true;
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(240, 128);
            this.valueTextBox.TabIndex = 0;
            this.valueTextBox.Text = "";
            this.valueTextBox.TextChanged += new System.EventHandler(this.valueTextBox_TextChanged);
            // 
            // plainLabel
            // 
            this.plainLabel.Name = "plainLabel";
            this.plainLabel.Size = new System.Drawing.Size(264, 23);
            this.plainLabel.TabIndex = 1;
            this.plainLabel.Text = "Enter values:";
            // 
            // PlainPropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.plainLabel,
                                                                          this.valueTextBox});
            this.Name = "PlainPropertyEditor";
            this.Size = new System.Drawing.Size(240, 150);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.Label plainLabel;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

    }
}
