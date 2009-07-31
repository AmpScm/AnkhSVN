using System;
using System.Collections.Generic;
using System.Text;

using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.Services.IssueTracker
{
    class IssueRepositorySettings : IIssueRepositorySettings
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

    }
}
