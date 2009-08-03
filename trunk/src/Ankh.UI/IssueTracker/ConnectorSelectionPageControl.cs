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
                    _connectorNodes = new Dictionary<string, ConnectorNode>();
                    IAnkhIssueService service = Context.GetService<IAnkhIssueService>();
                    if (service != null)
                    {
                        ICollection<IIssueRepositoryConnector> connectors = service.Connectors;
                        if (connectors != null && connectors.Count > 0)
                        {
                            List<ConnectorNode> connectorNodeList = new List<ConnectorNode>();
                            foreach (IIssueRepositoryConnector connector in connectors)
                            {
                                ConnectorNode cn = new ConnectorNode(Context, connector);
                                _connectorNodes.Add(connector.Name, cn);
                            }
                        }
                    }
                }
                return _connectorNodes.Values;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IAnkhIssueService service = Context.GetService<IAnkhIssueService>();
            IIssueRepositorySettings currentSettings = service == null
                ? null
                : service.CurrentIssueRepositorySettings;
            string currentConnectorName = currentSettings == null ? string.Empty : currentSettings.ConnectorName;
            removeCheckBox.Enabled = currentSettings != null;

            ICollection<ConnectorNode> connectors = ConnectorNodes;
            TreeNode selectedNode = null;
            if (connectors != null && connectors.Count > 0)
            {
                foreach (ConnectorNode connector in connectors)
                {
                    TreeNode connectorNode = CreateTreeNode(connector);
                    connectorTreeView.Nodes.Add(connectorNode);
                    if (string.Equals(currentConnectorName, connector.Connector.Name))
                    {
                        selectedNode = connectorNode;
                    }
                }
            }
            else
            {
                TreeNode noConnectorNode = new TreeNode("No Issue Repository Connector is registered.");
                connectorTreeView.Nodes.Add(noConnectorNode);
            }
            connectorTreeView.SelectedNode = selectedNode;

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

        internal bool RemoveIssueRepository
        {
            get
            {
                return removeCheckBox.Checked;
            }
        }
    }
}
