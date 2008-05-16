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
            this.Icon = Resources.MergeWizardIcon;
            this.Wizard = wizard;
        }

        private Wizard wizard = new MergeWizard();
    }
}