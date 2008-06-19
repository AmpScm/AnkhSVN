﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Diagnostics;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeOptionsPageControl : BasePageControl
    {
        private Dictionary<SvnDepth, string> mergeDepths;

        public MergeOptionsPageControl()
        {
            InitializeComponent();
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

            if (box.Text == Resources.SvnDepthInfinity)
                depth = SvnDepth.Infinity;
            else if (box.Text == Resources.SvnDepthChildren)
                depth = SvnDepth.Children;
            else if (box.Text == Resources.SvnDepthFiles)
                depth = SvnDepth.Files;
            else if (box.Text == Resources.SvnDepthEmpty)
                depth = SvnDepth.Empty;

            page.Depth = depth;
        }
        #endregion
    }
}
