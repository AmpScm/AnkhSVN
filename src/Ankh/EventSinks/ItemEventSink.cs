//// $Id$
//using System;
//
//using EnvDTE;
//using System.IO;
//using NSvn.Core;
//using System.Windows.Forms;
//using System.Collections;
//using System.Diagnostics;
//using System.Globalization;
//using System.Runtime.InteropServices;
//
//namespace Ankh.EventSinks
//{
//    /// <summary>
//    /// Base class serving as event sink for all item related events.
//    /// </summary>
//    public abstract class ItemEventSink : EventSink
//    {
//        protected ItemEventSink( IContext context ) : base( context )
//        {
//            InitializeTrackProjectDocumentsIfNecessary( context );
//        }
//
//        internal static TrackProjectDocuments TrackProjectDocuments
//        {
//            get { return trackProjectDocuments; }
//        }
//
//        private void InitializeTrackProjectDocumentsIfNecessary( IContext context )
//        {
//            if ( TrackProjectDocuments == null )
//                trackProjectDocuments = new TrackProjectDocuments( context );
//        }
//
//        protected void ItemAdded( ProjectItem item )
//        {
//            try
//            {
//                Trace.WriteLine( "In ItemAdded" );
//                //// do we want to automatically add it?
//                //// no autoadds if another operation is running.
//                for ( short i = 1; i <= item.FileCount; i++ )
//                {
//                    string file = item.get_FileNames( i );
//
//                    if ( file.ToLower().StartsWith( "file://" ) )
//                    {
//                        Uri uri = new Uri( file );
//                        file = uri.LocalPath;
//                    }
//                    TrackProjectDocuments.MapFileToProject( file, item.ContainingProject );
//                }
//            }
//            catch( Exception ex )
//            {
//                this.Context.ErrorHandler.Handle( ex );
//            }
//        }
//
//        /// <summary>
//        /// Schedules a Projectitem for removal on commit.
//        /// </summary>
//        /// <param name="item">Projectitem to be scheduled for removal.</param>
//        protected void ItemRemoved( ProjectItem item )
//        {
//            // is a rename back operation going on?
//            if ( this.Context.OperationRunning )
//                return;
//
//            try
//            {
//                IList items = 
//                    this.Context.SolutionExplorer.GetItemResources(item, true);
//
//                items = SvnItem.Filter( items, new ResourceFilterCallback( 
//                    this.DeletableFilter ) );
//                string[] paths = SvnItem.GetPaths( items );
//                if ( paths.Length > 0 )
//                {
//                    this.Context.StartOperation( "Deleting" );
//                    this.Context.Client.Delete( paths, true );
//                }
//
//                // we need a refresh of the containing project no matter what
//                this.Context.SolutionExplorer.Refresh( item.ContainingProject );
//            }
//            catch ( Exception ex )
//            {
//                this.Context.ErrorHandler.Handle( ex );
//            }
//            finally
//            {
//                if ( this.Context.OperationRunning )
//                    this.Context.EndOperation();
//            }
//        }
//
//        private bool DeletableFilter( SvnItem item )
//        {
//            // we don't want to svn delete files that actually exist on disk -
//            // they'll most likely just be "Exclude(d) from project"
//            item.Refresh( this.Context.Client );
//            return item.Status.TextStatus == StatusKind.Missing;
//        }
//
//        /// <summary>
//        /// Warn the user if he attempts to rename a versioned resource.
//        /// </summary>
//        /// <param name="item"></param>
//        /// <param name="oldName"></param>
//        protected void ItemRenamed( ProjectItem item, string oldName )
//        {
//            try
//            {   
//                string newName = item.get_FileNames(1);
//                string dir = Path.GetDirectoryName( newName );
//
//                string oldPath = Path.Combine( dir, oldName );
//                SvnItem oldItem = this.Context.StatusCache[oldPath];
//                oldItem.Refresh( this.Context.Client );
//
//                // is the item versioned?
//                if ( oldItem.IsVersioned )
//                {
//                    this.Context.UIShell.ShowMessageBox( 
//                        "You have attempted to rename a file that is under version control.\r\n" + 
//                        "Use Ankh->Rename file... to rename the file instead", 
//                        "Attempting to rename versioned item", 
//                        MessageBoxButtons.OK, MessageBoxIcon.Stop );
//                    
//                    // rename back
//                    File.Move( newName, oldPath );
//                    
//                    // remove from project and reinstate old one
//                    Project project = item.ContainingProject;
//                    this.renaming = true;
//                    item.Remove();
//                    this.renaming = false;
//                    project.ProjectItems.AddFromFile( oldPath );                    
//                }
//                else // if item not under vc
//                {
//                    // we must still ensure that the project is rescanned, since 
//                    // it will lose it's status icon otherwise
//                    this.Context.SolutionExplorer.Refresh( item.ContainingProject );
//                }
//            }
//            catch( Exception ex )
//            {
//                this.Context.ErrorHandler.Handle( ex );
//            }            
//        }
//
//        private static TrackProjectDocuments trackProjectDocuments;
//           
//        private bool renaming = false;
//    }
//}
