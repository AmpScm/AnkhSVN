using System;
using System.Collections.Generic;
using System.Text;

using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.Services.IssueTracker
{
    class IssueRepositorySettings : IIssueRepositorySettings, IEquatable<IIssueRepositorySettings>
    {
        private string _connectorName;
        private Uri _repositoryUri;
        private string _repositoryId;
        private Dictionary<string, object> _customProperties;

        #region IIssueRepositorySettings Members

        public string ConnectorName
        {
            get { return _connectorName; }
            internal set { _connectorName = value; }
        }

        public Uri RepositoryUri
        {
            get { return _repositoryUri; }
            internal set { _repositoryUri = value; }
        }

        public string RepositoryId
        {
            get { return _repositoryId; }
            internal set { _repositoryId = value; }
        }

        public Dictionary<string, object> CustomProperties
        {
            get { return _customProperties; }
            internal set { _customProperties = value; }
        }

        #endregion


        #region IEquatable<IIssueRepositorySettings> Members

        public bool Equals(IIssueRepositorySettings other)
        {
            return (true
                && other != null
                && string.Equals(ConnectorName, other.ConnectorName)
                && Uri.Equals(RepositoryUri, other.RepositoryUri)
                && string.Equals(RepositoryId, other.RepositoryId)
                && HasSameProperties(CustomProperties, other.CustomProperties)
                )
                ;
        }

        #endregion

        static bool HasSameProperties(Dictionary<string, object> props1, Dictionary<string, object> props2)
        {
            if (props1 == null && props2 == null) { return true; }
            int props1Count = props1 == null ? -1 : props1.Count;
            int props2Count = props2 == null ? -1 : props2.Count;
            if (props1Count != props2Count) {return false;}
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
