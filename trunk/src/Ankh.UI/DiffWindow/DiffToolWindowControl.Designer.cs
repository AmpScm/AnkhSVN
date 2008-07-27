using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.DiffWindow
{
    partial class DiffToolWindowControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        private void InitializeComponent()
        {
            this.diffControl1 = new Ankh.Diff.DiffUtils.Controls.DiffControl();
            this.SuspendLayout();
            // 
            // diffControl1
            // 
            this.diffControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffControl1.Location = new System.Drawing.Point(0, 0);
            this.diffControl1.Name = "diffControl1";
            this.diffControl1.ShowWhitespaceInLineDiff = true;
            this.diffControl1.Size = new System.Drawing.Size(773, 375);
            this.diffControl1.TabIndex = 0;
            this.diffControl1.ViewFont = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // DiffToolWindowControl
            // 
            this.Controls.Add(this.diffControl1);
            this.Name = "DiffToolWindowControl";
            this.Size = new System.Drawing.Size(773, 375);
            this.ResumeLayout(false);

        }
        #endregion

        private Ankh.Diff.DiffUtils.Controls.DiffControl diffControl1;
    }
}
