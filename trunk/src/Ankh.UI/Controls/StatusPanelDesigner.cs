// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;

namespace Ankh.UI.Controls
{
    class StatusPanelDesigner : ScrollableControlDesigner
    {
        StatusPanel _panel;

        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            _panel = (StatusPanel)component;
        }

        public override bool CanBeParentedTo(System.ComponentModel.Design.IDesigner parentDesigner)
        {
            return (parentDesigner != null) && parentDesigner.Component is StatusContainer;
        }

        public override SelectionRules SelectionRules
        {
            get
            {
                return base.SelectionRules & (SelectionRules.BottomSizeable | SelectionRules.Visible);
            }
        }

        DesignerVerbCollection _verbs;
        public override System.ComponentModel.Design.DesignerVerbCollection Verbs
        {
            get
            {
                _verbs = new DesignerVerbCollection();
                return _verbs;
            }
        }
    }
}
