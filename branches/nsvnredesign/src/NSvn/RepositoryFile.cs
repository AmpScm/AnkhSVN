// $Id$
using System;
using System.IO;
using NSvn.Common;
using NSvn.Core;

namespace NSvn
{
    /// <summary>
    /// Represents a file in the repository.
    /// </summary>
    [Obsolete("Will be removed in a future version")]
    public class RepositoryFile : RepositoryResource
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

        /// <summary>
        /// The name of the file.
        /// </summary>
        public override string Name
        {
            get
            {
                return System.IO.Path.GetFileName( this.Url );
            }
        }

        /// <summary>
        /// Writes the file from the repository into the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write the file into.</param>
        public void Cat( Stream stream )
        {
            Client.Cat( stream, this.Url, this.Revision, this.ClientContext );
        }        

        /// <summary>
        /// Accept a RepositoryResourceVisitor.
        /// </summary>
        /// <param name="visitor">The RepositoryResourceVisitor to accept.</param>
        public override void Accept( IRepositoryResourceVisitor visitor )
        {
            visitor.VisitFile( this );
        }
    }
}
