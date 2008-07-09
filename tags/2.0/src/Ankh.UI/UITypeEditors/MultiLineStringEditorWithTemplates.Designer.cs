using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class MultiLineStringEditorWithTemplates
    {
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Size = new System.Drawing.Size(524, 157);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(label1);
            this.panel1.Location = new System.Drawing.Point(0, 157);
            this.panel1.Size = new System.Drawing.Size(524, 29);
            this.panel1.Controls.SetChildIndex(label1, 0);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 8);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(178, 13);
            label1.TabIndex = 2;
            label1.Text = "Insert a template by pressing Ctrl-T";
            // 
            // MultiLineStringEditorWithTemplates
            // 
            this.ClientSize = new System.Drawing.Size(524, 186);
            this.KeyPreview = true;
            this.Name = "MultiLineStringEditorWithTemplates";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MultiLineStringEditorWithTemplates_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
