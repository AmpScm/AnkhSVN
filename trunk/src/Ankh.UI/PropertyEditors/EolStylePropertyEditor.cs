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
    /// Summary description for EolStylePropertyEditor.
    /// </summary>
    partial class EolStylePropertyEditor : PropertyEditControl
    {
        public string existingValue;

        public EolStylePropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            components = new System.ComponentModel.Container();
            CreateMyToolTip();

            existingValue = string.Empty;
            _dirty = true;
            RadioButton rb = ToRadioButton("native");
            if (rb != null)
            {
                rb.Checked = true;
                _selectedValue = (string)rb.Tag;
            }
        }
  
        public override SvnPropertyValue PropertyItem
        {
            get
            {
                if( !Valid)
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when Valid is false");
                }
          
                return new SvnPropertyValue(SvnPropertyNames.SvnEolStyle, _selectedValue);
            }
            set
            {
                // TODO: Make safe for null
                existingValue = value.StringValue;
                RadioButton rb = ToRadioButton(existingValue);
                if (rb != null)
                {
                    rb.Checked = true;
                }
            }
        }

        public override bool AllowNodeKind(SvnNodeKind kind)
        {
            return kind == SvnNodeKind.File;
        }

        public override string ToString()
        {
            return SvnPropertyNames.SvnEolStyle;
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

        private void RadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            string newValue = (string)((RadioButton)sender).Tag;
            _selectedValue = newValue;

            // Enables save button
            Dirty = (newValue != existingValue);
        }

        private void CreateMyToolTip()
        {
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( nativeRadioButton, "Default. Line endings dependant on operating system");
            conflictToolTip.SetToolTip( lfRadioButton, "End of line style is LF (Line Feed)");
            conflictToolTip.SetToolTip( crRadioButton, "End of line style is CR");
            conflictToolTip.SetToolTip( crlfRdioButton, "End of line style is CRLF");
        }

        private bool Dirty
        {
            set
            {
                if (_dirty != value)
                {
                    _dirty = value;

                    OnChanged(EventArgs.Empty);
                }
            }
        }

        private RadioButton ToRadioButton(string propertyValue)
        {
            if (string.IsNullOrEmpty(propertyValue))
            {
                return null;
            }
            foreach (Control c in eolStyleGroupBox.Controls)
            {
                if (c is RadioButton
                    && c.Tag is string
                    && propertyValue.Equals((string) c.Tag))
                {
                    return (RadioButton)c;
                }
            }
            return null;
        }

        string _selectedValue;
        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        bool _dirty;
       
    }
}

