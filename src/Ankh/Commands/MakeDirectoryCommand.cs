// $Id$
using System;
using System.Windows.Forms;
using System.Collections;
using Ankh.RepositoryExplorer;
using Ankh.UI;
using Utils;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for MakeDirectory.
    /// </summary>
    [VSNetCommand("MakeDirectoryCommand", 
         Tooltip="Create new directory here", Text = "New directory...",
         Bitmap = ResourceBitmaps.MakeDirectory ),
    VSNetControl( "ReposExplorer", Position = 1 ) ]
    public class MakeDirectoryCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // we only want directories
            if ( context.RepositoryExplorer.SelectedNode != null &&
                context.RepositoryExplorer.SelectedNode.IsDirectory )
            {
                return Enabled;
            }
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            context.RepositoryExplorer.MakeDir( new NewDirHandler(context) );           
        }

        /// <summary>
        /// This class handles the actual creation of the new dir.
        /// </summary>
        private class NewDirHandler : IProgressWorker, INewDirectoryHandler
        {
            public NewDirHandler( IContext context ) 
            {
                this.context = context;
            }

            public bool MakeDir(IRepositoryTreeNode parent, string dirname)
            {
                // first show the log message dialog
                this.url = UriUtils.Combine( ((INode)parent).Url, dirname );
                CommitOperation operation = new CommitOperation(
                    this, new string[]{ this.url }, context );
                operation.UrlPaths = true;

                if ( !operation.ShowLogMessageDialog() )
                    return false;

                this.Context.StartOperation( "Creating directory " + url );
                try
                {
                    bool completed= 
                        operation.Run( "Creating directory" );
                    return completed;
                }
                catch( Exception ex )
                {
                    this.Context.ErrorHandler.Handle( ex );
                    return false;
                }
                finally
                {
                    this.Context.EndOperation();
                }
            }

            protected IContext Context
            {
                get{ return this.context; }
            }

            public void Work( IContext context )
            {
                // create the dir.
                context.Client.MakeDir( new string[]{this.url} );
            }


            private string url;
            private IContext context;
        }
    }
}



