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
            this.plainGroupBox = new System.Windows.Forms.GroupBox();
            this.plainGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // valueTextBox
            // 
            this.valueTextBox.AcceptsReturn = true;
            this.valueTextBox.AcceptsTab = true;
            this.valueTextBox.Location = new System.Drawing.Point(6, 19);
            this.valueTextBox.Multiline = true;
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.Size = new System.Drawing.Size(232, 119);
            this.valueTextBox.TabIndex = 0;
            this.valueTextBox.TextChanged += new System.EventHandler(this.valueTextBox_TextChanged);
            // 
            // plainGroupBox
            // 
            this.plainGroupBox.Controls.Add(this.valueTextBox);
            this.plainGroupBox.Location = new System.Drawing.Point(3, 3);
            this.plainGroupBox.Name = "plainGroupBox";
            this.plainGroupBox.Size = new System.Drawing.Size(244, 144);
            this.plainGroupBox.TabIndex = 2;
            this.plainGroupBox.TabStop = false;
            this.plainGroupBox.Text = "Enter values";
            // 
            // PlainPropertyEditor
            // 
            this.Controls.Add(this.plainGroupBox);
            this.Name = "PlainPropertyEditor";
            this.Size = new System.Drawing.Size(250, 150);
            this.plainGroupBox.ResumeLayout(false);
            this.plainGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TextBox valueTextBox;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.GroupBox plainGroupBox;

    }
}
