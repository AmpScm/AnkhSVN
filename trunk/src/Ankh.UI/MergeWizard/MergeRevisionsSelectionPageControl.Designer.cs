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

namespace Ankh.UI.MergeWizard
{
    partial class MergeRevisionsSelectionPageControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeRevisionsSelectionPageControl));
            this.logToolControl1 = new Ankh.UI.SvnLog.LogControl(this.components);
            this.SuspendLayout();
            // 
            // logToolControl1
            // 
            this.logToolControl1.ShowChangedPaths = true;
            this.logToolControl1.Context = null;
            resources.ApplyResources(this.logToolControl1, "logToolControl1");
            this.logToolControl1.IncludeMergedRevisions = false;
            this.logToolControl1.ShowLogMessage = true;
            this.logToolControl1.Mode = Ankh.UI.SvnLog.LogMode.Log;
            this.logToolControl1.Name = "logToolControl1";
            this.logToolControl1.StrictNodeHistory = false;
            this.logToolControl1.BatchFinished += new System.EventHandler<Ankh.UI.SvnLog.BatchFinishedEventArgs>(this.logToolControl1_BatchFinished);
            // 
            // MergeRevisionsSelectionPageControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logToolControl1);
            this.Name = "MergeRevisionsSelectionPageControl";
            this.ResumeLayout(false);

        }

        #endregion

		private Ankh.UI.SvnLog.LogControl logToolControl1;
    }
}
