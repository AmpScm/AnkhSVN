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
    class MergeSourceManuallyRecordPage : MergeSourceBasePage
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeSourceManuallyRecordPage(MergeWizard wizard)
            : base(wizard, new MergeSourceManuallyRecordPageControl(), "Merge Source Manually Record")
        {
            IsPageComplete = false;
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceManuallyRecordPageHeaderMessage;
        }

        /// <see cref="Ankh.UI.MergeWizard.MergeSourceBasePage" />
        public override MergeWizard.MergeType MergeType
        {
            get { return MergeWizard.MergeType.ManuallyRecord; }
        }
    }
}