namespace Ankh.UI
{
    partial class LogDialogTest
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
			this.logDialogView1 = new Ankh.UI.LogDialogView();
			this.SuspendLayout();
			// 
			// logDialogView1
			// 
			this.logDialogView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logDialogView1.Location = new System.Drawing.Point(0, 0);
			this.logDialogView1.Name = "logDialogView1";
			this.logDialogView1.Size = new System.Drawing.Size(550, 402);
			this.logDialogView1.TabIndex = 0;
			// 
			// LogDialogTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(550, 402);
			this.Controls.Add(this.logDialogView1);
			this.Name = "LogDialogTest";
			this.Text = "LogDialogTest";
			this.ResumeLayout(false);

        }

        #endregion

		private LogDialogView logDialogView1;


    }
}