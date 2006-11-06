using System;
using NSvn.Core;

namespace Ankh
{
	/// <summary>
	/// Summary description for RepositoryRootInfo.
	/// </summary>
    public class RepositoryRootInfo
    {
        public RepositoryRootInfo( string url, Revision revision )
        {
            this.url = url;
            this.revision = revision;
        }

        public string Url
        {
            get{ return this.url; }

        }
        public Revision Revision
        {
            get{ return this.revision; }
        }

        private string url;
        private Revision revision;
    }
}
