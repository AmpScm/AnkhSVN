using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using Ankh.UI.MergeWizard;

namespace Ankh.UI.IssueTracker
{
    public class IssueTrackerConfigWizard : AnkhWizard
    {
        ConnectorSelectionPage _selectionPage;

        public IssueTrackerConfigWizard(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public override void AddPages()
        {
            base.AddPages();
            _selectionPage = new ConnectorSelectionPage(Context);
            AddPage(_selectionPage);
        }

        public override bool PerformFinish()
        {
            return true;
        }
    }
}
