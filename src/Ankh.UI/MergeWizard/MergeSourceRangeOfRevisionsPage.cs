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
    /// information for your typical merge scenario.
    /// </summary>
    class MergeSourceRangeOfRevisionsPage : WizardPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MergeSourceRangeOfRevisionsPage() : base("Merge Source Range Of Revisions")
        {
            NextPageRequired = false;
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceRangeOfRevisionsPageHeaderMessage;
            _control.WizardPage = this;
        }

        /// <summary>
        /// Gets/Sets whether or not the next page is required.
        /// </summary>
        public bool NextPageRequired
        {
            get { return _needsNextPage; }
            set
            {
                _needsNextPage = value;

                if (Form != null)
                    ((WizardDialog)Form).UpdateButtons();
            }
        }

        /// <see cref="WizardFramework.IWizardPage.IsPageComplete" />
        public override bool IsPageComplete
        {
            get
            {
                if (NextPageRequired)
                    return false;

                if (!((MergeSourceRangeOfRevisionsPageControl)Control).HasMergeSource)
                    return false;

                return base.IsPageComplete;
            }
        }

        /// <see cref="WizardFramework.IWizardPage.Control" />
        public override System.Windows.Forms.UserControl Control
        {
            get { return _control; }
        }

        private MergeSourceRangeOfRevisionsPageControl _control = new MergeSourceRangeOfRevisionsPageControl();
        private bool _needsNextPage = false;
    }
}
