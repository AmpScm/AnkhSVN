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

namespace Ankh.UI.PathSelector
{
    partial class PathSelector
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PathSelector));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.suppressLabel = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.fromLabel = new System.Windows.Forms.Label();
            this.toLabel = new System.Windows.Forms.Label();
            this.fromPanel = new System.Windows.Forms.Panel();
            this.revisionPickerStart = new Ankh.UI.PathSelector.VersionSelector();
            this.toPanel = new System.Windows.Forms.Panel();
            this.revisionPickerEnd = new Ankh.UI.PathSelector.VersionSelector();
            this.pendingList = new Ankh.UI.PendingChanges.Commits.PendingCommitsView(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.bottomPanel.SuspendLayout();
            this.fromPanel.SuspendLayout();
            this.toPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // suppressLabel
            // 
            resources.ApplyResources(this.suppressLabel, "suppressLabel");
            this.suppressLabel.Name = "suppressLabel";
            this.suppressLabel.Click += new System.EventHandler(this.suppressLabel_Click);
            // 
            // bottomPanel
            // 
            resources.ApplyResources(this.bottomPanel, "bottomPanel");
            this.bottomPanel.Controls.Add(this.suppressLabel);
            this.bottomPanel.Controls.Add(this.okButton);
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Name = "bottomPanel";
            // 
            // fromLabel
            // 
            resources.ApplyResources(this.fromLabel, "fromLabel");
            this.fromLabel.Name = "fromLabel";
            // 
            // toLabel
            // 
            resources.ApplyResources(this.toLabel, "toLabel");
            this.toLabel.Name = "toLabel";
            // 
            // fromPanel
            // 
            resources.ApplyResources(this.fromPanel, "fromPanel");
            this.fromPanel.Controls.Add(this.revisionPickerStart);
            this.fromPanel.Controls.Add(this.fromLabel);
            this.fromPanel.Name = "fromPanel";
            // 
            // revisionPickerStart
            // 
            resources.ApplyResources(this.revisionPickerStart, "revisionPickerStart");
            this.revisionPickerStart.Name = "revisionPickerStart";
            this.revisionPickerStart.SvnOrigin = null;
            // 
            // toPanel
            // 
            resources.ApplyResources(this.toPanel, "toPanel");
            this.toPanel.Controls.Add(this.revisionPickerEnd);
            this.toPanel.Controls.Add(this.toLabel);
            this.toPanel.Name = "toPanel";
            // 
            // revisionPickerEnd
            // 
            resources.ApplyResources(this.revisionPickerEnd, "revisionPickerEnd");
            this.revisionPickerEnd.Name = "revisionPickerEnd";
            this.revisionPickerEnd.SvnOrigin = null;
            // 
            // pendingList
            // 
            this.pendingList.AllowColumnReorder = true;
            resources.ApplyResources(this.pendingList, "pendingList");
            this.pendingList.CheckBoxes = true;
            this.pendingList.HideSelection = false;
            this.pendingList.Name = "pendingList";
            this.pendingList.ShowSelectAllCheckBox = true;
            this.pendingList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.pendingList_ItemChecked);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // PathSelector
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.pendingList);
            this.Controls.Add(this.toPanel);
            this.Controls.Add(this.fromPanel);
            this.Controls.Add(this.bottomPanel);
            this.Name = "PathSelector";
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.fromPanel.ResumeLayout(false);
            this.fromPanel.PerformLayout();
            this.toPanel.ResumeLayout(false);
            this.toPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Ankh.UI.PathSelector.VersionSelector revisionPickerStart;
        private Ankh.UI.PathSelector.VersionSelector revisionPickerEnd;
        private System.Windows.Forms.Label suppressLabel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Label fromLabel;
        private System.Windows.Forms.Label toLabel;
        private System.Windows.Forms.Panel fromPanel;
        private System.Windows.Forms.Panel toPanel;
        private PendingChanges.Commits.PendingCommitsView pendingList;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label label1;
    }
}
