using System;
using NSvn.Common;
using NSvn.Core;


namespace NSvn
{
	/// <summary>
	/// Represents an item in the repository.
	/// </summary>
	public abstract class RepositoryResource : SvnResource
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="url">The URL to the item in the repository.</param>
        /// <param name="revision">The revision of the item.</param>
		protected RepositoryResource( string url, Revision revision )
		{
            this.url = url;
            this.revision = revision;
			
		}

        /// <summary>
        /// Constructor. Defaults to the HEAD revision.
        /// </summary>
        /// <param name="url">The URL to the item in the repository</param>
        protected RepositoryResource( string url ) : 
            this( url, Revision.Head )
        {
            // empty
        }

        /// <summary>
        /// Accept a RepositoryResourceVisitor.
        /// </summary>
        /// <param name="visitor">The RepositoryResourceVisitor to accept.</param>
        public abstract void Accept( RepositoryResourceVisitor visitor );



        /// <summary>
        /// The URL to the item in the repository.
        /// </summary>
        public string Url
        {
            get{ return this.url; }
        }

        /// <summary>
        /// The revision of the item.
        /// </summary>
        public Revision Revision
        {
            get{ return this.revision; }
        }


        private string url;
        private Revision revision;
	}
}
