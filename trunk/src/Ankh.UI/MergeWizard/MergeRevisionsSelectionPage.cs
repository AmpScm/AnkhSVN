﻿using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of <code>WizardPage</code> to enable the user to select
    /// the merge revision(s).
    /// </summary>
    class MergeRevisionsSelectionPage : BasePage
    {
        public static readonly string PAGE_NAME = "Merge Revisions Selection Page";

        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeRevisionsSelectionPage(MergeWizard wizard)
            : base(wizard, new MergeRevisionsSelectionPageControl(), PAGE_NAME)
        {
            IsPageComplete = false;

            Title = Resources.MergeRevisionsSelectionPageTitle;
            this.Message = new WizardMessage(Resources.MergeRevisionsSelectionPageMessage);

            PageControl.WizardPage = this;
        }

        protected override void OnPageChanged(WizardPageChangeEventArgs e)
        {
            base.OnPageChanged(e);
        }
    }
}
