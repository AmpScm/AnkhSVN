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
using Ankh.Scc;

namespace Ankh
{
    [CLSCompliant(false)]
    public interface IAnkhIssueService
    {
        /// <summary>
        /// Gets all the registered Issue repository connectors.
        /// </summary>
        /// <remarks>This call DOES NOT trigger connector package initialization.</remarks>
        ICollection<IssueRepositoryConnector> Connectors { get; }

        /// <summary>
        /// Tries to find a registered connector with the given name.
        /// </summary>
        /// <remarks>This call DOES NOT trigger connector package initialization.</remarks>
        bool TryGetConnector(string name, out IssueRepositoryConnector connector);

        /// <summary>
        /// Gets the issue repository settings associated with the current solution.
        /// </summary>
        IssueRepositorySettings CurrentIssueRepositorySettings { get; }

        /// <summary>
        /// Gets or Sets the issue repository associated with the current solution.
        /// </summary>
        IssueRepository CurrentIssueRepository { get; set; }

        /// <summary>
        /// Occurs when current solution's Issue Tracker Repository association settings are changed
        /// </summary>
        event EventHandler IssueRepositoryChanged;

        /// <summary>
        /// Marks Issue Service as dirty which signals Issues page to refresh itself.
        /// </summary>
        void MarkDirty();
    }
}
