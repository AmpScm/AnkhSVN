using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class ProgressDialog
    {
        private System.ComponentModel.IContainer components;


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cancelButton = new System.Windows.Forms.Button();
            this.actionList = new System.Windows.Forms.ListView();
            this.actionColumn = new System.Windows.Forms.ColumnHeader();
            this.pathColumn = new System.Windows.Forms.ColumnHeader();
            this.progressLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(360, 166);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.CancelClick);
            // 
            // actionList
            // 
            this.actionList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.actionList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.actionColumn,
            this.pathColumn});
            this.actionList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.actionList.Location = new System.Drawing.Point(12, 16);
            this.actionList.Name = "actionList";
            this.actionList.Size = new System.Drawing.Size(423, 132);
            this.actionList.TabIndex = 2;
            this.actionList.UseCompatibleStateImageBehavior = false;
            this.actionList.View = System.Windows.Forms.View.Details;
            // 
            // actionColumn
            // 
            this.actionColumn.Text = "Action";
            this.actionColumn.Width = 74;
            // 
            // pathColumn
            // 
            this.pathColumn.Text = "Path";
            this.pathColumn.Width = 329;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(12, 112);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(0, 13);
            this.progressLabel.TabIndex = 3;
            // 
            // ProgressDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(447, 201);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.actionList);
            this.Controls.Add(this.cancelButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "{0} - Please wait...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ListView actionList;
        private System.Windows.Forms.ColumnHeader actionColumn;
        private System.Windows.Forms.ColumnHeader pathColumn;
        private System.Windows.Forms.Label progressLabel;
    }
}
