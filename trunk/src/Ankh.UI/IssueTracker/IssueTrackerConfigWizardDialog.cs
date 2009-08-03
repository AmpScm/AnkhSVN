using WizardFramework;

namespace Ankh.UI.IssueTracker
{
    public partial class IssueTrackerConfigWizardDialog : WizardDialog
    {
        IAnkhServiceProvider _context;

        public IssueTrackerConfigWizardDialog(IAnkhServiceProvider context)
        {
            InitializeComponent();

            _context = context;
            Wizard = new IssueTrackerConfigWizard(_context);
        }
    }
}
