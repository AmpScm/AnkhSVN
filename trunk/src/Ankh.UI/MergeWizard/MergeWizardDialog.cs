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
    partial class MergeWizardDialog : WizardDialog
    {
        IAnkhServiceProvider _context;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="utils"></param>
        public MergeWizardDialog(IAnkhServiceProvider context, MergeUtils utils, SvnItem mergeTarget)
        {
            Icon = Resources.MergeWizardIcon;
            Wizard = new MergeWizard(context, this);

            _context = context;
            Wizard.MergeUtils = utils;
            Wizard.MergeTarget = mergeTarget;
        }

        public new MergeWizard Wizard
        {
            get { return (MergeWizard)base.Wizard; }
            set { base.Wizard = value; }
        }        
    }
}