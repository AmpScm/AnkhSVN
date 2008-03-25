// $Id$
using System;
using System.Windows.Forms;
using System.Collections;
using Ankh.RepositoryExplorer;
using Ankh.UI;
using Utils;
using AnkhSvn.Ids;
using SharpSvn;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to creates a new directory here in the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.NewDirectory)]
    public class MakeDirectoryCommand : CommandBase
    {
        SvnCommitArgs _args;
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            // we only want directories
            if ( context.RepositoryExplorer.SelectedNode == null ||
                !context.RepositoryExplorer.SelectedNode.IsDirectory )
            {
                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            INode node = context.RepositoryExplorer.SelectedNode;
            System.Diagnostics.Debug.Assert(node != null);

            string dirname = context.UIShell.ShowNewDirectoryDialog();

            // did the user bail out?
            if (dirname == null)
                return;

            // first show the log message dialog
            this.url = UriUtils.Combine(node.Url, dirname);
            CommitOperation operation = new CommitOperation(
                _args,
                new SimpleProgressWorker(new SimpleProgressWorkerCallback(this.DoCreateDir)),
                new string[] { this.url },
                e.Context);
            operation.UrlPaths = true;

            if (!operation.ShowLogMessageDialog())
                return;

            using (context.StartOperation("Creating directory " + url))
            {
                operation.Run("Creating directory");
                context.RepositoryExplorer.Refresh(context.RepositoryExplorer.SelectedNode);
            }
        }

        #endregion

        private void DoCreateDir( AnkhWorkerArgs e )
        {
            // create the dir.
            e.Client.CreateDirectory( this.url );
        }

        private string url;        
    }
}



