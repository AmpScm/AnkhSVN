using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Resources;
using System.Reflection;

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
            AddPage(mergeType);
        }

        /// <see cref="WizardFramework.Wizard.PerformFinish" />
        public override bool PerformFinish()
        {
            // TODO: Implement
            return true;
        }

        private ResourceManager resman = new ResourceManager("Ankh.UI.MergeWizard.Resources", Assembly.GetExecutingAssembly());
    }
}
