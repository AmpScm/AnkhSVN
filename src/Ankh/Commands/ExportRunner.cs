// $Id: ViewRepositoryFileCommand.cs 738 2003-06-22 23:25:33Z Arild $
using EnvDTE;

using System.IO;
using System.Collections;
using NSvn.Core;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// A progress runner for checkouts.
    /// </summary>
    public class ExportRunner : IProgressWorker
    {
        public ExportRunner(  string path, Revision revision, 
            string url, bool recurse )
        { 
            this.path = path;
            this.url = url;
            this.revision = revision;
            this.recurse = recurse;
        }

        public ExportRunner( string path, Revision revision, 
            string url ) : this( path, revision, url, true )
        {
            // empty
        }

        public void Work( IContext context )
        {
            context.Client.Export( this.url, this.path, this.revision, this.recurse );
        }

        private Revision revision;
        private string url;
        private string path;
        private bool recurse;

    }
}



