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
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using CultureInfo=System.Globalization.CultureInfo;

namespace Ankh.Services.IssueTracker
{
    [GlobalService(typeof(IAnkhIssueService))]
    sealed class AnkhIssueService : AnkhService, IAnkhIssueService
    {
        private const string REGEX_GROUPNAME_ID = "id";

        private Dictionary<string, IssueRepositoryConnector> _nameConnectorMap;
        private IssueRepository _repository;
        IProjectCommitSettings _commitSettings;

        public AnkhIssueService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IProjectCommitSettings CommitSettings
        {
            get { return _commitSettings ?? (_commitSettings = GetService<IProjectCommitSettings>()); }
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
            bool isReallyDirty;
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
        /// <param name="text"></param>
        /// <param name="markers"></param>
        /// <returns></returns>
        public bool TryGetIssues(string text, out IEnumerable<TextMarker> markers)
        {
            if (string.IsNullOrEmpty(text))
            {
                markers = null;
                return false;
            }

            // potentially triggers 
            // Issue Tracker Connector Package initialization
            IssueRepository repository = CurrentIssueRepository;
            if (repository != null)
                markers = PerformRepositoryRegex(repository.IssueIdRegex, text);
            else
                markers = GetIssuesFromCommitSettings(text);

            return true;
        }

        public bool TryGetRevisions(string text, out IEnumerable<TextMarker> markers)
        {
            if (string.IsNullOrEmpty(text))
            {
                markers = null;
                return false;
            }

            markers = GetRevisionsFromCommitSettings(text);
            return true;
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
                    return;
                }
                catch { } // connector code
            }

            IAnkhWebBrowser web = GetService<IAnkhWebBrowser>();
            if (web != null)
            {
                Uri uri = CommitSettings.GetIssueTrackerUri(issueId);

                if (uri != null && !uri.IsFile && !uri.IsUnc)
                    web.Navigate(uri);
            }
        }

        public void OpenRevision(string revisionText)
        {
            if (string.IsNullOrEmpty(revisionText))
                throw new ArgumentNullException("revisionText");

            long rev;

            IssueRepository repository = CurrentIssueRepository;
            if (repository != null
                && repository.CanNavigateToRevision
                && long.TryParse(revisionText, out rev))
            {
                try
                {
                    repository.NavigateToRevision(rev);
                    return;
                }
                catch { } // connector code
            }

            IAnkhWebBrowser web = GetService<IAnkhWebBrowser>();
            if (web != null)
            {
                Uri uri = CommitSettings.GetRevisionUri(revisionText);
                if (uri != null && !uri.IsFile && !uri.IsUnc)
                    web.Navigate(uri);
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
                OnIssueRepositoryChanged();
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
        /// Raised when <code>CurrentIssueRepository is changed</code>
        /// </summary>
        public event EventHandler IssueRepositoryChanged;

        /// <summary>
        /// Fires IssueRepositoryChanged event.
        /// </summary>
        private void OnIssueRepositoryChanged()
        {
            if (IssueRepositoryChanged != null)
            {
                IssueRepositoryChanged(this, EventArgs.Empty);
            }
        }

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
            OnIssueRepositoryChanged();
        }

        #endregion

        private IEnumerable<TextMarker> PerformRepositoryRegex(Regex re, string text)
        {
            if (re == null)
                yield break;
            
            foreach (Match m in re.Matches(text))
            {
                if (!m.Success)
                    continue;

                // try to find an "ID" group
                Group grp = m.Groups[REGEX_GROUPNAME_ID];
                if (grp != null && grp.Success)
                {
                    foreach (Capture c in grp.Captures)
                        yield return new TextMarker(c.Index, c.Length, c.Value);
                }
                else
                {
                    foreach (Group g in m.Groups)
                    {
                        foreach (Capture c in g.Captures)
                            yield return new TextMarker(c.Index, c.Length, c.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets issue ids from project commit settings
        /// </summary>
        private IEnumerable<TextMarker> GetIssuesFromCommitSettings(string text)
        {
            if (CommitSettings == null)
                yield break;

            foreach (TextMarker issue in CommitSettings.GetIssues(text))
            {
                yield return issue;
            }
        }

        /// <summary>
        /// Gets issue ids from project commit settings
        /// </summary>
        private IEnumerable<TextMarker> GetRevisionsFromCommitSettings(string text)
        {
            if (CommitSettings == null)
                yield break;

            foreach (TextMarker rev in CommitSettings.GetRevisions(text))
            {
                yield return rev;
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

        IIssueTrackerSettings _settings;
        private IIssueTrackerSettings Settings
        {
            get { return _settings ?? (_settings = GetService<IIssueTrackerSettings>()); }
        }

        /// <summary>
        /// Reads the issue repository connector information from the registry
        /// </summary>
        private void ReadConnectorRegistry()
        {
            IAnkhPackage ankhPackage = GetService<IAnkhPackage>();
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

        #region IAnkhIssueService Members


        public void ShowConnectHelp()
        {
            // Shamelessly copied from the AnkhHelService

            UriBuilder ub = new UriBuilder("http://svc.ankhsvn.net/svc/go/");
            ub.Query = string.Format("t=ctrlHelp&v={0}&l={1}&dt={2}", GetService<IAnkhPackage>().UIVersion, CultureInfo.CurrentUICulture.LCID, Uri.EscapeUriString("Ankh.UI.PendingChanges.PendingIssuesPage"));

            try
            {
                bool showHelpInBrowser = true;
                IVsHelpSystem help = GetService<IVsHelpSystem>(typeof(SVsHelpService));
                if (help != null)
                    showHelpInBrowser = !VSErr.Succeeded(help.DisplayTopicFromURL(ub.Uri.AbsoluteUri, (uint)VHS_COMMAND.VHS_Default));

                if (showHelpInBrowser)
                    Help.ShowHelp(null, ub.Uri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        #endregion
    }
}
