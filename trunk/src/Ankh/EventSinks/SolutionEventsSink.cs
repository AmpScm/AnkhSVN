// $Id$
using System;
using EnvDTE;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Event sink for solution related events.
    /// </summary>
    internal class SolutionEventsSink : ItemEventSink
    {
        internal SolutionEventsSink( AnkhContext context ) : base( context )
        {
            this.solutionEvents = context.DTE.Events.SolutionEvents;
            this.solutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler(
                this.ProjectAdded );
            this.solutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler(
                this.ProjectRemoved );

            this.solutionItemsEvents = this.Context.DTE.Events.SolutionItemsEvents;
            this.solutionItemsEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(
                this.ItemAdded );
            this.solutionItemsEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
            this.solutionItemsEvents.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler(
                this.ItemRenamed );

            this.miscFilesEvents = this.Context.DTE.Events.MiscFilesEvents;
            this.miscFilesEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(
                this.ItemAdded );
            this.miscFilesEvents.ItemRemoved += new _dispProjectItemsEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
            this.miscFilesEvents.ItemRenamed += new _dispProjectItemsEvents_ItemRenamedEventHandler(
                this.ItemRenamed );


        }

        public override void Unhook()
        {
            this.solutionEvents.ProjectAdded -= new _dispSolutionEvents_ProjectAddedEventHandler(
                this.ProjectAdded );
            this.solutionEvents.ProjectRemoved -= new _dispSolutionEvents_ProjectRemovedEventHandler(
                this.ProjectRemoved );
            this.solutionItemsEvents.ItemAdded -= new _dispProjectItemsEvents_ItemAddedEventHandler(
                this.ItemAdded );
            this.solutionItemsEvents.ItemRemoved -= new _dispProjectItemsEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
            this.solutionItemsEvents.ItemRenamed -= new _dispProjectItemsEvents_ItemRenamedEventHandler(
                this.ItemRenamed );

            this.miscFilesEvents.ItemAdded -= new _dispProjectItemsEvents_ItemAddedEventHandler(
                this.ItemAdded );
            this.miscFilesEvents.ItemRemoved -= new _dispProjectItemsEvents_ItemRemovedEventHandler(
                this.ItemRemoved );
            this.miscFilesEvents.ItemRenamed -= new _dispProjectItemsEvents_ItemRenamedEventHandler(
                this.ItemRenamed );
        }

        protected void ProjectAdded( Project project )
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
            finally
            {
                EventSink.AddingProject = false;
            }
        }

        protected void ProjectRemoved( Project project )
        {           
        }

        private SolutionEvents solutionEvents;
        private ProjectItemsEvents solutionItemsEvents;
        private ProjectItemsEvents miscFilesEvents;
    }
}
