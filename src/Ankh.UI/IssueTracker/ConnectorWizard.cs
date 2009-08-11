
using Ankh.ExtensionPoints.IssueTracker;

using WizardFramework;

namespace Ankh.UI.IssueTracker
{
    public class ConnectorWizard : IssueTrackerWizard
    {
        IssueRepositoryConnectorBase _connector;
        IssueRepositoryConfigurationPageBase _page;
        IssueRepositoryBase _newRepository;

        public ConnectorWizard(IAnkhServiceProvider context, IssueRepositoryConnectorBase connector)
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

        protected override bool TryCreateIssueRepository(out IssueRepositoryBase repository)
        {
            repository = null;
            if (_page != null)
            {
                IssueRepositorySettingsBase settings = _page.Settings;
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

        internal IssueRepositorySettingsBase SolutionSettings
        {
            get
            {
                IAnkhIssueService iService = Context == null ? null : Context.GetService<IAnkhIssueService>();
                IssueRepositorySettingsBase settings = iService == null ? null : iService.CurrentIssueRepositorySettings;

                if (settings != null
                    && string.Equals(settings.ConnectorName, _connector.Name)
                    )
                {
                    return settings;
                }
                return null;
            }
        }

        internal IssueRepositoryBase NewIssueRepository
        {
            get
            {
                return _newRepository;
            }
        }
    }
}
