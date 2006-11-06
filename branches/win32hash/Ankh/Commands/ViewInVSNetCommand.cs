using System;
using EnvDTE;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
	/// <summary>
	/// A command that opens a file from the server in VS.NET
	/// </summary>
    [VSNetCommand("ViewInVsNet", Tooltip="View this file in VS.NET", 
         Text = "In VS.NET", Bitmap = ResourceBitmaps.ViewInVSNet ),
    VSNetControl( "ReposExplorer.View", Position = 1 ) ]
	public class ViewInVSNetCommand : ViewRepositoryFileCommand
	{
        public override void Execute(IContext context, string parameters)
        {
            try
            {
                context.StartOperation( "Opening" );

                INode node = context.RepositoryExplorer.SelectedNode;

                CatRunner runner = new CatRunner(node.Name, node.Revision, node.Url);
                context.UIShell.RunWithProgressDialog( runner, "Retrieving file" );

                context.DTE.ItemOperations.OpenFile( runner.Path, 
                    Constants.vsViewKindPrimary );

            }
            finally
            {
                context.EndOperation();
            }
        }        
	}
}
