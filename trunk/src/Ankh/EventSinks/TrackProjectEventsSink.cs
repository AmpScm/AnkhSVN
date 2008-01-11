using System.Text;

using EnvDTE;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using NSvn.Core;
using NSvn.Common;
using System.Windows.Forms;
using Utils;
using System.Collections.Specialized;
using System.Collections;
using Microsoft.VisualStudio.Shell.Interop;

using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Provides an implementation of the Visual Studio IVsTrackProjectDocumentsEvents2 interface, 
    /// which can be used to listen for Add, Remove and Rename operations.
    /// </summary>
    class TrackProjectDocumentsEventSink : EventSink, IVsTrackProjectDocumentsEvents2
    {

        public TrackProjectDocumentsEventSink( IContext context ) : base (context)
        {
            this.context = context;
            AdviseEvents();
        }

        public override void Unhook()
        {
            this.UnadviseEvents();
        }




        #region IVsTrackProjectDocumentsEvents2 Members

        public int OnAfterAddDirectoriesEx( int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterAddDirectoriesEx() of: {0}", this.ToString() ) );
            return this.OnAfterAdd( rgpszMkDocuments );
        }

        public int OnAfterAddFilesEx( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterAddFilesEx() of {0}", this.ToString() ) );
            return this.OnAfterAdd( rgpszMkDocuments );            
        }

        public int OnAfterRemoveDirectories( int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRemoveDirectories() of: {0}", this.ToString() ) );
            this.OnRemove( rgpszMkDocuments );
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRemoveFiles() of: {0}", this.ToString() ) );
            this.OnRemove( rgpszMkDocuments );
            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories( int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRenameDirectories() of: {0}", this.ToString() ) );
            return OnAfterRename( rgszMkOldNames, rgszMkNewNames, true );
        }

        public int OnAfterRenameFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRenameFiles() of: {0}", this.ToString() ) );
            return OnAfterRename( rgszMkOldNames, rgszMkNewNames, false );
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

            // OnQueryRemove expects this to be non-null.
            if ( rgResults == null )
            {
                rgResults = new VSQUERYREMOVEDIRECTORYRESULTS[ rgpszMkDocuments.Length ];
            }
            return OnQueryRemove( rgpszMkDocuments, pSummaryResult, rgResults );
        }

        private int OnQueryRemove( string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults )
        {
            try
            {
                int i = 0;
                pSummaryResult[ 0 ] = VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveOK;
                foreach ( string path in rgpszMkDocuments )
                {
                    SvnItem item = this.Context.StatusCache[ path ];
                    if ( item.IsVersioned )
                    {
                        if ( this.CanSvnDelete( item ) )
                        {
                            if ( item.IsDirectory )
                            {
                                this.BackupDirectory( path );
                            }

                            rgResults[ i ] = VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveOK;
                        }
                        else
                        {
                            rgResults[ i ] = VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveNotOK; 
                            pSummaryResult[ 0 ] = VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveNotOK;
                        }
                    }
                    else
                    {
                        rgResults[ i ] = VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveOK;
                    }
                }

                return VSConstants.S_OK;
            }
            catch ( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
                return VSConstants.E_FAIL;
            }
        }

        private void BackupDirectory( string directory )
        {
            string backupDirectory = GetBackupDirectoryName(directory);
            FileUtils.CopyDirectory( directory, backupDirectory );
        }

        private void RestoreBackupDirectory( string directory )
        {
            try
            {
                string backupDirectory = GetBackupDirectoryName( directory );
                // OnRemove can be called without OnQueryRemove*
                // in that case backupDirectory doesn't exist
                if ( Directory.Exists( backupDirectory ) ) 
                {
                    Directory.Move( backupDirectory, directory );
                }
            }
            finally
            {
                this.CreateNewBackupSuffix();
            }
        }

        private void CreateNewBackupSuffix()
        {
            this.BackupSuffix = Guid.NewGuid().ToString();
        }

        private string GetBackupDirectoryName( string directory )
        {
            string parentDir = PathUtils.GetParent( directory );
            string dirName = PathUtils.GetName( directory );

            return Path.Combine( parentDir, dirName + BackupSuffix );
        }


        public int OnQueryRemoveFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults )
        {
            VSQUERYREMOVEDIRECTORYRESULTS[] results = new VSQUERYREMOVEDIRECTORYRESULTS[ rgpszMkDocuments.Length ];

            VSQUERYREMOVEDIRECTORYRESULTS[] summaryResults = new VSQUERYREMOVEDIRECTORYRESULTS[ 1 ];

            int hr = this.OnQueryRemove( rgpszMkDocuments, results, summaryResults );

            // we need to convert to the appropriate enum (even if they have exactly the same flags...)
            if ( rgResults != null )
            {
                ConvertRemoveFileResults( results, rgResults ); 
            }
            ConvertRemoveFileResults( summaryResults, pSummaryResult );

            return hr;
        }

        private static void ConvertRemoveFileResults( VSQUERYREMOVEDIRECTORYRESULTS[] results, VSQUERYREMOVEFILERESULTS[] rgResults )
        {
            for ( int i = 0; i < results.Length; i++ )
            {
                rgResults[ i ] = results[ i ] == VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveNotOK ?
                    VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveNotOK :
                    VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK;
            }
        }

        public int OnQueryRenameDirectories( IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRenameDirectories() of: {0}", this.ToString() ) );

            VSQUERYRENAMEFILERESULTS[] fileResults = new VSQUERYRENAMEFILERESULTS[1];

            int hr = OnQueryRename( rgszMkOldNames, rgszMkNewNames, fileResults );

            // Convert VSQUERYRENAMEFILERESULTS to VSQUERYRENAMEDIRECTORYRESULTS.
            pSummaryResult[0] = (fileResults[0] == VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameOK) ? 
                VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameOK : 
                VSQUERYRENAMEDIRECTORYRESULTS.VSQUERYRENAMEDIRECTORYRESULTS_RenameNotOK;
            return hr;
        }

        public int OnQueryRenameFiles( IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRenameFiles() of: {0}", this.ToString() ) );
            return OnQueryRename( rgszMkOldNames, rgszMkNewNames, pSummaryResult );
        }

       
        #endregion

        /// <summary>
        /// Perform the actual rename here, for both files and directories.
        /// </summary>
        /// <param name="rgszMkOldNames"></param>
        /// <param name="rgszMkNewNames"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private int OnAfterRename( string[] rgszMkOldNames, string[] rgszMkNewNames, bool directory )
        {
            try
            {
                for ( int i = 0; i < rgszMkOldNames.Length; i++ )
                {
                    SvnItem item = this.context.StatusCache[rgszMkOldNames[i]];
                    item.Refresh( this.context.Client );
                    if ( item.IsVersioned || item.Status.TextStatus == StatusKind.Missing )
                    {
                        Trace.WriteLine( String.Format( "Renaming {0} to {1}", rgszMkOldNames[i], rgszMkNewNames[i] ) );
                        // We need to rename back first for svn move to work
                        if ( Directory.Exists( rgszMkNewNames[i] ) )
                        {
                            Directory.Move( rgszMkNewNames[i], rgszMkOldNames[i ]);
                        }
                        else
                        {
                            File.Move( rgszMkNewNames[i], rgszMkOldNames[i] );
                        }
                        this.context.Client.Move( rgszMkOldNames[i], rgszMkNewNames[i], true );
                    }
                    else
                    {
                        Trace.WriteLine( String.Format( "Not renaming {0} to {1}. TextStatus is {2}", 
                            rgszMkOldNames[i], rgszMkNewNames[i], item.Status.TextStatus ) );
                    }
                }
                return VSConstants.S_OK;
            }
            catch ( Exception ex )
            {
                this.context.ErrorHandler.Handle( ex );
                return VSConstants.E_FAIL;
            }
        }        

        /// <summary>
        /// Perform the actual Add here, for both files and directories.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        private int OnAfterAdd( string[] paths )
        {
            try
            {
                if ( this.context.Config.AutoAddNewFiles && !this.context.OperationRunning )
                {
                    foreach ( string file in paths )
                    {
                        try
                        {
                            SvnItem svnItem = this.context.StatusCache[file];

                            // make sure we have up to date info on this item.
                            svnItem.Refresh( this.context.Client );

                            if ( !svnItem.IsVersioned && svnItem.IsVersionable &&
                                !this.context.Client.IsIgnored( svnItem.Path ) )
                            {
                                this.context.Client.Add( file, Recurse.None );
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
                return VSConstants.S_OK;
            }
            catch ( Exception ex )
            {
                this.context.ErrorHandler.Handle( ex );
                return VSConstants.E_FAIL;
            }
        }

        /// <summary>
        /// Verify renames for both directories and files.
        /// </summary>
        /// <param name="rgszMkOldNames"></param>
        /// <param name="rgszMkNewNames"></param>
        /// <param name="pSummaryResult"></param>
        /// <returns></returns>
        private int OnQueryRename( string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILERESULTS[] pSummaryResult )
        {
            try
            {
                pSummaryResult[0] = VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameOK;

                for ( int i = 0; i < rgszMkOldNames.Length; i++ )
                {
                    SvnItem item = context.StatusCache[rgszMkOldNames[i]];
                    item.Refresh( this.Context.Client, EventBehavior.DontRaise );

                    if ( !IsUnmodifiedOrUnversioned( item ) )
                    {
                        pSummaryResult[0] = VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameNotOK;
                        this.context.OutputPane.WriteLine( "SVN prohibits renaming modified items. " + 
                            "Please commit the modifications before attempting a rename." );
                    }
                    else if ( IsCaseOnlySvnRename( item, rgszMkNewNames[i] ) )
                    {
                        pSummaryResult[0] = VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameNotOK;
                        this.Context.OutputPane.WriteLine( "Cannot rename {0} to {1}. SVN does not allow case only renames.",
                            rgszMkOldNames[i], rgszMkNewNames[i] );
                    }
                }

                return VSConstants.S_OK;
            }
            catch ( Exception ex )
            {
                context.ErrorHandler.Handle( ex );
                return VSConstants.E_FAIL;
            }
        }

        /// <summary>
        /// Perform the actual remove here (called by the OnAfterRemove methods). 
        /// </summary>
        /// <param name="rgpszMkDocuments"></param>
        /// <param name="rgResults"></param>
        /// <param name="pSummaryResult"></param>
        /// <returns></returns>
        private int OnRemove( string[] rgpszMkDocuments )
        {
            try
            {
                for( int i = 0; i < rgpszMkDocuments.Length; i++ )
                {
                    SvnItem item = context.StatusCache[ rgpszMkDocuments[i] ];
                    item.Refresh(this.context.Client, EventBehavior.DontRaise );

                    // VC++ projects don't delete the files until *after* OnAfterRemoveFiles has fired
                    bool wasUnmodified = item.Status.TextStatus == StatusKind.Normal &&
                        ( item.Status.PropertyStatus == StatusKind.None || item.Status.PropertyStatus == StatusKind.Normal );

                    // SVN since 1.4 gives an error if deleting an added-but-missing file.
                    bool wasAddedOrMissing = item.Status.TextStatus == StatusKind.Added || item.Status.TextStatus == StatusKind.Missing;

                    if ( (item.Status.TextStatus == StatusKind.Missing || item.IsDeleted ||
                        item.Status.TextStatus == StatusKind.Normal) && 
                        this.PromptDelete( item ) )
                    {
                        if ( item.IsDirectory )
                        {
                            this.RestoreBackupDirectory( item.Path );
                        }

                        try
                        {
                            context.Client.Delete( new string[] { item.Path }, true );
                        }
                        catch ( BadPathException )
                        {
                            // see comment above.
                            if ( !wasAddedOrMissing )
                            {
                                throw;
                            }
                        }

                        // VC++ gets annoyed if the file doesn't exist when trying to delete it.
                        if ( wasUnmodified && !item.IsDirectory )
                        {
                            File.Create( rgpszMkDocuments[ i ] ).Close();
                        }

                        item.Refresh( context.Client, EventBehavior.Raise );
                    }
                }
                return VSConstants.S_OK;
            }
            catch( Exception ex )
            {
                this.context.ErrorHandler.Handle( ex );
                return VSConstants.E_FAIL;
            }
        }

        private bool PromptDelete( SvnItem item )
        {
            String message = String.Format("Do you want to delete {0} from your Subversion working copy?", 
                item.Path);
            return this.context.UIShell.ShowMessageBox( message, "Delete from working copy", MessageBoxButtons.YesNo ) ==
                DialogResult.Yes;
        }

        private bool IsUnmodifiedOrUnversioned( SvnItem item )
        {
            return (
                ( item.IsVersioned && !item.IsModified ) ||
                ( !item.IsVersioned )
            );
        }

        private bool IsCaseOnlySvnRename( SvnItem item, string newPath )
        {
            if ( !item.IsVersioned )
                return false;

            string oldPath = PathUtils.NormalizePath( item.Path );
            newPath = PathUtils.NormalizePath( newPath );

            return oldPath == newPath;
        }

        private bool CanDeletePath(string path)
        {
            SvnItem item = this.context.StatusCache[path];
            return !item.IsVersioned || CanSvnDelete(item);
        }

        /// <summary>
        /// Is it possible to delete this item?
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool CanSvnDelete( SvnItem item )
        {
            if ( item.IsVersioned && !item.IsDeleted )
            {
                if ( item.IsModified )
                {
                    DialogResult dr = this.context.UIShell.ShowMessageBox( 
                        item.Path + " has local modifications. Delete anyway?",
                        "Local modifications detected", MessageBoxButtons.YesNo );

                    return dr == DialogResult.Yes;
                }
                else
                    return true;
            }
            else 
                return false;
        }

        /// <summary>
        /// Hook us up as a listener to the VS project documents events
        /// </summary>
        private void AdviseEvents()
        {
            // The DTE object is also a service provider, the starting point for retrieving any kind of service in the VS env
            IServiceProvider sp = this.context.ServiceProvider;
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


            //serviceGuid = typeof( SVsSolution ).GUID;
            //interfaceGuid = typeof( IVsSolution ).GUID;

            //sp.QueryService( ref serviceGuid, ref interfaceGuid, out svcPtr );
            //IVsSolution solution = (IVsSolution)Marshal.GetObjectForIUnknown( svcPtr );
            //Guid guid = Guid.Empty;

            //IEnumHierarchies enumerator;
            //int hr = solution.GetProjectEnum( (uint)VSENUMPROJFLAGS.EPF_ALLPROJECTS, ref guid, out enumerator );

            //IVsHierarchy[] hierarchies = new IVsHierarchy[1];
            //uint fetched ;
            //enumerator.Next( 1, hierarchies, out fetched );
            //while ( fetched == 1 )
            //{
            //    IVsHierarchy hi = hierarchies[0];
                

            //    uint root = unchecked( (uint)(int)VSITEMID.VSITEMID_ROOT );
            //    object var;
            //    hr = hi.GetProperty( root, (int)__VSHPROPID.VSHPROPID_Name, out var );
            //    if ( hr == VSConstants.S_OK )
            //    {
            //        string name = (string)var;
            //        uint cookie;
            //        hr = hi.AdviseHierarchyEvents( new HierarchyEvents( this.context, name ), out cookie );
            //    }

               

            //    //DisplayHierarchy( hi, root );

            //    //DisplayProperties( hi, root );

            //    enumerator.Next( 1, hierarchies, out fetched );
            //}

        }

        private void UnadviseEvents()
        {
            this.trackProjectDocuments.UnadviseTrackProjectDocumentsEvents( this.cookie );
        }


        private IContext context;
        private uint cookie;

        private IVsTrackProjectDocuments2 trackProjectDocuments;

        private string BackupSuffix = Guid.NewGuid().ToString();
        


    }

}
