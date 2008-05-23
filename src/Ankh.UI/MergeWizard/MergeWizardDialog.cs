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
        IAnkhServiceProvider _context;

        public MergeWizardDialog() : base()
        {
            this.Text = "Merge Wizard";
            this.Icon = Resources.MergeWizardIcon;
            this.Wizard = _wizard;
        }

        /// <summary>
        /// Returns an instance of the wizard.
        /// </summary>
        public MergeWizard GetWizard()
        {
            return (MergeWizard)Wizard;
        }

        Wizard _wizard = new MergeWizard();
    }
}