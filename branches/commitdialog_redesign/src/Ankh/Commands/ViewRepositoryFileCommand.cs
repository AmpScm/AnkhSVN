// $Id$
using EnvDTE;

using System.IO;
using System.Collections;
using NSvn.Core;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you view a repository file.
    /// </summary>
    [VSNetCommand("ViewRepositoryFile", Tooltip="View this file", Text = "In VS.NET" ),
    VSNetControl( "ReposExplorer.View", Position = 1 ) ]
    public abstract class ViewRepositoryFileCommand : CommandBase
    {
        #region ICommand Members
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // we enable it if it's a file.
            return context.RepositoryExplorer.SelectedNode != null &&
                !context.RepositoryExplorer.SelectedNode.IsDirectory ? 
                Enabled : Disabled;
        }
        #endregion

        #region CatRunner class
        /// <summary>
        /// For running cats on a separate thread.
        /// </summary>
        protected class CatRunner : IProgressWorker
        {
            public CatRunner( string name, Revision revision, string url ) 
            { 
                this.path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), 
                    name );
                this.url = url;
                this.revision = revision;
            }

            public CatRunner( Revision revision, string url,  
                 string path ) 
            {
                this.path = path;
                this.url = url;
                this.revision = revision;
            }

            /// <summary>
            /// The path the file was written to.
            /// </summary>
            public string Path
            {
                get{ return this.path; }
            }

            public void Work( IContext context )
            {
                using( FileStream fs = new FileStream( this.path, FileMode.Create, 
                           FileAccess.Write ) )
                {
                    context.Client.Cat( fs, this.url, this.revision );
                }
            }

            private Revision revision;
            private string url;
            private string path;

        }
        #endregion        
    }
}



