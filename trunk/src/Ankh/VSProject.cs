using System;
using System.Text;
using EnvDTE;
using System.IO;
using Utils;
using NSvn.Core;
using NSvn.Common;
using System.Collections;
using System.Reflection;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;

namespace Ankh
{
    public class VSProject
    {
        private VSProject( IContext context, Project project )
        {
            this.context = context;
            this.project = project;
        }

        private VSProject( IContext context, Project project, IVsHierarchy hierarchy )
            : this( context, project )
        {
            this.vsHierarchy = hierarchy;
        }

        static VSProject()
        {
            LoadVCFilterType();
        }

        public bool IsUnderSolutionRoot
        {
            get
            {
                string solutionDir = Path.GetDirectoryName(
                    this.project.DTE.Solution.FullName );
                string projectFileName = this.GetProjectFileName();

                // Can't find the project file, guess it's not anywhere
                if ( projectFileName == null )
                {
                    return false;
                }

                return PathUtils.IsSubPathOf(projectFileName, solutionDir);
            }
        }

        public bool IsSolutionFolder        
        {
            get 
            {
                return String.Compare( this.project.Kind, DteUtils.SolutionFolderKind, true ) == 0;
            }
        }


        public string Name
        {
            get { return this.project.Name; }
        }

        /// <summary>
        /// The project file name.
        /// </summary>
        public string ProjectFileName
        {
            get { return this.GetProjectFileName(); }
        }

        /// <summary>
        /// The project directory.
        /// </summary>
        public string ProjectDirectory
        {
            get
            {
                string filename = this.ProjectFileName;
                if ( filename != null )
                {
                    return Path.GetDirectoryName( filename );
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// An SvnItem for the projectfile.
        /// </summary>
        public SvnItem ProjectFileSvnItem
        {
            get
            {
                if ( this.projectFileSvnItem == null )
                {
                    this.projectFileSvnItem = this.context.StatusCache[ this.ProjectFileName ];
                }
                return this.projectFileSvnItem;
            }
        }

        /// <summary>
        /// An SvnItem for the project directory.
        /// </summary>
        public SvnItem ProjectDirectorySvnItem
        {
            get
            {
                if ( this.projectDirectorySvnItem == null )
                {
                    this.projectDirectorySvnItem = this.context.StatusCache[ this.ProjectDirectory ];
                }
                return this.projectDirectorySvnItem;
            }
        }

        /// <summary>
        /// Whether this project is already under version control.
        /// </summary>
        public bool IsVersioned
        {
            get 
            {
                SvnItem projectFileItem = this.ProjectFileSvnItem;
                projectFileSvnItem.Refresh( this.context.Client );
                return projectFileSvnItem.IsVersioned;
            }
        }

        /// <summary>
        /// Whether this project can be versioned.
        /// </summary>
        public bool IsVersionable
        {
            get 
            {
                SvnItem projectDirectoryItem = this.ProjectDirectorySvnItem;
                projectDirectoryItem.Refresh( this.context.Client );
                return projectDirectoryItem.IsVersionable;
            }
        }



        public IList AddProjectToSvn()
        {
            string filename;
            ArrayList paths = new ArrayList();
            
            string solutionDir = Path.GetDirectoryName(
                    this.context.DTE.Solution.FullName );

            try
            {
                // treat soln items and misc items specially
                if ( this.IsSpecialProject() )
                {
                    if ( this.project.ProjectItems != null )
                        this.AddProjectItems( this.project.ProjectItems, solutionDir, paths );
                    return new string[] { };
                }
                else
                {
                    // this.project.FileName returns null if the filename can't be retrieved
                    filename = this.GetProjectFileName();
                    if ( filename == null )
                    {
                        this.context.OutputPane.WriteLine( "Unable to add this.project. Cannot determine project file name." );
                        return new string[] { };
                    }
                }
            }
            catch ( ArgumentException )
            {
                this.context.OutputPane.WriteLine( "Unable to add this.project" );
                return new string[] { };
            }

            string dir = Path.GetDirectoryName( filename );
            try
            {
                if ( Directory.Exists( dir ) &&
                    !SvnUtils.IsWorkingCopyPath( dir ) )
                {
                    this.Add( dir, solutionDir, paths );
                }

                if ( File.Exists( filename ) )
                {
                    this.Add( filename, solutionDir, paths );
                }

                // ProjectItems can be null for some this.project types
                if ( this.project.ProjectItems != null )
                    this.AddProjectItems( this.project.ProjectItems, solutionDir, paths );

            }
            catch ( SvnClientException ex )
            {
                this.context.OutputPane.WriteLine( ex.Message );

            }
            return paths;
        }

        /// <summary>
        /// Retrieve the subprojects of this project (usually a solution folder).
        /// </summary>
        /// <param name="includeSolutionFolders"></param>
        /// <returns></returns>
        public VSProject[] GetSubProjects(bool includeSolutionFolders)
        {
            ArrayList subProjects = new ArrayList();
            this.DoGetSubProjects( this.project.ProjectItems, subProjects, includeSolutionFolders );

            return (VSProject[])subProjects.ToArray( typeof( VSProject ) );
        }

        private void DoGetSubProjects( ProjectItems projectItems, ArrayList subProjects, bool includeSolutionFolders )
        {
            if ( projectItems == null )
            {
                return;
            }

            foreach ( ProjectItem item in Enumerators.EnumerateProjectItems(projectItems) )
            {
                if ( item.Object is Project )
                {
                    VSProject project = VSProject.FromProject( this.context, item.Object as Project );

                    if ( includeSolutionFolders || !project.IsSolutionFolder )
                    {
                        subProjects.Add( project );
                    }

                    this.DoGetSubProjects( project.project.ProjectItems, subProjects, includeSolutionFolders );
                }
            }
        }

        /// <summary>
        /// Adds the this.project items belonging to a this.project.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="context"></param>
        private void AddProjectItems( ProjectItems items,
            string solutionDir, IList paths )
        {
            foreach ( ProjectItem item in Enumerators.EnumerateProjectItems( items ) )
            {
                // if it's a solution folder, item.Object will be the project
                if ( item.Object is Project )
                {
                    VSProject vsProject = VSProject.FromProject(this.context, item.Object as Project);
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
                        this.context.OutputPane.WriteLine( "Unable to add file" );
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
                                this.context.OutputPane.WriteLine(
                                    file + ": AnkhSVN does not currently support automatically " +
                                    "importing files " +
                                    "that are not under the solution root directory." );
                                continue;
                            }

                            this.Add( item.get_FileNames( i ), solutionDir, paths );
                        }
                    }
                    catch ( SvnClientException ex )
                    {
                        this.context.OutputPane.WriteLine( ex.Message );
                    }
                }
                try
                {
                    // add any subitems
                    if ( item.ProjectItems != null )
                        this.AddProjectItems( item.ProjectItems, solutionDir, paths );
                }
                catch ( NullReferenceException )
                {
                    this.context.OutputPane.WriteLine( "Unable to add subitems" );
                }
                catch ( InvalidCastException )
                {
                    this.context.OutputPane.WriteLine( "Unable to add subitems" );
                }
            }


        }

        private void Add( string filename, string solutionDir, IList paths )
        {
            if ( !SvnUtils.IsWorkingCopyPath( filename ) )
                this.AddWithIntermediateDirectories( filename, solutionDir, paths );
            else
            {
                this.context.Client.Add( filename, Recurse.None );
                paths.Add( filename );
            }
        }

        private void AddWithIntermediateDirectories( string filename, string solutionDir,
            IList paths )
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
                    this.context.Client.Add( path, Recurse.None );
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
        private string GetProjectFileName( )
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

                string solutionDir = Path.GetDirectoryName( this.project.DTE.Solution.FullName );
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


        public static VSProject FromProject( IContext context, Project project )
        {
            return new VSProject( context, project );
        }

        public static VSProject FromVsHierarchy( IContext context, IVsHierarchy hierarchy )
        {
            const uint root = unchecked( (uint)(int)VSITEMID.VSITEMID_ROOT );

            object projVar;
            Project project;
            int hr = hierarchy.GetProperty( root, (int)__VSHPROPID.VSHPROPID_ExtObject, out projVar );
            if ( hr == VSConstants.S_OK && projVar is EnvDTE.Project )
            {
                project = (Project)projVar;
                return new VSProject( context, project, hierarchy );
            }
            else
            {

                throw new NoProjectAutomationObjectException( GetProjectNameFromVsHierarchy(hierarchy));
            }
        }

        public static string GetProjectNameFromVsHierarchy( IVsHierarchy hierarchy)
        {
            object nameVar;
            const uint root = unchecked( (uint)(int)VSITEMID.VSITEMID_ROOT );
            int hr = hierarchy.GetProperty( root, (int)__VSHPROPID.VSHPROPID_Name, out nameVar );
            string name = hr == VSConstants.S_OK ? nameVar as string : string.Empty;
            return name;
        }

        private static readonly string[] SpecialProjects = new String[]{
            DteUtils.SolutionItemsKind, // soln items
            DteUtils.MiscItemsKind, // misc items
            DteUtils.WebProjects2005Kind  // 2005 web this.project
                                                                       };

        private Project project;
        private static Type vcFilterType;
        private IContext context;
        private SvnItem projectFileSvnItem;
        private SvnItem projectDirectorySvnItem;
        private IVsHierarchy vsHierarchy;

    }
}
