using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
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
            this.executableCheckBox = new System.Windows.Forms.CheckBox();
            this.conflictToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // executableCheckBox
            // 
            this.executableCheckBox.Location = new System.Drawing.Point(3, 13);
            this.executableCheckBox.Name = "executableCheckBox";
            this.executableCheckBox.Size = new System.Drawing.Size(160, 24);
            this.executableCheckBox.TabIndex = 1;
            this.executableCheckBox.Text = "Executable";
            this.executableCheckBox.Click += new System.EventHandler(this.executableCheckBox_Click);
            // 
            // ExecutablePropertyEditor
            // 
            this.Controls.Add(this.executableCheckBox);
            this.Name = "ExecutablePropertyEditor";
            this.Size = new System.Drawing.Size(250, 150);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.CheckBox executableCheckBox;
        private System.Windows.Forms.ToolTip conflictToolTip;
        private System.ComponentModel.IContainer components;
    }
}
