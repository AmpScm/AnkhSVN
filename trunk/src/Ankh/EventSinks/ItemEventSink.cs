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
    internal abstract class ItemEventSink : EventSink
    {
        protected ItemEventSink( AnkhContext context ) : base( context )
        {
            // empty
        }
     
 

        protected void ItemAdded( ProjectItem item )
        {
            try
            {
                // do we want to automatically add it?
                if ( this.Context.Config.AutoAddNewFiles )
                {
                    for( short i = 1; i <= item.FileCount; i++ )
                    {
                        string file = item.get_FileNames(i);
                        SvnItem svnItem = this.Context.SolutionExplorer.StatusCache[ file ];
                        if ( !svnItem.IsVersioned && svnItem.IsVersionable )
                            this.Context.Client.Add( file, false );
                    }
                }

                this.Context.SolutionExplorer.Refresh( item.ContainingProject );
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
                throw;
            }
        }

        /// <summary>
        /// Schedules a Projectitem for removal on commit.
        /// </summary>
        /// <param name="item">Projectitem to be scheduled for removal.</param>
        protected void ItemRemoved( ProjectItem item )
        {
            // is a rename back operation going on?
            if ( this.renaming )
                return;

            try
            {
                
                this.Context.StartOperation( "Deleting" );

                IList items = 
                    this.Context.SolutionExplorer.GetSelectionResources(true);
                string[] paths = SvnItem.GetPaths( items );
                this.Context.Client.Delete( paths, true );
                
                foreach( SvnItem svnItem in items )
                    svnItem.Refresh( this.Context.Client );

            }
            catch ( Exception ex )
            {
                Error.Handle( ex );
                throw;
            }
            finally
            {
                this.Context.EndOperation();
            }
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
                SvnItem oldItem = this.Context.SolutionExplorer.StatusCache["oldPath"];

                // is the item versioned?
                if ( oldItem.Status.TextStatus != StatusKind.None )
                {
                    MessageBox.Show( this.Context.HostWindow, 
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
                    this.Context.SolutionExplorer.RefreshSelectionParents();
                }
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
            }            
        }
            
        private bool renaming = false;
    }
}
