using System;
using System.Collections.Generic;
using System.Text;
using Ankh.IssueTracker;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.Services.IssueTracker;

namespace Ankh.Settings
{
    [GlobalService(typeof(IIssueTrackerSettings))]
    partial class SolutionSettings : IIssueTrackerSettings
    {
        #region IIssueTrackerSettings Members

        public string ConnectorName
        {
            get
            {
                RefreshIfDirty();
                return _cache == null ? null : _cache.IssueRepositoryConnectorName;
            }
        }


        public string RawIssueRepositoryUri
        {
            get
            {
                RefreshIfDirty();
                return _cache == null ? null : _cache.IssueRepositoryUri;
            }
        }

        public Uri IssueRepositoryUri
        {
            get
            {
                Uri result = null;
                string rawUri = RawIssueRepositoryUri;
                if (!string.IsNullOrEmpty(rawUri))
                {
                    if (!Uri.TryCreate(rawUri, UriKind.Absolute, out result))
                    {
                        result = null;
                    }

                }
                return result;
            }
        }

        public string IssueRepositoryId
        {
            get
            {
                RefreshIfDirty();
                return _cache == null ? null : _cache.IssueRepositoryId;
            }
        }

        public string RawPropertyNames
        {
            get
            {
                RefreshIfDirty();
                return _cache == null ? null : _cache.IssueRepositoryPropertyNames;
            }
        }

        public string RawPropertyValues
        {
            get
            {
                RefreshIfDirty();
                return _cache == null ? null : _cache.IssueRepositoryPropertyValues;
            }
        }

        public IDictionary<string, object> CustomProperties
        {
            get
            {
                Dictionary<string, object> customProperties = null;

                string propNames = RawPropertyNames;
                if (!string.IsNullOrEmpty(propNames))
                {
                    string[] propNameArray = propNames.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (propNameArray.Length > 0)
                    {
                        string propValues = RawPropertyValues;
                        if (!string.IsNullOrEmpty(propValues))
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
                return customProperties;
            }
        }

        public IssueRepositorySettingsBase ToIssueRepositorySettings()
        {
            return new IssueRepositorySettingsProxy(Context, ConnectorName);
        }

        #endregion

        public bool ShouldPersist(IssueRepositorySettingsBase other)
        {
            return !(true
                && other != null
                && string.Equals(ConnectorName, other.ConnectorName)
                && Uri.Equals(IssueRepositoryUri, other.RepositoryUri)
                && string.Equals(IssueRepositoryId, other.RepositoryId)
                && HasSameProperties(CustomProperties, other.CustomProperties)
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
