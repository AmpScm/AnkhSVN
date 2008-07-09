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
    /// information for a branch reintegration merge scenario.
    /// </summary>
    class MergeSourceReintegratePage : MergeSourceBasePage
    {
        public const string PAGE_NAME = "Merge Source Reintegrate";

        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeSourceReintegratePage(MergeWizard wizard) 
            : base(wizard, new MergeSourceReintegratePageControl(), PAGE_NAME)
        {
            IsPageComplete = false;
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceReintegratePageHeaderMessage;
        }

        /// <see cref="Ankh.UI.MergeWizard.MergeSourceBasePage" />
        public override MergeWizard.MergeType MergeType
        {
            get { return MergeWizard.MergeType.Reintegrate; }
        }

        protected override void OnPageChanging(WizardPageChangingEventArgs e)
        {
            base.OnPageChanging(e);

            ((MergeWizard)Wizard).LogMode = Ankh.UI.SvnLog.LogMode.MergesEligible;
        }
    }
}