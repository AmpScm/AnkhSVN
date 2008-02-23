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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogDialogTest));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.cbLogMessageViewer = new System.Windows.Forms.ToolStripButton();
            this.cbChangedPathsViewer = new System.Windows.Forms.ToolStripButton();
            this.logRevisionControl = new Ankh.UI.LogRevisionControl(this.components);
            this.logChangedPaths = new Ankh.UI.LogChangedPaths(this.components);
            this.logMessageView = new Ankh.UI.LogMessageView(this.components);
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.cbStopOnCopy = new System.Windows.Forms.ToolStripButton();
            this.cbIncludeMerged = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.logRevisionControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(550, 377);
            this.splitContainer1.SplitterDistance = 188;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.logChangedPaths);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.logMessageView);
            this.splitContainer2.Size = new System.Drawing.Size(550, 185);
            this.splitContainer2.SplitterDistance = 252;
            this.splitContainer2.TabIndex = 2;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(550, 377);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(550, 402);
            this.toolStripContainer1.TabIndex = 3;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip2);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbChangedPathsViewer,
            this.cbLogMessageViewer});
            this.toolStrip1.Location = new System.Drawing.Point(386, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(164, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // cbLogMessageViewer
            // 
            this.cbLogMessageViewer.Checked = true;
            this.cbLogMessageViewer.CheckOnClick = true;
            this.cbLogMessageViewer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLogMessageViewer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cbLogMessageViewer.Image = ((System.Drawing.Image)(resources.GetObject("cbLogMessageViewer.Image")));
            this.cbLogMessageViewer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbLogMessageViewer.Name = "cbLogMessageViewer";
            this.cbLogMessageViewer.Size = new System.Drawing.Size(70, 22);
            this.cbLogMessageViewer.Text = "Logmessage";
            this.cbLogMessageViewer.CheckedChanged += new System.EventHandler(this.toolStripButton1_CheckedChanged);
            // 
            // cbChangedPathsViewer
            // 
            this.cbChangedPathsViewer.Checked = true;
            this.cbChangedPathsViewer.CheckOnClick = true;
            this.cbChangedPathsViewer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbChangedPathsViewer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cbChangedPathsViewer.Image = ((System.Drawing.Image)(resources.GetObject("cbChangedPathsViewer.Image")));
            this.cbChangedPathsViewer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbChangedPathsViewer.Name = "cbChangedPathsViewer";
            this.cbChangedPathsViewer.Size = new System.Drawing.Size(84, 22);
            this.cbChangedPathsViewer.Text = "Changed paths";
            this.cbChangedPathsViewer.Click += new System.EventHandler(this.cbChangedPathsViewer_Click);
            // 
            // logRevisionControl
            // 
            this.logRevisionControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logRevisionControl.Location = new System.Drawing.Point(0, 0);
            this.logRevisionControl.Name = "logRevisionControl";
            this.logRevisionControl.Size = new System.Drawing.Size(550, 188);
            this.logRevisionControl.TabIndex = 0;
            // 
            // logChangedPaths
            // 
            this.logChangedPaths.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logChangedPaths.ItemSource = this.logRevisionControl;
            this.logChangedPaths.Location = new System.Drawing.Point(0, 0);
            this.logChangedPaths.Name = "logChangedPaths";
            this.logChangedPaths.Size = new System.Drawing.Size(252, 185);
            this.logChangedPaths.TabIndex = 0;
            // 
            // logMessageView
            // 
            this.logMessageView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessageView.ItemSource = this.logRevisionControl;
            this.logMessageView.Location = new System.Drawing.Point(0, 0);
            this.logMessageView.Name = "logMessageView";
            this.logMessageView.Size = new System.Drawing.Size(294, 185);
            this.logMessageView.TabIndex = 1;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbStopOnCopy,
            this.cbIncludeMerged});
            this.toolStrip2.Location = new System.Drawing.Point(5, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(285, 25);
            this.toolStrip2.TabIndex = 1;
            // 
            // cbStopOnCopy
            // 
            this.cbStopOnCopy.CheckOnClick = true;
            this.cbStopOnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cbStopOnCopy.Image = ((System.Drawing.Image)(resources.GetObject("cbStopOnCopy.Image")));
            this.cbStopOnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbStopOnCopy.Name = "cbStopOnCopy";
            this.cbStopOnCopy.Size = new System.Drawing.Size(114, 22);
            this.cbStopOnCopy.Text = "Stop on copy/rename";
            this.cbStopOnCopy.Click += new System.EventHandler(this.cbStopOnCopy_Click);
            // 
            // cbIncludeMerged
            // 
            this.cbIncludeMerged.CheckOnClick = true;
            this.cbIncludeMerged.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.cbIncludeMerged.Image = ((System.Drawing.Image)(resources.GetObject("cbIncludeMerged.Image")));
            this.cbIncludeMerged.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.cbIncludeMerged.Name = "cbIncludeMerged";
            this.cbIncludeMerged.Size = new System.Drawing.Size(130, 22);
            this.cbIncludeMerged.Text = "Include merged revisions";
            this.cbIncludeMerged.Click += new System.EventHandler(this.cbIncludeMerged_Click);
            // 
            // LogDialogTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 402);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "LogDialogTest";
            this.Text = "LogDialogTest";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private LogRevisionControl logRevisionControl;
        private LogMessageView logMessageView;
        private LogChangedPaths logChangedPaths;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton cbLogMessageViewer;
        private System.Windows.Forms.ToolStripButton cbChangedPathsViewer;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton cbStopOnCopy;
        private System.Windows.Forms.ToolStripButton cbIncludeMerged;
    }
}