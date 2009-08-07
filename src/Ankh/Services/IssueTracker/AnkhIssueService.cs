using System;
using System.Collections.Generic;

using Microsoft.Win32;

using Ankh.ExtensionPoints.IssueTracker;
using Ankh.UI;

namespace Ankh.Services.IssueTracker
{
    [GlobalService(typeof(IAnkhIssueService))]
    class AnkhIssueService : AnkhService, IAnkhIssueService
    {
        static readonly string SOLUTION_PROPERTY__CONNECTOR = "Connector"; // holds the connector name
        static readonly string SOLUTION_PROPERTY__URI = "Uri"; // holds the reposiory uri string
        static readonly string SOLUTION_PROPERTY__REPOSITORY_ID = "RepositoryId"; // holds the repository id
        static readonly string SOLUTION_PROPERTY__PROPERTY_NAMES = "PropertyNames"; // holds the connector's custom property names
        static readonly string SOLUTION_PROPERTY__PROPERTY_VALUES = "PropertyValues"; // holds the repository's custom property values

        private Dictionary<string, IIssueRepositoryConnector> _nameConnectorMap;
        private IIssueRepository _repository;
        private IIssueRepositorySettings _repositorySettings;
        private IIssueRepositorySettings _persistedSettings;
        private bool _isSolutionDirty;

        private readonly object _syncLock = new object();

        public AnkhIssueService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        #region IAnkhIssueService Members

        /// <summary>
        /// Gets the registered Issue Repository connector collection.
        /// </summary>
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

        /// <summary>
        /// Tries to find the issue repository connector identified by yhe <paramref name="name"/>.
        /// </summary>
        /// <returns>true if the connector exists, false otherwise</returns>
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

        /// <summary>
        /// Gets the current Issue Repository Settings
        /// </summary>
        /// <remarks>
        /// This property holds the latest repository settings including the changes made in the session.
        /// </remarks>
        public IIssueRepositorySettings CurrentIssueRepositorySettings
        {
            get
            {
                if (_repository != null)
                {
                    return _repository;
                }
                return _repositorySettings;
            }
        }

        /// <summary>
        /// Gets or sets the Current Issue Repository
        /// </summary>
        public IIssueRepository CurrentIssueRepository
        {
            get
            {
                if (_repository == null)
                {
                    IIssueRepositorySettings settings = CurrentIssueRepositorySettings;
                    if (settings != null)
                    {
                        string connectorName = settings.ConnectorName;
                        if (!string.IsNullOrEmpty(connectorName))
                        {
                            IIssueRepositoryConnector connector;
                            if (TryGetConnector(connectorName, out connector))
                            {
                                try
                                {
                                    _repository = connector.Create(settings);
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
                IIssueRepository oldRepository = _repository;
                _repositorySettings =_repository = value;
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

                IsSolutionDirty = HasChanged(CurrentIssueRepositorySettings, _persistedSettings);
            }
        }

        /// <summary>
        /// Compares two issue repository settings instance
        /// </summary>
        /// <returns>true/false</returns>
        private bool HasChanged(IIssueRepositorySettings settings1, IIssueRepositorySettings settings2)
        {
            return true
                && settings1 != settings2 // handles both null values
                && (false
                    || (settings1 is IComparable<IIssueRepositorySettings> && ((IComparable<IIssueRepositorySettings>)settings1).CompareTo(settings2) != 0)
                    || (settings2 is IComparable<IIssueRepositorySettings> && ((IComparable<IIssueRepositorySettings>)settings2).CompareTo(settings1) != 0)
                    );
        }

        /// <summary>
        /// Raised when <code>CurrentIssueRepository is changed</code>
        /// </summary>
        public event EventHandler IssueRepositoryChanged;

        #region VS Solution persistence

        /// <summary>
        /// Get or Sets the dirty property to indicate the need for persistence
        /// </summary>
        public bool IsSolutionDirty
        {
            get
            {
                return _isSolutionDirty;;
            }
            set
            {
                _isSolutionDirty = value;
            }
        }

        public bool HasSolutionData
        {
            get
            {
                return CurrentIssueRepositorySettings != null;
            }
        }

        public void ReadSolutionProperties(Ankh.Scc.IPropertyMap propertyBag)
        {
            string connector;
            if (propertyBag.TryGetValue(SOLUTION_PROPERTY__CONNECTOR, out connector))
            {
                string uriString;
                if (propertyBag.TryGetValue(SOLUTION_PROPERTY__URI, out uriString))
                {
                    Uri uri;
                    if (Uri.TryCreate(uriString, UriKind.Absolute, out uri))
                    {
                        string repoId;
                        if (!propertyBag.TryGetValue(SOLUTION_PROPERTY__REPOSITORY_ID, out repoId))
                        {
                            repoId = null;
                        }

                        Dictionary<string, object> customProperties = null;
                        string propNames;
                        if (propertyBag.TryGetValue(SOLUTION_PROPERTY__PROPERTY_NAMES, out propNames))
                        {
                            string[] propNameArray = propNames.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            if (propNameArray.Length > 0)
                            {
                                string propValues;
                                if (propertyBag.TryGetValue(SOLUTION_PROPERTY__PROPERTY_VALUES, out propValues))
                                {
                                    string[] propValueArray = propValues.Split(new string[] { "," }, StringSplitOptions.None);
                                    int propIndex = 0;
                                    if (propValueArray.Length == propNameArray.Length)
                                    {
                                        customProperties = new Dictionary<string, object>();
                                        foreach (string propName in propNameArray)
                                        {
                                            string propValue = propValueArray[propIndex++];
                                            if (!string.IsNullOrEmpty(propValue))
                                            {
                                                customProperties.Add(propName, propValue);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        IssueRepositorySettings iSettings = new IssueRepositorySettings();
                        iSettings.ConnectorName = connector;
                        iSettings.RepositoryUri = uri;
                        iSettings.RepositoryId = repoId;
                        iSettings.CustomProperties = customProperties;
                        _repositorySettings = _persistedSettings = iSettings;
                    }
                }
            }
        }

        public void WriteSolutionProperties(Ankh.Scc.IPropertyMap propertyBag)
        {
            if (_repository != null)
            {
                propertyBag.SetRawValue(SOLUTION_PROPERTY__CONNECTOR, _repository.ConnectorName);
                propertyBag.SetRawValue(SOLUTION_PROPERTY__URI, _repository.RepositoryUri.ToString());
                propertyBag.SetRawValue(SOLUTION_PROPERTY__REPOSITORY_ID, _repository.RepositoryId);
                Dictionary<string, object> customProperties = _repository.CustomProperties;
                if (customProperties != null) {
                    string[] propNameArray = new string[customProperties.Keys.Count];
                    customProperties.Keys.CopyTo(propNameArray, 0);
                    string propNames = string.Join(",", propNameArray);

                    List<string> propValueList = new List<string>();
                    foreach (string propName in propNameArray)
                    {
                        object propValue;
                        if (!customProperties.TryGetValue(propName, out propValue))
                        {
                            propValue = string.Empty;
                        }
                        propValueList.Add(propValue == null ? string.Empty : propValue.ToString());
                    }
                    string propValues = string.Join(",", propValueList.ToArray());
                    propertyBag.SetRawValue(SOLUTION_PROPERTY__PROPERTY_NAMES, propNames);
                    propertyBag.SetRawValue(SOLUTION_PROPERTY__PROPERTY_VALUES, propValues);
                }
            }
            _persistedSettings = CurrentIssueRepositorySettings;
            _isSolutionDirty = false;
        }

        #endregion

        #endregion

        #region IAnkhServiceEvents handlers

        /// <summary>
        /// Resets the solution specific attributes
        /// </summary>
        void OnSolutionClosed(object sender, EventArgs e)
        {
            _persistedSettings = null;
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
            _nameConnectorMap = new Dictionary<string, IIssueRepositoryConnector>();
            _repository = null;
            _repositorySettings = _persistedSettings = null;
            ReadRegistry();
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

        /// <summary>
        /// Reads the isse repository connector information from the registry
        /// </summary>
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
    }
}
