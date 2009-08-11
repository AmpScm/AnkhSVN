using WizardFramework;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public partial class IssueTrackerConfigWizardDialog : WizardDialog
    {
        IAnkhServiceProvider _context;
        IIssueRepository _newRepository;

        public IssueTrackerConfigWizardDialog(IAnkhServiceProvider context)
        {
            InitializeComponent();
            _context = context;
            _newRepository = null;
            Wizard = new IssueTrackerConfigWizard(_context);
        }

        public IIssueRepository NewIssueRepository
        {
            get { return _newRepository; }
            internal set { _newRepository = value; }
        }

        public IIssueRepositorySettings CurrentIssueRepositorySettings
        {
            get
            {
                IAnkhIssueService iService = _context == null ? null : _context.GetService<IAnkhIssueService>();
                return iService == null ? null : iService.CurrentIssueRepositorySettings;
            }
        }
    }
}
