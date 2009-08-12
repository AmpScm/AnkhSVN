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

namespace Ankh.ExtensionPoints.IssueTracker
{
    /// <summary>
    /// Base class for Issue Repository Connector service
    /// </summary>
    public abstract class IssueRepositoryConnector
    {
        /// <summary>
        /// Gets the registered connector's unique name 
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Creates an Issue Repository based on the settings.
        /// </summary>
        public abstract IssueRepository Create(IssueRepositorySettings settings);

        /// <summary>
        /// Gets the repository configuration page (to edit/setup an issue repository)
        /// </summary>
        public abstract IssueRepositoryConfigurationPage ConfigurationPage { get; }

    }
}
