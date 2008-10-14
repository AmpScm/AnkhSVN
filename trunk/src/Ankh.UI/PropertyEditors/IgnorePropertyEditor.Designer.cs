using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.PropertyEditors
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
            this.ignoreGroupBox = new System.Windows.Forms.GroupBox();
            this.ignoreTextBox = new System.Windows.Forms.TextBox();
            this.conflictToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ignoreGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ignoreGroupBox
            // 
            this.ignoreGroupBox.Controls.Add(this.ignoreTextBox);
            this.ignoreGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ignoreGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ignoreGroupBox.Name = "ignoreGroupBox";
            this.ignoreGroupBox.Size = new System.Drawing.Size(348, 196);
            this.ignoreGroupBox.TabIndex = 4;
            this.ignoreGroupBox.TabStop = false;
            this.ignoreGroupBox.Text = "Ignore the following lines or patterns";
            // 
            // ignoreTextBox
            // 
            this.ignoreTextBox.AcceptsReturn = true;
            this.ignoreTextBox.AcceptsTab = true;
            this.ignoreTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ignoreTextBox.Location = new System.Drawing.Point(3, 16);
            this.ignoreTextBox.Multiline = true;
            this.ignoreTextBox.Name = "ignoreTextBox";
            this.ignoreTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ignoreTextBox.Size = new System.Drawing.Size(342, 177);
            this.ignoreTextBox.TabIndex = 3;
            this.ignoreTextBox.TextChanged += new System.EventHandler(this.ignoreTextBox_TextChanged);
            // 
            // IgnorePropertyEditor
            // 
            this.Controls.Add(this.ignoreGroupBox);
            this.Name = "IgnorePropertyEditor";
            this.ignoreGroupBox.ResumeLayout(false);
            this.ignoreGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.GroupBox ignoreGroupBox;
        private System.Windows.Forms.ToolTip conflictToolTip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TextBox ignoreTextBox;
        
    }
}
