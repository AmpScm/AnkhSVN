using System;
using EnvDTE;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
	/// <summary>
	/// A command that opens a file from the server in VS.NET
	/// </summary>
    [VSNetCommand("ViewInVsNet", Tooltip="View this file in VS.NET", Text = "In VS.NET" ),
    VSNetControl( "ReposExplorer.View", Position = 1 ) ]
	internal class ViewInVSNetCommand : ViewRepositoryFileCommand
	{
        public override void Execute(AnkhContext context, string parameters)
        {
            try
            {
                context.StartOperation( "Opening" );

                INode node = (INode)context.RepositoryExplorer.SelectedNode;

                CatRunner runner = new CatRunner(context, node.Name, node.Revision, node.Url);
                runner.Start( "Retrieving file" );

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
