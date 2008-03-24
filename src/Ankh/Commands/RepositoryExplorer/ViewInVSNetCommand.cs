using System;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;
using Utils.Win32;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that opens a file from the server in VS.NET
    /// </summary>
    [Command(AnkhCommand.ViewInVsNet)]
    public class ViewInVSNetCommand : ViewRepositoryFileCommand
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            using (e.Context.BeginOperation("Opening"))
            {

                INode node = context.RepositoryExplorer.SelectedNode;

                CatRunner runner = new CatRunner(node.Name, node.Revision, new Uri(node.Url));
                context.UIShell.RunWithProgressDialog(runner, "Retrieving file");

                ((IDTEContext)e.Context).DTE.ItemOperations.OpenFile(runner.Path,
                    EnvDTE.Constants.vsViewKindPrimary);
            }
        }

        #endregion
    }
}