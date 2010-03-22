// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{

    partial class MergeSourceRangeOfRevisionsPage : MergeSourceBasePage
    {
        [Obsolete()]
        public MergeSourceRangeOfRevisionsPage()
        {
            InitializeComponent();
        }

        #region UI Events
        /// <summary>
        /// Enable the "Next" button since the revision(s) must be selected on the next page.
        /// </summary>
        private void selectRevisionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                NextPageRequired = true;
            }
        }

        /// <summary>
        /// Disable the "Next" button since the all applicable revisions will be merged.
        /// </summary>
        private void allRevisionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                NextPageRequired = false;
            }
        }
        #endregion
    }
}
