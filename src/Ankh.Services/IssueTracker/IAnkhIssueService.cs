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
using Ankh.VS;

namespace Ankh
{
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

        /// <summary>
        /// Gets the issue references from the specified text
        /// </summary>
        /// <param name="logmessage">text.</param>
        /// <returns></returns>
        /// <remarks>Precondition: Current solution is associated with a repository.</remarks>
        bool TryGetIssues(string text, out IEnumerable<TextMarker> issues);

        /// <summary>
        /// Passes the open request to the current issue repository
        /// </summary>
        /// <param name="issueId"></param>
        void OpenIssue(string issueId);

        /// <summary>
        /// 
        /// </summary>
        void ShowConnectHelp();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="combined"></param>
        /// <param name="markers"></param>
        /// <returns></returns>
        bool TryGetRevisions(string text, out IEnumerable<TextMarker> markers);

        void OpenRevision(string revisionText);
    }
}
