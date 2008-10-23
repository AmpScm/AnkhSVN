namespace Ankh.UI.SvnLog
{
    partial class LogControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.logRevisionControl1 = new Ankh.UI.SvnLog.LogRevisionControl(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.logChangedPaths1 = new Ankh.UI.LogChangedPaths(this.components);
            this.logMessageView1 = new Ankh.UI.LogMessageView(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.logRevisionControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(716, 421);
            this.splitContainer1.SplitterDistance = 291;
            this.splitContainer1.TabIndex = 1;
            // 
            // logRevisionControl1
            // 
            this.logRevisionControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logRevisionControl1.Location = new System.Drawing.Point(0, 0);
            this.logRevisionControl1.Name = "logRevisionControl1";
            this.logRevisionControl1.Size = new System.Drawing.Size(716, 291);
            this.logRevisionControl1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.logChangedPaths1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.logMessageView1);
            this.splitContainer2.Size = new System.Drawing.Size(716, 126);
            this.splitContainer2.SplitterDistance = 339;
            this.splitContainer2.TabIndex = 0;
            // 
            // logChangedPaths1
            // 
            this.logChangedPaths1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logChangedPaths1.ItemSource = this.logRevisionControl1;
            this.logChangedPaths1.Location = new System.Drawing.Point(0, 0);
            this.logChangedPaths1.Name = "logChangedPaths1";
            this.logChangedPaths1.Size = new System.Drawing.Size(339, 126);
            this.logChangedPaths1.TabIndex = 0;
            // 
            // logMessageView1
            // 
            this.logMessageView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessageView1.ItemSource = this.logRevisionControl1;
            this.logMessageView1.Location = new System.Drawing.Point(0, 0);
            this.logMessageView1.Name = "logMessageView1";
            this.logMessageView1.Size = new System.Drawing.Size(373, 126);
            this.logMessageView1.TabIndex = 0;
            // 
            // LogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "LogControl";
            this.Size = new System.Drawing.Size(716, 421);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Ankh.UI.SvnLog.LogRevisionControl logRevisionControl1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private LogChangedPaths logChangedPaths1;
        private LogMessageView logMessageView1;

    }
}
