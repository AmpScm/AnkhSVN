using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WizardFramework;

namespace Ankh.UI.IssueTracker
{
    public partial class ConnectorSelectionPageControl : UserControl
    {
        ConnectorSelectionPage _page;

        protected ConnectorSelectionPageControl()
        {
            InitializeComponent();
        }

        public ConnectorSelectionPageControl(ConnectorSelectionPage page)
        {
            InitializeComponent();
            _page = page;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (_page != null)
            {
                ICollection<ConnectorNode> connectors = _page.ConnectorNodes;
                if (connectors != null && connectors.Count > 0)
                {
                    foreach (ConnectorNode connector in connectors)
                    {
                        connectorTreeView.Nodes.Add(CreateTreeNode(connector));
                    }
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
            _page.SelectedNode = e.Node.Tag as IWizardNode;
        }

    }
}
