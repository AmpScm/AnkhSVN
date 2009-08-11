using System;
using System.Collections.Generic;

namespace Ankh.ExtensionPoints.IssueTracker
{
    /// <summary>
    /// Base class for Issue Repository Settings
    /// </summary>
    public abstract class IssueRepositorySettings
    {
        private string _connectorName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectorName">Unique connector name(as registered with the registry)</param>
        public IssueRepositorySettings(string connectorName)
        {
            _connectorName = connectorName;
        }

        /// <summary>
        /// Gets the issue repository connector's registered name
        /// </summary>
        public virtual string ConnectorName
        {
            get { return _connectorName; }
        }

        /// <summary>
        /// Gets Issue Repository URI
        /// </summary>
        public abstract Uri RepositoryUri { get; }

        /// <summary>
        /// Gets the unique id of the repository.
        /// </summary>
        /// <remarks>This value can be used to identify a specific (sub) issue repository on RepositoryUri</remarks>
        public virtual string RepositoryId
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Custom properties specific to the connector.
        /// </summary>
        public virtual IDictionary<string, object> CustomProperties
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether the <paramref name="otherSettings"/> is different from this settings
        /// </summary>
        public virtual bool IsDifferentFrom(IssueRepositorySettings otherSettings)
        {
            return !(true
                && otherSettings != null
                && string.Equals(ConnectorName, otherSettings.ConnectorName)
                && Uri.Equals(RepositoryUri, otherSettings.RepositoryUri)
                && string.Equals(RepositoryId, otherSettings.RepositoryId)
                && HasSameProperties(CustomProperties, otherSettings.CustomProperties)
                )
                ;
        }

        static bool HasSameProperties(IDictionary<string, object> props1, IDictionary<string, object> props2)
        {
            if (props1 == null && props2 == null) { return true; }
            int props1Count = props1 == null ? -1 : props1.Count;
            int props2Count = props2 == null ? -1 : props2.Count;
            if (props1Count != props2Count) { return false; }
            foreach (string key in props1.Keys)
            {
                if (!props2.ContainsKey(key)) { return false; }
                object value1 = props1[key];
                object value2 = props2[key];
                if (!object.Equals(value1, value2)) { return false; }
            }
            return true;
        }
    }
}
