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
    class MergeSourceManuallyRemovePage : WizardPage
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeSourceManuallyRemovePage()
            : base("Merge Source Manually Remove")
        {
            IsPageComplete = false;
            Title = resman.GetString("MergeSourceHeaderTitle");
            Description = resman.GetString("MergeSourceManuallyRemovePageHeaderMessage");
        }

        /// <see cref="WizardFramework.IWizardPage.Control" />
        public override System.Windows.Forms.UserControl Control
        {
            get { return control_prop; }
        }

        private System.Windows.Forms.UserControl control_prop = new MergeSourceManuallyRemovePageControl();
        private ResourceManager resman = new ResourceManager("Ankh.UI.MergeWizard.Resources", Assembly.GetExecutingAssembly());
    }
}