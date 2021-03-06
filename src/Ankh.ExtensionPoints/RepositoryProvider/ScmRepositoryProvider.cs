
using System;
namespace Ankh.ExtensionPoints.RepositoryProvider
{
    /// <summary>
    /// Base class for SCM Repository Provider service
    /// </summary>
    public abstract class ScmRepositoryProvider
    {
        RepositoryType _reposType;

        /// <summary>
        /// Gets the identifier of this provider
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Gets the user-friendly name of this provider.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Creates a SCM repository selection control which provides the UI to select  a repository
        /// </summary>
        /// <remarks>This loads the service package if not already loaded</remarks>
        public abstract ScmRepositorySelectionControl CreateSelectionControl();

        /// <summary>
        /// Gets or sets the type of the repositories that this provider supplies.
        /// </summary>
        public virtual RepositoryType Type
        {
            get { return _reposType; }
            set { _reposType = value; }
        }
    }
}
