using System;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for ViewInVSNetCommand.
	/// </summary>
    [VSNetCommand("ViewInVsNet", Tooltip="View this file in VS.NET", Text = "In VS.NET" ),
    VSNetControl( "ReposExplorer.View", Position = 1 ) ]
	internal class ViewInVSNetCommand : ViewRepositoryFileCommand
	{
        public override void Execute(AnkhContext context)
        {
            CatVisitor v = new CatVisitor( );
            context.RepositoryController.VisitSelectedNodes( v );

            foreach( string filename in v.FileNames )
                context.DTE.ItemOperations.OpenFile( filename, Constants.vsViewKindPrimary );
        }        
	}
}
