using System;
using EnvDTE;

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

                CatVisitor v = new CatVisitor( context );
                context.RepositoryController.VisitSelectedNodes( v );

                foreach( string filename in v.FileNames )
                    context.DTE.ItemOperations.OpenFile( filename, Constants.vsViewKindPrimary );

            }
            finally
            {
                context.EndOperation();
            }
        }        
	}
}
