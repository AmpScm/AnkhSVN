//// $Id$
//using EnvDTE;
//
//using System.IO;
//using System.Collections;
//
//namespace Ankh.Commands
//{
//    /// <summary>
//    /// A command that lets you view a repository file.
//    /// </summary>
//    [VSNetCommand("ViewRepositoryFile", Tooltip="View this file", Text = "In VS.NET" ),
//     VSNetControl( "ReposExplorer.View", Position = 1 ) ]
//    internal abstract class ViewRepositoryFileCommand : CommandBase
//    {
//       
//    
//        #region ICommand Members
//        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
//        {
//            IsRepositoryFileVisitor v = new IsRepositoryFileVisitor();
//            context.RepositoryController.VisitSelectedNodes( v );
//            return v.IsFile ? vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled :
//                vsCommandStatus.vsCommandStatusSupported;
//        }
//        #endregion
//
//        #region CatVisitor
//        protected class CatVisitor : RepositoryResourceVisitorBase
//        {    
//            public CatVisitor( AnkhContext context )
//            {
//                this.context = context;
//            }
//
//            public virtual IEnumerable FileNames
//            {
//                get{ return this.fileNames; }
//            }
//
//            public override void VisitFile(RepositoryFile file)
//            {
//                string filename = this.GetPath( file.Name );
//                if ( filename != null )
//                {
//                    this.context.OutputPane.WriteLine( "Retrieving {0}", file.Url );
//                    using( FileStream fs = new FileStream( filename, FileMode.Create, FileAccess.Write ) )
//                        file.Cat( fs );  
//
//                    this.fileNames.Add( filename );
//                }
//            }
//
//            protected virtual string GetPath( string filename )
//            {
//                return  Path.Combine( Path.GetTempPath(), filename );
//            }
//
//            private ArrayList fileNames = new ArrayList();
//            private AnkhContext context;
//        }
//        #endregion
//    }
//}
//
//
//
