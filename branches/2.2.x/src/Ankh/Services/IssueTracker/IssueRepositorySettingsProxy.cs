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
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.IssueTracker;

namespace Ankh.Services.IssueTracker
{
    public class IssueRepositorySettingsProxy : IssueRepositorySettings
    {
        readonly IAnkhServiceProvider _context;

        public IssueRepositorySettingsProxy(IAnkhServiceProvider context, string connectorName)
            : base(connectorName)
        {
            _context = context;
        }

        #region IssueRepositorySettingsBase Members

        public override Uri RepositoryUri
        {
            get
            {
                IIssueTrackerSettings settings = Settings;
                return settings == null ? null : settings.IssueRepositoryUri;
            }
        }

        public override string RepositoryId
        {
            get
            {
                IIssueTrackerSettings settings = Settings;
                return settings == null ? null : settings.IssueRepositoryId;
            }
        }

        public override IDictionary<string, object> CustomProperties
        {
            get
            {
                IIssueTrackerSettings settings = Settings;
                return settings == null ? null : settings.CustomProperties;
            }
        }

        #endregion

        private IIssueTrackerSettings Settings
        {
            get
            {
                if (_context == null) { return null; }
                return _context.GetService<IIssueTrackerSettings>();
            }
        }
    }
}
