using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeRevisionsSelectionPageControl : UserControl
    {
        private WizardPage _wizardPage;

        public MergeRevisionsSelectionPageControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets/Sets the wizard page associated with this control.
        /// </summary>
        public WizardPage WizardPage
        {
            set { _wizardPage = value; }
            get { return _wizardPage; }
        }
    }
}
