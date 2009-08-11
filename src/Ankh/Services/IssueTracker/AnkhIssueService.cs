using System;
using System.Collections.Generic;

using Microsoft.Win32;

using Ankh.ExtensionPoints.IssueTracker;
using Ankh.UI;
using Ankh.VS;
using Ankh.IssueTracker;

namespace Ankh.Services.IssueTracker
{
    [GlobalService(typeof(IAnkhIssueService))]
    class AnkhIssueService : AnkhService, IAnkhIssueService
    {
        private Dictionary<string, IssueRepositoryConnectorBase> _nameConnectorMap;
        private IssueRepositoryBase _repository;

        private readonly object _syncLock = new object();

        public AnkhIssueService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        #region IAnkhIssueService Members

        /// <summary>
        /// Gets the registered Issue Repository connector collection.
        /// </summary>
        public ICollection<IssueRepositoryConnectorBase> Connectors
        {
            get
            {
                if (_nameConnectorMap != null)
                {
                    IssueRepositoryConnectorBase[] result = new IssueRepositoryConnectorBase[_nameConnectorMap.Count];
                    _nameConnectorMap.Values.CopyTo(result, 0);
                    return result;
                }
                return new IssueRepositoryConnectorBase[]{};
            }
        }

        /// <summary>
        /// Tries to find the issue repository connector identified by yhe <paramref name="name"/>.
        /// </summary>
        /// <returns>true if the connector exists, false otherwise</returns>
        public bool TryGetConnector(string name, out IssueRepositoryConnectorBase connector)
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
        public IssueRepositorySettingsBase CurrentIssueRepositorySettings
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
                IssueRepositorySettingsBase currentRepositorySettings = settings.ToIssueRepositorySettings();
                if (currentRepositorySettings == null)
                {
                    isReallyDirty = _repository != null;
                }
                else
                {
                    isReallyDirty = currentRepositorySettings.IsDifferentFrom(_repository);
                }
            }
            if (isReallyDirty)
            {
                CurrentIssueRepository = null;
            }
        }

        /// <summary>
        /// Gets or sets the Current Issue Repository
        /// </summary>
        public IssueRepositoryBase CurrentIssueRepository
        {
            get
            {
                if (_repository == null)
                {
                    IssueRepositorySettingsBase settings = CurrentIssueRepositorySettings;
                    if (settings != null)
                    {
                        string connectorName = settings.ConnectorName;
                        if (!string.IsNullOrEmpty(connectorName))
                        {
                            IssueRepositoryConnectorBase connector;
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
                IssueRepositoryBase oldRepository = _repository;
                _repository = value;
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
        private bool HasChanged(IssueRepositorySettingsBase settings1, IssueRepositorySettingsBase settings2)
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

        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();
            _nameConnectorMap = new Dictionary<string, IssueRepositoryConnectorBase>();
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
                        {
                            RegistryKey connector = aKey.OpenSubKey(connectorKey);
                            string serviceName = (string)connector.GetValue("");
                            IssueRepositoryConnectorBase descriptor = new IssueRepositoryConnectorProxy(this, serviceName, connectorKey);
                            _nameConnectorMap.Add(serviceName, descriptor);
                        }
                    }
                }
            }
        }
    }
}
