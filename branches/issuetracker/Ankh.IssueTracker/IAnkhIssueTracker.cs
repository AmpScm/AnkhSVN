using System;
namespace Ankh
{
    [System.Runtime.InteropServices.Guid("1AB39F07-3FA8-4c63-BB92-F1EDF29E9DBD")]
    public interface IAnkhIssueTracker
    {
        void Register(IAnkhIssueProvider Provider);
        bool UnRegister(IAnkhIssueProvider Provider);
        IAnkhIssueProvider CurrentIssueProvider { get; }
        IAnkhIssueProvider GetProviderFor(Uri RepositoryUri);
    }

    [System.Runtime.InteropServices.Guid("B99C1635-F7DE-4910-8813-19754DF273A0")]
    public interface SAnkhIssueTracker
    {
    }
}
