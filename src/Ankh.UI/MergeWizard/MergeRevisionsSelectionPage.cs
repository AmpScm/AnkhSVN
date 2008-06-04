using System;
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
    class MergeRevisionsSelectionPage : WizardPage
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeRevisionsSelectionPage() : base("Merge Revisions Selection Pages")
        {
            IsPageComplete = false;

            Title = Resources.MergeRevisionsSelectionPageTitle;
            this.Message = new WizardMessage(Resources.MergeRevisionsSelectionPageMessage);

            _control.WizardPage = this;
        }

        /// <see cref="WizardFramework.IWizardPage.Control" />s
        public override UserControl Control
        {
            get { return _control; }
        }

        MergeRevisionsSelectionPageControl _control = new MergeRevisionsSelectionPageControl();
    }
}
