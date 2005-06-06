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

            public void MakeDir(IRepositoryTreeNode parent, TreeNode node, string dirname)
            {
                // first show the log message dialog
                this.url = UriUtils.Combine( ((INode)parent).Url, dirname );
                this.Context.Client.LogMessageCompleted += new LogMessageCompletedEventHandler(Client_LogMessageCompleted);
                this.Context.Client.ShowLogMessageDialog( 
                    new string[]{this.url}, true );
                this.node = node;
            }

            private void Client_LogMessageCompleted(object sender, LogMessageEventArgs e)
            {
                ((SvnClient)sender).LogMessageCompleted -= new LogMessageCompletedEventHandler(Client_LogMessageCompleted);		
                
                IList list = e.CommitItems;

                if ( list == null || list.Count == 0 )
                    OnDirectoryCreated(new DirectoryCreatedEventArgs(false, this.node));

                this.Context.StartOperation( "Creating directory " + url );
                try
                {
                    bool completed = 
                        this.Context.UIShell.RunWithProgressDialog( this, "Creating directory" );
                    this.Context.Client.CommitCompleted();
                    OnDirectoryCreated(new DirectoryCreatedEventArgs(completed, this.node));
                }
                catch( Exception ex )
                {
                    this.Context.ErrorHandler.Handle( ex );
                    OnDirectoryCreated(new DirectoryCreatedEventArgs(false, this.node));
                }
                finally
                {
                    this.Context.EndOperation();
                }
            }

            /// <summary>
            /// Invokes the <see cref="DirectoryCreated"/> event
            /// </summary>
            /// <param name="e">The <see cref="DirectoryCreatedEventArgs"/></param>
            protected virtual void OnDirectoryCreated(DirectoryCreatedEventArgs e)
            {
                if(this.directoryCreated != null)
                    this.directoryCreated(this, e);
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

            /// <summary>
            /// Fires after the directory has been created
            /// </summary>
            public event DirectoryCreatedEventHandler DirectoryCreated
            {
                add { this.directoryCreated += value; }
                remove { this.directoryCreated -= value; }
            }

            private event DirectoryCreatedEventHandler directoryCreated;
            private string url;
            private IContext context;
            private TreeNode node;
        }
    }
}



