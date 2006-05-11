using System;
using System.Text;
using EnvDTE;
using Interop.esproj;
using System.IO;
using Utils;
using NSvn.Core;
using System.Collections;
using System.Reflection;

namespace Ankh
{
    public class VSProject
    {
        private VSProject( Project project )
        {
            this.project = project;
        }

        static VSProject()
        {
            LoadVCFilterType();
        }
        
        public IList AddProjectToSvn( IContext context, string solutionDir )
        {
            string filename;
            ArrayList paths = new ArrayList();
            try
            {
                // treat soln items and misc items specially
                if ( this.IsSpecialProject() )
                {
                    if ( this.project.ProjectItems != null )
                        this.AddProjectItems( this.project.ProjectItems, context, solutionDir, paths );
                    return new string[] { };
                }
                else
                {
                    // this.project.FileName returns null if the filename can't be retrieved
                    filename = this.GetProjectFileName( context.DTE.Solution );
                    if ( filename == null )
                    {
                        context.OutputPane.WriteLine( "Unable to add this.project. Cannot determine project file name." );
                        return new string[] { };
                    }
                }
            }
            catch ( ArgumentException )
            {
                context.OutputPane.WriteLine( "Unable to add this.project" );
                return new string[] { };
            }

            string dir = Path.GetDirectoryName( filename );
            try
            {
                // for now we only support this.projects that are under the solution root
                if ( !PathUtils.IsSubPathOf( dir, solutionDir ) )
                {
                    context.OutputPane.WriteLine(
                        dir + ": AnkhSVN does not currently support automatically " +
                        "importing this.projects " +
                        "that are not under the solution root directory." );
                    return new string[] { };
                }

                if ( Directory.Exists( dir ) &&
                    !SvnUtils.IsWorkingCopyPath( dir ) )
                {
                    this.Add( dir, solutionDir, context, paths );
                }

                if ( File.Exists( filename ) )
                {
                    this.Add( filename, solutionDir, context, paths );
                }

                // ProjectItems can be null for some this.project types
                if ( this.project.ProjectItems != null )
                    this.AddProjectItems( this.project.ProjectItems, context, solutionDir, paths );

            }
            catch ( SvnClientException ex )
            {
                context.OutputPane.WriteLine( ex.Message );

            }
            return paths;
        }

        /// <summary>
        /// Adds the this.project items belonging to a this.project.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="context"></param>
        private void AddProjectItems( ProjectItems items, IContext context,
            string solutionDir, IList paths )
        {
            foreach ( ProjectItem item in Enumerators.EnumerateProjectItems( items ) )
            {
                // if it's a solution folder, item.Object will be the project
                if ( item.Object is Project )
                {
                    VSProject vsProject = VSProject.FromProject(item.Object as Project);
                    continue;
                }

                for ( short i = 1; i <= item.FileCount; i++ )
                {
                    string file;
                    try
                    {
                        file = item.get_FileNames( i );
                    }
                    catch ( Exception )
                    {
                        context.OutputPane.WriteLine( "Unable to add file" );
                        continue;
                    }
                    try
                    {
                        if ( ( File.Exists( file ) || Directory.Exists( file ) ) &&
                            ( vcFilterType != null &&
                             !vcFilterType.IsInstanceOfType( item.Object ) ) )
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

                            this.Add( item.get_FileNames( i ), solutionDir, context, paths );
                        }
                    }
                    catch ( SvnClientException ex )
                    {
                        context.OutputPane.WriteLine( ex.Message );
                    }
                }
                try
                {
                    // add any subitems
                    if ( item.ProjectItems != null )
                        this.AddProjectItems( item.ProjectItems, context, solutionDir, paths );
                }
                catch ( NullReferenceException )
                {
                    context.OutputPane.WriteLine( "Unable to add subitems" );
                }
                catch ( InvalidCastException )
                {
                    context.OutputPane.WriteLine( "Unable to add subitems" );
                }
            }


        }

        private void Add( string filename, string solutionDir, IContext context, IList paths )
        {
            if ( !SvnUtils.IsWorkingCopyPath( filename ) )
                this.AddWithIntermediateDirectories( filename, solutionDir, context, paths );
            else
            {
                context.Client.Add( filename, false );
                paths.Add( filename );
            }
        }

        private void AddWithIntermediateDirectories( string filename, string solutionDir,
            IContext context, IList paths )
        {
            // we know here that filename is a subpath of solutionDir
            string path;
            if ( solutionDir.EndsWith( "\\" ) )
                path = solutionDir;
            else
                path = solutionDir + "\\";

            string[] intermediate = filename.Substring( path.Length,
                filename.Length - path.Length ).Split( '\\' );

            foreach ( string dirname in intermediate )
            {
                path = Path.Combine( path, dirname );
                if ( !SvnUtils.IsWorkingCopyPath( path ) || File.Exists( path ) )
                {
                    context.Client.Add( path, false );
                    paths.Add( path );
                }
            }
        }


        /// <summary>
        ///  Returns the this.project file for a given this.project.
        ///  This method is required because this.project.FileName might not always
        ///  return a full path.
        /// </summary>
        /// <param name="this.project"></param>
        /// <param name="solution"></param>
        /// <returns></returns>
        private string GetProjectFileName( EnvDTE.Solution solution )
        {
            // lots of things that can throw here.
            try
            {
                if ( File.Exists( this.project.FileName ) )
                    return this.project.FileName;
            }
            catch ( Exception )
            {
                // swallow
            }
            try
            {
                if ( File.Exists( this.project.FullName ) )
                    return this.project.FullName;
            }
            catch ( Exception )
            {
                // swallow
            }
            try
            {
                string solutionDir = Path.GetDirectoryName( solution.FullName );
                string filename = Path.Combine( solutionDir, this.project.UniqueName );
                if ( File.Exists( filename ) )
                    return filename;
                else
                    return null;
            }
            catch ( Exception )
            {
                return null;
            }
        }



        private bool IsSpecialProject()
        {
            foreach ( string guid in SpecialProjects )
            {
                if ( String.Compare( this.project.Kind, guid, true ) == 0 )
                {
                    return true;
                }
            }
            return false;
        }

        private static void LoadVCFilterType()
        {
            try
            {
                Assembly asm = Assembly.Load(
                    "Microsoft.VisualStudio.VCProjectEngine" );
                vcFilterType = asm.GetType(
                    "Microsoft.VisualStudio.VCProjectEngine.VCFilter", true );
            }
            catch ( FileNotFoundException )
            {
                // appears the user doesn't have VC++ installed
                // oh well, we can degrade gracefully
                vcFilterType = null;
            }        
        }


        public static VSProject FromProject( Project project )
        {
            return new VSProject( project );
        }

        public static VSProject FromVsHierarchy( IVsHierarchy hierarchy )
        {
            throw new NotImplementedException();
        }

        private static readonly string[] SpecialProjects = new String[]{
            DteUtils.SolutionItemsKind, // soln items
            DteUtils.MiscItemsKind, // misc items
            DteUtils.WebProjects2005Kind  // 2005 web this.project
                                                                       };

        private Project project;
        private static Type vcFilterType;
    }
}
