using System.Collections.Generic;
using Ankh.ExtensionPoints.RepositoryProvider;

namespace Ankh
{
    public interface IAnkhRepositoryProviderService
    {
        /// <summary>
        /// Gets all the registered SCM repository providers.
        /// </summary>
        /// <remarks>This call DOES NOT trigger provider package initialization.</remarks>
        ICollection<ScmRepositoryProvider> RepositoryProviders { get; }

        /// <summary>
        /// Gets all the registered SCM repository providers for the given SCM type(svn, git).
        /// </summary>
        /// <remarks>This call DOES NOT trigger provider package initialization.</remarks>
        ICollection<ScmRepositoryProvider> GetRepositoryProviders(string type);

        /// <summary>
        /// Tries to find a registered provider with the given name.
        /// </summary>
        /// <remarks>This call DOES NOT trigger provider package initialization.</remarks>
        bool TryGetRepositoryProvider(string id, out ScmRepositoryProvider repoProvider);

        /// <summary>
        /// Shows the help for developing SCM repository provider
        /// </summary>
        void ShowProvideHelp();
    }
}
