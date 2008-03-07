// $Id$
using System;
using System.Windows.Forms;
using System.Collections;
using Ankh.RepositoryExplorer;
using Ankh.UI;
using Utils;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to creates a new directory here in the Repository Explorer.
    /// </summary>
    [VSNetCommand(AnkhCommand.NewDirectory,
		"MakeDirectoryCommand",
         Text = "Ne&w Directory...",
         Tooltip = "Create a new directory here.",
         Bitmap = ResourceBitmaps.MakeDirectory ),
         VSNetControl( "ReposExplorer", Position = 1 )]
    public class MakeDirectoryCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            // we only want directories
            if ( e.Context.RepositoryExplorer.SelectedNode == null ||
                !e.Context.RepositoryExplorer.SelectedNode.IsDirectory )
            {
                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

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

        #endregion

        private void DoCreateDir( IContext context )
        {
            // create the dir.
            context.Client.CreateDirectory( this.url );
        }

        private string url;        
    }
}



