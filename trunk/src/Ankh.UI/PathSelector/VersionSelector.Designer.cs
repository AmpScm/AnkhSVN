using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.PathSelector
{
    partial class VersionSelector
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.revisionTypeBox = new System.Windows.Forms.ComboBox();
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // revisionTypeBox
            // 
            this.revisionTypeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionTypeBox.Location = new System.Drawing.Point(0, 0);
            this.revisionTypeBox.Name = "revisionTypeBox";
            this.revisionTypeBox.Size = new System.Drawing.Size(200, 21);
            this.revisionTypeBox.TabIndex = 0;
            this.revisionTypeBox.SelectionChangeCommitted += new System.EventHandler(this.revisionTypeBox_SelectionChangeCommitted);
            this.revisionTypeBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.revisionTypeBox_KeyUp);
            // 
            // datePicker
            // 
            this.datePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.datePicker.Enabled = false;
            this.datePicker.Location = new System.Drawing.Point(200, 0);
            this.datePicker.Name = "datePicker";
            this.datePicker.Size = new System.Drawing.Size(144, 20);
            this.datePicker.TabIndex = 1;
            // 
            // RevisionPicker
            // 
            this.Controls.Add(this.datePicker);
            this.Controls.Add(this.revisionTypeBox);
            this.Name = "RevisionPicker";
            this.Size = new System.Drawing.Size(344, 20);
            this.ResumeLayout(false);

        }
        #endregion


        private System.Windows.Forms.ComboBox revisionTypeBox;
        private System.Windows.Forms.DateTimePicker datePicker;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
    }
}
