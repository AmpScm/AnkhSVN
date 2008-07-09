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
            this.components = new System.ComponentModel.Container();
            this.ignoreTextBox = new System.Windows.Forms.TextBox();
            this.ignoreGroupBox = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.conflictToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ignoreGroupBox.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ignoreTextBox
            // 
            this.ignoreTextBox.AcceptsReturn = true;
            this.ignoreTextBox.AcceptsTab = true;
            this.ignoreTextBox.Location = new System.Drawing.Point(3, 3);
            this.ignoreTextBox.Multiline = true;
            this.ignoreTextBox.Name = "ignoreTextBox";
            this.ignoreTextBox.Size = new System.Drawing.Size(226, 113);
            this.ignoreTextBox.TabIndex = 2;
            this.ignoreTextBox.TextChanged += new System.EventHandler(this.ignoreTextBox_TextChanged);
            // 
            // ignoreGroupBox
            // 
            this.ignoreGroupBox.Controls.Add(this.panel1);
            this.ignoreGroupBox.Location = new System.Drawing.Point(3, 3);
            this.ignoreGroupBox.Name = "ignoreGroupBox";
            this.ignoreGroupBox.Size = new System.Drawing.Size(244, 144);
            this.ignoreGroupBox.TabIndex = 4;
            this.ignoreGroupBox.TabStop = false;
            this.ignoreGroupBox.Text = "Ignore the following lines or patterns";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ignoreTextBox);
            this.panel1.Location = new System.Drawing.Point(6, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(232, 119);
            this.panel1.TabIndex = 3;
            // 
            // IgnorePropertyEditor
            // 
            this.Controls.Add(this.ignoreGroupBox);
            this.Name = "IgnorePropertyEditor";
            this.Size = new System.Drawing.Size(250, 150);
            this.ignoreGroupBox.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TextBox ignoreTextBox;
        private System.Windows.Forms.GroupBox ignoreGroupBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip conflictToolTip;
        private System.ComponentModel.IContainer components;
        
    }
}
