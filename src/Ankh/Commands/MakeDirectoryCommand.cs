// $Id$
using System;
using System.Windows.Forms;
using System.Collections;
using Ankh.RepositoryExplorer;
using Ankh.UI;
using Utils;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for MakeDirectory.
    /// </summary>
    [VSNetCommand("MakeDirectoryCommand", 
         Tooltip="Create new directory here", Text = "New directory...",
         Bitmap = ResourceBitmaps.MakeDirectory ),
    VSNetControl( "ReposExplorer", Position = 1 ) ]
    public class MakeDirectoryCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            // we only want directories
            if ( context.RepositoryExplorer.SelectedNode != null &&
                context.RepositoryExplorer.SelectedNode.IsDirectory )
            {
                return Enabled;
            }
            else
                return Disabled;
        }

        public override void Execute(AnkhContext context, string parameters)
        {
            context.RepositoryExplorer.MakeDir( new NewDirHandler(context) );           
        }

        /// <summary>
        /// This class handles the actual creation of the new dir.
        /// </summary>
        private class NewDirHandler : ProgressRunner, INewDirectoryHandler
        {
            public NewDirHandler( AnkhContext context ) : base(context)
            {}

            public bool MakeDir(IRepositoryTreeNode parent, string dirname)
            {
                // first show the log message dialog
                this.url = UriUtils.Combine( ((INode)parent).Url, dirname );
                IList list = this.Context.Client.ShowLogMessageDialog( 
                    new string[]{this.url}, true );
                if ( list == null || list.Count == 0 )
                    return false;

                this.Context.StartOperation( "Creating directory " + url );
                try
                {
                    this.Start( "Creating directory" );
                    this.Context.Client.CommitCompleted();
                    return !this.Cancelled;
                }
                catch( Exception ex )
                {
                    Error.Handle( ex );
                    return false;
                }
                finally
                {
                    this.Context.EndOperation();
                }
            }

            protected override void DoRun()
            {
                // create the dir.
                this.Context.Client.MakeDir( new string[]{this.url} );
            }


            private string url;
        }
    }
}



