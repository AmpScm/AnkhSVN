using WizardFramework;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public class ConnectorNode : IWizardNode
    {
        IIssueRepositoryConnector _connector;
        IWizard _wizard = null;
        IAnkhServiceProvider _context;

        public ConnectorNode(IAnkhServiceProvider contex, IIssueRepositoryConnector connector)
        {
            _connector = connector;
            _context = contex;
        }

        internal virtual string Label
        {
            get
            {
                return _connector.Name;
            }
        }

        internal IIssueRepositoryConnector Connector
        {
            get
            {
                return _connector;
            }
        }

        #region IWizardNode Members

        public void Dispose()
        {
            _connector = null;
        }

        public IWizard Wizard
        {
            get { return SupplyWizard(); }
        }

        #endregion

        protected virtual IWizard SupplyWizard()
        {
            if (_wizard == null)
            {
                _wizard = new ConnectorWizard(_context, _connector);
                _wizard.AddPages();
            }
            return _wizard;
        }
    }
}
