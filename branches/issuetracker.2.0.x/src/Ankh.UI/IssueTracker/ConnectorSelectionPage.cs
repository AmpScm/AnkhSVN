using System.Collections.Generic;
using WizardFramework;
using Ankh.Interop.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public class ConnectorSelectionPage : WizardSelectionPage
    {
        IAnkhServiceProvider _context;
        ConnectorSelectionPageControl _control;
        Dictionary<string, ConnectorNode> _connectorNodes;

        public ConnectorSelectionPage(IAnkhServiceProvider context)
            : base("Issue Tracker Connector Selection Page")
        {
            _context = context;
        }

        public override System.Windows.Forms.UserControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new ConnectorSelectionPageControl(this);
                }
                return _control;
            }
        }

        public ICollection<ConnectorNode> ConnectorNodes
        {
            get
            {
                if (_connectorNodes == null)
                {
                    IAnkhIssueService service = _context.GetService<IAnkhIssueService>(typeof(IAnkhIssueService));
                    if (service == null) { return null; }
                    ICollection<IIssueRepositoryConnector> connectors = service.Connectors;
                    if (connectors == null) { return null; }
                    _connectorNodes = new Dictionary<string,ConnectorNode>();
                    List<ConnectorNode> connectorNodeList = new List<ConnectorNode>();
                    foreach (IIssueRepositoryConnector connector in connectors)
                    {
                        ConnectorNode cn = new ConnectorNode(_context, connector);
                        _connectorNodes.Add(connector.Name, cn);
                    }
                }
                return _connectorNodes.Values;
            }
        }
    }
}
