using System;
using NSvn.Common;
using NSvn.Core;


namespace NSvn
{
	/// <summary>
	/// Represents an item in the repository.
	/// </summary>
	public class RepositoryItem
	{
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="url">The URL to the item in the repository.</param>
        /// <param name="revision">The revision of the item.</param>
		protected RepositoryItem( string url, Revision revision )
		{
            this.url = url;
            this.revision = revision;
			
		}

        /// <summary>
        /// Constructor. Defaults to the HEAD revision.
        /// </summary>
        /// <param name="url">The URL to the item in the repository</param>
        protected RepositoryItem( string url ) : 
            this( url, Revision.Head )
        {
            // empty
        }



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
