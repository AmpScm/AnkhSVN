using System;

namespace Ankh.Interop.IssueTracker
{
    [System.Runtime.InteropServices.Guid("0E080D53-D4A1-4609-8AB1-45ABD217BA4B")]
    public interface IIssueRepositoryConnector
    {
        string Name { get; }

        bool TryGetRepository(Uri issueRepositoryUri, out IIssueRepository repository);

        IIssueRepository Connect(Uri issueRepositoryUri, bool createNewConnection);
    }
}
