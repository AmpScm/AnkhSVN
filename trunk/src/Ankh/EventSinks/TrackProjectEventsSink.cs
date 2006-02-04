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
            return OnAfterRemove( rgpszMkDocuments );
        }

        public int OnAfterRemoveFiles( int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnAfterRemoveFiles() of: {0}", this.ToString() ) );
           
            return OnAfterRemove( rgpszMkDocuments );
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

            VSQUERYREMOVEFILERESULTS[] results = new VSQUERYREMOVEFILERESULTS[1];
            int hr = this.OnQueryRemove( rgpszMkDocuments, results );
            pSummaryResult[0] = results[0] == VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK ? 
                VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveOK :
                VSQUERYREMOVEDIRECTORYRESULTS.VSQUERYREMOVEDIRECTORYRESULTS_RemoveOK;
            return hr;
        }

        public int OnQueryRemoveFiles( IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRemoveFiles() of: {0}", this.ToString() ) );
            return this.OnQueryRemove( rgpszMkDocuments, pSummaryResult );
        }

        public int OnQueryRenameDirectories( IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults )
        {
            Trace.WriteLine( string.Format( CultureInfo.CurrentCulture, "Entering OnQueryRenameDirectories() of: {0}", this.ToString() ) );

            VSQUERYRENAMEFILERESULTS[] fileResults = new VSQUERYRENAMEFILERESULTS[1];

            int hr = OnQueryRename( rgszMkOldNames, rgszMkNewNames, fileResults );
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

        private int OnAfterRemove( string[] paths )
        {
            try
            {
                for( int i = 0; i < paths.Length; i++ ) 
                {
                    SvnItem item = this.context.StatusCache[ paths[i] ];
                    item.Refresh( this.context.Client );
                    if ( item.Status.TextStatus == StatusKind.Missing )
                    {
                        this.context.Client.Delete( new string[]{ item.Path }, true );
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
                                this.context.Client.Add( file, false );
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

        private int OnQueryRename( string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILERESULTS[] pSummaryResult )
        {
            try
            {
                pSummaryResult[0] = VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameOK;

                for ( int i = 0; i < rgszMkOldNames.Length; i++ )
                {
                    SvnItem item = context.StatusCache[rgszMkOldNames[i]];
                    if ( !CanRename( item ) )
                    {
                        pSummaryResult[0] = VSQUERYRENAMEFILERESULTS.VSQUERYRENAMEFILERESULTS_RenameNotOK;
                        this.context.OutputPane.WriteLine( "SVN prohibits renaming modified items. " + 
                            "Please commit the modifications before attempting a rename." );
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

        private int OnQueryRemove( string[] names, VSQUERYREMOVEFILERESULTS[] pSummaryResult )
        {
            try
            {
                pSummaryResult[0] = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK;
                for( int i = 0; i < names.Length; i++ )
                {
                    SvnItem item = context.StatusCache[ names[i] ];
                    if ( !CanDelete( item ) )
                    {
                        pSummaryResult[0] = VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveNotOK;
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

        private bool CanRename( SvnItem item )
        {
            return (
                ( item.IsVersioned && !item.IsModified ) ||
                ( !item.IsVersioned )
            );
        }

        private bool CanDelete( SvnItem item )
        {
            if ( item.IsVersioned )
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
                return true;
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


    }

}
