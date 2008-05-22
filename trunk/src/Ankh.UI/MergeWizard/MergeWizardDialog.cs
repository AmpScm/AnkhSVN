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
            this.Wizard = wizard;
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }

        private Wizard wizard = new MergeWizard();
    }
}