using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Resources;
using System.Reflection;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard dialog for AnkhSVN's merge capabilities.
    /// </summary>
    class MergeWizardDialog : WizardDialog
    {
        public MergeWizardDialog() : base()
        {
            this.Text = "Merge Wizard";
            this.Icon = (System.Drawing.Icon)resman.GetObject("MergeWizardIcon");
            this.Wizard = wizard;
        }

        private Wizard wizard = new MergeWizard();
        private ResourceManager resman = new ResourceManager("Ankh.UI.MergeWizard.Resources", Assembly.GetExecutingAssembly());
    }
}