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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecentChangesPage));
			this.syncView = new Ankh.UI.PendingChanges.Synchronize.SynchronizeListView();
			this.topLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.updateTime = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.refreshCombo = new System.Windows.Forms.ComboBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.panel1 = new System.Windows.Forms.Panel();
			this.topLayoutPanel.SuspendLayout();
			this.panel2.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// syncView
			// 
			this.syncView.Context = null;
			resources.ApplyResources(this.syncView, "syncView");
			this.syncView.Name = "syncView";
			// 
			// topLayoutPanel
			// 
			resources.ApplyResources(this.topLayoutPanel, "topLayoutPanel");
			this.topLayoutPanel.Controls.Add(this.updateTime, 2, 0);
			this.topLayoutPanel.Controls.Add(this.label1, 0, 0);
			this.topLayoutPanel.Controls.Add(this.panel2, 4, 0);
			this.topLayoutPanel.Name = "topLayoutPanel";
			// 
			// updateTime
			// 
			this.updateTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this.updateTime, "updateTime");
			this.updateTime.Name = "updateTime";
			this.updateTime.ReadOnly = true;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.refreshCombo);
			this.panel2.Controls.Add(this.checkBox1);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			// 
			// refreshCombo
			// 
			this.refreshCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.refreshCombo, "refreshCombo");
			this.refreshCombo.FormattingEnabled = true;
			this.refreshCombo.Items.AddRange(new object[] {
            resources.GetString("refreshCombo.Items"),
            resources.GetString("refreshCombo.Items1"),
            resources.GetString("refreshCombo.Items2"),
            resources.GetString("refreshCombo.Items3"),
            resources.GetString("refreshCombo.Items4"),
            resources.GetString("refreshCombo.Items5"),
            resources.GetString("refreshCombo.Items6")});
			this.refreshCombo.Name = "refreshCombo";
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
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
			this.splitContainer1.Panel2.Controls.Add(this.syncView);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.topLayoutPanel);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// RecentChangesPage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "RecentChangesPage";
			this.topLayoutPanel.ResumeLayout(false);
			this.topLayoutPanel.PerformLayout();
			this.panel2.ResumeLayout(false);
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
        private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox updateTime;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ComboBox refreshCombo;
		private System.Windows.Forms.CheckBox checkBox1;

    }
}
