// $Id: ViewRepositoryFileCommand.cs 738 2003-06-22 23:25:33Z Arild $
using EnvDTE;

using System.IO;
using System.Collections;
using NSvn.Core;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you view a repository file.
    /// </summary>
    internal abstract class CheckoutCommand : CommandBase
    {
        #region ICommand Members
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            return context.RepositoryExplorer.SelectedNode.IsDirectory ? 
                vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled :
                vsCommandStatus.vsCommandStatusSupported;

        }
        #endregion


        #region CheckoutRunner class
        /// <summary>
        /// For running checkouts on a separate thread.
        /// </summary>
        protected class CheckoutRunner : ProgressRunner
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
        #endregion
    }
}



