// $Id$
using System;
using EnvDTE;
using System.Windows.Forms;
using Ankh.UI;
using System.Collections;
using System.Diagnostics;
using Ankh.Solution;
using NSvn.Common;
using NSvn.Core;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that updates an item.
    /// </summary>
    [VSNetCommand("UpdateItem", Text = "Update...", Tooltip = "Updates the local item",
         Bitmap = ResourceBitmaps.Update),
    VSNetProjectItemControl( "", Position = 2 ),
    VSNetProjectNodeControl( "", Position = 2 ),
    VSNetControl( "Solution", Position = 2 ),
    VSNetFolderNodeControl( "", Position = 2)]
    public class UpdateItem : CommandBase
    {		
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // all items must be versioned if we are going to run update.
            IList resources = context.SolutionExplorer.GetSelectionResources( true,
                new ResourceFilterCallback(SvnItem.VersionedFilter) );
            if ( resources.Count > 0 )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            try
            {
                // save all files
                this.SaveAllDirtyDocuments( context );
                context.StartOperation( "Updating" );

                IList resources = context.SolutionExplorer.GetSelectionResources( true,
                    new ResourceFilterCallback(SvnItem.VersionedFilter) );


                // we assume by now that all items are working copy resources.                
                UpdateRunner runner = new UpdateRunner( context, resources );
                if ( !runner.MaybeShowUpdateDialog() )
                    return;

                // run the actual update on another thread
                context.ProjectFileWatcher.StartWatchingForChanges();
                bool completed = context.UIShell.RunWithProgressDialog( runner, "Updating" );

                // this *must* happen on the primary thread.
                if ( completed )
                {
                    if ( !context.ReloadSolutionIfNecessary() )
                        context.SolutionExplorer.RefreshSelection();
                }
            }
            finally
            {
                context.EndOperation();

            }

        }    

        #endregion

        #region UpdateVisitor
        private class UpdateRunner : IProgressWorker
        {
            public UpdateRunner( IContext context, IList resources ) 
            {
                this.context = context;
                this.resources = resources;
            }

            public IContext Context
            {
                get{ return this.context; }
            }

            /// <summary>
            /// Show the update dialog if wanted.
            /// </summary>
            /// <returns></returns>
            public bool MaybeShowUpdateDialog()
            {
                this.recurse = Recurse.None;
                this.revision = Revision.Head;

                // We're using the update dialog no matter what to
                // take advantage of it's path processing capabilities.
                // This is the best way to ensure holding down Shift is
                // equivalent to accepting the default in the dialog.
                using(UpdateDialog d = new UpdateDialog())
                {
                    d.GetPathInfo += new GetPathInfoDelegate(SvnItem.GetPathInfo);
                    d.Items = this.resources;
                    d.CheckedItems = this.resources;
                    d.Recursive = true;

                    if ( !CommandBase.Shift )
                    {
                        if ( d.ShowDialog( this.Context.HostWindow ) != DialogResult.OK )
                            return false;
                    }

                    recurse = d.Recursive ? Recurse.Full : Recurse.None;
                    this.resources = d.CheckedItems;
                    this.revision = d.Revision;
                }

                // the user hasn't cancelled the update
                return true;
            }

            /// <summary>
            /// The actual updating happens here.
            /// </summary>
            public void Work( IContext context )
            {   
                context.Client.Notification +=new NotificationDelegate(this.OnNotificationEventHandler );

                try
                {
                    string[] paths = SvnItem.GetPaths( this.resources );
                    context.Client.Update( paths, revision, recurse, false );
                }
                finally
                {       
                    context.Client.Notification  -= new NotificationDelegate(this.OnNotificationEventHandler );
                }
              
                if (this.conflictsOccurred) 
                    context.ConflictManager.NavigateTaskList();

            }
            
            /// <summary>
            ///  Handlke event for onNotification that conflicts occurred from update
            /// </summary>
            /// <param name="taskItem"></param>
            /// <param name="navigateHandled"></param>
            private void  OnNotificationEventHandler(Object sender, NotificationEventArgs args)
            {
                if ( args.ContentState == NSvn.Core.NotifyState.Conflicted)
                {
                    this.Context.ConflictManager.AddTask(args.Path);
                    this.conflictsOccurred = true;
                }
            }

            private IList resources = new ArrayList();
            private Revision revision;
            private Recurse recurse;
            private bool conflictsOccurred = false; 
            private IContext context;
        }            
        #endregion

    }
}



