using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;
using EnvDTE;
using System.IO;
using System.Collections;
using NSvn.Core;
using NSvn.Common;
using System.Reflection;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that allows the user to add a solution to a repository
    /// by checking out the repository directory to the solution directory, then
    /// recursively adding all solution items
    /// </summary>
    [VSNetCommand("AddSolutionToRepository", 
         Text="Add solution to Subversion repository...", 
         Tooltip= "Add this solution to Subversion repository", 
         Bitmap=ResourceBitmaps.AddSolutionToRepository ),
    VSNetControl( "Solution.Ankh", Position=1 ),
    VSNetControl( "File", Position=14 )]
    public class AddSolutionToRepositoryCommand : CommandBase
    {
        public AddSolutionToRepositoryCommand()
        {
                        
        }

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            if ( context.AnkhLoadedForSolution || 
                !File.Exists(context.DTE.Solution.FullName))
            {
                return Disabled;
            }
            else
                return context.SolutionIsOpen ? Enabled : Disabled;
        }

        public override void Execute( IContext context, string parameters)
        {
            this.SaveAllDirtyDocuments( context );

            string url;
            using( AddSolutionDialog dlg = new AddSolutionDialog() )
            {
                if ( dlg.ShowDialog( context.HostWindow ) != DialogResult.OK )
                    return;

                url = dlg.BaseUrl;

                // do we need to create a new repository directory?
                if ( dlg.CreateSubDirectory )
                {
                    context.StartOperation( "Creating repository directory" );
                    try
                    {
                        url = UriUtils.Combine( url, dlg.SubDirectoryName );
                        using( MakeDirWorker makeDirWorker = new MakeDirWorker( url, 
                                   dlg.LogMessage, context ) )
                        {
                            context.UIShell.RunWithProgressDialog( makeDirWorker, 
                                "Creating directory" ); 
                        }
                    }
                    finally
                    {
                        context.EndOperation();
                    }
                }
            }

            // now check out the repository directory into the solution dir
            context.StartOperation( "Checking out repository directory" );
            string solutionDir = Path.GetDirectoryName( 
                context.DTE.Solution.FullName );
            try
            {
                // check out the repository directory specified               
                CheckoutRunner checkoutRunner = new CheckoutRunner(  
                    solutionDir, Revision.Head, url );
                context.UIShell.RunWithProgressDialog( checkoutRunner, "Checking out" );
            }
            finally
            {
                context.EndOperation();
            }

            // walk the tree and add all the files
            context.StartOperation( "Adding files" );
            try
            {
                // the solution dir is already a wc                
                this.paths = new ArrayList();
                this.paths.Add( solutionDir );
                    
                context.Client.Add( context.DTE.Solution.FullName, false );
                this.paths.Add( context.DTE.Solution.FullName );
                this.AddProjects( context, solutionDir );  
            }
            catch( Exception )
            {
                // oops, bad stuff happened
                context.StartOperation( "Error: Reverting changes" );
                try
                {
                    context.Client.Revert( new string[]{ solutionDir }, true );
                    PathUtils.RecursiveDelete( 
                        Path.Combine(solutionDir, Client.AdminDirectoryName) );
                }
                finally
                {
                    context.EndOperation();
                }
                throw;                
            }
            finally
            {
                context.EndOperation();
            }

            // now commit the added files
            CommitOperation operation = new CommitOperation( new SimpleProgressWorker( 
                        new SimpleProgressWorkerCallback(this.DoCommit)), this.paths, context );

            if ( !operation.ShowLogMessageDialog( ) )
            {
                // oops - after all this work, the user cancelled
                context.StartOperation( "Aborted - reverting" );
                try
                {
                    context.Client.Revert( new string[]{ solutionDir }, true );
                    PathUtils.RecursiveDelete( 
                        Path.Combine(solutionDir, Client.AdminDirectoryName) );
                }
                finally
                {
                    context.EndOperation();
                }
            }
            else
            {  
                // go ahead with the commit
                context.StartOperation( "Committing added files" );
                try
                {
                    bool completed = operation.Run( "Committing" ); 

                    if ( !completed )
                        return;
                }
                finally
                {
                    context.EndOperation();
                }
                        
                // we want ankh to get enabled right away
                context.DTE.ExecuteCommand( "Ankh.ToggleAnkh", "" );

                // Make sure the URL typed gets remembered.
                RegistryUtils.CreateNewTypedUrl( url );
                    
            }
        }

        /// <summary>
        /// Adds projects.
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="context"></param>
        private void AddProjects( IContext context, string solutionDir )
        {
            foreach ( Project project in Enumerators.EnumerateProjects( context.DTE ) ) 
            {
                VSProject vsProject = VSProject.FromProject( project );
                this.paths.AddRange(vsProject.AddProjectToSvn( context, solutionDir ));
            }
            
        }

        

        private void DoCommit( IContext context )
        {
            string[] paths = (string[])(new ArrayList(this.paths).ToArray(typeof(string)));
            context.Client.Commit( paths, true );
        }

        /// <summary>
        /// A progress runner for creating repository directories.
        /// </summary>
        private class MakeDirWorker : IProgressWorker, IDisposable
        {
            public MakeDirWorker( string url, string logMessage, IContext context ) 
            {
                this.url = url;
                this.logMessage = logMessage;
                this.context = context;
                this.context.Client.LogMessage += 
                    new NSvn.Core.LogMessageDelegate(this.LogMessage);
            }

            ~MakeDirWorker()
            {
                this.Dispose(false);
            }

            public void Work( IContext context )
            {
                context.Client.MakeDir( new string[]{ this.url } );
            }

            public void Dispose()
            {
                this.Dispose(true);
            }

            private void Dispose( bool disposing )
            {
                this.context.Client.LogMessage -=
                    new NSvn.Core.LogMessageDelegate(this.LogMessage);
                if ( disposing )
                    GC.SuppressFinalize(this);
            }

            private void LogMessage(object sender, NSvn.Core.LogMessageEventArgs args)
            {
                args.Message = this.logMessage;
            }

            

            private IContext context;
            private string url;
            private string logMessage;            
            #region IDisposable Members

            
            #endregion
        }

       

        private ArrayList paths;
    }
}
