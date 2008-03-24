using System;

using SharpSvn;

namespace Ankh
{
    /// <summary>
    /// Summary description for RepositoryRootInfo.
    /// </summary>
    public class RepositoryRootInfo
    {
        public RepositoryRootInfo( string url, SvnRevision revision )
        {
            this.url = url;
            this.revision = revision;
        }

        public string Url
        {
            get{ return this.url; }

        }
        public SvnRevision Revision
        {
            get { return this.revision; }
        }

        private string url;
        private SvnRevision revision;
    }
}
