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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Ankh.UI.PendingChanges.Commits;

namespace Ankh.UI.PendingChanges
{
    /// <summary>
    /// This class is used to implement CodeEditorUserControl
    /// </summary>
    /// <seealso cref="UserControl"/>
    public class LogMessageEditor : VSTextEditor
    {
        
        public LogMessageEditor()
        {
            BackColor = SystemColors.Window;
            base.ForceLanguageService = new Guid(AnkhId.LogMessageLanguageServiceId);
        }

        public LogMessageEditor(IContainer container)
            : base(container)
        {
            base.ForceLanguageService = new Guid(AnkhId.LogMessageLanguageServiceId);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Guid? ForceLanguageService
        {
            get { return base.ForceLanguageService; }
            set { throw new InvalidOperationException(); }
        }

        IPendingChangeSource _pasteSrc;
        /// <summary>
        /// Gets or sets the paste source.
        /// </summary>
        /// <value>The paste source.</value>
        [DefaultValue(null)]
        public IPendingChangeSource PasteSource
        {
            get { return _pasteSrc; }
            set { _pasteSrc = value; }
        }             
    }    
}

