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
			this.label1 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.stealLocksCheckBox = new System.Windows.Forms.CheckBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.label2 = new System.Windows.Forms.Label();
			this.pathSelectionTreeView = new Ankh.UI.PathSelector.PathSelectionTreeView();
			this.logMessageEditor = new Ankh.UI.PendingChanges.LogMessageEditor(this.components);
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
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
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
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.pathSelectionTreeView);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.label1);
			this.splitContainer1.Panel2.Controls.Add(this.logMessageEditor);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// pathSelectionTreeView
			// 
			resources.ApplyResources(this.pathSelectionTreeView, "pathSelectionTreeView");
			this.pathSelectionTreeView.CheckBoxes = true;
			this.pathSelectionTreeView.Context = null;
			this.pathSelectionTreeView.Name = "pathSelectionTreeView";
			this.pathSelectionTreeView.Recursive = false;
			this.pathSelectionTreeView.SingleCheck = false;
			this.pathSelectionTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.pathSelectionTreeView_AfterCheck);
			// 
			// logMessageEditor
			// 
			resources.ApplyResources(this.logMessageEditor, "logMessageEditor");
			this.logMessageEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.logMessageEditor.Name = "logMessageEditor";
			// 
			// LockDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.suppressLabel);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.stealLocksCheckBox);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.btnCancel);
			this.Name = "LockDialog";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion

        internal Ankh.UI.PathSelector.PathSelectionTreeView pathSelectionTreeView;

        protected System.Windows.Forms.Label suppressLabel;

        private Ankh.UI.PendingChanges.LogMessageEditor logMessageEditor;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label label1;
        protected System.Windows.Forms.Button btnCancel;
        protected System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox stealLocksCheckBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label2;
    }
}
