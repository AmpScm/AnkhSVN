// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PendingConflictsPage));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.conflictHeader = new System.Windows.Forms.Label();
            this.conflictEditSplitter = new System.Windows.Forms.SplitContainer();
            this.conflictView = new Ankh.UI.PendingChanges.Conflicts.ConflictListView();
            this.resolvePanel = new System.Windows.Forms.FlowLayoutPanel();
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
            this.resolvePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.conflictEditSplitter);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.conflictHeader);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // conflictHeader
            // 
            this.conflictHeader.AutoEllipsis = true;
            resources.ApplyResources(this.conflictHeader, "conflictHeader");
            this.conflictHeader.Name = "conflictHeader";
            // 
            // conflictEditSplitter
            // 
            resources.ApplyResources(this.conflictEditSplitter, "conflictEditSplitter");
            this.conflictEditSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.conflictEditSplitter.Name = "conflictEditSplitter";
            // 
            // conflictEditSplitter.Panel1
            // 
            this.conflictEditSplitter.Panel1.Controls.Add(this.conflictView);
            // 
            // conflictEditSplitter.Panel2
            // 
            this.conflictEditSplitter.Panel2.Controls.Add(this.resolvePanel);
            // 
            // conflictView
            // 
            this.conflictView.Context = null;
            resources.ApplyResources(this.conflictView, "conflictView");
            this.conflictView.Name = "conflictView";
            // 
            // resolvePanel
            // 
            resources.ApplyResources(this.resolvePanel, "resolvePanel");
            this.resolvePanel.BackColor = System.Drawing.Color.SkyBlue;
            this.resolvePanel.Controls.Add(this.resolveButton0);
            this.resolvePanel.Controls.Add(this.resolveButton1);
            this.resolvePanel.Controls.Add(this.resolveButton2);
            this.resolvePanel.Controls.Add(this.resolveButton3);
            this.resolvePanel.Controls.Add(this.resolveButton4);
            this.resolvePanel.Controls.Add(this.resolveButton5);
            this.resolvePanel.Controls.Add(this.resolveButton6);
            this.resolvePanel.Controls.Add(this.resolveButton7);
            this.resolvePanel.Controls.Add(this.resolveTopLabel);
            this.resolvePanel.Controls.Add(this.resolveBottomLabel);
            this.resolvePanel.Controls.Add(this.resolveLinkLabel);
            this.resolvePanel.Name = "resolvePanel";
            // 
            // resolveButton0
            // 
            resources.ApplyResources(this.resolveButton0, "resolveButton0");
            this.resolveButton0.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton0.Name = "resolveButton0";
            this.resolveButton0.UseVisualStyleBackColor = true;
            // 
            // resolveButton1
            // 
            resources.ApplyResources(this.resolveButton1, "resolveButton1");
            this.resolveButton1.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton1.Name = "resolveButton1";
            this.resolveButton1.UseVisualStyleBackColor = true;
            // 
            // resolveButton2
            // 
            resources.ApplyResources(this.resolveButton2, "resolveButton2");
            this.resolveButton2.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton2.Name = "resolveButton2";
            this.resolveButton2.UseVisualStyleBackColor = true;
            // 
            // resolveButton3
            // 
            resources.ApplyResources(this.resolveButton3, "resolveButton3");
            this.resolveButton3.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton3.Name = "resolveButton3";
            this.resolveButton3.UseVisualStyleBackColor = true;
            // 
            // resolveButton4
            // 
            resources.ApplyResources(this.resolveButton4, "resolveButton4");
            this.resolveButton4.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton4.Name = "resolveButton4";
            this.resolveButton4.UseVisualStyleBackColor = true;
            // 
            // resolveButton5
            // 
            resources.ApplyResources(this.resolveButton5, "resolveButton5");
            this.resolveButton5.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton5.Name = "resolveButton5";
            this.resolveButton5.UseVisualStyleBackColor = true;
            // 
            // resolveButton6
            // 
            resources.ApplyResources(this.resolveButton6, "resolveButton6");
            this.resolveButton6.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton6.Name = "resolveButton6";
            this.resolveButton6.UseVisualStyleBackColor = true;
            // 
            // resolveButton7
            // 
            resources.ApplyResources(this.resolveButton7, "resolveButton7");
            this.resolvePanel.SetFlowBreak(this.resolveButton7, true);
            this.resolveButton7.MinimumSize = new System.Drawing.Size(75, 23);
            this.resolveButton7.Name = "resolveButton7";
            this.resolveButton7.UseVisualStyleBackColor = true;
            // 
            // resolveTopLabel
            // 
            resources.ApplyResources(this.resolveTopLabel, "resolveTopLabel");
            this.resolvePanel.SetFlowBreak(this.resolveTopLabel, true);
            this.resolveTopLabel.Name = "resolveTopLabel";
            this.resolveTopLabel.UseMnemonic = false;
            // 
            // resolveBottomLabel
            // 
            resources.ApplyResources(this.resolveBottomLabel, "resolveBottomLabel");
            this.resolveBottomLabel.Name = "resolveBottomLabel";
            // 
            // resolveLinkLabel
            // 
            resources.ApplyResources(this.resolveLinkLabel, "resolveLinkLabel");
            this.resolveLinkLabel.Name = "resolveLinkLabel";
            // 
            // PendingConflictsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PendingConflictsPage";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.conflictEditSplitter.Panel1.ResumeLayout(false);
            this.conflictEditSplitter.Panel2.ResumeLayout(false);
            this.conflictEditSplitter.ResumeLayout(false);
            this.resolvePanel.ResumeLayout(false);
            this.resolvePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer conflictEditSplitter;
        private Ankh.UI.PendingChanges.Conflicts.ConflictListView conflictView;
        private System.Windows.Forms.FlowLayoutPanel resolvePanel;
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
