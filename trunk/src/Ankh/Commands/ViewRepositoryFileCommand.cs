// $Id$
using EnvDTE;
using NSvn;
using System.IO;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you view a repository file.
    /// </summary>
    [VSNetCommand("ViewRepositoryFile", Tooltip="View this file", Text = "In VS.NET" ),
     VSNetControl( "ReposExplorer.View", Position = 1 ) ]
    internal class ViewRepositoryFile : CommandBase
    {
       
    
        #region ICommand Members
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            IsRepositoryFileVisitor v = new IsRepositoryFileVisitor();
            context.RepositoryController.VisitSelectedNodes( v );
            return v.IsFile ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled :
                vsCommandStatus.vsCommandStatusSupported;
        }

        public override void Execute(AnkhContext context)
        {
            CatVisitor v = new CatVisitor( context.DTE );
            context.RepositoryController.VisitSelectedNodes( v );
        }
        #endregion

        private class CatVisitor : RepositoryResourceVisitorBase
        {
            public CatVisitor( _DTE dte )
            {
                this.dte = dte;
            }

            public override void VisitFile(RepositoryFile file)
            {
                string filename = Path.Combine( Path.GetTempPath(), file.Name );
                using( FileStream fs = new FileStream( filename, FileMode.Create, FileAccess.Write ) )
                    file.Cat( fs );                

                this.dte.ItemOperations.OpenFile( filename, Constants.vsViewKindPrimary );
            }

            private _DTE dte;
        }
    }
}



