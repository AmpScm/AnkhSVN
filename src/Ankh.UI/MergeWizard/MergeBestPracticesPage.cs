using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WizardFramework;
using System.Resources;
using System.Reflection;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard page for handling the merge best
    /// practices checking of AnkhSVN's merge capabilities
    /// </summary>
    class MergeBestPracticesPage : WizardPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MergeBestPracticesPage()
            : base("Merge Best Practices")
        {
            IsPageComplete = true;

            Title = resman.GetString("MergeBestPracticesPageHeaderTitle");
            this.Message = new WizardMessage(resman.GetString("MergeBestPracticesPageHeaderMessage"));
        }

        /// <see cref="WizardFramework.WizardPage.Control" />
        public override System.Windows.Forms.UserControl Control
        {
            get { return control_prop; }
        }

        private UserControl control_prop = new MergeBestPracticesPageControl();
        private ResourceManager resman = new ResourceManager("Ankh.UI.MergeWizard.Resources", Assembly.GetExecutingAssembly());
    }
}
