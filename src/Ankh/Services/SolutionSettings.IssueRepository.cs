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

        public IssueRepositorySettings ToIssueRepositorySettings()
        {
            string connectorName = ConnectorName;
            return string.IsNullOrEmpty(connectorName) ? null : new IssueRepositorySettingsProxy(Context, ConnectorName);
        }

        #endregion

        public bool ShouldPersist(IssueRepositorySettings other)
        {
            return !(true
                && other != null
                && string.Equals(ConnectorName, other.ConnectorName)
                && object.Equals(IssueRepositoryUri, other.RepositoryUri)
                && string.Equals(IssueRepositoryId, other.RepositoryId)
                && HasSameProperties(CustomProperties, other.CustomProperties)
                )
                ;
        }

        static bool HasSameProperties(IDictionary<string, object> props1, IDictionary<string, object> props2)
        {
            if (props1 == null && props2 == null)
                return true;

            if (props1 == null || props2 == null)
                return false; // if props1 == null, then props2 != null and the other way around

            if (props1.Count != props2.Count)
                return false;

            foreach (string key in props1.Keys)
            {
                if (!props2.ContainsKey(key))
                    return false;

                object value1 = props1[key];
                object value2 = props2[key];
                if (!object.Equals(value1, value2))
                    return false;
            }
            return true;
        }
    }
}
