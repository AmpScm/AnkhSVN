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
    partial class LogRevisionControl
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
            this.logView = new Ankh.UI.SvnLog.LogRevisionView(this.components);
            this.SuspendLayout();
            // 
            // logView
            // 
            this.logView.AllowColumnReorder = true;
            this.logView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logView.HideSelection = false;
            this.logView.Location = new System.Drawing.Point(0, 0);
            this.logView.LogSource = null;
            this.logView.Name = "logView";
            this.logView.OwnerDraw = true;
            this.logView.Size = new System.Drawing.Size(552, 324);
            this.logView.Sorting = System.Windows.Forms.SortOrder.None;
            this.logView.TabIndex = 0;
            this.logView.Scrolled += new System.EventHandler(this.logView_Scrolled);
            this.logView.ShowContextMenu += new System.Windows.Forms.MouseEventHandler(this.logRevisionControl1_ShowContextMenu);
            this.logView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.logRevisionControl1_ItemSelectionChanged);
            // 
            // LogRevisionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logView);
            this.Name = "LogRevisionControl";
            this.Size = new System.Drawing.Size(552, 324);
            this.ResumeLayout(false);

        }

        #endregion

        private LogRevisionView logView;

    }
}
