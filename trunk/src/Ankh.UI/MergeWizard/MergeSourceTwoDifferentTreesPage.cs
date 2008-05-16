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
    class MergeSourceTwoDifferentTreesPage : WizardPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MergeSourceTwoDifferentTreesPage() : base("Merge Source Two Different Trees")
        {
            IsPageComplete = false;
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceTwoDifferentTreesPageHeaderMessage;
        }

        /// <see cref="WizardFramework.IWizardPage.Control" />
        public override System.Windows.Forms.UserControl Control
        {
            get { return control_prop; }
        }

        private MergeSourceTwoDifferentTreesPageControl control_prop = new MergeSourceTwoDifferentTreesPageControl();
    }
}
