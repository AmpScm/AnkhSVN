using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;
using EnvDTE;
using System.IO;
using System.Collections;
using NSvn.Core;
using NSvn.Common;

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
         Bitmap=ResourceBitmaps.Default ),
    VSNetControl( "Solution.Ankh", Position=1 ),
    VSNetControl( "File", Position=14 )]
    internal class AddSolutionToRepositoryCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            if ( context.AnkhLoadedForSolution || 
                !File.Exists(context.DTE.Solution.FullName))
            {
                return Disabled;
            }
            else
                return context.SolutionIsOpen ? Enabled : Disabled;
        }

        public override void Execute( AnkhContext context, string parameters)
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
                        MakeDirRunner makeDirRunner = new MakeDirRunner( url, 
                            dlg.LogMessage, context );
                        makeDirRunner.Start( "Creating directory" );
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
                CheckoutRunner checkoutRunner = new CheckoutRunner( context, 
                    solutionDir, Revision.Head, url );
                checkoutRunner.Start( "Checking out" );
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

                this.AddProjects( context.DTE.Solution.Projects, context, solutionDir );  
            }
            finally
            {
                context.EndOperation();
            }

            // now commit the added files
            this.paths = context.Client.ShowLogMessageDialog( this.paths, false );
            if ( this.paths == null )
            {
                // oops - after all this work, the user cancelled
                context.StartOperation( "Aborted - reverting" );
                try
                {
                    context.Client.Revert( new string[]{ solutionDir }, true );
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
                    new ProgressRunner( context, 
                        new ProgressRunnerCallback(this.DoCommit) ).Start( "Committing" ); 
                    context.Client.CommitCompleted();
                }
                finally
                {
                    context.EndOperation();
                }
                        
                // we want ankh to get enabled right away
                context.DTE.ExecuteCommand( "Ankh.ToggleAnkh", "" );
                    
            }
        }

        /// <summary>
        /// Adds projects.
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="context"></param>
        private void AddProjects( Projects projects, AnkhContext context, string solutionDir )
        {
            foreach( Project project in projects )
            {
                // project.FileName throws an ArgumentException if the project 
                // hasn't been loaded.
                string filename;
                try
                {
                    filename = this.GetProjectFileName( project, 
                        context.DTE.Solution );
                }
                catch( ArgumentException )
                {
                    context.OutputPane.WriteLine( "Unable to add project" );
                    continue;
                }

                string dir = Path.GetDirectoryName( filename );
                try
                {
                    // for now we only support projects that are under the solution root
                    if ( !PathUtils.IsSubPathOf( dir, solutionDir ) )
                    {
                        context.OutputPane.WriteLine( 
                            dir + ": AnkhSVN does not currently support automatically " + 
                            "importing projects " +  
                            "that are not under the solution root directory." );
                        continue;
                    }

                    if ( Directory.Exists( dir ) && 
                        !SvnUtils.IsWorkingCopyPath( dir ) )
                    {
                        this.Add( dir, solutionDir, context );
                    }

                    if ( File.Exists( filename ) )
                    {
                        this.Add( filename, solutionDir, context );
                    }

                    // ProjectItems can be null for some project types
                    if ( project.ProjectItems != null )
                        this.AddProjectItems( project.ProjectItems, context, solutionDir );
                }
                catch( SvnClientException ex )
                {
                    context.OutputPane.WriteLine( ex.Message );
                }
            }
        }

        /// <summary>
        /// Adds the project items belonging to a project.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="context"></param>
        private void AddProjectItems( ProjectItems items, AnkhContext context, 
            string solutionDir )
        {
            foreach( ProjectItem item in items )
            {
                for( short i = 1; i <= item.FileCount; i++ )
                {
                    string file;
                    try
                    {
                        file = item.get_FileNames(i);
                    }
                    catch( NullReferenceException )
                    {
                        context.OutputPane.WriteLine( "Unable to add file" );
                        continue;
                    }
                    try
                    {
                        if ( File.Exists( file ) || Directory.Exists( file ) )
                        {
                            // for now we only support files that are under the solution root
                            if ( !PathUtils.IsSubPathOf( file, solutionDir ) )
                            {
                                context.OutputPane.WriteLine( 
                                    file + ": AnkhSVN does not currently support automatically " + 
                                    "importing files " +  
                                    "that are not under the solution root directory." );
                                continue;
                            }

                            this.Add( item.get_FileNames(i), solutionDir, context );
                        }   
                    }
                    catch( SvnClientException ex )
                    {
                        context.OutputPane.WriteLine( ex.Message );
                    }
                }
                try
                {
                    // add any subitems
                    if ( item.ProjectItems != null )
                        this.AddProjectItems( item.ProjectItems, context, solutionDir );
                }
                catch( NullReferenceException )
                {
                    context.OutputPane.WriteLine( "Unable to add subitems" );
                }
                catch( InvalidCastException )
                {
                    context.OutputPane.WriteLine( "Unable to add subitems" );
                }
            }

            
        }

        private void Add( string filename, string solutionDir, AnkhContext context )
        {
            if ( !SvnUtils.IsWorkingCopyPath( filename ) )
                this.AddWithIntermediateDirectories( filename, solutionDir, context );
            else
            {                
                context.Client.Add( filename, false );            
                this.paths.Add( filename );
            }
        }

        private void AddWithIntermediateDirectories( string filename, string solutionDir, 
            AnkhContext context )
        {
            // we know here that filename is a subpath of solutionDir
            string path;
            if ( solutionDir.EndsWith( "\\" ) )
                path = solutionDir;
            else 
                path = solutionDir + "\\";

            string[] intermediate = filename.Substring( path.Length, 
                filename.Length - path.Length ).Split( '\\' );
            
            foreach( string dirname in intermediate )
            {
                path = Path.Combine( path, dirname );
                if ( !SvnUtils.IsWorkingCopyPath( path ) || File.Exists( path ) )
                {
                    context.Client.Add( path, false );
                    this.paths.Add( path );
                }                    
            }
        }


        /// <summary>
        ///  Returns the project file for a given project.
        ///  This method is required because project.FileName might not always
        ///  return a full path.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="solution"></param>
        /// <returns></returns>
        private string GetProjectFileName( Project project, EnvDTE.Solution solution )
        {
            if ( File.Exists( project.FileName ) )
                return project.FileName;
            else
            {
                string solutionDir = Path.GetDirectoryName( solution.FullName );
                return Path.Combine( solutionDir, project.UniqueName );
            }
        }

        private void DoCommit( AnkhContext context )
        {
            string[] paths = (string[])(new ArrayList(this.paths).ToArray(typeof(string)));
            context.Client.Commit( paths, true );
        }

        /// <summary>
        /// A progress runner for creating repository directories.
        /// </summary>
        private class MakeDirRunner : ProgressRunner
        {
            public MakeDirRunner( string url, string logMessage, AnkhContext context ) : 
                base( context )
            {
                this.url = url;
                this.logMessage = logMessage;
                context.Client.LogMessage += 
                    new NSvn.Core.LogMessageDelegate(this.LogMessage);
            }

            protected override void DoRun()
            {
                this.Context.Client.MakeDir( new string[]{ this.url } );
            }

            private void LogMessage(object sender, NSvn.Core.LogMessageEventArgs args)
            {
                args.Message = this.logMessage;
            }

            private string url;
            private string logMessage;            
        }

        private IList paths;
    }
}
