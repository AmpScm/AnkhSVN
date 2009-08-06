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
using WizardFramework;
using System.Resources;
using System.Reflection;
using System.ComponentModel;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard dialog for AnkhSVN's merge capabilities.
    /// </summary>
    partial class MergeWizardDialog : WizardDialog
    {
        IAnkhServiceProvider _context;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="utils"></param>
        public MergeWizardDialog(IAnkhServiceProvider context, MergeUtils utils, SvnItem mergeTarget)
        {
            InitializeComponent();

            Icon = MergeStrings.MergeWizardIcon;
            Wizard = new MergeWizard(context, this);

            _context = context;
            Wizard.MergeUtils = utils;
            Wizard.MergeTarget = mergeTarget;
        }

        public new MergeWizard Wizard
        {
            get { return (MergeWizard)base.Wizard; }
            set { base.Wizard = value; }
        }        
    }
}
