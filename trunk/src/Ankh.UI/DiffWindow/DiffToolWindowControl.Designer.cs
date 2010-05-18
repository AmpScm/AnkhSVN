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

using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.DiffWindow
{
    partial class DiffToolWindowControl
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
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiffToolWindowControl));
			this.diffControl1 = new Ankh.Diff.DiffUtils.Controls.DiffControl();
			this.SuspendLayout();
			// 
			// diffControl1
			// 
			resources.ApplyResources(this.diffControl1, "diffControl1");
			this.diffControl1.Name = "diffControl1";
			this.diffControl1.ShowWhitespaceInLineDiff = true;
			this.diffControl1.ViewFont = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			// 
			// DiffToolWindowControl
			// 
			this.Controls.Add(this.diffControl1);
			this.Name = "DiffToolWindowControl";
			resources.ApplyResources(this, "$this");
			this.ResumeLayout(false);

        }
        #endregion

        private Ankh.Diff.DiffUtils.Controls.DiffControl diffControl1;
    }
}
