using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.PropertyEditors
{
    partial class NeedsLockPropertyEditor
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.needsLockToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.needsLockTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // needsLockTextBox
            // 
            this.needsLockTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.needsLockTextBox.Location = new System.Drawing.Point(0, 0);
            this.needsLockTextBox.Multiline = true;
            this.needsLockTextBox.Name = "needsLockTextBox";
            this.needsLockTextBox.ReadOnly = true;
            this.needsLockTextBox.Size = new System.Drawing.Size(348, 196);
            this.needsLockTextBox.TabIndex = 0;
            // 
            // NeedsLockPropertyEditor
            // 
            this.Controls.Add(this.needsLockTextBox);
            this.Name = "NeedsLockPropertyEditor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ToolTip needsLockToolTip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TextBox needsLockTextBox;
    }
}
