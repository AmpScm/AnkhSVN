using System;
using System.Collections.Generic;
using System.Text;

using Ankh.ExtensionPoints.IssueTracker;
using Ankh.IssueTracker;

namespace Ankh.Services.IssueTracker
{
    public class IssueRepositorySettingsProxy : IssueRepositorySettings
    {
        private string _connectorName;
        private Uri _repositoryUri;
        private string _repositoryId;
        private Dictionary<string, object> _customProperties;

        IAnkhServiceProvider _context;

        public IssueRepositorySettingsProxy(IAnkhServiceProvider context, string connectorName)
            : base(connectorName)
        {
            _context = context;
        }

        #region IssueRepositorySettingsBase Members

        public override Uri RepositoryUri
        {
            get
            {
                IIssueTrackerSettings settings = Settings;
                return settings == null ? null : settings.IssueRepositoryUri;
            }
        }

        public override string RepositoryId
        {
            get
            {
                IIssueTrackerSettings settings = Settings;
                return settings == null ? null : settings.IssueRepositoryId;
            }
        }

        public override IDictionary<string, object> CustomProperties
        {
            get
            {
                IIssueTrackerSettings settings = Settings;
                return settings == null ? null : settings.CustomProperties;
            }
        }

        #endregion

        private IIssueTrackerSettings Settings
        {
            get
            {
                if (_context == null) { return null; }
                return _context.GetService<IIssueTrackerSettings>();
            }
        }
    }
}
