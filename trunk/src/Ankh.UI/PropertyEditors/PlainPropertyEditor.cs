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
    /// Property editor for plain properties
    /// </summary>
    internal partial class PlainPropertyEditor : PropertyEditControl, IPropertyEditor
    {
        public event EventHandler Changed;
        
        public PlainPropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();
        }

        public void Reset()
        {
            this.valueTextBox.Text = "";
        }

        public bool Valid
        {
            get{ return this.valueTextBox.Text.Trim() != "";}
        }

        public PropertyItem PropertyItem
        {
            get
            {   
                if( !this.Valid)
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when Valid is false");
                }

                return new TextPropertyItem(this.valueTextBox.Text);
            }

            set
            {
                TextPropertyItem item = (TextPropertyItem)value;
                this.valueTextBox.Text = item.Text;
            }
        }

        /// <summary>
        /// Custom property
        /// </summary>
        public SvnNodeKind GetAllowedNodeKind()
        {
            return SvnNodeKind.Directory | SvnNodeKind.File;
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

        private void valueTextBox_TextChanged(object sender, System.EventArgs e)
        {
            if (Changed != null)
                Changed( this, EventArgs.Empty );
        }


        private void CreateMyToolTip()
        {
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip( this.valueTextBox, 
                "Enter value of your self defined property" );      
        }
    }
}

