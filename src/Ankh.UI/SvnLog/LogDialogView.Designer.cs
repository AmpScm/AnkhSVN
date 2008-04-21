namespace Ankh.UI
{
	partial class LogDialogView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogDialogView));
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.logRevisionControl1 = new Ankh.UI.LogRevisionControl(this.components);
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.logChangedPaths1 = new Ankh.UI.LogChangedPaths(this.components);
			this.logMessageView1 = new Ankh.UI.LogMessageView(this.components);
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.includeMergedButton = new System.Windows.Forms.ToolStripButton();
			this.stopOnCopyButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(511, 252);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(511, 277);
			this.toolStripContainer1.TabIndex = 0;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
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
			this.splitContainer1.Size = new System.Drawing.Size(511, 252);
			this.splitContainer1.SplitterDistance = 140;
			this.splitContainer1.TabIndex = 0;
			// 
			// logRevisionControl1
			// 
			this.logRevisionControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logRevisionControl1.Location = new System.Drawing.Point(0, 0);
			this.logRevisionControl1.Name = "logRevisionControl1";
			this.logRevisionControl1.Size = new System.Drawing.Size(511, 140);
			this.logRevisionControl1.TabIndex = 0;
			this.logRevisionControl1.ScrollPositionChanged += new System.EventHandler<System.Windows.Forms.ScrollEventArgs>(this.logRevisionControl1_ScrollPositionChanged);
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
			this.splitContainer2.Size = new System.Drawing.Size(511, 108);
			this.splitContainer2.SplitterDistance = 269;
			this.splitContainer2.TabIndex = 0;
			// 
			// logChangedPaths1
			// 
			this.logChangedPaths1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logChangedPaths1.ItemSource = this.logRevisionControl1;
			this.logChangedPaths1.Location = new System.Drawing.Point(0, 0);
			this.logChangedPaths1.Name = "logChangedPaths1";
			this.logChangedPaths1.Size = new System.Drawing.Size(269, 108);
			this.logChangedPaths1.TabIndex = 0;
			// 
			// logMessageView1
			// 
			this.logMessageView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logMessageView1.ItemSource = this.logRevisionControl1;
			this.logMessageView1.Location = new System.Drawing.Point(0, 0);
			this.logMessageView1.Name = "logMessageView1";
			this.logMessageView1.Size = new System.Drawing.Size(238, 108);
			this.logMessageView1.TabIndex = 0;
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.includeMergedButton,
            this.stopOnCopyButton});
			this.toolStrip1.Location = new System.Drawing.Point(3, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(303, 25);
			this.toolStrip1.TabIndex = 1;
			// 
			// includeMergedButton
			// 
			this.includeMergedButton.CheckOnClick = true;
			this.includeMergedButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.includeMergedButton.Image = ((System.Drawing.Image)(resources.GetObject("includeMergedButton.Image")));
			this.includeMergedButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.includeMergedButton.Name = "includeMergedButton";
			this.includeMergedButton.Size = new System.Drawing.Size(143, 22);
			this.includeMergedButton.Text = "Include merged revisions";
			this.includeMergedButton.CheckedChanged += new System.EventHandler(this.includeMergedButton_CheckedChanged);
			// 
			// stopOnCopyButton
			// 
			this.stopOnCopyButton.CheckOnClick = true;
			this.stopOnCopyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.stopOnCopyButton.Image = ((System.Drawing.Image)(resources.GetObject("stopOnCopyButton.Image")));
			this.stopOnCopyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.stopOnCopyButton.Name = "stopOnCopyButton";
			this.stopOnCopyButton.Size = new System.Drawing.Size(126, 22);
			this.stopOnCopyButton.Text = "Stop on copy/rename";
			this.stopOnCopyButton.CheckedChanged += new System.EventHandler(this.stopOnCopyButton_CheckedChanged);
			// 
			// LogDialogView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.toolStripContainer1);
			this.Name = "LogDialogView";
			this.Size = new System.Drawing.Size(511, 277);
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private LogRevisionControl logRevisionControl1;
		private LogChangedPaths logChangedPaths1;
		private LogMessageView logMessageView1;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton includeMergedButton;
		private System.Windows.Forms.ToolStripButton stopOnCopyButton;
	}
}
