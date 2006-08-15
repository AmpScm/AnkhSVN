// $Id: ViewRepositoryFileCommand.cs 738 2003-06-22 23:25:33Z Arild $
using EnvDTE;

using System.IO;
using System.Collections;
using NSvn.Core;
using NSvn.Common;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// A progress runner for checkouts.
    /// </summary>
    public class CheckoutRunner : IProgressWorker
    {
        public CheckoutRunner( string path, Revision revision, 
            string url, Recurse recurse ) 
        { 
            this.path = path;
            this.url = url;
            this.revision = revision;
            this.recurse = recurse;
        }

        public CheckoutRunner( string path, Revision revision, 
            string url ) : this( path, revision, url, Recurse.Full )
        {
            // empty
        }

        public void Work( IContext context )
        {
            context.Client.Checkout( this.url, this.path, this.revision, this.recurse );
        }

        private Revision revision;
        private string url;
        private string path;
        private Recurse recurse;

    }
}



