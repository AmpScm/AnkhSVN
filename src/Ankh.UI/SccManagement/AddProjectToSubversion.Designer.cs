namespace Ankh.UI.SccManagement
{
    partial class AddProjectToSubversion
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
            this.markAsManaged = new System.Windows.Forms.CheckBox();
            this.writeUrlInSolution = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // bodyPanel
            // 
            this.bodyPanel.TabIndex = 0;
            // 
            // markAsManaged
            // 
            this.markAsManaged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.markAsManaged.AutoSize = true;
            this.markAsManaged.Checked = true;
            this.markAsManaged.CheckState = System.Windows.Forms.CheckState.Checked;
            this.markAsManaged.Location = new System.Drawing.Point(12, 469);
            this.markAsManaged.Name = "markAsManaged";
            this.markAsManaged.Size = new System.Drawing.Size(218, 17);
            this.markAsManaged.TabIndex = 2;
            this.markAsManaged.Text = "&Mark Project as Managed by Subversion";
            this.markAsManaged.UseVisualStyleBackColor = true;
            // 
            // writeUrlInSolution
            // 
            this.writeUrlInSolution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.writeUrlInSolution.AutoSize = true;
            this.writeUrlInSolution.Checked = true;
            this.writeUrlInSolution.CheckState = System.Windows.Forms.CheckState.Checked;
            this.writeUrlInSolution.Location = new System.Drawing.Point(12, 446);
            this.writeUrlInSolution.Name = "writeUrlInSolution";
            this.writeUrlInSolution.Size = new System.Drawing.Size(222, 17);
            this.writeUrlInSolution.TabIndex = 1;
            this.writeUrlInSolution.Text = "Add &CheckOut information to Solution File";
            this.writeUrlInSolution.UseVisualStyleBackColor = true;
            // 
            // AddProjectToSubversion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 511);
            this.Controls.Add(this.writeUrlInSolution);
            this.Controls.Add(this.markAsManaged);
            this.Name = "AddProjectToSubversion";
            this.Controls.SetChildIndex(this.bodyPanel, 0);
            this.Controls.SetChildIndex(this.markAsManaged, 0);
            this.Controls.SetChildIndex(this.writeUrlInSolution, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox markAsManaged;
        private System.Windows.Forms.CheckBox writeUrlInSolution;
    }
}