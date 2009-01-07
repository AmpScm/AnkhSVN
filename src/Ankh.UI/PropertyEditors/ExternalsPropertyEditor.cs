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
    /// Editor for externals properties.
    /// </summary>
    partial class ExternalsPropertyEditor : PropertyEditControl
    {
        string originalValue;

        public ExternalsPropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new System.ComponentModel.Container();
            CreateMyToolTip();
        }

        /// <summary>
        /// Indicates whether the property item is valid.
        /// </summary>
        public override bool Valid
        {
            get
            {
                return !string.IsNullOrEmpty(_externalsText);
            }
        }

        string _externalsText;
        public string ExternalsText
        {
            get { return _externalsText; }
            set { _externalsText = value; }
        }

        /// <summary>
        /// Sets and gets the property item.
        /// </summary>
        public override SvnPropertyValue PropertyItem
        {
            get
            {
                if (!this.Valid)
                {
                    throw new InvalidOperationException(
                        "Can not get a property item when valid is false");
                }

                return new SvnPropertyValue(SvnPropertyNames.SvnExternals, ExternalsText);
            }

            set
            {
                if (value != null)
                {
                    ExternalsText = originalValue = value.StringValue;
                }
                else
                    ExternalsText = originalValue = "";
            }
        }

        /// <summary>
        /// Indicates the type of property.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SvnPropertyNames.SvnExternals;
        }

        /// <summary>
        /// Directory property
        /// </summary>
        public override SvnNodeKind GetAllowedNodeKind()
        {
            return SvnNodeKind.Directory;
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
            // Set up the ToolTip text for the Button and Checkbox.
            conflictToolTip.SetToolTip(externalGrid,
                "Example: subdir1/foo   http://url.for.external.source/foo. Could be used to make your own module.");
        }

    }
}

