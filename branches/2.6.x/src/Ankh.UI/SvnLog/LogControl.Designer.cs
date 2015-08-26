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

namespace Ankh.UI.SvnLog
{
    sealed partial class LogControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogControl));
            this.splitContainer1 = new Ankh.UI.VSSelectionControls.SmartSplitContainer(this.components);
            this.revisionBox = new Ankh.UI.SvnLog.LogRevisionControl(this.components);
            this.splitContainer2 = new Ankh.UI.VSSelectionControls.SmartSplitContainer(this.components);
            this.changedPathBox = new Ankh.UI.SvnLog.LogChangedPathsView(this.components);
            this.changedPathLabel = new System.Windows.Forms.Label();
            this.logBox = new Ankh.UI.PendingChanges.LogMessageEditor(this.components);
            this.logLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.revisionBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            // 
            // revisionBox
            // 
            resources.ApplyResources(this.revisionBox, "revisionBox");
            this.revisionBox.HideSelection = false;
            this.revisionBox.LogSource = null;
            this.revisionBox.Name = "revisionBox";
            this.revisionBox.Sorting = System.Windows.Forms.SortOrder.None;
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.changedPathBox);
            this.splitContainer2.Panel1.Controls.Add(this.changedPathLabel);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.logBox);
            this.splitContainer2.Panel2.Controls.Add(this.logLabel);
            // 
            // changedPathBox
            // 
            resources.ApplyResources(this.changedPathBox, "changedPathBox");
            this.changedPathBox.HideSelection = false;
            this.changedPathBox.ItemSource = null;
            this.changedPathBox.LogSource = null;
            this.changedPathBox.Name = "changedPathBox";
            // 
            // changedPathLabel
            // 
            resources.ApplyResources(this.changedPathLabel, "changedPathLabel");
            this.changedPathLabel.Name = "changedPathLabel";
            // 
            // logBox
            // 
            this.logBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.logBox, "logBox");
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            // 
            // logLabel
            // 
            resources.ApplyResources(this.logLabel, "logLabel");
            this.logLabel.Name = "logLabel";
            // 
            // LogControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "LogControl";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private LogRevisionControl revisionBox;
        private Ankh.UI.VSSelectionControls.SmartSplitContainer splitContainer1;
        private Ankh.UI.VSSelectionControls.SmartSplitContainer splitContainer2;
        private PendingChanges.LogMessageEditor logBox;
        private System.Windows.Forms.Label logLabel;
        private LogChangedPathsView changedPathBox;
        private System.Windows.Forms.Label changedPathLabel;

    }
}
