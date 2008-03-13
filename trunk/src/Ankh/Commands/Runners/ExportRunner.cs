// $Id: ViewRepositoryFileCommand.cs 738 2003-06-22 23:25:33Z Arild $
using System.IO;
using System.Collections;

using Ankh.RepositoryExplorer;
using SharpSvn;

namespace Ankh.Commands
{
    /// <summary>
    /// A progress runner for checkouts.
    /// </summary>
    public class ExportRunner : IProgressWorker
    {
        public ExportRunner(string path, SvnRevision revision, 
            string url, SvnDepth depth )
        { 
            this.path = path;
            this.url = url;
            this.revision = revision;
            this.depth = depth;
        }

        public ExportRunner(string path, SvnRevision revision, 
            string url ) : this( path, revision, url, SvnDepth.Infinity )
        {
            // empty
        }

        public void Work(AnkhWorkerArgs e)
        {
            SvnExportArgs args = new SvnExportArgs();
            args.Depth = depth;
            args.Revision = revision;
            e.Client.Export(this.url, this.path, args);
        }

        private SvnRevision revision;
        private string url;
        private string path;
        private SvnDepth depth;
    }
}



