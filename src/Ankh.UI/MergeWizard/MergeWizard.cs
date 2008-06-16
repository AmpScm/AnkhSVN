using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// This is the wizard implementation for AnkhSVN's merge capability.
    /// </summary>
    public class MergeWizard : AnkhWizard
    {
        /// <summary>
        /// Enumeration of available merge types.
        /// </summary>
        public enum MergeType
        {
            RangeOfRevisions,
            Reintegrate,
            TwoDifferentTrees,
            ManuallyRecord,
            ManuallyRemove
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeWizard(IAnkhServiceProvider context, IWizardContainer container)
            : base(context)
        {
            Container = container;
            this.WindowTitle = Resources.MergeWizardTitle;

            mergeTypePage = new MergeTypePage(this);
            bestPracticesPage = new MergeBestPracticesPage(this);
            mergeSourceRangeOfRevisionsPage = new MergeSourceRangeOfRevisionsPage(this);
            mergeSourceReintegratePage = new MergeSourceReintegratePage(this);
            mergeSourceTwoDifferentTreesPage = new MergeSourceTwoDifferentTreesPage(this);
            mergeSourceManuallyRecordPage = new MergeSourceManuallyRecordPage(this);
            mergeSourceManuallyRemovePage = new MergeSourceManuallyRemovePage(this);
            mergeRevisionsSelectionPage = new MergeRevisionsSelectionPage(this);
            mergeOptionsPage = new MergeOptionsPage(this);
        }

        public override void AddPages()
        {
            AddPage(mergeTypePage);
            AddPage(bestPracticesPage);
            AddPage(mergeSourceRangeOfRevisionsPage);
            AddPage(mergeSourceReintegratePage);
            AddPage(mergeSourceTwoDifferentTreesPage);
            AddPage(mergeSourceManuallyRecordPage);
            AddPage(mergeSourceManuallyRemovePage);
            AddPage(mergeRevisionsSelectionPage);
            AddPage(mergeOptionsPage);
        }

        /// <see cref="WizardFramework.IWizard.GetNextPage" />
        public override IWizardPage GetNextPage(IWizardPage page)
        {
            // Handle the main page
            if (page is MergeTypePage)
            {
                // Handle displaying the best practices page
                if (((MergeTypePage)page).ShowBestPracticesPage 
                    && ((MergeBestPracticesPage)bestPracticesPage).DisplayBestPracticesPage)
                    return bestPracticesPage;
                else
                    switch (((MergeTypePage)page).SelectedMergeType)
                    {
                        case MergeType.RangeOfRevisions:
                            return mergeSourceRangeOfRevisionsPage;
                        case MergeType.Reintegrate:
                            return mergeSourceReintegratePage;
                        case MergeType.TwoDifferentTrees:
                            return mergeSourceTwoDifferentTreesPage;
                        case MergeType.ManuallyRecord:
                            return mergeSourceManuallyRecordPage;
                        case MergeType.ManuallyRemove:
                            return mergeSourceManuallyRemovePage;
                        default:
                            return null;
                    }
            }

            // Handle the best practices page
            if (page is MergeBestPracticesPage)
                return null; // For now, if you see the best practices page,
                             // you have to fix the issue and then reattempt to merge.

            // Handle the range of revisions page
            if (page is MergeSourceRangeOfRevisionsPage)
            {
                if (((MergeSourceRangeOfRevisionsPage)page).NextPageRequired)
                    return mergeRevisionsSelectionPage;
                else
                    return mergeOptionsPage;
            }

            // Handle the reintegrate page
            // Handle the revision selection page
            // Handle the two different trees page
            if (page is MergeSourceReintegratePage ||
                page is MergeRevisionsSelectionPage ||
                page is MergeSourceTwoDifferentTreesPage)
                return mergeOptionsPage;

            // Handle the manually record/remove pages
            if (page is MergeSourceManuallyRecordPage || page is MergeSourceManuallyRemovePage)
                return mergeRevisionsSelectionPage;

            // Handle the merge options page
            if (page is MergeOptionsPage)
                return null;

            return null;
        }

        /// <see cref="WizardFramework.IWizard.CanFinish" />
        public override bool CanFinish
        {
            get
            {
                if (Container.CurrentPage is MergeTypePage)
                    return false;

                if (Container.CurrentPage is MergeBestPracticesPage)
                    return false;

                if (Container.CurrentPage is MergeSourceRangeOfRevisionsPage)
                {
                    return Container.CurrentPage.IsPageComplete &&
                        !((MergeSourceRangeOfRevisionsPage)Container.CurrentPage).NextPageRequired;
                }

                if (Container.CurrentPage is MergeSourceManuallyRecordPage)
                    return false;
                
                return Container.CurrentPage.IsPageComplete;
            }
        }

        /// <see cref="WizardFramework.IWizard.PerformFinish" />
        public override bool PerformFinish()
        {
            // TODO: Implement
            this.Form.DialogResult = DialogResult.OK;

            return true;
        }

        /// <see cref="WizardFramework.IWizard.DefaultPageImage" />
        public override System.Drawing.Image DefaultPageImage
        {
            get
            {
                return Resources.MergeWizardHeaderImage;
            }
        }

        public MergeUtils MergeUtils
        {
            get { return _mergeUtils; }
            set { _mergeUtils = value; }
        }


        /// <summary>
        /// Gets or sets the merge target.
        /// </summary>
        /// <value>The merge target.</value>
        public SvnItem MergeTarget
        {
            get { return _mergeTarget; }
            set { _mergeTarget = value; }
        }

        string _mergeSource;
        /// <summary>
        /// Gets or sets the merge source.
        /// </summary>
        /// <value>The merge source.</value>
        public string MergeSource
        {
            get { return _mergeSource; }
            set { _mergeSource = value; }
        }

        internal MergeWizardDialog WizardDialog
        {
            get { return (MergeWizardDialog)Form; }
            //set { Form = value; }
        }

        private WizardPage mergeTypePage;
        private WizardPage bestPracticesPage;
        private WizardPage mergeSourceRangeOfRevisionsPage;
        private WizardPage mergeSourceReintegratePage;
        private WizardPage mergeSourceTwoDifferentTreesPage;
        private WizardPage mergeSourceManuallyRecordPage;
        private WizardPage mergeSourceManuallyRemovePage;
        private WizardPage mergeRevisionsSelectionPage;
        private WizardPage mergeOptionsPage;

        MergeUtils _mergeUtils = null;
        SvnItem _mergeTarget = null;
    }
}
