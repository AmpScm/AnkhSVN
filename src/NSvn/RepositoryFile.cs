using System;
using NSvn.Common;
using NSvn.Core;

namespace NSvn
{
	/// <summary>
	/// Represents a file in the repository.
	/// </summary>
    public class RepositoryFile : RepositoryItem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="url">The URL to the file in the repository.</param>
        /// <param name="revision">The revision of the item</param>
        public RepositoryFile( string url, Revision revision ) : 
            base( url, revision )
        {
            // Empty
        }

        public RepositoryFile( string url ) :
            base( url )
        {
            // empty
        }
    }
}
