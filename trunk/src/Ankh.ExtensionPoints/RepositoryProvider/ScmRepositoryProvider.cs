
namespace Ankh.ExtensionPoints.RepositoryProvider
{
    /// <summary>
    /// Base class for SCM Repository Provider service
    /// </summary>
    public abstract class ScmRepositoryProvider
    {
        public static readonly string TYPE_SVN = "svn";
        public static readonly string TYPE_GIT = "git";

        private string scmType;

        /// <summary>
        /// Gets the identifier of this provider
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Gets the user-friendly name of this provider.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the SCM repository selection control which provides the UI to select  a repository
        /// </summary>
        public abstract ScmRepositorySelectionControl RepositorySelectionControl { get; }

        /// <summary>
        /// Gets or sets the type of the repositories that this provider supplies.
        /// </summary>
        public virtual string ScmType
        {
            get { return this.scmType; }
            set { this.scmType = value; }
        }
    }
}
