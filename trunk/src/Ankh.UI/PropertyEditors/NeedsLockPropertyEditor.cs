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

// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI.PropertyEditors
{
    /// <summary>
    /// Property editor for executable properties.
    /// </summary>
    internal partial class NeedsLockPropertyEditor : PropertyEditControl, IPropertyEditor
    {

        public event EventHandler Changed;

        public NeedsLockPropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();
            Reset();
        }

        void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        public void Reset()
        {
            if (this.needsLockTextBox != null && !this.needsLockTextBox.IsDisposed)
            {
                this.needsLockTextBox.Text = FEEDBACK_TEXT;
            }
        }

        public bool Valid
        {

            get { return true; }
        }

        public PropertyItem PropertyItem
        {
            get
            {
                if (!this.Valid)
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when valid is false");
                }

                return new TextPropertyItem(FEEDBACK_TEXT);
            }

            set { }
        }

        /// <summary>
        /// File property
        /// </summary>
        public SvnNodeKind GetAllowedNodeKind()
        {
            return SvnNodeKind.File;
        }

        public override string ToString()
        {
            return SvnPropertyNames.SvnNeedsLock;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void CreateMyToolTip()
        {
            // Set up the ToolTip text for the Button and Textbox.
            needsLockToolTip.SetToolTip(this.needsLockTextBox, FEEDBACK_TEXT);
        }

        private static readonly string FEEDBACK_TEXT = "File needs lock.";
    }
}

