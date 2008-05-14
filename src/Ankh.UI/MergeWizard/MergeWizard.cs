using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// This is the wizard implementation for AnkhSVN's merge capability.
    /// </summary>
    class MergeWizard : Wizard
    {
        public MergeWizard()
            : base()
        {
            this.WindowTitle = resman.GetString("MergeWizardTitle");
        }

        public override void AddPages()
        {
            AddPage(mergeTypePage);
            AddPage(bestPracticesPage);
            AddPage(mergeSourceRangeOfRevisionsPage);
            AddPage(mergeSourceReintegratePage);
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
                        case MergeTypePage.MergeType.RangeOfRevisions:
                            return mergeSourceRangeOfRevisionsPage;
                        case MergeTypePage.MergeType.Reintegrate:
                            return mergeSourceReintegratePage;
                        case MergeTypePage.MergeType.TwoDifferentTrees:
                            return null;
                        case MergeTypePage.MergeType.ManuallyRecord:
                            return null;
                        case MergeTypePage.MergeType.ManuallyRemove:
                            return null;
                        default:
                            return null;
                    }
            }

            // Handle the best practices page
            if (page is MergeBestPracticesPage)
                return null; // For now, if you see the best practices page,
                             // you have to fix the issue and then reattemp to merge.

            return base.GetNextPage(page);
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

                return base.CanFinish;
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
                return (System.Drawing.Image)resman.GetObject("MergeWizardHeaderImage");
            }
        }

        private WizardPage mergeTypePage = new MergeTypePage();
        private WizardPage bestPracticesPage = new MergeBestPracticesPage();
        private WizardPage mergeSourceRangeOfRevisionsPage = new MergeSourceRangeOfRevisionsPage();
        private WizardPage mergeSourceReintegratePage = new MergeSourceReintegratePage();
        private ResourceManager resman = new ResourceManager("Ankh.UI.MergeWizard.Resources", Assembly.GetExecutingAssembly());
    }
}
