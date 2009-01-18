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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeSummaryPageControl : BasePageControl
    {
        public MergeSummaryPageControl()
        {
            InitializeComponent();
        }

        private void WizardDialog_PageChangeEvent(object sender, WizardPageChangeEventArgs e)
        {
            if (e.CurrentPage == WizardPage)
            {
                WizardPage.Message = null;

                PopulateUI();
            }
        }

        private void PopulateUI()
        {
            if (!DesignMode)
            {
                MergeWizard mergeWizard = ((MergeWizard)WizardPage.Wizard);
                MergeWizard.MergeType mergeType = ((MergeTypePage)mergeWizard.GetPage(MergeTypePage.PAGE_NAME)).SelectedMergeType;
                MergeOptionsPage mergeOptions = (MergeOptionsPage)mergeWizard.GetPage(MergeOptionsPage.PAGE_NAME);

                // Populate Merge Target
                mergeTargetTextBox.Text = mergeWizard.MergeTarget.FullPath;
                // Populate Merge Source 1
                if (mergeType == MergeWizard.MergeType.TwoDifferentTrees)
                    mergeSource1TextBox.Text = ((MergeSourceTwoDifferentTreesPage)mergeWizard.GetPage(MergeSourceTwoDifferentTreesPage.PAGE_NAME)).MergeSourceOne;
                else
                    mergeSource1TextBox.Text = mergeWizard.MergeSource.Target.ToString();

                // Populate Merge Source 2
                if (mergeType == MergeWizard.MergeType.TwoDifferentTrees)
                    if (((MergeSourceTwoDifferentTreesPage)mergeWizard.GetPage(MergeSourceTwoDifferentTreesPage.PAGE_NAME)).HasSecondMergeSourceUrl)
                        mergeSource2TextBox.Text = ((MergeSourceTwoDifferentTreesPage)mergeWizard.GetPage(MergeSourceTwoDifferentTreesPage.PAGE_NAME)).MergeSourceTwo;
                    else
                        mergeSource2TextBox.Text = ((MergeSourceTwoDifferentTreesPage)mergeWizard.GetPage(MergeSourceTwoDifferentTreesPage.PAGE_NAME)).MergeSourceOne;
                else
                    mergeSource2TextBox.Text = Resources.NotApplicableShort;

                // Populate Revisions
                if (mergeType == MergeWizard.MergeType.TwoDifferentTrees)
                {
                    MergeSourceTwoDifferentTreesPage tdtPage = ((MergeSourceTwoDifferentTreesPage)mergeWizard.GetPage(MergeSourceTwoDifferentTreesPage.PAGE_NAME));

                    revisionsTextBox.Text = (tdtPage.MergeFromRevision != -1 ? tdtPage.MergeFromRevision.ToString() : Resources.HEAD) + "-" +
                        (tdtPage.MergeToRevision != -1 ? tdtPage.MergeToRevision.ToString() : Resources.HEAD);
                }
                else
                    if (mergeWizard.MergeRevisions == null)
                        revisionsTextBox.Text = Resources.All;
                    else
                        revisionsTextBox.Text = mergeWizard.MergeRevisionsAsString;

                // Populate Binary Conflicts
                if (mergeOptions.BinaryConflictResolution == MergeOptionsPage.ConflictResolutionOption.MARK)
                    binaryConflictsTextBox.Text = Resources.ConflictHandlingMark;
                else if (mergeOptions.BinaryConflictResolution == MergeOptionsPage.ConflictResolutionOption.MINE)
                    binaryConflictsTextBox.Text = Resources.ConflictHandlingMine;
                else if (mergeOptions.BinaryConflictResolution == MergeOptionsPage.ConflictResolutionOption.PROMPT)
                    binaryConflictsTextBox.Text = Resources.ConflictHandlingPrompt;
                else if (mergeOptions.BinaryConflictResolution == MergeOptionsPage.ConflictResolutionOption.THEIRS)
                    binaryConflictsTextBox.Text = Resources.ConflictHandlingTheirs;
                else if (mergeOptions.BinaryConflictResolution == MergeOptionsPage.ConflictResolutionOption.BASE)
                    binaryConflictsTextBox.Text = Resources.ConflictHandlingBase;

                // Populate Text Conflicts
                if (mergeOptions.TextConflictResolution == MergeOptionsPage.ConflictResolutionOption.MARK)
                    textConflictsTextBox.Text = Resources.ConflictHandlingMark;
                else if (mergeOptions.TextConflictResolution == MergeOptionsPage.ConflictResolutionOption.MINE)
                    textConflictsTextBox.Text = Resources.ConflictHandlingMine;
                else if (mergeOptions.TextConflictResolution == MergeOptionsPage.ConflictResolutionOption.PROMPT)
                    textConflictsTextBox.Text = Resources.ConflictHandlingPrompt;
                else if (mergeOptions.TextConflictResolution == MergeOptionsPage.ConflictResolutionOption.THEIRS)
                    textConflictsTextBox.Text = Resources.ConflictHandlingTheirs;
                else if (mergeOptions.TextConflictResolution == MergeOptionsPage.ConflictResolutionOption.BASE)
                    textConflictsTextBox.Text = Resources.ConflictHandlingBase;

                // Populate Depth
                if (mergeOptions.Depth == SharpSvn.SvnDepth.Children)
                    depthTextBox.Text = Resources.SvnDepthChildren;
                else if (mergeOptions.Depth == SharpSvn.SvnDepth.Empty)
                    depthTextBox.Text = Resources.SvnDepthEmpty;
                else if (mergeOptions.Depth == SharpSvn.SvnDepth.Files)
                    depthTextBox.Text = Resources.SvnDepthFiles;
                else if (mergeOptions.Depth == SharpSvn.SvnDepth.Infinity)
                    depthTextBox.Text = Resources.SvnDepthInfinity;
                else if (mergeOptions.Depth == SharpSvn.SvnDepth.Unknown)
                    depthTextBox.Text = Resources.SvnDepthUnknown;

                // Populate Ignore Ancestry
                if (mergeOptions.IgnoreAncestry)
                    ignoreAncestryTextBox.Text = Resources.Yes;
                else
                    ignoreAncestryTextBox.Text = Resources.No;

                // Populate Allow Unversioned Obstructions
                if (mergeOptions.AllowUnversionedObstructions)
                    unversionedObstructionsTextBox.Text = Resources.Yes;
                else
                    unversionedObstructionsTextBox.Text = Resources.No;
            }
        }

        #region UI Events
        private void MergeSummaryPageControl_Load(object sender, EventArgs e)
        {
            PopulateUI();

            ((MergeWizard)WizardPage.Wizard).WizardDialog.PageChanged += new EventHandler<WizardPageChangeEventArgs>(WizardDialog_PageChangeEvent);
        }

        private void performDryRunCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeWizard)WizardPage.Wizard).PerformDryRun = performDryRunCheckBox.Checked;
        }
        #endregion
    }
}
