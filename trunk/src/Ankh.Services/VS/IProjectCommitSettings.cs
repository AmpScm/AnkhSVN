// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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

namespace Ankh.VS
{
    public interface IProjectCommitSettings
    {        
        /// <summary>
        /// Gets the issue tracker URI for the specified issue
        /// </summary>
        /// <param name="issueId">The issue id.</param>
        /// <returns></returns>
        Uri GetIssueTrackerUri(string issueId);

        /// <summary>
        /// Gets the raw bug tracker URI.
        /// </summary>
        /// <value>The raw bug tracker URI.</value>
        string RawIssueTrackerUri { get; }

        /// <summary>
        /// Gets a value indicating whether to warn if no issue is provided
        /// </summary>
        /// <value><c>true</c> if [warn if no issue]; otherwise, <c>false</c>.</value>
        bool WarnIfNoIssue { get; }

        /// <summary>
        /// Gets the raw issue tracker message.
        /// </summary>
        /// <value>The raw issue tracker message.</value>
        string RawIssueTrackerMessage { get; }

        /// <summary>
        /// Gets a value indicating whether to append the issue tracker message. If false it should be
        /// inserted before the message
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [append issue tracker message]; otherwise, <c>false</c>.
        /// </value>
        bool AppendIssueTrackerMessage { get; }

        /// <summary>
        /// Gets the issue label. If null or empty a default label is used
        /// </summary>
        /// <value>The issue label.</value>
        string IssueLabel { get; }

        /// <summary>
        /// Gets a value indicating whether [show log message box].
        /// </summary>
        /// <value><c>true</c> if [show log message box]; otherwise, <c>false</c>.</value>
        bool ShowIssueBox { get; }

        /// <summary>
        /// Gets a value indicating whether issue id's are numeric only
        /// </summary>
        /// <value><c>true</c> if [nummeric issues]; otherwise, <c>false</c>.</value>
        /// <remarks>You can always use ',' to specify multiple issues</remarks>
        bool NummericIssueIds { get; }

        /// <summary>
        /// Gets the issue references from the specified logmessage
        /// </summary>
        /// <param name="text">The logmessage.</param>
        /// <returns></returns>
        IEnumerable<TextMarker> GetIssues(string text);

        /// <summary>
        /// Builds a log message from the specified message and issueId
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="issueId">The issue id.</param>
        /// <returns></returns>
        string BuildLogMessage(string message, string issueId);

        /// <summary>
        /// Gets the log summary.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        string GetLogSummary(string message);

        /// <summary>
        /// Gets the revision references from the specified logmessage
        /// </summary>
        /// <param name="text">The logmessage.</param>
        /// <returns></returns>
        IEnumerable<TextMarker> GetRevisions(string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="revisionText"></param>
        /// <returns></returns>
        Uri GetRevisionUri(string revisionText);
    }
}
