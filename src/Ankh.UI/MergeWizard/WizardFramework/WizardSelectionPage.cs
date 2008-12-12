using System;
using System.Collections.Generic;
using System.Text;

/* 
 * WizardSelectionPage.cs
 * 
 * Copyright (c) 2008 CollabNet, Inc. ("CollabNet"), http://www.collab.net,
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, 
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 * 
 **/
namespace WizardFramework
{
    public abstract class WizardSelectionPage : WizardPage
    {
        /// <summary>
        /// Constructor for a named selection page.
        /// </summary>
        /// <param name="name">The selection page name.</param>
        protected WizardSelectionPage(string name)
            : base(name)
        {
            IsPageComplete = false;
        }

        #region WizardSelectionPage Members
        /// <summary>
        /// Adds the node to the list of selected nodes if the
        /// node is not already in the list.
        /// </summary>
        /// <param name="node">The node.</param>
        private void AddSelectedNode(IWizardNode node)
        {
            if (node == null) return;

            if (selectedWizardNodes.Contains(node)) return;

            selectedWizardNodes.Add(node);
        }

        /// <summary>
        /// The <code>WizardSelectionPage</code> implementation of
        /// <code>CanFlipToNextPage</code> returns <code>true</code>
        /// if there is a selected node.
        /// </summary>
        public override bool CanFlipToNextPage
        {
            get
            {
                return selectedNode_prop != null;
            }
        }

        /// <summary>
        /// Disposes of all nested wizards.  Feel free to extend.
        /// </summary>
        protected override void Dispose(bool disposings)
        {
            base.Dispose(disposings);

            foreach (IWizardNode wizard in selectedWizardNodes)
            {
                wizard.Dispose();
            }
        }

        /// <summary>
        /// The <code>WizardSelectionPage</code> implementation of the
        /// <code>IWizard</code> <code>NextPage</code> returns the first page
        /// of the selected wizard if there is one.
        /// </summary>
        public override IWizardPage NextPage
        {
            get
            {
                if (selectedNode_prop == null) return null;

                IWizard wizard = selectedNode_prop.Wizard;

                if (wizard == null)
                {
                    selectedNode_prop = null;

                    return null;
                }

                return wizard.StartingPage;
            }
        }

        /// <summary>
        /// Gets/Sets the currently selected wizard node of this page.
        /// </summary>
        public IWizardNode SelectedNode
        {
            get { return selectedNode_prop; }
            set
            {
                IWizardNode newSelectedNode = value;
                AddSelectedNode(newSelectedNode);
                this.selectedNode_prop = newSelectedNode; 
                if (this.IsCurrentPage)
                {
                    Container.UpdateButtons();
                }
            }
        }

        private List<IWizardNode> selectedWizardNodes = new List<IWizardNode>();
        private IWizardNode selectedNode_prop = null;
        #endregion
    }
}