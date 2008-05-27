using System;
using Ankh.RepositoryExplorer;
using Ankh.Ids;
using Utils.Win32;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// A command that opens a file from the server in VS.NET
    /// </summary>
    [Command(AnkhCommand.ViewInVsNet)]
    public class ViewInVSNetCommand : ViewRepositoryFileCommand
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = false;
        }
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            /*IContext context = e.GetService<IContext>();
            IExplorersShell shell = e.GetService<IExplorersShell>();
            EnvDTE._DTE dte = e.GetService<EnvDTE._DTE>(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));

            using (e.Context.BeginOperation("Opening"))
            {
                INode node = shell.RepositoryExplorerService.SelectedNode;

                CatRunner runner = new CatRunner(node.Name, node.Revision, new Uri(node.Url));
                e.GetService<IProgressRunner>().Run("Retrieving file", runner.Work);

                dte.ItemOperations.OpenFile(runner.Path,
                    EnvDTE.Constants.vsViewKindPrimary);
            }*/
        }

        #endregion
    }
}