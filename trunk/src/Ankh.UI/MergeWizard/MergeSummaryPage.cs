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

using Ankh.UI.WizardFramework;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeSummaryPage : BaseWizardPage
    {
        public const string PAGE_NAME = "Merge Summary Page";

        public MergeSummaryPage()
        {
            Name = PAGE_NAME;
            IsPageComplete = true;

            Text = MergeStrings.MergeSummaryPageHeaderTitle;
            this.Description = MergeStrings.MergeSummaryPageHeaderMessage;
            InitializeComponent();
        }

        private void WizardDialog_PageChangeEvent(object sender, EventArgs e)
        {
            if (Wizard.CurrentPage == this)
            {
                Message = null;

                PopulateUI();
            }
        }

        private void PopulateUI()
        {
            if (!DesignMode)
            {
                MergeWizard mergeWizard = ((MergeWizard)Wizard);
                MergeWizard.MergeType mergeType = mergeWizard.GetPage<MergeTypePage>().SelectedMergeType;
                MergeOptionsPage mergeOptions = mergeWizard.GetPage<MergeOptionsPage>();

                // Populate Merge Target
                mergeTargetTextBox.Text = mergeWizard.MergeTarget.FullPath;
                // Populate Merge Source 1
                if (mergeType == MergeWizard.MergeType.TwoDifferentTrees)
                    mergeSource1TextBox.Text = mergeWizard.GetPage<MergeSourceTwoDifferentTreesPage>().MergeSourceOne;
                else
                    mergeSource1TextBox.Text = mergeWizard.MergeSource.Target.ToString();

                // Populate Merge Source 2
                if (mergeType == MergeWizard.MergeType.TwoDifferentTrees)
                    if (mergeWizard.GetPage<MergeSourceTwoDifferentTreesPage>().HasSecondMergeSourceUrl)
                        mergeSource2TextBox.Text = mergeWizard.GetPage<MergeSourceTwoDifferentTreesPage>().MergeSourceTwo;
                    else
                        mergeSource2TextBox.Text = mergeWizard.GetPage<MergeSourceTwoDifferentTreesPage>().MergeSourceOne;
                else
                    mergeSource2TextBox.Text = MergeStrings.NotApplicableShort;

                // Populate Revisions
                if (mergeType == MergeWizard.MergeType.TwoDifferentTrees)
                {
                    MergeSourceTwoDifferentTreesPage tdtPage = mergeWizard.GetPage<MergeSourceTwoDifferentTreesPage>();

                    revisionsTextBox.Text = (tdtPage.MergeFromRevision != -1 ? tdtPage.MergeFromRevision.ToString() : MergeStrings.HEAD) + "-" +
                        (tdtPage.MergeToRevision != -1 ? tdtPage.MergeToRevision.ToString() : MergeStrings.HEAD);
                }
                else
                    if (mergeWizard.MergeRevisions == null)
                        revisionsTextBox.Text = MergeStrings.All;
                    else
                        revisionsTextBox.Text = MergeWizard.MergeRevisionsAsString(mergeWizard.MergeRevisions);

                // Populate Binary Conflicts
                if (mergeOptions.BinaryConflictResolution == MergeOptionsPage.ConflictResolutionOption.MARK)
                    binaryConflictsTextBox.Text = MergeStrings.ConflictHandlingMark;
                else if (mergeOptions.BinaryConflictResolution == MergeOptionsPage.ConflictResolutionOption.MINE)
                    binaryConflictsTextBox.Text = MergeStrings.ConflictHandlingMine;
                else if (mergeOptions.BinaryConflictResolution == MergeOptionsPage.ConflictResolutionOption.PROMPT)
                    binaryConflictsTextBox.Text = MergeStrings.ConflictHandlingPrompt;
                else if (mergeOptions.BinaryConflictResolution == MergeOptionsPage.ConflictResolutionOption.THEIRS)
                    binaryConflictsTextBox.Text = MergeStrings.ConflictHandlingTheirs;
                else if (mergeOptions.BinaryConflictResolution == MergeOptionsPage.ConflictResolutionOption.BASE)
                    binaryConflictsTextBox.Text = MergeStrings.ConflictHandlingBase;

                // Populate Text Conflicts
                if (mergeOptions.TextConflictResolution == MergeOptionsPage.ConflictResolutionOption.MARK)
                    textConflictsTextBox.Text = MergeStrings.ConflictHandlingMark;
                else if (mergeOptions.TextConflictResolution == MergeOptionsPage.ConflictResolutionOption.MINE)
                    textConflictsTextBox.Text = MergeStrings.ConflictHandlingMine;
                else if (mergeOptions.TextConflictResolution == MergeOptionsPage.ConflictResolutionOption.PROMPT)
                    textConflictsTextBox.Text = MergeStrings.ConflictHandlingPrompt;
                else if (mergeOptions.TextConflictResolution == MergeOptionsPage.ConflictResolutionOption.THEIRS)
                    textConflictsTextBox.Text = MergeStrings.ConflictHandlingTheirs;
                else if (mergeOptions.TextConflictResolution == MergeOptionsPage.ConflictResolutionOption.BASE)
                    textConflictsTextBox.Text = MergeStrings.ConflictHandlingBase;

                // Populate Depth
                if (mergeOptions.Depth == SharpSvn.SvnDepth.Children)
                    depthTextBox.Text = MergeStrings.SvnDepthChildren;
                else if (mergeOptions.Depth == SharpSvn.SvnDepth.Empty)
                    depthTextBox.Text = MergeStrings.SvnDepthEmpty;
                else if (mergeOptions.Depth == SharpSvn.SvnDepth.Files)
                    depthTextBox.Text = MergeStrings.SvnDepthFiles;
                else if (mergeOptions.Depth == SharpSvn.SvnDepth.Infinity)
                    depthTextBox.Text = MergeStrings.SvnDepthInfinity;
                else if (mergeOptions.Depth == SharpSvn.SvnDepth.Unknown)
                    depthTextBox.Text = MergeStrings.SvnDepthUnknown;

                // Populate Ignore Ancestry
                if (mergeOptions.IgnoreAncestry)
                    ignoreAncestryTextBox.Text = MergeStrings.Yes;
                else
                    ignoreAncestryTextBox.Text = MergeStrings.No;

                // Populate Allow Unversioned Obstructions
                if (mergeOptions.AllowUnversionedObstructions)
                    unversionedObstructionsTextBox.Text = MergeStrings.Yes;
                else
                    unversionedObstructionsTextBox.Text = MergeStrings.No;
            }
        }

        #region UI Events
        private void MergeSummaryPage_Load(object sender, EventArgs e)
        {
            PopulateUI();

            ((MergeWizard)Wizard).PageChanged += new EventHandler(WizardDialog_PageChangeEvent);
        }

        private void performDryRunCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ((MergeWizard)Wizard).PerformDryRun = performDryRunCheckBox.Checked;
        }
        #endregion
    }
}
