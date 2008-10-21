namespace Ankh.UI.SvnLog
{
    partial class LogToolWindowControl
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
            this.logControl = new Ankh.UI.SvnLog.LogControl(this.components);
            this.SuspendLayout();
            // 
            // logControl
            // 
            this.logControl.ChangedPathsVisible = true;
            this.logControl.Context = null;
            this.logControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logControl.IncludeMerged = false;
            this.logControl.Location = new System.Drawing.Point(0, 0);
            this.logControl.LogMessageVisible = true;
            this.logControl.Mode = Ankh.UI.SvnLog.LogMode.Log;
            this.logControl.Name = "logControl";
            this.logControl.Size = new System.Drawing.Size(482, 254);
            this.logControl.StrictNodeHistory = false;
            this.logControl.TabIndex = 0;
            // 
            // LogToolWindowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 254);
            this.Controls.Add(this.logControl);
            this.Name = "LogToolWindowControl";
            this.ResumeLayout(false);

        }

        #endregion

        private LogControl logControl;
    }
}
