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
    internal class CheckoutRunner : ProgressRunner
    {
        public CheckoutRunner( AnkhContext context, string path, Revision revision, string url ) 
            : base( context )
        { 
            this.path = path;
            this.url = url;
            this.revision = revision;
        }

        protected override void DoRun()
        {
            this.Context.Client.Checkout( this.url, this.path, this.revision, true );
        }

        private Revision revision;
        private string url;
        private string path;

    }
}



