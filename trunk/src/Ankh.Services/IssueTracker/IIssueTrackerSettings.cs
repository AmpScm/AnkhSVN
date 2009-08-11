using System;
using System.Collections.Generic;
using System.Text;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.IssueTracker
{
    public interface IIssueTrackerSettings : Ankh.ExtensionPoints.IssueTracker.IIssueRepositorySettings
    {
        string ConnectorName { get; }
        string RawIssueRepositoryUri { get; }
        Uri IssueRepositoryUri { get; }
        string IssueRepositoryId { get; }
        string RawPropertyNames { get; }
        string RawPropertyValues { get; }
        IDictionary<string, object> CustomProperties { get; }
        bool ShouldPersist(IIssueRepositorySettings otherSettings);
    }
}
