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
    class MergeSourceReintegratePage : WizardPage
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeSourceReintegratePage() : base("Merge Source Reintegrate")
        {
            IsPageComplete = false;
            Title = resman.GetString("MergeSourceHeaderTitle");
            Description = resman.GetString("MergeSourceReintegratePageHeaderMessage");
        }

        /// <see cref="WizardFramework.IWizardPage.Control" />
        public override System.Windows.Forms.UserControl Control
        {
            get { return control_prop; }
        }

        private System.Windows.Forms.UserControl control_prop = new MergeSourceReintegratePageControl();
        private ResourceManager resman = new ResourceManager("Ankh.UI.MergeWizard.Resources", Assembly.GetExecutingAssembly());
    }
}