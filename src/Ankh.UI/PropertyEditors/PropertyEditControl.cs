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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using SharpSvn;

namespace Ankh.UI.PropertyEditors
{
    class PropertyEditControl : UserControl
    {
        public PropertyEditControl()
        {
            Size = new Size(348, 196);
        }

        [Localizable(true), DefaultValue(typeof(Size), "348;196")]
        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }

        public virtual SvnPropertyValue PropertyItem
        {
            get { return null; }
            set { throw new InvalidOperationException(); }
        }

        string _propertyName;

        public virtual string PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        public virtual bool Valid
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the kind of the allowed node.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This code assumes .File and .Directory are bitflags</remarks>
        public virtual SvnNodeKind GetAllowedNodeKind()
        {
            return SvnNodeKind.File | SvnNodeKind.Directory;
        }

        public event EventHandler Changed;

        /// <summary>
        /// Raises the <see cref="E:Changed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }
    }
}
