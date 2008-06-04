using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Resources;
using System.Reflection;
using System.ComponentModel;

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
        public MergeWizardDialog(ISite site, MergeUtils utils, SvnItem mergeTarget)
            : this(site)
        {
            
            Wizard.MergeUtils = utils;
            Wizard.MergeTarget = mergeTarget;
        }

        public new MergeWizard Wizard
        {
            get { return (MergeWizard)base.Wizard; }
            set { base.Wizard = value; }
        }
        /// Constructor.
        /// </summary>
        public MergeWizardDialog(ISite site) : base()
        {
            Site = site;
            Text = "Merge Wizard";
            Icon = Resources.MergeWizardIcon;
            Wizard = new MergeWizard(this);
        }
    }
}