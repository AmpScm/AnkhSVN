using System;
using System.Collections.Generic;

namespace Ankh.ExtensionPoints.IssueTracker
{
    [System.Runtime.InteropServices.Guid("9817F698-D118-4a20-AB2C-6491F2028838")]
    public interface IIssueRepositorySettings
    {
        /// <summary>
        /// Gets the issue repository connector's registered name
        /// </summary>
        string ConnectorName { get; }

        /// <summary>
        /// Gets Issue Repository URI
        /// </summary>
        Uri RepositoryUri { get; }

        /// <summary>
        /// Gets the unique id of the repository.
        /// </summary>
        /// <remarks>This value can be used to identify a specific (sub) issue repository on RepositoryUri</remarks>
        string RepositoryId { get; }

        /// <summary>
        /// Custom properties specific to the connector.
        /// </summary>
        Dictionary<string, object> CustomProperties { get; }
    }
}
