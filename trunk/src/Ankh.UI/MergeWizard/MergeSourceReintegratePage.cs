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
        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeSourceReintegratePage() : base("Merge Source Reintegrate")
        {
            IsPageComplete = false;
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceReintegratePageHeaderMessage;
        }

        /// <see cref="WizardFramework.IWizardPage.Control" />
        public override System.Windows.Forms.UserControl Control
        {
            get { return control_prop; }
        }

        /// <see cref="Ankh.UI.MergeWizard.MergeSourceBasePage" />
        public override MergeWizard.MergeType MergeType
        {
            get { return MergeWizard.MergeType.Reintegrate; }
        }

        private MergeSourceReintegratePageControl control_prop = new MergeSourceReintegratePageControl();
    }
}