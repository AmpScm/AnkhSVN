using System;

namespace NSvn
{
	/// <summary>
	/// Convenience base class for repository resource visitors.
	/// </summary>
	public class RepositoryResourceVisitorBase : IRepositoryResourceVisitor
	{
        #region IRepositoryResourceVisitor Members
        public virtual void VisitDirectory(RepositoryDirectory directory)
        {
            // empty
        }

        public virtual void VisitFile(RepositoryFile file)
        {
            // empty
        }
        #endregion
    }
}
