using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.ExtensionPoints.IssueTracker
{
    /// <summary>
    /// Base class for Issue Repository Connector service
    /// </summary>
    public abstract class IssueRepositoryConnectorBase
    {
        /// <summary>
        /// Gets the registered connector's unique name 
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Creates an Issue Repository based on the settings.
        /// </summary>
        public abstract IssueRepositoryBase Create(IssueRepositorySettingsBase settings);

        /// <summary>
        /// Gets the repository configuration page (to edit/setup an issue repository)
        /// </summary>
        public abstract IssueRepositoryConfigurationPageBase ConfigurationPage { get; }

    }
}
