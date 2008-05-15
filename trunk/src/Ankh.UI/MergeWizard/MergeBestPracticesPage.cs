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
    /// practices checking of AnkhSVN's merge capabilities.
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
            Description = resman.GetString("MergeBestPracticesPageHeaderMessage");
        }

        /// <summary>
        /// Returns whether or not the best practices page needs to be displayed.
        /// This value is different than <code>MergeTypePage.ShowBestPractices</code>
        /// in that this code actually validates the WC for best practices and if all
        /// best practices are adhered to, the page doesn't need to be displayed.
        /// </summary>
        public bool DisplayBestPracticesPage
        {
            get
            {
                // TODO: Implement checks to validate the WC and return true
                // when there are best practices failures.
                return false;
            }
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
