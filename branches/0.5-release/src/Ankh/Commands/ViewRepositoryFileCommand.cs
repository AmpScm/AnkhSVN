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
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
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
        protected class CatRunner : ProgressRunner
        {
            public CatRunner( AnkhContext context, string name, Revision revision, string url ) : 
                base( context )
            { 
                this.path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), 
                    name );
                this.url = url;
                this.revision = revision;
            }

            public CatRunner( AnkhContext context,  Revision revision, string url,  
                 string path ) : base(context)
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

            protected override void DoRun()
            {
                using( FileStream fs = new FileStream( this.path, FileMode.Create, 
                           FileAccess.Write ) )
                {
                    this.Context.Client.Cat( fs, this.url, this.revision );
                }
            }

            private Revision revision;
            private string url;
            private string path;

        }
        #endregion        
    }
}



