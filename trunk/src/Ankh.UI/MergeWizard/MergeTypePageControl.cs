using System;
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

            ((MergeTypePage)WizardPage).SelectedMergeType = MergeWizard.MergeType.RangeOfRevisions;
        }

        private void reintegrateABranchRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)WizardPage).SelectedMergeType = MergeWizard.MergeType.Reintegrate;
        }

        private void twoDifferentTreesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)WizardPage).SelectedMergeType = MergeWizard.MergeType.TwoDifferentTrees;
        }

        private void manuallyRecordMergeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)WizardPage).SelectedMergeType = MergeWizard.MergeType.ManuallyRecord;
        }

        private void manuallyRemoveRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)WizardPage).SelectedMergeType = MergeWizard.MergeType.ManuallyRemove;
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
            if (radioButton.Name == "rangeOfRevisionsRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = Resources.RangeOfRevisionsDescription;
                page.mergeTypePictureBox.Image = Resources.RangeOfRevisionsImage;
            }
            else if (radioButton.Name == "reintegrateABranchRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = Resources.ReintegrateABranchDescription;
                page.mergeTypePictureBox.Image = Resources.ReintegrateABranchImage;
            }
            else if (radioButton.Name == "twoDifferentTreesRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = Resources.TwoDifferentTreesDescription;
                page.mergeTypePictureBox.Image = Resources.TwoDifferentTreesImage;
            }
            else if (radioButton.Name == "manuallyRecordRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = Resources.ManuallyRecordDescription;
                page.mergeTypePictureBox.Image = Resources.ManuallyRecordImage;
            }
            else if (radioButton.Name == "manuallyRemoveRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = Resources.ManuallyRemoveDescription;
                page.mergeTypePictureBox.Image = Resources.ManuallyRemoveImage;
            }
        }
    }
}
