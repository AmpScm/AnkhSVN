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
        public MergeSummaryPage()
        {
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
            if (DesignMode)
                return;

            MergeWizard.MergeType mergeType =
                Wizard.GetPage<MergeTypePage>().SelectedMergeType;
            MergeOptionsPage mergeOptions =
                Wizard.GetPage<MergeOptionsPage>();

            string sourceOne = null;
            string sourceTwo = null;
            bool hasSecondSource = false;
            long fromRevision = -1;
            long toRevision = -1;

            if (mergeType == MergeWizard.MergeType.TwoDifferentTrees)
            {
                MergeSourceTwoDifferentTreesPage twoTreePage =
                    Wizard.GetPage<MergeSourceTwoDifferentTreesPage>();

                sourceOne = twoTreePage.MergeSourceOne;
                sourceTwo = twoTreePage.MergeSourceTwo;
                hasSecondSource = twoTreePage.HasSecondMergeSourceUrl;
                fromRevision = twoTreePage.MergeFromRevision;
                toRevision = twoTreePage.MergeToRevision;
            }

            string mergeRevisions = null;
            if (Wizard.MergeRevisions != null)
                mergeRevisions = MergeWizard.MergeRevisionsAsString(Wizard.MergeRevisions);

            MergeSummaryModel model = MergeSummaryLogic.Build(
                new MergeSummaryInput(
                    mergeType,
                    Wizard.MergeTarget.FullPath,
                    Wizard.MergeSource.Target.ToString(),
                    sourceOne,
                    sourceTwo,
                    hasSecondSource,
                    fromRevision,
                    toRevision,
                    mergeRevisions,
                    mergeOptions.BinaryConflictResolution,
                    mergeOptions.TextConflictResolution,
                    mergeOptions.Depth,
                    mergeOptions.IgnoreAncestry,
                    mergeOptions.AllowUnversionedObstructions));

            mergeTargetTextBox.Text = model.MergeTarget;
            mergeSource1TextBox.Text = model.MergeSourceOne;
            mergeSource2TextBox.Text = model.MergeSourceTwo;
            revisionsTextBox.Text = model.Revisions;
            binaryConflictsTextBox.Text = model.BinaryConflicts;
            textConflictsTextBox.Text = model.TextConflicts;
            depthTextBox.Text = model.Depth;
            ignoreAncestryTextBox.Text = model.IgnoreAncestry;
            unversionedObstructionsTextBox.Text = model.AllowUnversionedObstructions;
        }

        #region UI Events
        private void MergeSummaryPage_Load(object sender, EventArgs e)
        {
            PopulateUI();

            Wizard.PageChanged += new EventHandler(WizardDialog_PageChangeEvent);
        }

        private void performDryRunCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Wizard.PerformDryRun = performDryRunCheckBox.Checked;
        }
        #endregion
    }
}
