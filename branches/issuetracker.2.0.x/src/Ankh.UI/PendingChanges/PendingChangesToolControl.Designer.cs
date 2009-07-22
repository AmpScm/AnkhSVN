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
    partial class PendingChangesToolControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PendingChangesToolControl));
            this.pendingChangesTabs = new System.Windows.Forms.ToolStrip();
            this.fileChangesButton = new System.Windows.Forms.ToolStripButton();
            this.issuesButton = new System.Windows.Forms.ToolStripButton();
            this.recentChangesButton = new System.Windows.Forms.ToolStripButton();
            this.conflictsButton = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pendingChangesTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // pendingChangesTabs
            // 
            this.pendingChangesTabs.Dock = System.Windows.Forms.DockStyle.Left;
            this.pendingChangesTabs.GripMargin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.pendingChangesTabs.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.pendingChangesTabs.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.pendingChangesTabs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileChangesButton,
            this.issuesButton,
            this.recentChangesButton,
            this.conflictsButton});
            this.pendingChangesTabs.Location = new System.Drawing.Point(0, 0);
            this.pendingChangesTabs.Name = "pendingChangesTabs";
            this.pendingChangesTabs.Padding = new System.Windows.Forms.Padding(0);
            this.pendingChangesTabs.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.pendingChangesTabs.Size = new System.Drawing.Size(36, 300);
            this.pendingChangesTabs.TabIndex = 0;
            this.pendingChangesTabs.TabStop = true;
            this.pendingChangesTabs.Text = "toolStrip1";
            // 
            // fileChangesButton
            // 
            this.fileChangesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fileChangesButton.Image = ((System.Drawing.Image)(resources.GetObject("fileChangesButton.Image")));
            this.fileChangesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileChangesButton.Name = "fileChangesButton";
            this.fileChangesButton.Size = new System.Drawing.Size(35, 36);
            this.fileChangesButton.Text = "Local File Changes";
            this.fileChangesButton.Click += new System.EventHandler(this.fileChangesButton_Click);
            // 
            // issuesButton
            // 
            this.issuesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.issuesButton.Enabled = true;
            this.issuesButton.Image = ((System.Drawing.Image)(resources.GetObject("issuesButton.Image")));
            this.issuesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.issuesButton.Name = "issuesButton";
            this.issuesButton.Size = new System.Drawing.Size(35, 36);
            this.issuesButton.Text = "Issues";
            this.issuesButton.Click += new System.EventHandler(this.issuesButton_Click);
            // 
            // recentChangesButton
            // 
            this.recentChangesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.recentChangesButton.Image = ((System.Drawing.Image)(resources.GetObject("recentChangesButton.Image")));
            this.recentChangesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.recentChangesButton.Name = "recentChangesButton";
            this.recentChangesButton.Size = new System.Drawing.Size(35, 36);
            this.recentChangesButton.Text = "Recent Changes";
            this.recentChangesButton.Click += new System.EventHandler(this.recentChangesButton_Click);
            // 
            // conflictsButton
            // 
            this.conflictsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.conflictsButton.Image = ((System.Drawing.Image)(resources.GetObject("conflictsButton.Image")));
            this.conflictsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.conflictsButton.Name = "conflictsButton";
            this.conflictsButton.Size = new System.Drawing.Size(35, 36);
            this.conflictsButton.Text = "Conflicts";
            this.conflictsButton.Click += new System.EventHandler(this.conflictsButton_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(36, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 300);
            this.panel1.TabIndex = 1;
            // 
            // PendingChangesToolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 300);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pendingChangesTabs);
            this.Name = "PendingChangesToolControl";
            this.pendingChangesTabs.ResumeLayout(false);
            this.pendingChangesTabs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip pendingChangesTabs;
        private System.Windows.Forms.ToolStripButton fileChangesButton;
        private System.Windows.Forms.ToolStripButton issuesButton;
        private System.Windows.Forms.ToolStripButton recentChangesButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton conflictsButton;
    }
}
