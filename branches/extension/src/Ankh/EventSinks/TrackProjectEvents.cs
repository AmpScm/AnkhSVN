using System.Text;

using EnvDTE;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using NSvn.Core;
using System.Windows.Forms;
using Utils;
using System.Collections.Specialized;
using System.Collections;

//using Microsoft.VisualStudio.Shell.Interop;
//using Microsoft.VisualStudio;
//using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


namespace Ankh.EventSinks
{
    /// <summary>
    /// Provides an implementation of the Visual Studio IVsTrackProjectDocumentsEvents2 interface, 
    /// which can be used to listen for Add, Remove and Rename operations.
    /// </summary>
    class TrackProjectDocuments : IVsTrackProjectDocumentsEvents2
    {

        public TrackProjectDocuments( IContext context )
        {
            this.context = context;
            this.projects = new Hashtable();
            AdviseEvents();
        }

        

        /// <summary>
        /// Create a mapping between a file path and an automation Project object, for refresh purposes.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="project"></param>
        public void MapFileToProject( string path, Project project )
        {
            this.projects.Add( PathUtils.NormalizePath(path), project );
        }



        #region IVsTrackProjectDocumentsEvents2 Members

        public int OnAfterAddDirectoriesEx( int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterAddDirectoriesEx() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterAddFilesEx( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags )
        {
            try
            {
                if ( this.context.Config.AutoAddNewFiles && !this.context.OperationRunning )
                {
                    foreach ( string file in rgpszMkDocuments )
                    {
                        try
                        {
                            SvnItem svnItem = this.context.StatusCache[file];

                            // make sure we have up to date info on this item.
                            svnItem.Refresh( this.context.Client );

                            if ( !svnItem.IsVersioned && svnItem.IsVersionable &&
                                !this.context.Client.IsIgnored( svnItem.Path ) )
                            {
                                this.context.Client.Add( file, false );
                                this.Refresh( file );
                            }
                        }
                        catch ( SvnClientException ex )
                        {
                            // don't propagate this exception
                            // just tell the user and move on
                            this.context.ErrorHandler.Write( "Unable to add file: ", ex,
                                this.context.OutputPane );
                        }
                    }

                    
                }
                Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterAddFilesEx() of: {0}", this.ToString() ) );
                return VSConstants.S_OK;
            }
            catch ( Exception ex )
            {
                this.context.ErrorHandler.Handle( ex );
                return VSConstants.E_FAIL;
            }
        }

        private void Refresh( string path )
        {
            string canonicalPath = PathUtils.NormalizePath( path );
            Project project = (Project)this.projects[canonicalPath];

            // We won't have a Project for VC++ projects, however RefreshSelection works fine 
            // there (but not for managed projects :-( )
            if ( project != null )
            {
                this.context.SolutionExplorer.Refresh( project );
            }
            else
            {
                this.context.SolutionExplorer.RefreshSelection();
            }
        }

        public int OnAfterRemoveDirectories( int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRemoveDirectories() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRemoveFiles() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories( int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRenameDirectories() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRenameFiles() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnAfterSccStatusChanged( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterSccStatusChanged() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryAddDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryAddDirectories() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryAddFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryAddFiles() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveDirectories( IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRemoveDirectories() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRemoveFiles() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories( IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRenameDirectories() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }

        public int OnQueryRenameFiles( IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRenameFiles() of: {0}", this.ToString() ) );
            return VSConstants.S_OK;
        }
        #endregion

        /// <summary>
        /// Hook us up as a listener to the VS project documents events
        /// </summary>
        private void AdviseEvents()
        {
            // The DTE object is also a service provider, the starting point for retrieving any kind of service in the VS env
            IServiceProvider sp = (IServiceProvider)this.context.DTE;
            IntPtr svcPtr;

            // This is the service and interface we want
            Guid serviceGuid = typeof( SVsTrackProjectDocuments ).GUID;
            Guid interfaceGuid = typeof( IVsTrackProjectDocuments2 ).GUID;

            // Get an interface and convert it to a C# reference
            sp.QueryService( ref serviceGuid, ref interfaceGuid, out svcPtr );
            this.trackProjectDocuments = (IVsTrackProjectDocuments2)Marshal.GetObjectForIUnknown( svcPtr );

            // ATPDE will return S_OK while not setting the cookie if the interop definition of the IVTPD interface is wrong
            if ( this.trackProjectDocuments.AdviseTrackProjectDocumentsEvents( this, out this.cookie ) != VSConstants.S_OK || this.cookie == 0 )
            {
                throw new Exception( "Couldn't register project documents events interface" );
            };
        }


        private IContext context;
        private uint cookie;
        private Hashtable projects;

        private IVsTrackProjectDocuments2 trackProjectDocuments;


    }

}
