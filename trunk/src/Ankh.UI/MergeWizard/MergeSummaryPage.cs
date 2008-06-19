using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Wizard page that will display the current merge options.
    /// </summary>
    class MergeSummaryPage : BasePage
    {
        public static readonly string PAGE_NAME = "Merge Summary Page";

        public MergeSummaryPage(MergeWizard wizard)
            : base(wizard, new MergeSummaryPageControl(), PAGE_NAME)
        {
            IsPageComplete = true;

            Title = Resources.MergeSummaryPageHeaderTitle;
            this.Description = Resources.MergeSummaryPageHeaderMessage;

            PageControl.WizardPage = this;
        }
    }
}
