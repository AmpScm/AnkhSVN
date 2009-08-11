
using Ankh.ExtensionPoints.IssueTracker;

using WizardFramework;

namespace Ankh.UI.IssueTracker
{
    public class ConnectorWizard : IssueTrackerWizard
    {
        IssueRepositoryConnector _connector;
        IssueRepositoryConfigurationPage _page;
        IssueRepository _newRepository;

        public ConnectorWizard(IAnkhServiceProvider context, IssueRepositoryConnector connector)
            : base(context)
        {
            _connector = connector;
            _newRepository = null;
        }

        public override void AddPages()
        {
            base.AddPages();
            _page = _connector.ConfigurationPage;
            if (_page != null)
            {
                ConnectorWizardPage wizardPage = new ConnectorWizardPage(_connector.Name, _page);
                wizardPage.Title = _connector.Name;
                wizardPage.Description = "Configure issue tracker repository.";
                AddPage(wizardPage);
            }
        }

        protected override bool TryCreateIssueRepository(out IssueRepository repository)
        {
            repository = null;
            if (_page != null)
            {
                IssueRepositorySettings settings = _page.Settings;
                if (settings != null)
                {
                    try
                    {
                        repository = _connector.Create(settings);
                        return true;
                    }
                    catch { } // connector code
                }
            }
            return false;
        }

        internal IssueRepositorySettings SolutionSettings
        {
            get
            {
                IAnkhIssueService iService = Context == null ? null : Context.GetService<IAnkhIssueService>();
                IssueRepositorySettings settings = iService == null ? null : iService.CurrentIssueRepositorySettings;

                if (settings != null
                    && string.Equals(settings.ConnectorName, _connector.Name)
                    )
                {
                    return settings;
                }
                return null;
            }
        }

        internal IssueRepository NewIssueRepository
        {
            get
            {
                return _newRepository;
            }
        }
    }
}
