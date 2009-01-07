// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI.PropertyEditors
{
    /// <summary>
    /// Property editor for the predefined ignore property.
    /// </summary>
    partial class IgnorePropertyEditor : PropertyEditControl
    {
        public IgnorePropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();
        }

        public override void Reset()
        {
            this.ignoreTextBox.Text = this.originalValue;
        }

        public override bool Valid
        {
            get
            { 
                if (!this.dirty)
                {
                    return false;
                }
                else 
                {
                    string value = this.ignoreTextBox.Text.Trim();
                    return (!string.IsNullOrEmpty(value));
                }
            }
        }

        public override SvnPropertyValue PropertyItem
        {
            get
            {
                if ( !this.Valid )
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when Valid is false");
                }
				
                return new SvnPropertyValue(SvnPropertyNames.SvnIgnore, ignoreTextBox.Text);
            }
            set
            {
                if (value != null)
                {
                    ignoreTextBox.Text = originalValue = value.StringValue;
                }
                else
                    ignoreTextBox.Text = originalValue = "";
            }
        }

        /// <summary>
        /// Directory property
        /// </summary>
        public override SvnNodeKind GetAllowedNodeKind()
        {
            return SvnNodeKind.Directory;
        }

        public override string ToString()
        {
            return SvnPropertyNames.SvnIgnore;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        private void ignoreTextBox_TextChanged(object sender, System.EventArgs e)
        {
            string newValue = this.ignoreTextBox.Text;
            // Enables/Disables save button
            this.dirty = !newValue.Equals(this.originalValue);

            OnChanged(EventArgs.Empty);
        }

        private void CreateMyToolTip()
        {         
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( this.ignoreTextBox, 
                "Eks *.obj, subdir. Names of file-categories and directories to be ignored.");
        }

        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        private bool dirty;

        private string originalValue = string.Empty;
    }
}

