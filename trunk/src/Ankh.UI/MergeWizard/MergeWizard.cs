using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// This is the wizard implementation for AnkhSVN's merge capability.
    /// </summary>
    class MergeWizard : Wizard
    {
        public MergeWizard()
            : base()
        {
            this.WindowTitle = resman.GetString("MergeWizardTitle");
        }

        public override void AddPages()
        {
            WizardPage mergeType = new MergeTypePage();
            WizardPage bestPractices = new MergeBestPracticesPage();
            AddPage(mergeType);
            AddPage(bestPractices);
        }

        /// <see cref="WizardFramework.Wizard.PerformFinish" />
        public override bool PerformFinish()
        {
            // TODO: Implement
            this.Form.DialogResult = DialogResult.OK;

            return true;
        }

        /// <see cref="WizardFramework.IWizard.DefaultPageImage" />
        public override System.Drawing.Image DefaultPageImage
        {
            get
            {
                return (System.Drawing.Image)resman.GetObject("MergeWizardHeaderImage");
            }
        }

        private ResourceManager resman = new ResourceManager("Ankh.UI.MergeWizard.Resources", Assembly.GetExecutingAssembly());
    }
}
