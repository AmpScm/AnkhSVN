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

using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.SccManagement
{
    partial class LockDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LockDialog));
            this.suppressLabel = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.stealLocksCheckBox = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new Ankh.UI.VSSelectionControls.SmartSplitContainer(this.components);
            this.pendingList = new Ankh.UI.PendingChanges.Commits.PendingCommitsView(this.components);
            this.logMessage = new Ankh.UI.PendingChanges.LogMessageEditor(this.components);
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // suppressLabel
            // 
            resources.ApplyResources(this.suppressLabel, "suppressLabel");
            this.suppressLabel.Name = "suppressLabel";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // stealLocksCheckBox
            // 
            resources.ApplyResources(this.stealLocksCheckBox, "stealLocksCheckBox");
            this.stealLocksCheckBox.Name = "stealLocksCheckBox";
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
            // 
            // logMessage
            // 
            resources.ApplyResources(this.logMessage, "logMessage");
            this.logMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.logMessage.Name = "logMessage";
            this.logMessage.PendingChangeUI = this.pendingList;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // LockDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.suppressLabel);
            this.Controls.Add(this.stealLocksCheckBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.btnCancel);
            this.EnableTheming = true;
            this.Name = "LockDialog";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        protected System.Windows.Forms.Label suppressLabel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        protected System.Windows.Forms.Button btnCancel;
        protected System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox stealLocksCheckBox;
        private Ankh.UI.VSSelectionControls.SmartSplitContainer splitContainer1;
        private PendingChanges.Commits.PendingCommitsView pendingList;
        private PendingChanges.LogMessageEditor logMessage;
        private System.Windows.Forms.Label label1;
    }
}
