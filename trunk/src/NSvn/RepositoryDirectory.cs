using System;
using NSvn.Common;
using NSvn.Core;

namespace NSvn
{
	/// <summary>
	/// Represents a directory in a repository.
	/// </summary>
	public class RepositoryDirectory : RepositoryResource
	{
        /// <summary>
        /// Constructor. Defaults to the HEAD revision.
        /// </summary>
        /// <param name="url">The URL to the directory in the repository.</param>
		public RepositoryDirectory( string url ) : base( url )
		{
			// empty
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="url">The URL to the directory in the repository.</param>
        /// <param name="revision">The revision of the directory.</param>
        public RepositoryDirectory( string url, Revision revision ) :
            base( url, revision )
        {
            // empty
        }

        /// <summary>
        /// Checks out the directory to a local directory.
        /// </summary>
        /// <param name="localPath">The local path to check out to.</param>
        /// <param name="recurse">Whether subdirectories should also
        /// be checked out.</param>
        public WorkingCopyDirectory Checkout( string localPath, bool recurse )
        {
            Client.Checkout( url, localPath, revision, recurse,
                new ClientContext() );

            return new WorkingCopyDirectory( localPath );
        }

        /// <summary>
        /// Checkout this directory to a local path.
        /// </summary>
        public static WorkingCopyDirectory Checkout( string url, 
            string localPath, Revision revision, bool recurse )
        {
            // TODO: what to do with ClientContext here?
            return new RepositoryDirectory( url, revision ).Checkout( 
                localPath, recurse );

            
        }
	}
}
