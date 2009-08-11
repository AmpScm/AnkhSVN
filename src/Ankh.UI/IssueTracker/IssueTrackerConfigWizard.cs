using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public class IssueTrackerConfigWizard : IssueTrackerWizard
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

        protected override bool TryCreateIssueRepository(out IssueRepositoryBase repository)
        {
            repository = null;
            if (Container.CurrentPage == _selectionPage)
            {
                return _selectionPage.RemoveIssueRepository;
            }
            return false;
        }
    }
}
