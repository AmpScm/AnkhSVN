using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class KeywordsPropertyEditor
    {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.urlCheckBox = new System.Windows.Forms.CheckBox();
            this.revisionCheckBox = new System.Windows.Forms.CheckBox();
            this.authorCheckBox = new System.Windows.Forms.CheckBox();
            this.allCheckBox = new System.Windows.Forms.CheckBox();
            this.dateCheckBox = new System.Windows.Forms.CheckBox();
            this.keywordsGoupBox = new System.Windows.Forms.GroupBox();
            this.keywordsGoupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // urlCheckBox
            // 
            this.urlCheckBox.Location = new System.Drawing.Point(6, 93);
            this.urlCheckBox.Name = "urlCheckBox";
            this.urlCheckBox.Size = new System.Drawing.Size(104, 20);
            this.urlCheckBox.TabIndex = 5;
            this.urlCheckBox.Tag = "URL";
            this.urlCheckBox.Text = "URL";
            this.urlCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // revisionCheckBox
            // 
            this.revisionCheckBox.Location = new System.Drawing.Point(6, 43);
            this.revisionCheckBox.Name = "revisionCheckBox";
            this.revisionCheckBox.Size = new System.Drawing.Size(104, 20);
            this.revisionCheckBox.TabIndex = 3;
            this.revisionCheckBox.Tag = "Revision";
            this.revisionCheckBox.Text = "Revision";
            this.revisionCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // authorCheckBox
            // 
            this.authorCheckBox.Location = new System.Drawing.Point(6, 68);
            this.authorCheckBox.Name = "authorCheckBox";
            this.authorCheckBox.Size = new System.Drawing.Size(104, 20);
            this.authorCheckBox.TabIndex = 4;
            this.authorCheckBox.Tag = "Author";
            this.authorCheckBox.Text = "Author";
            this.authorCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // allCheckBox
            // 
            this.allCheckBox.Location = new System.Drawing.Point(6, 117);
            this.allCheckBox.Name = "allCheckBox";
            this.allCheckBox.Size = new System.Drawing.Size(104, 20);
            this.allCheckBox.TabIndex = 6;
            this.allCheckBox.Text = "All";
            this.allCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // dateCheckBox
            // 
            this.dateCheckBox.Location = new System.Drawing.Point(6, 17);
            this.dateCheckBox.Name = "dateCheckBox";
            this.dateCheckBox.Size = new System.Drawing.Size(104, 20);
            this.dateCheckBox.TabIndex = 7;
            this.dateCheckBox.Tag = "Date";
            this.dateCheckBox.Text = "Date";
            this.dateCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // keywordsGoupBox
            // 
            this.keywordsGoupBox.Controls.Add(this.dateCheckBox);
            this.keywordsGoupBox.Controls.Add(this.revisionCheckBox);
            this.keywordsGoupBox.Controls.Add(this.allCheckBox);
            this.keywordsGoupBox.Controls.Add(this.urlCheckBox);
            this.keywordsGoupBox.Controls.Add(this.authorCheckBox);
            this.keywordsGoupBox.Location = new System.Drawing.Point(5, 6);
            this.keywordsGoupBox.Name = "keywordsGoupBox";
            this.keywordsGoupBox.Size = new System.Drawing.Size(242, 141);
            this.keywordsGoupBox.TabIndex = 8;
            this.keywordsGoupBox.TabStop = false;
            this.keywordsGoupBox.Text = "Select what to substitute";
            // 
            // KeywordsPropertyEditor
            // 
            this.Controls.Add(this.keywordsGoupBox);
            this.Name = "KeywordsPropertyEditor";
            this.Size = new System.Drawing.Size(250, 150);
            this.keywordsGoupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.CheckBox dateCheckBox;
        private System.Windows.Forms.CheckBox urlCheckBox;
        private System.Windows.Forms.CheckBox revisionCheckBox;
        private System.Windows.Forms.CheckBox allCheckBox;
        private System.Windows.Forms.CheckBox authorCheckBox;
        private System.Windows.Forms.GroupBox keywordsGoupBox;
    }
}
