namespace Ankh.UI.SccManagement
{
    partial class ItemUpdateDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pendingList = new Ankh.UI.PendingChanges.Commits.PendingCommitsView(this.components);
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.revisionPickerStart = new Ankh.UI.PathSelector.VersionSelector();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pendingList
            // 
            this.pendingList.AllowColumnReorder = true;
            this.pendingList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pendingList.CheckBoxes = true;
            this.pendingList.HideSelection = false;
            this.pendingList.Location = new System.Drawing.Point(12, 12);
            this.pendingList.Name = "pendingList";
            this.pendingList.ShowSelectAllCheckBox = true;
            this.pendingList.Size = new System.Drawing.Size(600, 185);
            this.pendingList.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(456, 274);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(537, 274);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.revisionPickerStart);
            this.groupBox1.Location = new System.Drawing.Point(12, 203);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(600, 61);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Version";
            // 
            // revisionPickerStart
            // 
            this.revisionPickerStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPickerStart.Location = new System.Drawing.Point(6, 19);
            this.revisionPickerStart.Name = "revisionPickerStart";
            this.revisionPickerStart.Size = new System.Drawing.Size(588, 33);
            this.revisionPickerStart.SvnOrigin = null;
            this.revisionPickerStart.TabIndex = 0;
            // 
            // ItemUpdateDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(624, 309);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pendingList);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Name = "ItemUpdateDialog";
            this.Text = "Update";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Ankh.UI.PendingChanges.Commits.PendingCommitsView pendingList;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private Ankh.UI.PathSelector.VersionSelector revisionPickerStart;
    }
}