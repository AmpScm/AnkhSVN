using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.PropertyEditors
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
            this.components = new System.ComponentModel.Container();
            this.externalsGroupBox = new System.Windows.Forms.GroupBox();
            this.externalsTextBox = new System.Windows.Forms.TextBox();
            this.conflictToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.externalsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // externalsGroupBox
            // 
            this.externalsGroupBox.Controls.Add(this.externalsTextBox);
            this.externalsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.externalsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.externalsGroupBox.Name = "externalsGroupBox";
            this.externalsGroupBox.Size = new System.Drawing.Size(348, 196);
            this.externalsGroupBox.TabIndex = 4;
            this.externalsGroupBox.TabStop = false;
            this.externalsGroupBox.Text = "Write path and URL";
            // 
            // externalsTextBox
            // 
            this.externalsTextBox.AcceptsReturn = true;
            this.externalsTextBox.AcceptsTab = true;
            this.externalsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.externalsTextBox.Location = new System.Drawing.Point(3, 16);
            this.externalsTextBox.Multiline = true;
            this.externalsTextBox.Name = "externalsTextBox";
            this.externalsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.externalsTextBox.Size = new System.Drawing.Size(342, 177);
            this.externalsTextBox.TabIndex = 3;
            this.externalsTextBox.TextChanged += new System.EventHandler(this.externalsTextBox_TextChanged);
            // 
            // ExternalsPropertyEditor
            // 
            this.Controls.Add(this.externalsGroupBox);
            this.Name = "ExternalsPropertyEditor";
            this.Size = new System.Drawing.Size(348, 196);
            this.externalsGroupBox.ResumeLayout(false);
            this.externalsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.GroupBox externalsGroupBox;
        private System.Windows.Forms.ToolTip conflictToolTip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TextBox externalsTextBox;
    }
}
