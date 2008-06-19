using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using WizardFramework;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// Implementation of a wizard page for retrieving the merge source
    /// information for merging two different trees together.
    /// </summary>
    class MergeSourceTwoDifferentTreesPage : BasePage
    {
        public static readonly string PAGE_NAME = "Merge Source Two Different Trees";

        /// <summary>
        /// Constructor
        /// </summary>
        public MergeSourceTwoDifferentTreesPage(MergeWizard wizard) 
            : base(wizard, new MergeSourceTwoDifferentTreesPageControl(), PAGE_NAME)
        {
            IsPageComplete = false;
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceTwoDifferentTreesPageHeaderMessage;
            PageControl.WizardPage = this;
        }

        /// <summary>
        /// Gets/Sets the first merge source.
        /// </summary>
        public string MergeSourceOne
        {
            get { return _mergeSourceOne; }
            set { _mergeSourceOne = value; }
        }

        /// <summary>
        /// Gets/Sets the second merge source.
        /// </summary>
        public string MergeSourceTwo
        {
            get { return _mergeSourceTwo; }
            set { _mergeSourceTwo = value; }
        }

        /// <summary>
        /// Gets/Sets whether or not merge source one and
        /// merge source two have the same url.
        /// </summary>
        public bool HasSecondMergeSourceUrl
        {
            get { return _hasSecondMergeSourceUrl; }
            set { _hasSecondMergeSourceUrl = value; }
        }

        private bool _hasSecondMergeSourceUrl = false;
        private string _mergeSourceOne;
        private string _mergeSourceTwo;
    }
}
