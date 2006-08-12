using System;
using NSvn.Core;
using System.IO;

namespace Ankh.Commands
{
    /// <summary>
    /// For running cats on a separate thread.
    /// </summary>
    public class CatRunner : IProgressWorker
    {
        public CatRunner( string name, Revision revision, string url ) 
        { 
            this.path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), 
                name );
            this.url = url;
            this.revision = revision;
        }

        public CatRunner( Revision revision, string url,  
            string path ) 
        {
            this.path = path;
            this.url = url;
            this.revision = revision;
        }

        public CatRunner( Revision revision, string url )
        {
            this.revision = revision;
            this.url = url;
            this.path = System.IO.Path.GetTempFileName();
        }

        /// <summary>
        /// The path the file was written to.
        /// </summary>
        public string Path
        {
            get{ return this.path; }
        }

        public void Work( IContext context )
        {
            using( FileStream fs = new FileStream( this.path, FileMode.Create, 
                       FileAccess.Write ) )
            {
                context.Client.Cat( fs, this.url, this.revision );
            }
        }

        private Revision revision;
        private string url;
        private string path;

    }
}
