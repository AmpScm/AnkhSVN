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
using System.Drawing;
using System.Windows.Forms;
using Ankh.UI.WizardFramework;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeBestPracticesPage : BaseWizardPage
    {
        public static readonly WizardMessage READY_FOR_MERGE = new WizardMessage(MergeStrings.ReadyForMerge,
            WizardMessage.MessageType.None);
        public static readonly WizardMessage NOT_READY_FOR_MERGE = new WizardMessage(MergeStrings.NotReadyForMerge,
            WizardMessage.MessageType.Error);

        /// <summary>
        /// Enumeration for the best practices checks performed by AnkhSVN.
        /// </summary>
        public enum BestPractices
        {
            NO_LOCAL_MODIFICATIONS,
            NO_MIXED_REVISIONS,
            NO_SWITCHED_CHILDREN,
            COMPLETE_WORKING_COPY,
            VALID_REVISION
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeBestPracticesPage()
        {
            IsPageComplete = true;

            Text = MergeStrings.MergeBestPracticesPageHeaderTitle;
            Description = MergeStrings.MergeBestPracticesPageHeaderMessage;
            InitializeComponent();
        }

        /// <summary>
        /// Returns whether or not the best practices page needs to be displayed.
        /// This value is different than <code>MergeTypePage.ShowBestPractices</code>
        /// in that this code actually validates the WC for best practices and if all
        /// best practices are adhered to, the page doesn't need to be displayed.
        /// </summary>
        public bool DisplayBestPracticesPage
        {
            get
            {
                MergeWizard wizard = (MergeWizard)Wizard;

                if (wizard.MergeUtils == null)
                    return false; // Work around issue where the WizardFramework calls this
                // before the MergeUtils is set when instantiating the WizardDialog.

                using (SvnWorkingCopyClient client = wizard.MergeUtils.GetWcClient())
                {
                    SvnItem mergeTarget = wizard.MergeTarget;

                    SvnWorkingCopyVersion wcRevision;
                    client.GetVersion(mergeTarget.FullPath, out wcRevision);

                    bool hasLocalModifications = wcRevision.Modified;
                    bool hasMixedRevisions = (wcRevision.Start != wcRevision.End);
                    bool hasSwitchedChildren = wcRevision.Switched;
                    bool isIncomplete = wcRevision.IncompleteWorkingCopy;

                    bool statusNotOk = hasLocalModifications || hasMixedRevisions || hasSwitchedChildren || isIncomplete;

                    Message = statusNotOk ? NOT_READY_FOR_MERGE : READY_FOR_MERGE;

                    // Update the images based on the return of the best practices checks
                    UpdateBestPracticeStatus(BestPractices.NO_LOCAL_MODIFICATIONS, !hasLocalModifications);
                    UpdateBestPracticeStatus(BestPractices.NO_MIXED_REVISIONS, !hasMixedRevisions);
                    UpdateBestPracticeStatus(BestPractices.NO_SWITCHED_CHILDREN, !hasSwitchedChildren);
                    UpdateBestPracticeStatus(BestPractices.COMPLETE_WORKING_COPY, !isIncomplete);

                    return statusNotOk;
                }
            }
        }

        public void UpdateBestPracticeStatus(MergeBestPracticesPage.BestPractices bestPractice,
            bool passed)
        {
            PictureBox pBox;

            switch (bestPractice)
            {
                case MergeBestPracticesPage.BestPractices.NO_LOCAL_MODIFICATIONS:
                    pBox = noUncommitedModificationsPictureBox;
                    break;
                case MergeBestPracticesPage.BestPractices.NO_MIXED_REVISIONS:
                    pBox = singleRevisionPictureBox;
                    break;
                case MergeBestPracticesPage.BestPractices.NO_SWITCHED_CHILDREN:
                    pBox = noSwitchedChildrenPictureBox;
                    break;
                case MergeBestPracticesPage.BestPractices.COMPLETE_WORKING_COPY:
                    pBox = completeWorkingCopyPictureBox;
                    break;
                default:
                    pBox = null;
                    break;
            }

            if (pBox != null)
            {
                if (passed)
                    pBox.Image = MergeStrings.SuccessImage;
                else
                    pBox.Image = MergeStrings.ErrorImage;
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            Font boldFont = new Font(Font, FontStyle.Bold);
            noUncommitedModificationsLabel.Font = boldFont;
            singleRevisionLabel.Font = boldFont;
            noSwitchedChildrenLabel.Font = boldFont;
            completeWorkingCopyLabel.Font = boldFont;
            validRevisionLabel.Font = boldFont;
        }
    }
}
