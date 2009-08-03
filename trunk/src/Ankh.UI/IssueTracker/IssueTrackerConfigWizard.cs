using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;

namespace Ankh.UI.IssueTracker
{
    public class IssueTrackerConfigWizard : Wizard
    {
        ConnectorSelectionPageControl _selectionPage;

        public IssueTrackerConfigWizard(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public override void AddPages()
        {
            base.AddPages();
            _selectionPage = new ConnectorSelectionPageControl();
            AddPage(_selectionPage);
        }

        public override bool PerformFinish()
        {
            if (_selectionPage != null
                && _selectionPage.RemoveIssueRepository)
            {
                IAnkhIssueService iService = Context.GetService<IAnkhIssueService>();
                if (iService != null)
                {
                    iService.CurrentIssueRepository = null;
                }
            }
            return true;
        }
    }
}
