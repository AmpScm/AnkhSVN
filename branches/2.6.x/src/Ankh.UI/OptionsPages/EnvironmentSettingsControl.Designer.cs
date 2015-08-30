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
    partial class EnvironmentSettingsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnvironmentSettingsControl));
            this.authenticationEdit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.proxyEdit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.clientSideHooks = new System.Windows.Forms.CheckBox();
            this.interactiveMergeOnConflict = new System.Windows.Forms.CheckBox();
            this.autoAddFiles = new System.Windows.Forms.CheckBox();
            this.flashWindowAfterOperation = new System.Windows.Forms.CheckBox();
            this.autoLockFiles = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pcDefaultDoubleClick = new System.Windows.Forms.ComboBox();
            this.preferPutty = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // authenticationEdit
            // 
            resources.ApplyResources(this.authenticationEdit, "authenticationEdit");
            this.authenticationEdit.Name = "authenticationEdit";
            this.authenticationEdit.UseVisualStyleBackColor = true;
            this.authenticationEdit.Click += new System.EventHandler(this.authenticationEdit_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // proxyEdit
            // 
            resources.ApplyResources(this.proxyEdit, "proxyEdit");
            this.proxyEdit.Name = "proxyEdit";
            this.proxyEdit.UseVisualStyleBackColor = true;
            this.proxyEdit.Click += new System.EventHandler(this.proxyEdit_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.preferPutty);
            this.groupBox1.Controls.Add(this.clientSideHooks);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.proxyEdit);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.authenticationEdit);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // clientSideHooks
            // 
            resources.ApplyResources(this.clientSideHooks, "clientSideHooks");
            this.clientSideHooks.Name = "clientSideHooks";
            this.clientSideHooks.UseVisualStyleBackColor = true;
            // 
            // interactiveMergeOnConflict
            // 
            resources.ApplyResources(this.interactiveMergeOnConflict, "interactiveMergeOnConflict");
            this.interactiveMergeOnConflict.Name = "interactiveMergeOnConflict";
            this.interactiveMergeOnConflict.UseVisualStyleBackColor = true;
            // 
            // autoAddFiles
            // 
            resources.ApplyResources(this.autoAddFiles, "autoAddFiles");
            this.autoAddFiles.Name = "autoAddFiles";
            this.autoAddFiles.UseVisualStyleBackColor = true;
            // 
            // flashWindowAfterOperation
            // 
            resources.ApplyResources(this.flashWindowAfterOperation, "flashWindowAfterOperation");
            this.flashWindowAfterOperation.Name = "flashWindowAfterOperation";
            this.flashWindowAfterOperation.UseVisualStyleBackColor = true;
            // 
            // autoLockFiles
            // 
            resources.ApplyResources(this.autoLockFiles, "autoLockFiles");
            this.autoLockFiles.Name = "autoLockFiles";
            this.autoLockFiles.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // pcDefaultDoubleClick
            // 
            resources.ApplyResources(this.pcDefaultDoubleClick, "pcDefaultDoubleClick");
            this.pcDefaultDoubleClick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pcDefaultDoubleClick.FormattingEnabled = true;
            this.pcDefaultDoubleClick.Items.AddRange(new object[] {
            resources.GetString("pcDefaultDoubleClick.Items"),
            resources.GetString("pcDefaultDoubleClick.Items1")});
            this.pcDefaultDoubleClick.Name = "pcDefaultDoubleClick";
            // 
            // preferPutty
            // 
            resources.ApplyResources(this.preferPutty, "preferPutty");
            this.preferPutty.Name = "preferPutty";
            this.preferPutty.UseVisualStyleBackColor = true;
            // 
            // EnvironmentSettingsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.autoAddFiles);
            this.Controls.Add(this.autoLockFiles);
            this.Controls.Add(this.interactiveMergeOnConflict);
            this.Controls.Add(this.flashWindowAfterOperation);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pcDefaultDoubleClick);
            this.Name = "EnvironmentSettingsControl";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button authenticationEdit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button proxyEdit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox interactiveMergeOnConflict;
        private System.Windows.Forms.CheckBox autoAddFiles;
        private System.Windows.Forms.CheckBox flashWindowAfterOperation;
        private System.Windows.Forms.CheckBox autoLockFiles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox pcDefaultDoubleClick;
        private System.Windows.Forms.CheckBox clientSideHooks;
        private System.Windows.Forms.CheckBox preferPutty;
    }
}
