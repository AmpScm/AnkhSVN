﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeTypePageControl : BasePageControl
    {
        public MergeTypePageControl()
        {
            InitializeComponent();
        }

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
        private void ToggleMergeTypeInformation(MergeTypePageControl page, RadioButton radioButton)
        {
            MergeTypePage wizardPage = ((MergeTypePage)WizardPage);

            if (radioButton.Name == "rangeOfRevisionsRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = Resources.RangeOfRevisionsDescription;
                page.mergeTypePictureBox.Image = Resources.RangeOfRevisionsImage;

                wizardPage.SelectedMergeType = MergeWizard.MergeType.RangeOfRevisions;
            }
            else if (radioButton.Name == "reintegrateABranchRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = Resources.ReintegrateABranchDescription;
                page.mergeTypePictureBox.Image = Resources.ReintegrateABranchImage;

                wizardPage.SelectedMergeType = MergeWizard.MergeType.Reintegrate;
            }
            else if (radioButton.Name == "twoDifferentTreesRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = Resources.TwoDifferentTreesDescription;
                page.mergeTypePictureBox.Image = Resources.TwoDifferentTreesImage;

                wizardPage.SelectedMergeType = MergeWizard.MergeType.TwoDifferentTrees;
            }
            else if (radioButton.Name == "manuallyRecordRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = Resources.ManuallyRecordDescription;
                page.mergeTypePictureBox.Image = Resources.ManuallyRecordImage;

                wizardPage.SelectedMergeType = MergeWizard.MergeType.ManuallyRecord;
            }
            else if (radioButton.Name == "manuallyRemoveRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = Resources.ManuallyRemoveDescription;
                page.mergeTypePictureBox.Image = Resources.ManuallyRemoveImage;

                wizardPage.SelectedMergeType = MergeWizard.MergeType.ManuallyRemove;
            }
        }
    }
}
