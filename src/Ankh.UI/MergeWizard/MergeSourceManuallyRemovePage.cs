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
    /// information for a merge where you manually record revision changes.
    /// </summary>
    class MergeSourceManuallyRemovePage : MergeSourceBasePage
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeSourceManuallyRemovePage(MergeWizard wizard)
            : base(wizard, new MergeSourceManuallyRemovePageControl(), "Merge Source Manually Remove")
        {
            IsPageComplete = false;
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceManuallyRemovePageHeaderMessage;

            ((MergeSourceManuallyRemovePageControl)PageControl).EnableSelectButton(false);
        }
        /// <see cref="Ankh.UI.MergeWizard.MergeSourceBasePage" />
        public override MergeWizard.MergeType MergeType
        {
            get { return MergeWizard.MergeType.ManuallyRemove; }
        }
    }
}