namespace Ankh.UI.PendingChanges
{
    partial class PendingConflictsPage
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.topLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lastRevBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lastRevLabel = new System.Windows.Forms.Label();
            this.updateTime = new System.Windows.Forms.Label();
            this.conflictEditSplitter = new System.Windows.Forms.SplitContainer();
            this.conflictView = new Ankh.UI.PendingChanges.Conflicts.ConflictListView();
            this.resolvePannel = new System.Windows.Forms.FlowLayoutPanel();
            this.resolveButton0 = new System.Windows.Forms.Button();
            this.resolveButton1 = new System.Windows.Forms.Button();
            this.resolveButton2 = new System.Windows.Forms.Button();
            this.resolveButton3 = new System.Windows.Forms.Button();
            this.resolveButton4 = new System.Windows.Forms.Button();
            this.resolveButton5 = new System.Windows.Forms.Button();
            this.resolveButton6 = new System.Windows.Forms.Button();
            this.resolveButton7 = new System.Windows.Forms.Button();
            this.resolveTopLabel = new System.Windows.Forms.Label();
            this.resolveBottomLabel = new System.Windows.Forms.Label();
            this.resolveLinkLabel = new System.Windows.Forms.LinkLabel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.topLayoutPanel.SuspendLayout();
            this.conflictEditSplitter.Panel1.SuspendLayout();
            this.conflictEditSplitter.Panel2.SuspendLayout();
            this.conflictEditSplitter.SuspendLayout();
            this.resolvePannel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.conflictEditSplitter);
            this.splitContainer1.Size = new System.Drawing.Size(768, 300);
            this.splitContainer1.SplitterDistance = 25;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.topLayoutPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 25);
            this.panel1.TabIndex = 2;
            // 
            // topLayoutPanel
            // 
            this.topLayoutPanel.ColumnCount = 5;
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.topLayoutPanel.Controls.Add(this.lastRevBox, 2, 0);
            this.topLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.topLayoutPanel.Controls.Add(this.lastRevLabel, 1, 0);
            this.topLayoutPanel.Controls.Add(this.updateTime, 4, 0);
            this.topLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.topLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.topLayoutPanel.Name = "topLayoutPanel";
            this.topLayoutPanel.RowCount = 1;
            this.topLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.topLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topLayoutPanel.Size = new System.Drawing.Size(764, 21);
            this.topLayoutPanel.TabIndex = 1;
            // 
            // lastRevBox
            // 
            this.lastRevBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lastRevBox.Enabled = false;
            this.lastRevBox.Location = new System.Drawing.Point(311, 3);
            this.lastRevBox.Name = "lastRevBox";
            this.lastRevBox.ReadOnly = true;
            this.lastRevBox.Size = new System.Drawing.Size(74, 13);
            this.lastRevBox.TabIndex = 6;
            this.lastRevBox.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Conflicts:";
            // 
            // lastRevLabel
            // 
            this.lastRevLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lastRevLabel.AutoSize = true;
            this.lastRevLabel.Location = new System.Drawing.Point(239, 3);
            this.lastRevLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lastRevLabel.Name = "lastRevLabel";
            this.lastRevLabel.Size = new System.Drawing.Size(66, 13);
            this.lastRevLabel.TabIndex = 5;
            this.lastRevLabel.Text = "Last update:";
            this.lastRevLabel.Visible = false;
            // 
            // updateTime
            // 
            this.updateTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.updateTime.AutoSize = true;
            this.updateTime.Location = new System.Drawing.Point(761, 3);
            this.updateTime.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.updateTime.Name = "updateTime";
            this.updateTime.Size = new System.Drawing.Size(0, 13);
            this.updateTime.TabIndex = 7;
            // 
            // conflictEditSplitter
            // 
            this.conflictEditSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conflictEditSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.conflictEditSplitter.IsSplitterFixed = true;
            this.conflictEditSplitter.Location = new System.Drawing.Point(0, 0);
            this.conflictEditSplitter.Margin = new System.Windows.Forms.Padding(0);
            this.conflictEditSplitter.Name = "conflictEditSplitter";
            this.conflictEditSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // conflictEditSplitter.Panel1
            // 
            this.conflictEditSplitter.Panel1.Controls.Add(this.conflictView);
            // 
            // conflictEditSplitter.Panel2
            // 
            this.conflictEditSplitter.Panel2.Controls.Add(this.resolvePannel);
            this.conflictEditSplitter.Panel2.MouseEnter += new System.EventHandler(this.conflictEditSplitter_Panel2_MouseEnter);
            this.conflictEditSplitter.Panel2MinSize = 32;
            this.conflictEditSplitter.Size = new System.Drawing.Size(768, 274);
            this.conflictEditSplitter.SplitterDistance = 220;
            this.conflictEditSplitter.SplitterWidth = 2;
            this.conflictEditSplitter.TabIndex = 0;
            // 
            // conflictView
            // 
            this.conflictView.Context = null;
            this.conflictView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conflictView.Location = new System.Drawing.Point(0, 0);
            this.conflictView.Margin = new System.Windows.Forms.Padding(0);
            this.conflictView.Name = "conflictView";
            this.conflictView.Size = new System.Drawing.Size(768, 220);
            this.conflictView.TabIndex = 1;
            // 
            // resolvePannel
            // 
            this.resolvePannel.BackColor = System.Drawing.Color.SkyBlue;
            this.resolvePannel.Controls.Add(this.resolveButton0);
            this.resolvePannel.Controls.Add(this.resolveButton1);
            this.resolvePannel.Controls.Add(this.resolveButton2);
            this.resolvePannel.Controls.Add(this.resolveButton3);
            this.resolvePannel.Controls.Add(this.resolveButton4);
            this.resolvePannel.Controls.Add(this.resolveButton5);
            this.resolvePannel.Controls.Add(this.resolveButton6);
            this.resolvePannel.Controls.Add(this.resolveButton7);
            this.resolvePannel.Controls.Add(this.resolveTopLabel);
            this.resolvePannel.Controls.Add(this.resolveBottomLabel);
            this.resolvePannel.Controls.Add(this.resolveLinkLabel);
            this.resolvePannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resolvePannel.Enabled = false;
            this.resolvePannel.Location = new System.Drawing.Point(0, 0);
            this.resolvePannel.Name = "resolvePannel";
            this.resolvePannel.Size = new System.Drawing.Size(768, 52);
            this.resolvePannel.TabIndex = 0;
            // 
            // resolveButton0
            // 
            this.resolveButton0.AutoSize = true;
            this.resolveButton0.Enabled = false;
            this.resolveButton0.Location = new System.Drawing.Point(3, 1);
            this.resolveButton0.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton0.Name = "resolveButton0";
            this.resolveButton0.Size = new System.Drawing.Size(75, 23);
            this.resolveButton0.TabIndex = 0;
            this.resolveButton0.UseVisualStyleBackColor = true;
            // 
            // resolveButton1
            // 
            this.resolveButton1.AutoSize = true;
            this.resolveButton1.Enabled = false;
            this.resolveButton1.Location = new System.Drawing.Point(84, 1);
            this.resolveButton1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton1.Name = "resolveButton1";
            this.resolveButton1.Size = new System.Drawing.Size(75, 23);
            this.resolveButton1.TabIndex = 1;
            this.resolveButton1.UseVisualStyleBackColor = true;
            // 
            // resolveButton2
            // 
            this.resolveButton2.AutoSize = true;
            this.resolveButton2.Enabled = false;
            this.resolveButton2.Location = new System.Drawing.Point(165, 1);
            this.resolveButton2.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton2.Name = "resolveButton2";
            this.resolveButton2.Size = new System.Drawing.Size(75, 23);
            this.resolveButton2.TabIndex = 2;
            this.resolveButton2.UseVisualStyleBackColor = true;
            // 
            // resolveButton3
            // 
            this.resolveButton3.AutoSize = true;
            this.resolveButton3.Enabled = false;
            this.resolveButton3.Location = new System.Drawing.Point(246, 1);
            this.resolveButton3.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton3.Name = "resolveButton3";
            this.resolveButton3.Size = new System.Drawing.Size(75, 23);
            this.resolveButton3.TabIndex = 3;
            this.resolveButton3.UseVisualStyleBackColor = true;
            // 
            // resolveButton4
            // 
            this.resolveButton4.AutoSize = true;
            this.resolveButton4.Enabled = false;
            this.resolveButton4.Location = new System.Drawing.Point(327, 1);
            this.resolveButton4.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton4.Name = "resolveButton4";
            this.resolveButton4.Size = new System.Drawing.Size(75, 23);
            this.resolveButton4.TabIndex = 4;
            this.resolveButton4.UseVisualStyleBackColor = true;
            // 
            // resolveButton5
            // 
            this.resolveButton5.AutoSize = true;
            this.resolveButton5.Enabled = false;
            this.resolveButton5.Location = new System.Drawing.Point(408, 1);
            this.resolveButton5.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton5.Name = "resolveButton5";
            this.resolveButton5.Size = new System.Drawing.Size(75, 23);
            this.resolveButton5.TabIndex = 5;
            this.resolveButton5.UseVisualStyleBackColor = true;
            // 
            // resolveButton6
            // 
            this.resolveButton6.Enabled = false;
            this.resolveButton6.Location = new System.Drawing.Point(489, 3);
            this.resolveButton6.Name = "resolveButton6";
            this.resolveButton6.Size = new System.Drawing.Size(75, 23);
            this.resolveButton6.TabIndex = 9;
            this.resolveButton6.UseVisualStyleBackColor = true;
            // 
            // resolveButton7
            // 
            this.resolveButton7.Enabled = false;
            this.resolvePannel.SetFlowBreak(this.resolveButton7, true);
            this.resolveButton7.Location = new System.Drawing.Point(570, 3);
            this.resolveButton7.Name = "resolveButton7";
            this.resolveButton7.Size = new System.Drawing.Size(75, 23);
            this.resolveButton7.TabIndex = 10;
            this.resolveButton7.UseVisualStyleBackColor = true;
            // 
            // resolveTopLabel
            // 
            this.resolveTopLabel.AutoSize = true;
            this.resolvePannel.SetFlowBreak(this.resolveTopLabel, true);
            this.resolveTopLabel.Location = new System.Drawing.Point(3, 29);
            this.resolveTopLabel.Name = "resolveTopLabel";
            this.resolveTopLabel.Size = new System.Drawing.Size(86, 13);
            this.resolveTopLabel.TabIndex = 6;
            this.resolveTopLabel.Text = "resolveTopLabel";
            // 
            // resolveBottomLabel
            // 
            this.resolveBottomLabel.AutoSize = true;
            this.resolveBottomLabel.Location = new System.Drawing.Point(3, 42);
            this.resolveBottomLabel.Name = "resolveBottomLabel";
            this.resolveBottomLabel.Size = new System.Drawing.Size(35, 13);
            this.resolveBottomLabel.TabIndex = 7;
            this.resolveBottomLabel.Text = "label3";
            // 
            // resolveLinkLabel
            // 
            this.resolveLinkLabel.AutoSize = true;
            this.resolveLinkLabel.Location = new System.Drawing.Point(44, 42);
            this.resolveLinkLabel.Name = "resolveLinkLabel";
            this.resolveLinkLabel.Size = new System.Drawing.Size(0, 13);
            this.resolveLinkLabel.TabIndex = 8;
            // 
            // PendingConflictsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PendingConflictsPage";
            this.Text = "Conflicts";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.topLayoutPanel.ResumeLayout(false);
            this.topLayoutPanel.PerformLayout();
            this.conflictEditSplitter.Panel1.ResumeLayout(false);
            this.conflictEditSplitter.Panel2.ResumeLayout(false);
            this.conflictEditSplitter.ResumeLayout(false);
            this.resolvePannel.ResumeLayout(false);
            this.resolvePannel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel topLayoutPanel;
        private System.Windows.Forms.TextBox lastRevBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lastRevLabel;
        private System.Windows.Forms.Label updateTime;
        private System.Windows.Forms.SplitContainer conflictEditSplitter;
        private Ankh.UI.PendingChanges.Conflicts.ConflictListView conflictView;
        private System.Windows.Forms.FlowLayoutPanel resolvePannel;
        private System.Windows.Forms.Button resolveButton0;
        private System.Windows.Forms.Button resolveButton1;
        private System.Windows.Forms.Button resolveButton2;
        private System.Windows.Forms.Button resolveButton3;
        private System.Windows.Forms.Button resolveButton4;
        private System.Windows.Forms.Button resolveButton5;
        private System.Windows.Forms.Label resolveTopLabel;
        private System.Windows.Forms.Label resolveBottomLabel;
        private System.Windows.Forms.LinkLabel resolveLinkLabel;
        private System.Windows.Forms.Button resolveButton6;
        private System.Windows.Forms.Button resolveButton7;



    }
}
