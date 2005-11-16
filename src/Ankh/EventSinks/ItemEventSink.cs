// $Id$
using System;

using EnvDTE;
using System.IO;
using NSvn.Core;
using System.Windows.Forms;
using System.Collections;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Base class serving as event sink for all item related events.
    /// </summary>
    public abstract class ItemEventSink : EventSink
    {
        protected ItemEventSink( IContext context ) : base( context )
        {
            // empty
        }

        protected void ItemAdded( ProjectItem item )
        {
            try
            {
                // do we want to automatically add it?
                // no autoadds if another operation is running.
                if ( this.Context.Config.AutoAddNewFiles && !this.Context.OperationRunning )
                {
                    for( short i = 1; i <= item.FileCount; i++ )
                    {
                        string file = item.get_FileNames(i);

                        // we don't want to (automatically) add the .svn(_svn) dir 
                        // or its contents
                        if ( Path.GetDirectoryName( file ).IndexOf( 
                            Client.AdminDirectoryName ) >= 0 )
                        {
                            return;
                        }

                        // is this an URI?
                        if ( file.ToLower().StartsWith( "file://" ) )
                        {
                            Uri uri = new Uri( file );
                            file = uri.LocalPath;
                        }

                        // does this file even exist?
                        if ( !File.Exists( file ) )
                            return;
                        try
                        {
                            SvnItem svnItem = this.Context.StatusCache[ file ];
                            
                            // make sure we have up to date info on this item.
                            svnItem.Refresh(this.Context.Client);
                        
                            if ( !svnItem.IsVersioned && svnItem.IsVersionable &&
                                !this.Context.Client.IsIgnored( svnItem.Path ) )
                                this.Context.Client.Add( file, false );
                        }
                        catch( SvnClientException ex )
                        {
                            // don't propagate this exception
                            // just tell the user and move on
                            this.Context.ErrorHandler.Write( "Unable to add file: ", ex, 
                                this.Context.OutputPane );
                        }
                    }
                }
                this.Context.SolutionExplorer.Refresh( item.ContainingProject );
            }
            catch( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }
        }

        /// <summary>
        /// Schedules a Projectitem for removal on commit.
        /// </summary>
        /// <param name="item">Projectitem to be scheduled for removal.</param>
        protected void ItemRemoved( ProjectItem item )
        {
            // is a rename back operation going on?
            if ( this.Context.OperationRunning )
                return;

            try
            {
                IList items = 
                    this.Context.SolutionExplorer.GetItemResources(item, true);

                items = SvnItem.Filter( items, new ResourceFilterCallback( 
                    this.DeletableFilter ) );
                string[] paths = SvnItem.GetPaths( items );
                if ( paths.Length > 0 )
                {
                    this.Context.StartOperation( "Deleting" );
                    this.Context.Client.Delete( paths, true );
                }

                // we need a refresh of the containing project no matter what
                this.Context.SolutionExplorer.Refresh( item.ContainingProject );
            }
            catch ( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }
            finally
            {
                if ( this.Context.OperationRunning )
                    this.Context.EndOperation();
            }
        }

        private bool DeletableFilter( SvnItem item )
        {
            // we don't want to svn delete files that actually exist on disk -
            // they'll most likely just be "Exclude(d) from project"
            item.Refresh( this.Context.Client );
            return item.Status.TextStatus == StatusKind.Missing;
        }

        /// <summary>
        /// Warn the user if he attempts to rename a versioned resource.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="oldName"></param>
        protected void ItemRenamed( ProjectItem item, string oldName )
        {
            try
            {   
                string newName = item.get_FileNames(1);
                string dir = Path.GetDirectoryName( newName );

                string oldPath = Path.Combine( dir, oldName );
                SvnItem oldItem = this.Context.StatusCache[oldPath];
                oldItem.Refresh( this.Context.Client );

                // is the item versioned?
                if ( oldItem.IsVersioned )
                {
                    this.Context.UIShell.ShowMessageBox( 
                        "You have attempted to rename a file that is under version control.\r\n" + 
                        "Use Ankh->Rename file... to rename the file instead", 
                        "Attempting to rename versioned item", 
                        MessageBoxButtons.OK, MessageBoxIcon.Stop );
                    
                    // rename back
                    File.Move( newName, oldPath );
                    
                    // remove from project and reinstate old one
                    Project project = item.ContainingProject;
                    this.renaming = true;
                    item.Remove();
                    this.renaming = false;
                    project.ProjectItems.AddFromFile( oldPath );                    
                }
                else // if item not under vc
                {
                    // we must still ensure that the project is rescanned, since 
                    // it will lose it's status icon otherwise
                    this.Context.SolutionExplorer.Refresh( item.ContainingProject );
                }
            }
            catch( Exception ex )
            {
                this.Context.ErrorHandler.Handle( ex );
            }            
        }
           
        private bool renaming = false;
    }
}
