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
    class MergeSourceRangeOfRevisionsPage : MergeSourceBasePage<MergeSourceRangeOfRevisionsPageControl>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MergeSourceRangeOfRevisionsPage(MergeWizard wizard) 
            : base(wizard, "Merge Source Range Of Revisions")
        {
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceRangeOfRevisionsPageHeaderMessage;
            Control.Load += new EventHandler(Control_Load);
        }

        void Control_Load(object sender, EventArgs e)
        {
            NextPageRequired = false;
        }

        protected override void OnPageChanged(WizardPageChangeEventArgs e)
        {
            Wizard.MergeSource = PageControl.MergeSource;
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

        /// <see cref="Ankh.UI.MergeWizard.MergeSourceBasePage" />
        public override MergeWizard.MergeType MergeType
        {
            get { return MergeWizard.MergeType.RangeOfRevisions; }
        }

        private bool _needsNextPage = false;
    }
}
