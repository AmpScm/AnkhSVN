// $Id$
using System;
using NSvn;
using EnvDTE;
using System.IO;
using NSvn.Core;

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
            try
            {
                //this.Context.OutputPane.StartActionText( "Delete" );
                this.Context.SolutionExplorer.VisitResources( 
                    item, new RemoveProjectVisitor(), false );
                this.Context.SolutionExplorer.Refresh ( item.ContainingProject );
                //this.Context.OutputPane.EndActionText();

            }
            catch ( Exception ex )
            {
                Error.Handle( ex );
                throw;
            }
        }

        protected void ItemRenamed( ProjectItem item, string oldName )
        {
            try
            {
                // rename on a folder will call rename on all subitems.
                if ( item.Name == oldName )
                    return;

                // assume theres only one
                string newPath = item.get_FileNames(1);

                // find the parent dir
                string noTrailing = newPath[ newPath.Length - 1 ] == '\\' ? newPath.Substring( 0, newPath.Length-1 ) : 
                    newPath;
                string parentDir = Path.GetDirectoryName( noTrailing );

                string oldPath = Path.Combine( parentDir, oldName );
               
                try
                {
                    this.Context.OutputPane.StartActionText( "Renaming" );

                    RenameVisitor v = new RenameVisitor( oldPath, newPath );
                    this.Context.SolutionExplorer.VisitResources( item, v, false );
                }
                catch( SvnClientException )
                {
                    // unable to rename - abort.
                    this.Context.OutputPane.WriteLine( "Ankh was unable to rename the item, most likely due to uncommitted changes." +
                        Environment.NewLine + "The item is now out of Ankh's control. Sorry. Hope to do better next time.");
                }
                finally
                {
                    this.Context.OutputPane.EndActionText();
                }

                // we need to refresh the parents, since the actual treenode is replaced.
               this.Context.SolutionExplorer.RefreshSelectionParents();
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
                throw;
            }
        }

        /// <summary>
        /// A visitor that schedules a remove of visited item on commit
        /// </summary>
        private class RemoveProjectVisitor : LocalResourceVisitorBase
        {
            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
            {
                // Checks if file doesn't exists. 
                if ( !File.Exists( resource.Path ) )
                {
                    resource.Remove( true );
                }
            }
        }

        /// <summary>
        /// A visitor that renames an item.
        /// </summary>
        private class RenameVisitor : LocalResourceVisitorBase
        {
            public RenameVisitor( string oldPath, string newPath )
            {
                this.oldPath = oldPath; 
                this.newPath = newPath;
            }

            public override void VisitWorkingCopyFile(WorkingCopyFile file)
            {
                File.Move( this.newPath, this.oldPath );

                // now have SVN rename it.
                file.Move( this.newPath, true );
            }

            public override void VisitWorkingCopyDirectory(WorkingCopyDirectory dir)
            {
                // strip off the trailing \ if necessary
                Directory.Move( this.newPath, this.oldPath );

                dir.Move( this.newPath, true );
            }



            private string oldPath;
            private string newPath;

        }
    }
}
