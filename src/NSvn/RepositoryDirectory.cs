// $Id$
using System;
using NSvn.Common;
using NSvn.Core;
using System.Text.RegularExpressions;

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
        /// Retrieve the child resources of this directory.
        /// </summary>
        /// <returns>A RepositoryResourceDictionary containing the child resources of this
        /// directory.</returns>
        public RepositoryResourceDictionary GetChildren()
        {
            RepositoryResourceDictionary dict = new RepositoryResourceDictionary();

            DirectoryEntry[] entries = Client.List( this.Url, this.Revision, false, 
                this.ClientContext );
            foreach( DirectoryEntry entry in entries )
            {
                string url = System.IO.Path.Combine( this.Url, entry.Path ).Replace( "\\", "/" );
                // dir or file?
                RepositoryResource resource;
                if ( entry.NodeKind == NodeKind.Directory )
                    resource = new RepositoryDirectory( url, this.Revision );
                else if ( entry.NodeKind == NodeKind.File )
                    resource = new RepositoryFile( url, this.Revision );
                else
                    throw new InvalidOperationException( "Expected a file or a directory" );

                resource.Context = this.Context;
                dict[entry.Path] = resource;
            }

            return dict;
        }

        /// <summary>
        /// Checks out the directory to a local directory.
        /// </summary>
        /// <param name="localPath">The local path to check out to.</param>
        /// <param name="recurse">Whether subdirectories should also
        /// be checked out.</param>
        public WorkingCopyDirectory Checkout( string localPath, bool recurse )
        {
            Client.Checkout( this.Url, localPath, this.Revision, recurse,
                this.ClientContext );

            return new WorkingCopyDirectory( localPath );
        }

        /// <summary>
        /// Checkout this directory to a local path.
        /// </summary>
        public static WorkingCopyDirectory Checkout( string url, 
            string localPath, Revision revision, bool recurse )
        {
            return new RepositoryDirectory( url, revision ).Checkout( 
                localPath, recurse );
        }

        /// <summary>
        /// Accept a RepositoryResourceVisitor.
        /// </summary>
        /// <param name="visitor">The RepositoryResourceVisitor to accept.</param>
        public override void Accept( IRepositoryResourceVisitor visitor )
        {
            visitor.VisitDirectory( this );
        }

        /// <summary>
        /// The name of the directory
        /// </summary>
        public override string Name
        {
            get
            {
                return NAME.Match( this.Url ).Groups[3].Value;
            }
        }


        

        private readonly Regex NAME = new Regex( 
            @"\w{3,4}:///?[\w\-\.\:]+(:\d+)?(/\w+)*/(\w+)+/?" );
    }
}
