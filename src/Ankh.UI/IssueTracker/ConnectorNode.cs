// $Id$
//
// Copyright 2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using WizardFramework;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public class ConnectorNode : IWizardNode
    {
        IssueRepositoryConnector _connector;
        IWizard _wizard = null;
        IAnkhServiceProvider _context;

        public ConnectorNode(IAnkhServiceProvider contex, IssueRepositoryConnector connector)
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

        internal IssueRepositoryConnector Connector
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
