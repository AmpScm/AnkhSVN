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
            this.components = new System.ComponentModel.Container();
            this.logMessageEditor1 = new Ankh.UI.PendingChanges.LogMessageEditor(this.components);
            this.blameMarginControl1 = new Ankh.UI.Blame.BlameMarginControl();
            this.SuspendLayout();
            // 
            // logMessageEditor1
            // 
            this.logMessageEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logMessageEditor1.Location = new System.Drawing.Point(121, 0);
            this.logMessageEditor1.Name = "logMessageEditor1";
            this.logMessageEditor1.Size = new System.Drawing.Size(180, 301);
            this.logMessageEditor1.TabIndex = 2;
            this.logMessageEditor1.Text = "logMessageEditor1";
            this.logMessageEditor1.Scroll += new System.EventHandler<Ankh.UI.PendingChanges.TextViewScrollEventArgs>(logMessageEditor1_Scroll);
            // 
            // blameMarginControl1
            // 
            this.blameMarginControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.blameMarginControl1.Location = new System.Drawing.Point(0, 0);
            this.blameMarginControl1.Name = "blameMarginControl1";
            this.blameMarginControl1.Size = new System.Drawing.Size(115, 301);
            this.blameMarginControl1.TabIndex = 1;
            this.blameMarginControl1.Text = "blameMarginControl1";
            // 
            // BlameToolWindowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.logMessageEditor1);
            this.Controls.Add(this.blameMarginControl1);
            this.Name = "BlameToolWindowControl";
            this.ResumeLayout(false);

        }

        #endregion

        private BlameMarginControl blameMarginControl1;
        private Ankh.UI.PendingChanges.LogMessageEditor logMessageEditor1;
    }
}
