
using Ankh.ExtensionPoints.IssueTracker;

using WizardFramework;

namespace Ankh.UI.IssueTracker
{
    public class ConnectorWizard : Wizard
    {
        IIssueRepositoryConnector _connector;
        IIssueRepositoryConfigurationPage _page;

        public ConnectorWizard(IAnkhServiceProvider context, IIssueRepositoryConnector connector)
            : base(context)
        {
            _connector = connector;
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

        public override bool PerformFinish()
        {
            if (_page != null)
            {
                IIssueRepositorySettings settings = _page.Settings;
                if (settings != null)
                {
                    IAnkhIssueService service = Context.GetService<IAnkhIssueService>(typeof(IAnkhIssueService));
                    if (service != null)
                    {
                        service.CurrentIssueRepository = _connector.Create(settings);
                        return true;
                    }
                }
            }
            return false;
        }

        internal IIssueRepositorySettings SolutionSettings
        {
            get
            {
                if (Context != null)
                {
                    IAnkhIssueService iService = Context.GetService<IAnkhIssueService>();
                    if (iService != null)
                    {
                        IIssueRepositorySettings settings = iService.CurrentIssueRepositorySettings;

                        if (settings != null
                            && string.Equals(settings.ConnectorName, _connector.Name)
                            )
                        {
                            return settings;
                        }
                    }
                }
                return null;
            }
        }
    }
}
