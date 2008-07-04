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
    class MergeSourceRangeOfRevisionsPage : MergeSourceBasePage
    {
        public const string PAGE_NAME = "Merge Source Range of Revisions";

        /// <summary>
        /// Constructor
        /// </summary>
        public MergeSourceRangeOfRevisionsPage(MergeWizard wizard) 
            : base(wizard, new MergeSourceRangeOfRevisionsPageControl(), PAGE_NAME)
        {
            Title = Resources.MergeSourceHeaderTitle;
            Description = Resources.MergeSourceRangeOfRevisionsPageHeaderMessage;
            Control.Load += new EventHandler(Control_Load);
        }

        void Control_Load(object sender, EventArgs e)
        {
            NextPageRequired = false;
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

        protected override void OnPageChanging(WizardPageChangingEventArgs e)
        {
            base.OnPageChanging(e);

            ((MergeWizard)Wizard).LogMode = Ankh.UI.SvnLog.LogMode.MergesEligible;
        }

        private bool _needsNextPage = false;
    }
}
