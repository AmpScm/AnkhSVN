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
    partial class RecentChangesPage
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
            this.syncView = new Ankh.UI.PendingChanges.Synchronize.SynchronizeListView();
            this.topLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lastRevBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lastRevLabel = new System.Windows.Forms.Label();
            this.updateTime = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.topLayoutPanel.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // syncView
            // 
            this.syncView.Context = null;
            this.syncView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.syncView.Location = new System.Drawing.Point(0, 0);
            this.syncView.Margin = new System.Windows.Forms.Padding(0);
            this.syncView.Name = "syncView";
            this.syncView.Size = new System.Drawing.Size(768, 276);
            this.syncView.TabIndex = 0;
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
            this.topLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.topLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topLayoutPanel.Size = new System.Drawing.Size(766, 21);
            this.topLayoutPanel.TabIndex = 1;
            // 
            // lastRevBox
            // 
            this.lastRevBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lastRevBox.Enabled = false;
            this.lastRevBox.Location = new System.Drawing.Point(312, 3);
            this.lastRevBox.Name = "lastRevBox";
            this.lastRevBox.ReadOnly = true;
            this.lastRevBox.Size = new System.Drawing.Size(74, 13);
            this.lastRevBox.TabIndex = 6;
            this.lastRevBox.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 2, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Changes:";
            // 
            // lastRevLabel
            // 
            this.lastRevLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lastRevLabel.AutoSize = true;
            this.lastRevLabel.Location = new System.Drawing.Point(240, 2);
            this.lastRevLabel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
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
            this.updateTime.Location = new System.Drawing.Point(763, 3);
            this.updateTime.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.updateTime.Name = "updateTime";
            this.updateTime.Size = new System.Drawing.Size(0, 13);
            this.updateTime.TabIndex = 7;
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
            this.splitContainer1.Panel2.Controls.Add(this.syncView);
            this.splitContainer1.Size = new System.Drawing.Size(768, 300);
            this.splitContainer1.SplitterDistance = 23;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.topLayoutPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 23);
            this.panel1.TabIndex = 2;
            // 
            // RecentChangesPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "RecentChangesPage";
            this.Text = "Recent Changes";
            this.topLayoutPanel.ResumeLayout(false);
            this.topLayoutPanel.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Ankh.UI.PendingChanges.Synchronize.SynchronizeListView syncView;
        private System.Windows.Forms.TableLayoutPanel topLayoutPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lastRevLabel;
        private System.Windows.Forms.TextBox lastRevBox;
        private System.Windows.Forms.Label updateTime;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;

    }
}
