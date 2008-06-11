﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WizardFramework;
using System.Resources;
using System.Reflection;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard page for handling the merge best
    /// practices checking of AnkhSVN's merge capabilities.
    /// </summary>
    public class MergeBestPracticesPage : BasePage
    {
        public static readonly WizardMessage READY_FOR_MERGE = new WizardMessage(Resources.ReadyForMerge,
            WizardMessage.NONE);
        public static readonly WizardMessage NOT_READY_FOR_MERGE = new WizardMessage(Resources.NotReadyForMerge,
            WizardMessage.ERROR);

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
        public MergeBestPracticesPage(MergeWizard wizard)
            : base(wizard, new MergeBestPracticesPageControl(), "Merge Best Practices")
        {
            IsPageComplete = true;

            Title = Resources.MergeBestPracticesPageHeaderTitle;
            Description = Resources.MergeBestPracticesPageHeaderMessage;

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
                MergeBestPracticesPageControl pageControl = (MergeBestPracticesPageControl)PageControl;

                if (wizard.MergeUtils == null)
                    return false; // Work around issue where the WizardFramework calls this
                                  // before the MergeUtils is set when instantiating the WizardDialog.

                using (SvnClient client = wizard.MergeUtils.GetClient())
                {
                    WizardDialog dialog = (WizardDialog)wizard.Form;
                    SvnItem mergeTarget = wizard.MergeTarget;
                    SvnWorkingCopyVersion wcRevision;

                    client.GetWorkingCopyVersion(wizard.MergeUtils.WorkingCopyRootPath,
                        out wcRevision);
                    
                    bool hasLocalModifications = wcRevision.Modified;
                    bool hasMixedRevisions = (wcRevision.Start != wcRevision.End);
                    bool hasSwitchedChildren = wcRevision.Switched;
                    bool isIncomplete = wcRevision.IncompleteWorkingCopy;
                    bool status = hasLocalModifications || hasMixedRevisions || hasSwitchedChildren || isIncomplete;

                    if (status)
                    {
                        Message = MergeBestPracticesPage.NOT_READY_FOR_MERGE;
                    }
                    else
                    {
                        Message = MergeBestPracticesPage.READY_FOR_MERGE;
                    }

                    // Update the images based on the return of the best practices checks
                    if (hasLocalModifications)
                        pageControl.UpdateBestPracticeStatus(BestPractices.NO_LOCAL_MODIFICATIONS, false);
                    else
                        pageControl.UpdateBestPracticeStatus(BestPractices.NO_LOCAL_MODIFICATIONS, true);

                    if (hasMixedRevisions)
                        pageControl.UpdateBestPracticeStatus(BestPractices.NO_MIXED_REVISIONS, false);
                    else
                        pageControl.UpdateBestPracticeStatus(BestPractices.NO_MIXED_REVISIONS, true);

                    if (hasSwitchedChildren)
                        pageControl.UpdateBestPracticeStatus(BestPractices.NO_SWITCHED_CHILDREN, false);
                    else
                        pageControl.UpdateBestPracticeStatus(BestPractices.NO_SWITCHED_CHILDREN, true);

                    if (isIncomplete)
                        pageControl.UpdateBestPracticeStatus(BestPractices.COMPLETE_WORKING_COPY, false);
                    else
                        pageControl.UpdateBestPracticeStatus(BestPractices.COMPLETE_WORKING_COPY, true);

                    return status;
                }
            }
        }
    }
}
