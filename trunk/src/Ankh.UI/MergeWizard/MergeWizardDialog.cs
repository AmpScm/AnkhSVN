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
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="utils"></param>
        public MergeWizardDialog(MergeUtils utils, SvnItem mergeTarget)
            : this()
        {
            ((MergeWizard)Wizard).MergeUtils = utils;
            ((MergeWizard)Wizard).MergeTarget = mergeTarget;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeWizardDialog() : base()
        {
            this.Text = "Merge Wizard";
            this.Icon = Resources.MergeWizardIcon;
            this.Wizard = _wizard;
        }

        Wizard _wizard = new MergeWizard();
    }
}