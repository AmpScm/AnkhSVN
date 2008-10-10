namespace Ankh.UI.Blame
{
    partial class BlameToolWindowControl
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
            this.editorHost1 = new Ankh.UI.Blame.EditorHost();
            this.blameMarginControl1 = new Ankh.UI.Blame.BlameMarginControl();
            this.SuspendLayout();
            // 
            // editorHost1
            // 
            this.editorHost1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.editorHost1.Location = new System.Drawing.Point(91, 0);
            this.editorHost1.Name = "editorHost1";
            this.editorHost1.Size = new System.Drawing.Size(210, 301);
            this.editorHost1.TabIndex = 0;
            this.editorHost1.Text = "editorHost1";
            // 
            // blameMarginControl1
            // 
            this.blameMarginControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.blameMarginControl1.Location = new System.Drawing.Point(-1, 0);
            this.blameMarginControl1.Name = "blameMarginControl1";
            this.blameMarginControl1.Size = new System.Drawing.Size(86, 301);
            this.blameMarginControl1.TabIndex = 1;
            this.blameMarginControl1.Text = "blameMarginControl1";
            // 
            // BlameToolWindowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.blameMarginControl1);
            this.Controls.Add(this.editorHost1);
            this.Name = "BlameToolWindowControl";
            this.ResumeLayout(false);

        }

        #endregion

        private EditorHost editorHost1;
        private BlameMarginControl blameMarginControl1;
    }
}
