using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of <code>WizardFramework.WizardPage</code> for handling
    /// conflict resolution.
    /// </summary>
    class MergeOptionsPage : BasePage
    {
        public static readonly string PAGE_NAME = "Merge Options Page";

        public MergeOptionsPage(MergeWizard wizard)
            : base(wizard, new MergeOptionsPageControl(), PAGE_NAME)
        {
            IsPageComplete = true;

            Title = Resources.MergeOptionsHeaderTitle;
            this.Description = Resources.MergeOptionsHeaderMessage;

            PageControl.WizardPage = this;
        }
    }
}
