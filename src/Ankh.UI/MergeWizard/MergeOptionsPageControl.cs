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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Diagnostics;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeOptionsPageControl : BasePageControl
    {
        private Dictionary<SvnDepth, string> mergeDepths;

        public MergeOptionsPageControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ((MergeWizard)WizardPage.Wizard).WizardDialog.PageChanged += new EventHandler<WizardPageChangedEventArgs>(WizardDialog_PageChangeEvent);
        }

        #region UI Events
        private void MergeOptionsPageControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                // Moved from Constructor to _Load for timing reasons.
                mergeDepths = ((MergeWizard)WizardPage.Wizard).MergeUtils.MergeDepths;

                // Decided against using BindingSource due to rendering time
                // and the requirement of threading to keep the UI from 
                // "freezing" when initially displayed.
                foreach (KeyValuePair<SvnDepth, string> kvp in mergeDepths)
                {
                    depthComboBox.Items.Add(kvp.Value);
                }

                UIUtils.ResizeDropDownForLongestEntry(depthComboBox);

                depthComboBox.SelectedIndex = 0;
            }
        }

        private void textConflictsPromptRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).TextConflictResolution = MergeOptionsPage.ConflictResolutionOption.PROMPT;
        }

        private void textConflictsMarkRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).TextConflictResolution = MergeOptionsPage.ConflictResolutionOption.MARK;
        }

        private void textConflictsMyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).TextConflictResolution = MergeOptionsPage.ConflictResolutionOption.MINE;
        }

        private void textConflictsRepositoryRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).TextConflictResolution = MergeOptionsPage.ConflictResolutionOption.THEIRS;
        }

        private void textConflictsBaseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).TextConflictResolution = MergeOptionsPage.ConflictResolutionOption.BASE;
        }

        private void binaryConflictsPromptRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).BinaryConflictResolution = MergeOptionsPage.ConflictResolutionOption.PROMPT;
        }

        private void binaryConflictsMarkRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).BinaryConflictResolution = MergeOptionsPage.ConflictResolutionOption.MARK;
        }

        private void binaryConflictsMyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).BinaryConflictResolution = MergeOptionsPage.ConflictResolutionOption.MINE;
        }

        private void binaryConflictsRepositoryRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).BinaryConflictResolution = MergeOptionsPage.ConflictResolutionOption.THEIRS;
        }

        private void binaryConflictsBaseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).BinaryConflictResolution = MergeOptionsPage.ConflictResolutionOption.BASE;
        }

        private void ignoreAncestryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).IgnoreAncestry = ((CheckBox)sender).Checked;
        }

        private void unversionedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeOptionsPage)WizardPage).AllowUnversionedObstructions = ((CheckBox)sender).Checked;
        }

        private void depthComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            MergeOptionsPage page = ((MergeOptionsPage)WizardPage);
            SvnDepth depth = SvnDepth.Unknown;

            if (box.Text == MergeStrings.SvnDepthInfinity)
                depth = SvnDepth.Infinity;
            else if (box.Text == MergeStrings.SvnDepthChildren)
                depth = SvnDepth.Children;
            else if (box.Text == MergeStrings.SvnDepthFiles)
                depth = SvnDepth.Files;
            else if (box.Text == MergeStrings.SvnDepthEmpty)
                depth = SvnDepth.Empty;

            page.Depth = depth;
        }
        #endregion

        private void WizardDialog_PageChangeEvent(object sender, WizardPageChangedEventArgs e)
        {
            if (e.CurrentPage == WizardPage)
            {
                PopulateUI();
            }
        }

        private void PopulateUI()
        {
            // clear the message, 
            WizardPage.Message = null;
        }
    }
}
