using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WizardFramework;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public partial class ConnectorSelectionPageControl : WizardSelectionPage
    {
        Dictionary<string, ConnectorNode> _connectorNodes;

        public ConnectorSelectionPageControl()
            : base("Issue Tracker Connector Selection Page")
        {
            InitializeComponent();
            base.Title = "Issue Tracker Connectors";
            base.Description = "Select a repository connector.";
        }

        public ICollection<ConnectorNode> ConnectorNodes
        {
            get
            {
                if (_connectorNodes == null)
                {
                    IAnkhIssueService service = Context.GetService<IAnkhIssueService>(typeof(IAnkhIssueService));
                    if (service == null) { return null; }
                    ICollection<IIssueRepositoryConnector> connectors = service.Connectors;
                    if (connectors == null) { return null; }
                    _connectorNodes = new Dictionary<string, ConnectorNode>();
                    List<ConnectorNode> connectorNodeList = new List<ConnectorNode>();
                    foreach (IIssueRepositoryConnector connector in connectors)
                    {
                        ConnectorNode cn = new ConnectorNode(Context, connector);
                        _connectorNodes.Add(connector.Name, cn);
                    }
                }
                return _connectorNodes.Values;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
                ICollection<ConnectorNode> connectors = ConnectorNodes;
                if (connectors != null && connectors.Count > 0)
                {
                    foreach (ConnectorNode connector in connectors)
                    {
                        connectorTreeView.Nodes.Add(CreateTreeNode(connector));
                    }
                }
        }

        private TreeNode CreateTreeNode(ConnectorNode connector)
        {
            TreeNode node = new TreeNode(connector.Label);
            node.Tag = connector;
            return node;
        }

        private void connectorTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectedNode = e.Node.Tag as IWizardNode;
        }

        private void removeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            IsPageComplete = removeCheckBox.Checked;
            if (removeCheckBox.Checked)
            {
                connectorTreeView.SelectedNode = null;
                SelectedNode = null;
            }
            connectorTreeView.Enabled = !removeCheckBox.Checked;
        }
    }
}
