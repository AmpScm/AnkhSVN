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

namespace Ankh.UI.Annotate
{
    partial class AnnotateEditorControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.blameMarginControl1 = new Ankh.UI.Annotate.AnnotateMarginControl();
            this.editor = new Ankh.UI.PendingChanges.VSTextEditor(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.blameMarginControl1);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.editor);
            this.splitContainer1.Size = new System.Drawing.Size(300, 300);
            this.splitContainer1.SplitterDistance = 115;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 3;
            // 
            // blameMarginControl1
            // 
            this.blameMarginControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blameMarginControl1.Location = new System.Drawing.Point(0, 0);
            this.blameMarginControl1.Name = "blameMarginControl1";
            this.blameMarginControl1.Size = new System.Drawing.Size(115, 300);
            this.blameMarginControl1.TabIndex = 1;
            this.blameMarginControl1.Text = "blameMarginControl1";
            // 
            // editor
            // 
            this.editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editor.Location = new System.Drawing.Point(0, 0);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(183, 300);
            this.editor.TabIndex = 2;
            this.editor.Text = "logMessageEditor1";
            this.editor.Scroll += new System.EventHandler<Ankh.UI.PendingChanges.TextViewScrollEventArgs>(this.logMessageEditor1_Scroll);
            // 
            // AnnotateEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "AnnotateEditorControl";
            this.Size = new System.Drawing.Size(300, 300);
            this.Text = " (Annotated)";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AnnotateMarginControl blameMarginControl1;
        private Ankh.UI.PendingChanges.VSTextEditor editor;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
