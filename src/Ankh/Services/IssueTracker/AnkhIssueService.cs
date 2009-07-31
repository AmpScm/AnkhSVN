using System;
using System.Collections.Generic;

using Microsoft.Win32;

using Ankh.ExtensionPoints.IssueTracker;
using Ankh.UI;

namespace Ankh.Services.IssueTracker
{
    //[GlobalService(typeof(IAnkhIssueService))]
    class AnkhIssueService : AnkhService, IAnkhIssueService
    {
        private Dictionary<string, IIssueRepositoryConnector> _nameConnectorMap;
        private IIssueRepository _repository;
        private bool _settingsRead;

        private readonly object _syncLock = new object();

        public AnkhIssueService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        #region IAnkhIssueService Members

        public ICollection<IIssueRepositoryConnector> Connectors
        {
            get
            {
                if (_nameConnectorMap != null)
                {
                    IIssueRepositoryConnector[] result = new IIssueRepositoryConnector[_nameConnectorMap.Count];
                    _nameConnectorMap.Values.CopyTo(result, 0);
                    return result;
                }
                return new IIssueRepositoryConnector[]{};
            }
        }

        public bool TryGetConnector(string name, out IIssueRepositoryConnector connector)
        {
            connector = null;
            if (_nameConnectorMap != null
                && _nameConnectorMap.Count > 0)
            {
                return _nameConnectorMap.TryGetValue(name, out connector);
            }
            return false;
        }

        public IIssueRepository CurrentIssueRepository
        {
            get
            {
                lock (_syncLock)
                {
                    if (_repository == null
                        && !_settingsRead)
                    {
                        IIssueRepositorySettings settings = ReadRepositorySettings();
                        if (settings != null)
                        {
                            string connectorName = settings.ConnectorName;
                            if (!string.IsNullOrEmpty(connectorName))
                            {
                                IIssueRepositoryConnector connector;
                                if (TryGetConnector(connectorName, out connector))
                                {
                                    _repository = connector.Create(settings);
                                }
                            }
                        }
                    }
                }
                return _repository;
            }
            set
            {
                _repository = value;

                if (IssueRepositoryChanged != null)
                {
                    IssueRepositoryChanged(this, EventArgs.Empty);
                }

                SetSolutionSettings(_repository);
            }
        }

        public event EventHandler IssueRepositoryChanged;

        #endregion

        protected override void OnPreInitialize()
        {
            base.OnPreInitialize();
            _nameConnectorMap = new Dictionary<string, IIssueRepositoryConnector>();
            _settingsRead = false;
            ReadRegistry();
        }

        private void ReadRegistry()
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
                            IIssueRepositoryConnector descriptor = new IssueRepositoryConnectorProxy(this, serviceName, connectorKey);
                            _nameConnectorMap.Add(serviceName, descriptor);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads solution settings and creates an instance of IIssueRepositorySettings instance.
        /// </summary>
        private IIssueRepositorySettings ReadRepositorySettings()
        {
            // TODO read solution settings
            _settingsRead = true;
            return null;
        }

        /// <summary>
        /// Writes solution settings.
        /// </summary>
        private void SetSolutionSettings(IIssueRepositorySettings settings)
        {
            // TODO set solution settings
        }
    }
}
