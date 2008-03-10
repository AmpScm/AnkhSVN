using System;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;
using Utils.Win32;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that opens a file from the server in VS.NET
    /// </summary>
    [VSNetCommand(AnkhCommand.ViewInVsNet,
		"ViewInVsNet",
         Text = "In &Visual Studio",
         Tooltip = "View this file in Visual Studio.", 
         Bitmap = ResourceBitmaps.ViewInVSNet ),
         VSNetControl( "ReposExplorer.View", Position = 1 )]
    public class ViewInVSNetCommand : ViewRepositoryFileCommand
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            using (context.StartOperation("Opening"))
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