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
            try
            {
                Assembly asm = Assembly.Load( 
                    "Microsoft.VisualStudio.VCProjectEngine");
                this.vcFilterType = asm.GetType(
                    "Microsoft.VisualStudio.VCProjectEngine.VCFilter", true );
            }
            catch( FileNotFoundException )
            {
                // appears the user doesn't have VC++ installed
                // oh well, we can degrade gracefully
                this.vcFilterType = null;
            }                     
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

                this.AddProjects( context.DTE.Solution.Projects, context, solutionDir );  
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
        private void AddProjects( Projects projects, IContext context, string solutionDir )
        {
            for( int i = 1; i <= projects.Count; i++ )
            {
                Project project = projects.Item(i);
                AddProject(context, solutionDir, project);
            }
            
        }

        private void AddProject( IContext context, string solutionDir, Project project )
        {
            string filename;
            try
            {
                // treat soln items and misc items specially
                if ( this.IsSpecialProject(project) )
                {
                    if ( project.ProjectItems != null )
                        this.AddProjectItems(project.ProjectItems, context, solutionDir);
                    return;
                }
                else
                {
                    // project.FileName returns null if the filename can't be retrieved
                    filename = this.GetProjectFileName(project,
                        context.DTE.Solution);
                    if ( filename == null )
                    {
                        context.OutputPane.WriteLine("Unable to add project");
                        return;
                    }
                }
            }
            catch ( ArgumentException )
            {
                context.OutputPane.WriteLine("Unable to add project");
                return;
            }

            string dir = Path.GetDirectoryName(filename);
            try
            {
                // for now we only support projects that are under the solution root
                if ( !PathUtils.IsSubPathOf(dir, solutionDir) )
                {
                    context.OutputPane.WriteLine(
                        dir + ": AnkhSVN does not currently support automatically " +
                        "importing projects " +
                        "that are not under the solution root directory.");
                    return;
                }

                if ( Directory.Exists(dir) &&
                    !SvnUtils.IsWorkingCopyPath(dir) )
                {
                    this.Add(dir, solutionDir, context);
                }

                if ( File.Exists(filename) )
                {
                    this.Add(filename, solutionDir, context);
                }

                // ProjectItems can be null for some project types
                if ( project.ProjectItems != null )
                    this.AddProjectItems(project.ProjectItems, context, solutionDir);
            }
            catch ( SvnClientException ex )
            {
                context.OutputPane.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Adds the project items belonging to a project.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="context"></param>
        private void AddProjectItems( ProjectItems items, IContext context, 
            string solutionDir )
        {
            foreach( ProjectItem item in items )
            {
                // if it's a solution folder, item.Object will be the project
                if ( item.Object is Project )
                {
                    this.AddProject( context, solutionDir, item.Object as Project );
                    continue;
                }

                for( short i = 1; i <= item.FileCount; i++ )
                {
                    

                    string file;
                    try
                    {
                        file = item.get_FileNames(i);
                    }
                    catch( Exception )
                    {
                        context.OutputPane.WriteLine( "Unable to add file" );
                        continue;
                    }
                    try
                    {
                        if ( (File.Exists( file ) || Directory.Exists( file )) &&
                            ( this.vcFilterType != null && 
                             !this.vcFilterType.IsInstanceOfType(item.Object)) )
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

        private void Add( string filename, string solutionDir, IContext context )
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
            IContext context )
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
            // lots of things that can throw here.
            try
            {
                if ( File.Exists( project.FileName ) )
                    return project.FileName;
            }
            catch( Exception )
            {
                // swallow
            }
            try
            {
                if ( File.Exists( project.FullName ) )
                    return project.FullName;
            }
            catch( Exception )
            {
                // swallow
            }
            try
            {
                string solutionDir = Path.GetDirectoryName( solution.FullName );
                string filename = Path.Combine( solutionDir, project.UniqueName );
                if ( File.Exists( filename ) )
                    return filename;
                else 
                    return null;
            }
            catch( Exception )
            {
                return null;
            }            
        }

        

        private bool IsSpecialProject( Project project )
        {
            foreach( string guid in SpecialProjects )
            {
                if ( String.Compare( project.Kind, guid, true ) == 0 )
                {
                    return true;
                }
            }
            return false;
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

        private static readonly string[] SpecialProjects = new String[]{
            DteUtils.SolutionItemsKind, // soln items
            DteUtils.MiscItemsKind, // misc items
			DteUtils.WebProjects2005Kind  // 2005 web project
                                                                       };

        private IList paths;
        private readonly Type vcFilterType;
    }
}
