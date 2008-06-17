using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard page for retrieving the merge source
    /// information for merging two different trees together.
    /// </summary>
    class MergeSourceTwoDifferentTreesPage : BasePage
    {
        public static readonly string PAGE_NAME = "Merge Source Two Different Trees";

        /// <summary>
        /// Constructor
        /// </summary>
        public MergeSourceTwoDifferentTreesPage(MergeWizard wizard) 
            : base(wizard, new MergeSourceTwoDifferentTreesPageControl(), PAGE_NAME)
        {
            IsPageComplete = false;
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceTwoDifferentTreesPageHeaderMessage;
            PageControl.WizardPage = this;
        }
    }
}
