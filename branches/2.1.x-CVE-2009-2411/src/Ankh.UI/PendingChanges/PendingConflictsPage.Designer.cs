// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.conflictHeader = new System.Windows.Forms.Label();
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
            this.splitContainer1.Panel1MinSize = 23;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.conflictEditSplitter);
            this.splitContainer1.Size = new System.Drawing.Size(768, 300);
            this.splitContainer1.SplitterDistance = 23;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Info;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.conflictHeader);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 23);
            this.panel1.TabIndex = 2;
            // 
            // conflictHeader
            // 
            this.conflictHeader.AutoEllipsis = true;
            this.conflictHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conflictHeader.Location = new System.Drawing.Point(0, 0);
            this.conflictHeader.Margin = new System.Windows.Forms.Padding(2, 0, 3, 0);
            this.conflictHeader.Name = "conflictHeader";
            this.conflictHeader.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.conflictHeader.Size = new System.Drawing.Size(766, 21);
            this.conflictHeader.TabIndex = 0;
            this.conflictHeader.Text = "0 Conflicts: 0 Version, 0 Tree Conflicts; 0 Automatic merges.";
            this.conflictHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.conflictEditSplitter.Panel2MinSize = 32;
            this.conflictEditSplitter.Size = new System.Drawing.Size(768, 276);
            this.conflictEditSplitter.SplitterDistance = 238;
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
            this.conflictView.Size = new System.Drawing.Size(768, 238);
            this.conflictView.TabIndex = 0;
            // 
            // resolvePannel
            // 
            this.resolvePannel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
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
            this.resolvePannel.Location = new System.Drawing.Point(0, 0);
            this.resolvePannel.Name = "resolvePannel";
            this.resolvePannel.Size = new System.Drawing.Size(768, 36);
            this.resolvePannel.TabIndex = 0;
            // 
            // resolveButton0
            // 
            this.resolveButton0.AutoSize = true;
            this.resolveButton0.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resolveButton0.Enabled = false;
            this.resolveButton0.Location = new System.Drawing.Point(3, 1);
            this.resolveButton0.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton0.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton0.Name = "resolveButton0";
            this.resolveButton0.Size = new System.Drawing.Size(75, 23);
            this.resolveButton0.TabIndex = 0;
            this.resolveButton0.Text = " ";
            this.resolveButton0.UseVisualStyleBackColor = true;
            // 
            // resolveButton1
            // 
            this.resolveButton1.AutoSize = true;
            this.resolveButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resolveButton1.Enabled = false;
            this.resolveButton1.Location = new System.Drawing.Point(84, 1);
            this.resolveButton1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton1.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton1.Name = "resolveButton1";
            this.resolveButton1.Size = new System.Drawing.Size(75, 23);
            this.resolveButton1.TabIndex = 1;
            this.resolveButton1.Text = " ";
            this.resolveButton1.UseVisualStyleBackColor = true;
            // 
            // resolveButton2
            // 
            this.resolveButton2.AutoSize = true;
            this.resolveButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resolveButton2.Enabled = false;
            this.resolveButton2.Location = new System.Drawing.Point(165, 1);
            this.resolveButton2.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton2.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton2.Name = "resolveButton2";
            this.resolveButton2.Size = new System.Drawing.Size(75, 23);
            this.resolveButton2.TabIndex = 2;
            this.resolveButton2.Text = " ";
            this.resolveButton2.UseVisualStyleBackColor = true;
            // 
            // resolveButton3
            // 
            this.resolveButton3.AutoSize = true;
            this.resolveButton3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resolveButton3.Enabled = false;
            this.resolveButton3.Location = new System.Drawing.Point(246, 1);
            this.resolveButton3.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton3.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton3.Name = "resolveButton3";
            this.resolveButton3.Size = new System.Drawing.Size(75, 23);
            this.resolveButton3.TabIndex = 3;
            this.resolveButton3.Text = " ";
            this.resolveButton3.UseVisualStyleBackColor = true;
            // 
            // resolveButton4
            // 
            this.resolveButton4.AutoSize = true;
            this.resolveButton4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resolveButton4.Enabled = false;
            this.resolveButton4.Location = new System.Drawing.Point(327, 1);
            this.resolveButton4.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton4.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton4.Name = "resolveButton4";
            this.resolveButton4.Size = new System.Drawing.Size(75, 23);
            this.resolveButton4.TabIndex = 4;
            this.resolveButton4.Text = " ";
            this.resolveButton4.UseVisualStyleBackColor = true;
            // 
            // resolveButton5
            // 
            this.resolveButton5.AutoSize = true;
            this.resolveButton5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resolveButton5.Enabled = false;
            this.resolveButton5.Location = new System.Drawing.Point(408, 1);
            this.resolveButton5.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton5.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton5.Name = "resolveButton5";
            this.resolveButton5.Size = new System.Drawing.Size(75, 23);
            this.resolveButton5.TabIndex = 5;
            this.resolveButton5.Text = " ";
            this.resolveButton5.UseVisualStyleBackColor = true;
            // 
            // resolveButton6
            // 
            this.resolveButton6.AutoSize = true;
            this.resolveButton6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resolveButton6.Enabled = false;
            this.resolveButton6.Location = new System.Drawing.Point(489, 1);
            this.resolveButton6.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton6.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton6.Name = "resolveButton6";
            this.resolveButton6.Size = new System.Drawing.Size(75, 23);
            this.resolveButton6.TabIndex = 6;
            this.resolveButton6.Text = " ";
            this.resolveButton6.UseVisualStyleBackColor = true;
            // 
            // resolveButton7
            // 
            this.resolveButton7.AutoSize = true;
            this.resolveButton7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resolveButton7.Enabled = false;
            this.resolvePannel.SetFlowBreak(this.resolveButton7, true);
            this.resolveButton7.Location = new System.Drawing.Point(570, 1);
            this.resolveButton7.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.resolveButton7.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton7.Name = "resolveButton7";
            this.resolveButton7.Size = new System.Drawing.Size(75, 23);
            this.resolveButton7.TabIndex = 7;
            this.resolveButton7.Text = " ";
            this.resolveButton7.UseVisualStyleBackColor = true;
            // 
            // resolveTopLabel
            // 
            this.resolveTopLabel.AutoSize = true;
            this.resolvePannel.SetFlowBreak(this.resolveTopLabel, true);
            this.resolveTopLabel.Location = new System.Drawing.Point(3, 25);
            this.resolveTopLabel.Name = "resolveTopLabel";
            this.resolveTopLabel.Size = new System.Drawing.Size(86, 13);
            this.resolveTopLabel.TabIndex = 8;
            this.resolveTopLabel.Text = "resolveTopLabel";
            this.resolveTopLabel.UseMnemonic = false;
            // 
            // resolveBottomLabel
            // 
            this.resolveBottomLabel.AutoSize = true;
            this.resolveBottomLabel.Location = new System.Drawing.Point(3, 38);
            this.resolveBottomLabel.Name = "resolveBottomLabel";
            this.resolveBottomLabel.Size = new System.Drawing.Size(35, 13);
            this.resolveBottomLabel.TabIndex = 9;
            this.resolveBottomLabel.Text = "label3";
            // 
            // resolveLinkLabel
            // 
            this.resolveLinkLabel.AutoSize = true;
            this.resolveLinkLabel.Location = new System.Drawing.Point(44, 38);
            this.resolveLinkLabel.Name = "resolveLinkLabel";
            this.resolveLinkLabel.Size = new System.Drawing.Size(0, 13);
            this.resolveLinkLabel.TabIndex = 10;
            // 
            // PendingConflictsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PendingConflictsPage";
            this.Text = "Conflicts and Merges";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
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
        private System.Windows.Forms.Label conflictHeader;



    }
}
