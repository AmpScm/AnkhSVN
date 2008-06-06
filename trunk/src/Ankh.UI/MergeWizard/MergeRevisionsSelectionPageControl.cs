﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WizardFramework;
using SharpSvn;

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
        /// Gets or sets the merge source.
        /// </summary>
        /// <value>The merge source.</value>
        public string MergeSource
        {
            get { return ((MergeWizard)WizardPage.Wizard).MergeSource; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            logToolControl1.Site = Site;
            if(MergeSource != null)
                logToolControl1.Start(new string[]{MergeSource.ToString()});
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
