// $Id$
using System;
using EnvDTE;
using NSvn;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Event sink for the ProjectEvents events.
    /// </summary>
    internal class ProjectsEventSink : EventSink
    {
        internal ProjectsEventSink( ProjectsEvents events, AnkhContext context ) :
            base( context )
        {
            this.events = events;
            events.ItemAdded += new _dispProjectsEvents_ItemAddedEventHandler(
                this.ItemAdded );
            events.ItemRemoved += new _dispProjectsEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
            events.ItemRenamed += new _dispProjectsEvents_ItemRenamedEventHandler(
                this.ItemRenamed );           
        }

        public override void Unhook()
        {
            this.events.ItemAdded -= new _dispProjectsEvents_ItemAddedEventHandler(
                this.ItemAdded );
            this.events.ItemRemoved -= new _dispProjectsEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
            this.events.ItemRenamed -= new _dispProjectsEvents_ItemRenamedEventHandler(
                this.ItemRenamed );
        }

        protected void ItemAdded( Project project )
        {
            
        }

        /// <summary>
        /// Schedules a Project for removal on commit.
        /// </summary>
        /// <param name="item">Projectitem to be scheduled for removal.</param>
        protected void ItemRemoved( Project project )
        {   
            try
            {
                this.Context.SolutionExplorer.SyncWithTreeView();
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
                throw;
            }
        }

        protected void ItemRenamed( Project item, string oldName )
        {
            try
            {
                string s = item.FileName;
            }
            catch( Exception ex )
            {
                Error.Handle( ex );
                throw;
            }
        }
       
        private ProjectsEvents events;
    }
}
