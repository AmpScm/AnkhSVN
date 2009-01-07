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
using System.Text.RegularExpressions;
using SharpSvn;

namespace Ankh.UI.PropertyEditors
{
    /// <summary>
    /// Editor for the mime-type properties.
    /// </summary>
    partial class MimeTypePropertyEditor : PropertyEditControl
    {
        public MimeTypePropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();
        }

        /// <summary>
        /// Resets the textbox.
        /// </summary>
        public override void Reset()
        {
            this.mimeTextBox.Text = this.originalValue;
        }

        /// <summary>
        /// Indicates whether the property is valid.
        /// </summary>
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
                    return validateMimeType.IsMatch(this.mimeTextBox.Text);
                }
            } 
        }

        /// <summary>
        /// Sets and gets the property item.
        /// </summary>
        public override SvnPropertyValue PropertyItem
        {
            get
            {
                if ( !this.Valid )
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when Valid is false");
                }	
                return new SvnPropertyValue(SvnPropertyNames.SvnMimeType, mimeTextBox.Text);
            }

            set
            {
                if (value != null)
                {
                    mimeTextBox.Text = originalValue = value.StringValue;
                }
                else
                    mimeTextBox.Text = originalValue = "";
            }
        }

        /// <summary>
        /// File property
        /// </summary>
        public override SvnNodeKind GetAllowedNodeKind()
        {
            return SvnNodeKind.File;
        }

        /// <summary>
        /// Indicates the type of property.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SvnPropertyNames.SvnMimeType;
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
        /// <summary>
        /// Dispatches the Changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mimeTextBox_TextChanged(object sender, System.EventArgs e)
        {
            // Enables save button
            string newValue = this.mimeTextBox.Text;
            this.dirty = !newValue.Equals(this.originalValue);

            OnChanged(EventArgs.Empty);
        }

        private void CreateMyToolTip()
        {
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( this.mimeTextBox, 
                "Defult is text/*, everything else is binary");
        }

        
        private static readonly Regex validateMimeType = 
            new Regex(@"\w{2,}/\*{1}|(\w{2,})", RegexOptions.Compiled);
        /// <summary>
        /// Flag for enabled/disabled save button
        /// </summary>
        private bool dirty;

        private string originalValue = string.Empty;
    }
}

