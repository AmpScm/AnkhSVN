// $Id$
using System;

using EnvDTE;
using System.IO;
using NSvn.Core;
using System.Windows.Forms;

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
//            try
//            {
//                // is a rename back operation going on?
//                if ( this.renaming )
//                    return;
//
//                this.Context.StartOperation( "Deleting" );
//                this.Context.SolutionExplorer.VisitResources( 
//                    item, new RemoveProjectVisitor(), false );
//                this.Context.SolutionExplorer.Refresh ( item.ContainingProject );
//                this.Context.EndOperation();
//
//            }
//            catch ( Exception ex )
//            {
//                Error.Handle( ex );
//                throw;
//            }
        }

        /// <summary>
        /// Warn the user if he attempts to rename a versioned resource.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="oldName"></param>
        protected void ItemRenamed( ProjectItem item, string oldName )
        {
//            try
//            {   
//                string newName = item.get_FileNames(1);
//                string dir = Path.GetDirectoryName( newName );
//
//                string oldPath = Path.Combine( dir, oldName );
//                // is the item versioned?
//                if ( Client.SingleStatus( oldPath ).TextStatus != StatusKind.None )
//                {
//                    MessageBox.Show( this.Context.HostWindow, 
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
//                    this.Context.SolutionExplorer.RefreshSelectionParents();
//                }
//            }
//            catch( Exception ex )
//            {
//                Error.Handle( ex );
//            }            
        }
            

//        /// <summary>
//        /// A visitor that schedules a remove of visited item on commit
//        /// </summary>
//        private class RemoveProjectVisitor : LocalResourceVisitorBase
//        {
//            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
//            {
//                // Checks if file doesn't exist. This should not trigger if we are renaming.
//                if ( !File.Exists( resource.Path ) )
//                {
//                    resource.Remove( true );
//                }
//            }
//        }

        private bool renaming = false;
    }
}
