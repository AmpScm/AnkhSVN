using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard page for handling the merge type
    /// selection of AnkhSVN's merge capabilities
    /// </summary>
    class MergeTypePage : BasePage
    {
        public MergeTypePage(MergeWizard wizard)
            : base(wizard, new MergeTypePageControl(), "Merge Type")
        {
            IsPageComplete = true;

            Title = Resources.MergeTypePageHeaderTitle;
            this.Message = new WizardMessage(Resources.MergeTypePageHeaderMessage);

            PageControl.WizardPage = this;
        }

        /// <summary>
        /// Returns whether or not to show the best practices page.
        /// </summary>
        public bool ShowBestPracticesPage
        {
            get { return ((MergeTypePageControl)PageControl).IsPerformBestPracticesChecked; }
        }

        public MergeWizard.MergeType SelectedMergeType
        {
            get
            {
                return mergeType_prop;
            }

            set
            {
                mergeType_prop = value;
            }
        }

        private MergeWizard.MergeType mergeType_prop = MergeWizard.MergeType.RangeOfRevisions;
    }
}
