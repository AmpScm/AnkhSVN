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
using System.Reflection;
using System.Resources;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard page for handling the merge type
    /// selection of AnkhSVN's merge capabilities
    /// </summary>
    class MergeTypePage : BasePage
    {
        public const string PAGE_NAME = "Merge Type Page";

        public MergeTypePage(MergeWizard wizard)
            : base(wizard, new MergeTypePageControl(), PAGE_NAME)
        {
            IsPageComplete = true;

            Title = Resources.MergeTypePageHeaderTitle;
            this.Description = Resources.MergeTypePageHeaderMessage;

            PageControl.WizardPage = this;
        }

        /// <summary>
        /// Returns whether or not to show the best practices page.
        /// </summary>
        public bool ShowBestPracticesPage
        {
            get { return ((MergeTypePageControl)PageControl).IsPerformBestPracticesChecked; }
        }

        public MergeWizard.MergeType SelectedMergeType
        {
            get
            {
                return mergeType_prop;
            }

            set
            {
                mergeType_prop = value;
            }
        }

        private MergeWizard.MergeType mergeType_prop = MergeWizard.MergeType.RangeOfRevisions;
    }
}
