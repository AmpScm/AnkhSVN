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
    public partial class MergeTypePage : BaseWizardPage
    {
        public MergeTypePage()
        {
            IsPageComplete = true;

            Text = MergeStrings.MergeTypePageHeaderTitle;
            Description = MergeStrings.MergeTypePageHeaderMessage;
            InitializeComponent();
        }

        /// <summary>
        /// Returns whether or not to show the best practices page.
        /// </summary>
        public bool ShowBestPracticesPage
        {
            get { return IsPerformBestPracticesChecked; }
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

        #region UI Events
        private void rangeofRevisionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);
        }

        private void reintegrateABranchRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);
        }

        private void twoDifferentTreesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);
        }

        private void manuallyRecordMergeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);
        }

        private void manuallyRemoveRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);
        }
        #endregion

        /// <summary>
        /// Gets whether or not the checkbox for performing pre-merge
        /// best practices checks is checked.
        /// </summary>
        public bool IsPerformBestPracticesChecked
        {
            get { return showBestPracticesPageCheckbox.Checked; }
        }


        /// <summary>
        /// Handles toggling the image and label contents based on the
        /// currently selected merge type radio button.
        /// </summary>
        private void ToggleMergeTypeInformation(MergeTypePage page, RadioButton radioButton)
        {
            if (radioButton.Name == "rangeOfRevisionsRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = MergeStrings.RangeOfRevisionsDescription;
                page.mergeTypePictureBox.Image = MergeStrings.RangeOfRevisionsImage;

                SelectedMergeType = MergeWizard.MergeType.RangeOfRevisions;
            }
            else if (radioButton.Name == "reintegrateABranchRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = MergeStrings.ReintegrateABranchDescription;
                page.mergeTypePictureBox.Image = MergeStrings.ReintegrateABranchImage;

                SelectedMergeType = MergeWizard.MergeType.Reintegrate;
            }
            else if (radioButton.Name == "twoDifferentTreesRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = MergeStrings.TwoDifferentTreesDescription;
                page.mergeTypePictureBox.Image = MergeStrings.TwoDifferentTreesImage;

                SelectedMergeType = MergeWizard.MergeType.TwoDifferentTrees;
            }
            else if (radioButton.Name == "manuallyRecordRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = MergeStrings.ManuallyRecordDescription;
                page.mergeTypePictureBox.Image = MergeStrings.ManuallyRecordImage;

                SelectedMergeType = MergeWizard.MergeType.ManuallyRecord;
            }
            else if (radioButton.Name == "manuallyRemoveRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = MergeStrings.ManuallyRemoveDescription;
                page.mergeTypePictureBox.Image = MergeStrings.ManuallyRemoveImage;

                SelectedMergeType = MergeWizard.MergeType.ManuallyRemove;
            }
        }
    }
}
