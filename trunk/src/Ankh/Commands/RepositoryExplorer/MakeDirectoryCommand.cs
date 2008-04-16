// $Id$
using System;
using System.Windows.Forms;
using System.Collections;
using Ankh.RepositoryExplorer;
using Ankh.UI;
using Utils;
using Ankh.Ids;
using SharpSvn;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to creates a new directory here in the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.NewDirectory)]
    public class MakeDirectoryCommand : CommandBase
    {
        SvnCommitArgs _args = null;
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();
            IExplorersShell shell = e.GetService<IExplorersShell>();

            // we only want directories
            if (shell.RepositoryExplorerService.SelectedNode == null ||
                !shell.RepositoryExplorerService.SelectedNode.IsDirectory)
            {
                e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            /*
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
            */
        }

        #endregion

        private void DoCreateDir(ProgressWorkerArgs e)
        {
            e.Client.RemoteCreateDirectory(this.url);
        }

        Uri url = null;
    }
}



