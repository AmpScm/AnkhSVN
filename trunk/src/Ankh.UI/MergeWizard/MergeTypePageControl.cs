using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;

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
        }

        private void reintegrateABranchRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ToggleMergeTypeInformation(this, (RadioButton)sender);
        }

        private void changesetBasedRadioButton_CheckedChanged(object sender, EventArgs e)
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
            else if (radioButton.Name == "changesetBasedRadioButton")
            {
                page.mergeTypeDescriptionLabel.Text = resman.GetString("ChangesetBasedDescription");
                page.mergeTypePictureBox.Image = (System.Drawing.Image)resman.GetObject("ChangesetBasedImage");
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

        private ResourceManager resman = new ResourceManager("Ankh.UI.MergeWizard.Resources", Assembly.GetExecutingAssembly());
    }
}
