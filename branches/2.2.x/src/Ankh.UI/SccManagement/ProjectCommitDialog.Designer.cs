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

namespace Ankh.UI.SccManagement
{
    partial class ProjectCommitDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectCommitDialog));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pendingList = new Ankh.UI.PendingChanges.Commits.PendingCommitsView(this.components);
            this.issueNumberBox = new System.Windows.Forms.TextBox();
            this.issueLabel = new System.Windows.Forms.Label();
            this.logMessage = new Ankh.UI.PendingChanges.LogMessageEditor(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.keepLocksBox = new System.Windows.Forms.CheckBox();
            this.keepChangelistsBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pendingList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.issueNumberBox);
            this.splitContainer1.Panel2.Controls.Add(this.issueLabel);
            this.splitContainer1.Panel2.Controls.Add(this.logMessage);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            // 
            // pendingList
            // 
            this.pendingList.AllowColumnReorder = true;
            this.pendingList.CheckBoxes = true;
            resources.ApplyResources(this.pendingList, "pendingList");
            this.pendingList.HideSelection = false;
            this.pendingList.Name = "pendingList";
            this.pendingList.ShowSelectAllCheckBox = true;
            this.pendingList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.pendingList_ItemChecked);
            this.pendingList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pendingList_MouseDoubleClick);
            // 
            // issueNumberBox
            // 
            resources.ApplyResources(this.issueNumberBox, "issueNumberBox");
            this.issueNumberBox.Name = "issueNumberBox";
            this.issueNumberBox.TextChanged += new System.EventHandler(this.issueNumberBox_TextChanged);
            this.issueNumberBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.issueNumberBox_KeyPress);
            // 
            // issueLabel
            // 
            resources.ApplyResources(this.issueLabel, "issueLabel");
            this.issueLabel.Name = "issueLabel";
            // 
            // logMessage
            // 
            resources.ApplyResources(this.logMessage, "logMessage");
            this.logMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logMessage.Name = "logMessage";
            this.logMessage.PasteSource = this.pendingList;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // keepLocksBox
            // 
            resources.ApplyResources(this.keepLocksBox, "keepLocksBox");
            this.keepLocksBox.Name = "keepLocksBox";
            this.keepLocksBox.UseVisualStyleBackColor = true;
            // 
            // keepChangelistsBox
            // 
            resources.ApplyResources(this.keepChangelistsBox, "keepChangelistsBox");
            this.keepChangelistsBox.Name = "keepChangelistsBox";
            this.keepChangelistsBox.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // ProjectCommitDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.keepChangelistsBox);
            this.Controls.Add(this.keepLocksBox);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ProjectCommitDialog";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Ankh.UI.PendingChanges.Commits.PendingCommitsView pendingList;
        private Ankh.UI.PendingChanges.LogMessageEditor logMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox issueNumberBox;
        private System.Windows.Forms.Label issueLabel;
        private System.Windows.Forms.CheckBox keepLocksBox;
        private System.Windows.Forms.CheckBox keepChangelistsBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}
