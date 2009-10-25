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

using Ankh.ExtensionPoints.IssueTracker;
using Ankh.IssueTracker;
using Ankh.UI;
using Ankh.VS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ankh.Services.IssueTracker
{
    [GlobalService(typeof(IAnkhIssueService))]
    class AnkhIssueService : AnkhService, IAnkhIssueService
    {
        private const string REGEX_GROUPNAME_ID = "id";

        private Dictionary<string, IssueRepositoryConnector> _nameConnectorMap;
        private IssueRepository _repository;
        private Regex _issueIdRegex = null;

        private readonly object _syncLock = new object();

        public AnkhIssueService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        #region IAnkhIssueService Members

        /// <summary>
        /// Gets the registered Issue Repository connector collection.
        /// </summary>
        public ICollection<IssueRepositoryConnector> Connectors
        {
            get
            {
                if (_nameConnectorMap != null)
                {
                    IssueRepositoryConnector[] result = new IssueRepositoryConnector[_nameConnectorMap.Count];
                    _nameConnectorMap.Values.CopyTo(result, 0);
                    return result;
                }
                return new IssueRepositoryConnector[] { };
            }
        }

        /// <summary>
        /// Tries to find the issue repository connector identified by yhe <paramref name="name"/>.
        /// </summary>
        /// <returns>true if the connector exists, false otherwise</returns>
        public bool TryGetConnector(string name, out IssueRepositoryConnector connector)
        {
            connector = null;
            if (_nameConnectorMap != null
                && _nameConnectorMap.Count > 0)
            {
                return _nameConnectorMap.TryGetValue(name, out connector);
            }
            return false;
        }

        /// <summary>
        /// Gets the current Issue Repository Settings
        /// </summary>
        /// <remarks>
        /// This property holds the latest repository settings including the changes made in the session.
        /// </remarks>
        public IssueRepositorySettings CurrentIssueRepositorySettings
        {
            get
            {
                if (_repository != null)
                {
                    return _repository;
                }
                IIssueTrackerSettings settings = Settings;
                return settings == null ? null : settings.ToIssueRepositorySettings();
            }
        }

        /// <summary>
        /// Clears current repository object
        /// </summary>
        /// <remarks>if the new settings are "really" different from the current settings, reset</remarks>
        public void MarkDirty()
        {
            bool isReallyDirty = false;
            IIssueTrackerSettings settings = Settings;
            if (settings == null)
            {
                isReallyDirty = _repository != null;
            }
            else
            {
                IssueRepositorySettings currentRepositorySettings = settings.ToIssueRepositorySettings();
                if (currentRepositorySettings == null)
                {
                    isReallyDirty = _repository != null;
                }
                else
                {
                    isReallyDirty = !currentRepositorySettings.ValueEquals(_repository);
                }
            }
            if (isReallyDirty)
            {
                CurrentIssueRepository = null;
            }
        }

        /// <summary>
        /// Gets the issue references from the specified text
        /// </summary>
        /// <param name="logmessage">text.</param>
        /// <returns></returns>
        public bool TryGetIssues(string text, out IEnumerable<IssueMarker> issues)
        {
            // potentially triggers 
            // Issue Tracker Connector Package initialization
            IssueRepository repository = CurrentIssueRepository;
            if (repository != null)
            {
                if (_issueIdRegex == null)
                {
                    string pattern = repository.IssueIdPattern;
                    if (!string.IsNullOrEmpty(pattern))
                    {
                        _issueIdRegex = new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.Multiline);
                    }
                }
                if (_issueIdRegex != null)
                {
                    issues = PerformRegex(text);
                    return true;
                }
            }
            else if (GetService<IProjectCommitSettings>() != null)
            {
                issues = GetIssuesFromCommitSettings(text);
                return true;
            }
            // meaning 
            // no solution
            // or no issue repository is associated with the solution
            // or issue repository does not provide an issue id pattern
            issues = new IssueMarker[0];
            return false;
        }

        /// <summary>
        /// Passes the open request to the current issue repository if available,
        /// otherwise tries to open web browser base on project settings
        /// </summary>
        /// <param name="issueId">Issue Id</param>
        public void OpenIssue(string issueId)
        {
            if (string.IsNullOrEmpty(issueId))
                throw new ArgumentNullException("issueId");

            IssueRepository repository = CurrentIssueRepository;
            if (repository != null)
            {
                try
                {
                    repository.NavigateTo(issueId);
                }
                catch { } // connector code
            }
            else
            {
                IProjectCommitSettings projectSettings = GetService<IProjectCommitSettings>();
                if (projectSettings != null)
                {
                    IAnkhWebBrowser web = GetService<IAnkhWebBrowser>();
                    if (web != null)
                    {
                        Uri uri = projectSettings.GetIssueTrackerUri(issueId);
                        if (uri != null && !uri.IsFile && !uri.IsUnc)
                            web.Navigate(uri);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Current Issue Repository
        /// </summary>
        public IssueRepository CurrentIssueRepository
        {
            get
            {
                if (_repository == null)
                {
                    IssueRepositorySettings settings = CurrentIssueRepositorySettings;
                    if (settings != null)
                    {
                        string connectorName = settings.ConnectorName;
                        if (!string.IsNullOrEmpty(connectorName))
                        {
                            IssueRepositoryConnector connector;
                            if (TryGetConnector(connectorName, out connector))
                            {
                                try
                                {
                                    if (settings.RepositoryUri != null)
                                    {
                                        _repository = connector.Create(settings);
                                    }
                                }
                                catch { } // TODO connector error
                            }
                        }
                    }
                }
                return _repository;
            }
            set
            {
                IssueRepository oldRepository = _repository;
                _repository = value;
                _issueIdRegex = null; // reset RegEx
                if (IssueRepositoryChanged != null)
                {
                    IssueRepositoryChanged(this, EventArgs.Empty);
                }

                if (oldRepository != null && oldRepository != _repository)
                {
                    IDisposable disposableRepository = oldRepository as IDisposable;
                    if (disposableRepository != null)
                    {
                        try
                        {
                            disposableRepository.Dispose();
                        }
                        catch { } // Connector code
                    }
                }
            }
        }

        /// <summary>
        /// Compares two issue repository settings instance
        /// </summary>
        /// <returns>true/false</returns>
        private bool HasChanged(IssueRepositorySettings settings1, IssueRepositorySettings settings2)
        {
            return true;
            /*
            && settings1 != settings2 // handles both null values
            && (false
                || (settings1 is IEquatable<IIssueRepositorySettings> && !((IEquatable<IIssueRepositorySettings>)settings1).Equals(settings2))
                || (settings2 is IEquatable<IIssueRepositorySettings> && !((IEquatable<IIssueRepositorySettings>)settings2).Equals(settings1))
                );*/
        }

        /// <summary>
        /// Raised when <code>CurrentIssueRepository is changed</code>
        /// </summary>
        public event EventHandler IssueRepositoryChanged;

        #endregion

        #region IAnkhServiceEvents handlers

        /// <summary>
        /// Resets the solution specific attributes
        /// </summary>
        void OnSolutionClosed(object sender, EventArgs e)
        {
            CurrentIssueRepository = null; // handles IsSolutionDirty
        }

        /// <summary>
        /// Notifies the handlers (for example PendingChanges window)
        /// </summary>
        void OnSolutionOpened(object sender, EventArgs e)
        {
            if (IssueRepositoryChanged != null)
            {
                IssueRepositoryChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        private IEnumerable<IssueMarker> PerformRegex(string text)
        {
            foreach (Match m in _issueIdRegex.Matches(text))
            {
                if (!m.Success)
                    continue;

                // try to find an "ID" group
                Group grp = m.Groups[REGEX_GROUPNAME_ID];
                if (grp != null && grp.Success)
                {
                    foreach (Capture c in grp.Captures)
                        yield return new IssueMarker(c.Index, c.Length, c.Value);
                }
                else
                {
                    foreach (Group g in m.Groups)
                    {
                        foreach (Capture c in g.Captures)
                            yield return new IssueMarker(c.Index, c.Length, c.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets issue ids from project commit settings
        /// </summary>
        private IEnumerable<IssueMarker> GetIssuesFromCommitSettings(string text)
        {
            foreach (IssueMarker issue in GetService<IProjectCommitSettings>().GetIssues(text))
            {
                yield return issue;
            }
        }

        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();
            _nameConnectorMap = new Dictionary<string, IssueRepositoryConnector>();
            _repository = null;
            ReadConnectorRegistry();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            AnkhServiceEvents events = GetService<AnkhServiceEvents>();
            if (events != null)
            {
                // register solution event handlers
                events.SolutionOpened += new EventHandler(OnSolutionOpened);
                events.SolutionClosed += new EventHandler(OnSolutionClosed);
            }
        }

        private IIssueTrackerSettings Settings
        {
            get
            {
                IIssueTrackerSettings settings = Context == null ? null : Context.GetService<IIssueTrackerSettings>();
                return settings;
            }
        }

        /// <summary>
        /// Reads the issue repository connector information from the registry
        /// </summary>
        private void ReadConnectorRegistry()
        {
            IAnkhPackage ankhPackage = GetService<IAnkhPackage>(typeof(IAnkhPackage));
            if (ankhPackage != null)
            {
                using (RegistryKey key = ankhPackage.ApplicationRegistryRoot)
                {
                    using (RegistryKey aKey = key.OpenSubKey("IssueRepositoryConnectors"))
                    {
                        if (aKey == null)
                            return;

                        string[] connectorKeys = aKey.GetSubKeyNames();
                        foreach (string connectorKey in connectorKeys)
                            using (RegistryKey connector = aKey.OpenSubKey(connectorKey))
                            {
                                string serviceName = (string)connector.GetValue("");
                                IssueRepositoryConnector descriptor = new IssueRepositoryConnectorProxy(this, serviceName, connectorKey);
                                _nameConnectorMap.Add(serviceName, descriptor);
                            }
                    }
                }
            }
        }
    }
}
