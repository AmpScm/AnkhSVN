namespace Ankh.UI.OptionsPages
{
    partial class EnvironmentSettingsControl
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

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.authenticationEdit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // authenticationEdit
            // 
            this.authenticationEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.authenticationEdit.Location = new System.Drawing.Point(322, 245);
            this.authenticationEdit.Name = "authenticationEdit";
            this.authenticationEdit.Size = new System.Drawing.Size(75, 23);
            this.authenticationEdit.TabIndex = 0;
            this.authenticationEdit.Text = "Edit";
            this.authenticationEdit.UseVisualStyleBackColor = true;
            this.authenticationEdit.Click += new System.EventHandler(this.authenticationEdit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 250);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Subversion &Authentication Cache:";
            // 
            // EnvironmentSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.authenticationEdit);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "EnvironmentSettingsControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button authenticationEdit;
        private System.Windows.Forms.Label label1;
    }
}
