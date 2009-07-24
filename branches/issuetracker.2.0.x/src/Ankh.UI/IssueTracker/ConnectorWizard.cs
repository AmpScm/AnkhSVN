using Ankh.UI.MergeWizard;
using Ankh.Interop.IssueTracker;
using System.Windows.Forms;

namespace Ankh.UI.IssueTracker
{
    public class ConnectorWizard : AnkhWizard
    {
        IIssueRepositoryConnector _connector;
        IIssueRepositoryConfigurationPage _page;
        Control _pageControl;

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
                _pageControl = Control.FromHandle(_page.Handle);
                if (_pageControl != null)
                {
                    AddPage(new ConnectorWizardPage(_connector.Name, _pageControl));
                }
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
    }
}
