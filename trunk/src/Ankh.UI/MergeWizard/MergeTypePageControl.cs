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
    public partial class MergeTypePageControl : UserControl
    {
        public MergeTypePageControl()
        {
            InitializeComponent();
        }

        #region UI Events
        private void rangeofRevisionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)wizardPage_prop).SelectedMergeType = MergeWizard.MergeType.RangeOfRevisions;
        }

        private void reintegrateABranchRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)wizardPage_prop).SelectedMergeType = MergeWizard.MergeType.Reintegrate;
        }

        private void twoDifferentTreesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)wizardPage_prop).SelectedMergeType = MergeWizard.MergeType.TwoDifferentTrees;
        }

        private void manuallyRecordMergeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)wizardPage_prop).SelectedMergeType = MergeWizard.MergeType.ManuallyRecord;
        }

        private void manuallyRemoveRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)wizardPage_prop).SelectedMergeType = MergeWizard.MergeType.ManuallyRemove;
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
        /// Gets/Sets the wizard page associated with this UserControl.
        /// </summary>
        public WizardPage WizardPage
        {
            get
            {
                return wizardPage_prop;
            }

            set
            {
                wizardPage_prop = value;
            }
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

        private WizardFramework.WizardPage wizardPage_prop = null;
    }
}
