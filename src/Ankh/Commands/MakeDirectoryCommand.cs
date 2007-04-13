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
    /// A command for creating a new remote directory (in the repos explorer).
    /// </summary>
    [VSNetCommand("MakeDirectoryCommand", 
         Tooltip="Create new directory here", Text = "Ne&w directory...",
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
            INode node = context.RepositoryExplorer.SelectedNode;
            System.Diagnostics.Debug.Assert( node != null );

            string dirname = context.UIShell.ShowNewDirectoryDialog();

            // did the user bail out?
            if ( dirname == null )
                return;

            // first show the log message dialog
            this.url = UriUtils.Combine( node.Url, dirname );
            CommitOperation operation = new CommitOperation(
                new SimpleProgressWorker( new SimpleProgressWorkerCallback(this.DoCreateDir) ), 
                new string[]{ this.url }, context );
            operation.UrlPaths = true;

            if ( !operation.ShowLogMessageDialog() )
                return;

            context.StartOperation( "Creating directory " + url );
            try
            {
                operation.Run( "Creating directory" );
                context.RepositoryExplorer.Refresh( context.RepositoryExplorer.SelectedNode );
            }
            finally
            {
                context.EndOperation();
            }           

        }

        private void DoCreateDir( IContext context )
        {
            // create the dir.
            context.Client.MakeDir( new string[]{this.url} );
        }

        private string url;        
    }
}



