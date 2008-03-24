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
            this.keywordLabel = new System.Windows.Forms.Label();
            this.urlCheckBox = new System.Windows.Forms.CheckBox();
            this.revisionCheckBox = new System.Windows.Forms.CheckBox();
            this.authorCheckBox = new System.Windows.Forms.CheckBox();
            this.allCheckBox = new System.Windows.Forms.CheckBox();
            this.dateCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // keywordLabel
            // 
            this.keywordLabel.Name = "keywordLabel";
            this.keywordLabel.Size = new System.Drawing.Size(160, 16);
            this.keywordLabel.TabIndex = 1;
            this.keywordLabel.Text = "Select what to substitute:";
            // 
            // urlCheckBox
            // 
            this.urlCheckBox.Location = new System.Drawing.Point(0, 88);
            this.urlCheckBox.Name = "urlCheckBox";
            this.urlCheckBox.TabIndex = 5;
            this.urlCheckBox.Tag = "URL";
            this.urlCheckBox.Text = "URL";
            this.urlCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // revisionCheckBox
            // 
            this.revisionCheckBox.Location = new System.Drawing.Point(0, 40);
            this.revisionCheckBox.Name = "revisionCheckBox";
            this.revisionCheckBox.TabIndex = 3;
            this.revisionCheckBox.Tag = "Revision";
            this.revisionCheckBox.Text = "Revision";
            this.revisionCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // authorCheckBox
            // 
            this.authorCheckBox.Location = new System.Drawing.Point(0, 64);
            this.authorCheckBox.Name = "authorCheckBox";
            this.authorCheckBox.TabIndex = 4;
            this.authorCheckBox.Tag = "Author";
            this.authorCheckBox.Text = "Author";
            this.authorCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // allCheckBox
            // 
            this.allCheckBox.Location = new System.Drawing.Point(0, 112);
            this.allCheckBox.Name = "allCheckBox";
            this.allCheckBox.TabIndex = 6;
            this.allCheckBox.Text = "All";
            this.allCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // dateCheckBox
            // 
            this.dateCheckBox.Location = new System.Drawing.Point(0, 16);
            this.dateCheckBox.Name = "dateCheckBox";
            this.dateCheckBox.TabIndex = 7;
            this.dateCheckBox.Tag = "Date";
            this.dateCheckBox.Text = "Date";
            this.dateCheckBox.Click += new System.EventHandler(this.CheckBox_Click);
            // 
            // KeywordsPropertyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.dateCheckBox,
                                                                          this.keywordLabel,
                                                                          this.urlCheckBox,
                                                                          this.revisionCheckBox,
                                                                          this.authorCheckBox,
                                                                          this.allCheckBox});
            this.Name = "KeywordsPropertyEditor";
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Label keywordLabel;
        private System.Windows.Forms.CheckBox dateCheckBox;
        private System.Windows.Forms.CheckBox urlCheckBox;
        private System.Windows.Forms.CheckBox revisionCheckBox;
        private System.Windows.Forms.CheckBox allCheckBox;
        private System.Windows.Forms.CheckBox authorCheckBox;
    }
}
