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
            this.externalsGroupBox = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.externalsGroupBox.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // externalsTextBox
            // 
            this.externalsTextBox.AcceptsReturn = true;
            this.externalsTextBox.AcceptsTab = true;
            this.externalsTextBox.Location = new System.Drawing.Point(3, 3);
            this.externalsTextBox.Multiline = true;
            this.externalsTextBox.Name = "externalsTextBox";
            this.externalsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.externalsTextBox.Size = new System.Drawing.Size(226, 116);
            this.externalsTextBox.TabIndex = 2;
            this.externalsTextBox.TextChanged += new System.EventHandler(this.externalsTextBox_TextChanged);
            // 
            // externalsGroupBox
            // 
            this.externalsGroupBox.Controls.Add(this.panel1);
            this.externalsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.externalsGroupBox.Name = "externalsGroupBox";
            this.externalsGroupBox.Size = new System.Drawing.Size(244, 144);
            this.externalsGroupBox.TabIndex = 4;
            this.externalsGroupBox.TabStop = false;
            this.externalsGroupBox.Text = "Write path and URL";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.externalsTextBox);
            this.panel1.Location = new System.Drawing.Point(6, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(232, 119);
            this.panel1.TabIndex = 0;
            // 
            // ExternalsPropertyEditor
            // 
            this.Controls.Add(this.externalsGroupBox);
            this.Name = "ExternalsPropertyEditor";
            this.Size = new System.Drawing.Size(250, 150);
            this.externalsGroupBox.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.TextBox externalsTextBox;
        private System.Windows.Forms.GroupBox externalsGroupBox;
        private System.Windows.Forms.Panel panel1;
    }
}
