// $Id$
//
// Copyright 2009 The AnkhSVN Project
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

namespace Ankh.UI.OptionsPages
{
    partial class SvnAuthenticationCacheEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SvnAuthenticationCacheEditor));
            this.credentialList = new Ankh.UI.VSSelectionControls.SmartListView();
            this.serverHeader = new System.Windows.Forms.ColumnHeader();
            this.realmHeader = new System.Windows.Forms.ColumnHeader();
            this.cachedHeader = new System.Windows.Forms.ColumnHeader();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // credentialList
            // 
            resources.ApplyResources(this.credentialList, "credentialList");
            this.credentialList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.serverHeader,
            this.realmHeader,
            this.cachedHeader});
            this.credentialList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.credentialList.HideSelection = false;
            this.credentialList.Name = "credentialList";
            this.credentialList.SelectedIndexChanged += new System.EventHandler(this.credentialList_SelectedIndexChanged);
            // 
            // serverHeader
            // 
            resources.ApplyResources(this.serverHeader, "serverHeader");
            // 
            // realmHeader
            // 
            resources.ApplyResources(this.realmHeader, "realmHeader");
            // 
            // cachedHeader
            // 
            resources.ApplyResources(this.cachedHeader, "cachedHeader");
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.removeButton);
            this.groupBox1.Controls.Add(this.credentialList);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // removeButton
            // 
            resources.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.Name = "removeButton";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // closeButton
            // 
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // SvnAuthenticationCacheEditor
            // 
            this.AcceptButton = this.closeButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.closeButton;
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "SvnAuthenticationCacheEditor";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Ankh.UI.VSSelectionControls.SmartListView credentialList;
        private System.Windows.Forms.ColumnHeader serverHeader;
        private System.Windows.Forms.ColumnHeader realmHeader;
        private System.Windows.Forms.ColumnHeader cachedHeader;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button closeButton;
    }
}