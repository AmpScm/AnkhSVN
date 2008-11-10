using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.PropertyEditors
{
    partial class ExecutablePropertyEditor
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.conflictToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.executableTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // executableTextBox
            // 
            this.executableTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.executableTextBox.Location = new System.Drawing.Point(0, 0);
            this.executableTextBox.Multiline = true;
            this.executableTextBox.Name = "executableTextBox";
            this.executableTextBox.ReadOnly = true;
            this.executableTextBox.Size = new System.Drawing.Size(348, 196);
            this.executableTextBox.TabIndex = 0;
            this.executableTextBox.Text = "File is executable.";
            // 
            // ExecutablePropertyEditor
            // 
            this.Controls.Add(this.executableTextBox);
            this.Name = "ExecutablePropertyEditor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ToolTip conflictToolTip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TextBox executableTextBox;
    }
}
