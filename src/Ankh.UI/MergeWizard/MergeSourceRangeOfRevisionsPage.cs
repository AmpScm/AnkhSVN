using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    class MergeSourceRangeOfRevisionsPage : WizardPage
    {
        public MergeSourceRangeOfRevisionsPage() : base("Merge Source Range Of Revisions")
        {
            IsPageComplete = false;
            Title = resman.GetString("MergeSourceHeaderTitle");
            this.Message = new WizardMessage(resman.GetString("MergeSourceRangeOfRevisionsPageHeaderMessage"));
        }

        /// <see cref="WizardFramework.IWizardPage.Control" />
        public override System.Windows.Forms.UserControl Control
        {
            get { return control_prop; }
        }

        private System.Windows.Forms.UserControl control_prop = new MergeSourceRangeOfRevisionsPageControl();
        private ResourceManager resman = new ResourceManager("Ankh.UI.MergeWizard.Resources", Assembly.GetExecutingAssembly());
    }
}
