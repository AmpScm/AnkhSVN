using System;
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
    public class MergeBestPracticesPage : WizardPage
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
        public MergeBestPracticesPage()
            : base("Merge Best Practices")
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
                if (((MergeWizard)Wizard).MergeUtils == null)
                    return false; // Work around issue where the WizardFramework calls this
                                  // before the MergeUtils is set when instantiating the WizardDialog.

                using (SvnClient client = ((MergeWizard)Wizard).MergeUtils.GetClient())
                {
                    MergeWizard wizard = ((MergeWizard)Wizard);
                    WizardDialog dialog = (WizardDialog)wizard.Form;
                    SvnItem mergeTarget = wizard.MergeTarget;
                    SvnWorkingCopyVersion wcRevision;

                    client.GetWorkingCopyVersion(((MergeWizard)Wizard).MergeUtils.WorkingCopyRootPath,
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

                    dialog.UpdateMessage();

                    // Update the images based on the return of the best practices checks
                    if (hasLocalModifications)
                        control_prop.UpdateBestPracticeStatus(BestPractices.NO_LOCAL_MODIFICATIONS, false);
                    else
                        control_prop.UpdateBestPracticeStatus(BestPractices.NO_LOCAL_MODIFICATIONS, true);

                    if (hasMixedRevisions)
                        control_prop.UpdateBestPracticeStatus(BestPractices.NO_MIXED_REVISIONS, false);
                    else
                        control_prop.UpdateBestPracticeStatus(BestPractices.NO_MIXED_REVISIONS, true);

                    if (hasSwitchedChildren)
                        control_prop.UpdateBestPracticeStatus(BestPractices.NO_SWITCHED_CHILDREN, false);
                    else
                        control_prop.UpdateBestPracticeStatus(BestPractices.NO_SWITCHED_CHILDREN, true);

                    if (isIncomplete)
                        control_prop.UpdateBestPracticeStatus(BestPractices.COMPLETE_WORKING_COPY, false);
                    else
                        control_prop.UpdateBestPracticeStatus(BestPractices.COMPLETE_WORKING_COPY, true);

                    return status;
                }
            }
        }

        /// <see cref="WizardFramework.WizardPage.Control" />
        public override System.Windows.Forms.UserControl Control
        {
            get { return control_prop; }
        }

        private MergeBestPracticesPageControl control_prop = new MergeBestPracticesPageControl();
    }
}
