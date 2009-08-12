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

using System;
using System.Collections.Generic;
using System.Text;

using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.Services.IssueTracker
{
    /// <summary>
    /// Acts as a proxy to the the actual Issue Tracker Repository Connector.
    /// </summary>
    /// <remarks>
    /// This proxy serves "descriptive" properties w/o initializing the actual connector.
    /// The actual connector package initialization is delayed until a non-descriptive property is needed.
    /// Currently, "connector name" is the only descriptive property.
    /// </remarks>
    class IssueRepositoryConnectorProxy : IssueRepositoryConnector
    {
        private IssueRepositoryConnector _delegate = null;
        private IAnkhServiceProvider _context;
        private string _name = null;
        private string _delegateId = null;

        public IssueRepositoryConnectorProxy(IAnkhServiceProvider context, string name, string delegateServiceId)
        {
            _context = context;
            _name = name;
            _delegateId = delegateServiceId;
        }

        private IssueRepositoryConnector Delegate
        {
            get
            {
                if (_delegate == null
                    && !string.IsNullOrEmpty(_delegateId))
                {
                    Type serviceType = Type.GetTypeFromCLSID(new Guid(_delegateId));
                    if (serviceType != null)
                    {
                        _delegate = _context.GetService<IssueRepositoryConnector>(serviceType);
                    }
                }
                return _delegate;
            }
        }

        #region IIssueRepositoryConnector Members

        public override IssueRepository Create(IssueRepositorySettings settings)
        {
            IssueRepositoryConnector dlg = Delegate;
            if (dlg != null)
            {
                return dlg.Create(settings);
            }
            return null;
        }

        public override IssueRepositoryConfigurationPage ConfigurationPage
        {
            get
            {
                IssueRepositoryConnector dlg = Delegate;
                if (dlg != null)
                {
                    return dlg.ConfigurationPage;
                }
                return null;
            }
        }

        public override string Name
        {
            get { return _name; }
        }

        #endregion

    }
}
