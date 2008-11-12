namespace Ankh.UI.Annotate
{
    partial class AnnotateEditorControl
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
            this.logMessageEditor1 = new Ankh.UI.PendingChanges.VSTextEditor(this.components);
            this.blameMarginControl1 = new Ankh.UI.Annotate.AnnotateMarginControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // logMessageEditor1
            // 
            this.logMessageEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessageEditor1.Location = new System.Drawing.Point(0, 0);
            this.logMessageEditor1.Name = "logMessageEditor1";
            this.logMessageEditor1.Size = new System.Drawing.Size(184, 300);
            this.logMessageEditor1.TabIndex = 2;
            this.logMessageEditor1.Text = "logMessageEditor1";
            this.logMessageEditor1.Scroll += new System.EventHandler<Ankh.UI.PendingChanges.TextViewScrollEventArgs>(this.logMessageEditor1_Scroll);
            // 
            // blameMarginControl1
            // 
            this.blameMarginControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blameMarginControl1.Location = new System.Drawing.Point(0, 0);
            this.blameMarginControl1.Name = "blameMarginControl1";
            this.blameMarginControl1.Size = new System.Drawing.Size(115, 300);
            this.blameMarginControl1.TabIndex = 1;
            this.blameMarginControl1.Text = "blameMarginControl1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.blameMarginControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.logMessageEditor1);
            this.splitContainer1.Size = new System.Drawing.Size(300, 300);
            this.splitContainer1.SplitterDistance = 115;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 3;
            // 
            // AnnotateEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "AnnotateEditorControl";
            this.Size = new System.Drawing.Size(300, 300);
            this.Text = " (Annotated)";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AnnotateMarginControl blameMarginControl1;
        private Ankh.UI.PendingChanges.VSTextEditor logMessageEditor1;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
