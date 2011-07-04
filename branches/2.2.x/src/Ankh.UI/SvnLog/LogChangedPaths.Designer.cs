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
    partial class LogChangedPaths
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogChangedPaths));
            this.label1 = new System.Windows.Forms.Label();
            this.changedPaths = new Ankh.UI.SvnLog.LogChangedPathsView(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // changedPaths
            // 
            this.changedPaths.DataSource = null;
            resources.ApplyResources(this.changedPaths, "changedPaths");
            this.changedPaths.HideSelection = false;
            this.changedPaths.Name = "changedPaths";
            this.changedPaths.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.changedPaths_MouseDoubleClick);
            this.changedPaths.ShowContextMenu += new System.Windows.Forms.MouseEventHandler(this.changedPaths_ShowContextMenu);
            // 
            // LogChangedPaths
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.changedPaths);
            this.Controls.Add(this.label1);
            this.Name = "LogChangedPaths";
            this.ResumeLayout(false);

        }

        #endregion

        private Ankh.UI.SvnLog.LogChangedPathsView changedPaths;
        private System.Windows.Forms.Label label1;
    }
}
