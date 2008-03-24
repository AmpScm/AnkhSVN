using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    partial class PopUpListForm
    {
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listBox = new System.Windows.Forms.ListBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(140, 104);
            this.listBox.TabIndex = 0;
            this.listBox.DoubleClick += new System.EventHandler(this.listBox_DoubleClick);
            this.listBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyDown);
            // 
            // timer
            // 
            this.timer.Interval = 500;
            // 
            // PopUpListForm
            // 
            this.ClientSize = new System.Drawing.Size(140, 104);
            this.ControlBox = false;
            this.Controls.Add(this.listBox);
            this.Name = "PopUpListForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox_KeyDown);
            this.ResumeLayout(false);

        }
        private System.ComponentModel.IContainer components;
        private Timer timer;
        private ListBox listBox;
    }
}
