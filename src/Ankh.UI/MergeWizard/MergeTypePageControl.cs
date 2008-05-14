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

            ((MergeTypePage)wizardPage_prop).SelectedMergeType = MergeTypePage.MergeType.RangeOfRevisions;
        }

        private void reintegrateABranchRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)wizardPage_prop).SelectedMergeType = MergeTypePage.MergeType.Reintegrate;
        }

        private void twoDifferentTreesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)wizardPage_prop).SelectedMergeType = MergeTypePage.MergeType.TwoDifferentTrees;
        }

        private void manuallyRecordMergeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)wizardPage_prop).SelectedMergeType = MergeTypePage.MergeType.ManuallyRecord;
        }

        private void manuallyRemoveRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);

            ((MergeTypePage)wizardPage_prop).SelectedMergeType = MergeTypePage.MergeType.ManuallyRemove;
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
                page.mergeTypeDescriptionLabel.Text = resman.GetString("RangeOfRevisionsDescription");
                page.mergeTypePictureBox.Image = (System.Drawing.Image)resman.GetObject("RangeOfRevisionsImage");
            }
            else if (radioButton.Name == "reintegrateABranchRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = resman.GetString("ReintegrateABranchDescription");
                page.mergeTypePictureBox.Image = (System.Drawing.Image)resman.GetObject("ReintegrateABranchImage");
            }
            else if (radioButton.Name == "twoDifferentTreesRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = resman.GetString("TwoDifferentTreesDescription");
                page.mergeTypePictureBox.Image = (System.Drawing.Image)resman.GetObject("TwoDifferentTreesImage");
            }
            else if (radioButton.Name == "manuallyRecordRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = resman.GetString("ManuallyRecordDescription");
                page.mergeTypePictureBox.Image = (System.Drawing.Image)resman.GetObject("ManuallyRecordImage");
            }
            else if (radioButton.Name == "manuallyRemoveRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = resman.GetString("ManuallyRemoveDescription");
                page.mergeTypePictureBox.Image = (System.Drawing.Image)resman.GetObject("ManuallyRemoveImage");
            }
        }

        private WizardFramework.WizardPage wizardPage_prop = null;
        private ResourceManager resman = new ResourceManager("Ankh.UI.MergeWizard.Resources", Assembly.GetExecutingAssembly());
    }
}
