using System;

using System.IO;
using SharpSvn;

namespace Ankh.Commands
{
    /// <summary>
    /// For running cats on a separate thread.
    /// </summary>
    public class CatRunner : IProgressWorker
    {
        public CatRunner(string name, SvnRevision revision, Uri url) 
        { 
            this.path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), 
                name );
            this.url = url;
            this.revision = revision;
        }

        public CatRunner(SvnRevision revision, Uri url,  
            string path ) 
        {
            this.path = path;
            this.url = url;
            this.revision = revision;
        }

        public CatRunner(SvnRevision revision, Uri url)
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

        public void Work(AnkhWorkerArgs e)
        {
            using (FileStream fs = new FileStream(this.path, FileMode.Create,
                       FileAccess.Write))
            {
                SvnWriteArgs args = new SvnWriteArgs();
                args.Revision = this.revision;
                e.Client.Write(this.url, fs, args);
            }
        }

        private SvnRevision revision;
        private Uri url;
        private string path;

    }
}
