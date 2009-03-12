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
using Ankh.VS;
using SharpSvn;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Ankh.Settings
{
    [GlobalService(typeof(IProjectCommitSettings))]
    partial class SolutionSettings : IProjectCommitSettings
    {
        public Uri GetIssueTrackerUri(string issueId)
        {
            if (issueId == null)
                throw new ArgumentNullException("issueId");

            if (string.IsNullOrEmpty(issueId))
                return null;

            RefreshIfDirty();
            SettingsCache cache = _cache;

            if (cache == null || cache.BugTrackUrl == null)
                return null;

            string url = cache.BugTrackUrl.Replace("%BUGID%", Uri.EscapeDataString(issueId));

            if (url.StartsWith("^/"))
            {
                Uri repositoryRoot = RepositoryRoot;

                if (repositoryRoot == null)
                    return null;

                url = repositoryRoot.AbsoluteUri + url.Substring(2);
            }

            Uri result;
            if (Uri.TryCreate(url, UriKind.Absolute, out result))
                return result;

            return null;
        }

        public string RawIssueTrackerUri
        {
            get
            {
                RefreshIfDirty();
                return _cache.BugTrackUrl;
            }
        }

        public bool WarnIfNoIssue
        {
            get
            {
                RefreshIfDirty();

                return _cache.BugTrackWarnIfNoIssue.HasValue && _cache.BugTrackWarnIfNoIssue.Value;
            }
        }

        public string RawIssueTrackerMessage
        {
            get
            {
                RefreshIfDirty();

                return _cache.BugTrackMessage;
            }
        }

        public bool AppendIssueTrackerMessage
        {
            get
            {
                RefreshIfDirty();
                return !_cache.BugTrackAppend.HasValue || _cache.BugTrackAppend.Value;
            }
        }

        public string IssueLabel
        {
            get
            {
                RefreshIfDirty();

                return _cache.BugTrackLabel;
            }
        }

        public bool ShowIssueBox
        {
            get
            {
                RefreshIfDirty();

                if (!string.IsNullOrEmpty(_cache.BugTrackLogRegexes))
                    return false; // Has higher priority
                else if (!string.IsNullOrEmpty(_cache.BugTrackMessage))
                    return true;

                return false;
            }
        }

        public bool NummericIssueIds
        {
            get
            {
                RefreshIfDirty();

                return !_cache.BugTrackNumber.HasValue || _cache.BugTrackNumber.Value;
            }
        }

        public ReadOnlyCollection<string> RawLogIssueRegexes
        {
            get 
            {
                RefreshIfDirty();

                if (_cache.LogRegexes != null)
                    return _cache.LogRegexes;

                List<string> rl = new List<string>();

                if(!string.IsNullOrEmpty(_cache.BugTrackLogRegexes))
                    foreach (String s in _cache.BugTrackLogRegexes.Split('\n'))
                    {
                        rl.Add(s.TrimEnd('\r'));
                    }

                return _cache.LogRegexes = rl.AsReadOnly();
            }
        }

        public IEnumerable<IssueMarker> GetIssues(string logmessage)
        {
            ReadOnlyCollection<string> items = RawLogIssueRegexes;

            SettingsCache sc = _cache;

            if (_cache.BrokenRegex)
                return new IssueMarker[0];

            if (sc.AllInOneRe == null && sc.LogPrepareRe == null && items != null)
                try
                {
                    switch (items.Count)
                    {
                        case 1:
                            sc.AllInOneRe = new Regex(items[0], RegexOptions.CultureInvariant | RegexOptions.Multiline);
                            break;
                        case 2:
                            sc.LogPrepareRe = new Regex(items[0], RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.Multiline);
                            sc.LogSplitRe = new Regex(items[1], RegexOptions.CultureInvariant | RegexOptions.Multiline);
                            break;
                        default:
                            sc.BrokenRegex = true;
                            break;
                    }
                }
                catch (ArgumentException)
                {
                    sc.BrokenRegex = true;
                }

            if (sc.AllInOneRe != null)
                return PerformAllInOne(sc, logmessage);
            else if (sc.LogPrepareRe != null && sc.LogSplitRe != null)
                return PerformSplit(sc, logmessage);

            sc.BrokenRegex = true;
            return new IssueMarker[0];
        }

        private IEnumerable<IssueMarker> PerformAllInOne(SettingsCache sc, string logmessage)
        {
            foreach (Match m in sc.AllInOneRe.Matches(logmessage))
            {
                if (!m.Success)
                    continue;

                bool first = true;
                foreach (Group g in m.Groups)
                {
                    if (first)
                        first = false;
                    else
                        foreach (Capture c in g.Captures)
                            yield return new IssueMarker(c.Index, c.Length, c.Value);
                }
            }
        }

        private IEnumerable<IssueMarker> PerformSplit(SettingsCache cache, string logmessage)
        {
            foreach (Match m in cache.LogPrepareRe.Matches(logmessage))
            {
                if (!m.Success)
                    continue;

                foreach (Capture c in m.Captures)
                {
                    string text = logmessage.Substring(c.Index, c.Length);

                    foreach (Match sm in cache.LogSplitRe.Matches(c.Value))
                    {
                        if (!sm.Success)
                            continue;

                        foreach (Capture sc in sm.Captures)
                        {
                            yield return new IssueMarker(c.Index + sc.Index, sc.Length, sc.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the log summary.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public string GetLogSummary(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "";

            RefreshIfDirty();

            SettingsCache sc = _cache;

            if (sc.LogSummary == null)
                return message;
            else if (_cache.LogSummaryRe == null)
            {
                try
                {
                    _cache.LogSummaryRe = new Regex(_cache.LogSummary, RegexOptions.Multiline | RegexOptions.CultureInvariant);
                }
                catch (ArgumentException)
                {
                    sc.LogSummary = null;
                    return message;
                }
            }

            Match m = _cache.LogSummaryRe.Match(message);

            if (m.Success && m.Groups.Count > 0 && m.Groups[1].Length > 0)
                return m.Groups[1].Value;

            return message;
        }

        /// <summary>
        /// Builds a log message from the specified message and issueId
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="issueId">The issue id.</param>
        /// <returns></returns>
        public string BuildLogMessage(string message, string issueId)
        {
            if (!ShowIssueBox || string.IsNullOrEmpty(issueId))
                return message;

            issueId = issueId.Trim();

            if (string.IsNullOrEmpty(issueId))
                return message;

            StringBuilder sb = new StringBuilder();

            if(!AppendIssueTrackerMessage)
            {
                sb.Append(RawIssueTrackerMessage.Replace("%BUGID%", issueId));
                if (!RawIssueTrackerMessage.EndsWith("\n"))
                    sb.AppendLine();
            }

            sb.Append(message);

            if (!message.EndsWith("\n"))
                sb.AppendLine();

            if (AppendIssueTrackerMessage)
                sb.AppendLine(RawIssueTrackerMessage.Replace("%BUGID%", issueId).TrimEnd());

            return sb.ToString();
        }
    }
}
