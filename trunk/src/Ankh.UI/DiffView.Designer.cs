using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    partial class DiffView
    {
        private void InitializeComponent()
        {
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(328, 184);
            this.webBrowser.TabIndex = 0;
            // 
            // DiffView
            // 
            this.Controls.Add(this.webBrowser);
            this.Name = "DiffView";
            this.Size = new System.Drawing.Size(328, 184);
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private WebBrowser webBrowser;
    }
}
