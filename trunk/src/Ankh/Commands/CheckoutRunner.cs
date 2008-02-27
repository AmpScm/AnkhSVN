// $Id: ViewRepositoryFileCommand.cs 738 2003-06-22 23:25:33Z Arild $
using EnvDTE;

using System.IO;
using System.Collections;


using Ankh.RepositoryExplorer;
using SharpSvn;
using System;

namespace Ankh.Commands
{
    /// <summary>
    /// A progress runner for checkouts.
    /// </summary>
    public class CheckoutRunner : IProgressWorker
    {
        public CheckoutRunner(string path, SvnRevision revision,
            Uri url, SvnDepth depth)
        {
            this.path = path;
            this.url = url;
            this.revision = revision;
            this.depth = depth;
        }

        public CheckoutRunner( string path, SvnRevision revision, 
            Uri url ) : this( path, revision, url, SvnDepth.Infinity )
        {
            // empty
        }

        public void Work(IContext context)
        {
            SvnUpdateResult result;
            SvnCheckOutArgs args = new SvnCheckOutArgs();
            args.Revision = revision;
            args.Depth = depth;
            context.Client.CheckOut(this.url, this.path, args, out result);
        }

        private SvnRevision revision;
        private Uri url;
        private string path;
        private SvnDepth depth;

    }
}



