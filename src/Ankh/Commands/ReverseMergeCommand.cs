using System;
using System.Collections;
using Ankh.UI;
using System.Windows.Forms;
using NSvn.Core;
using NSvn.Common;
using System.IO;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to revert current item to a specific revision.
    /// </summary>
    [VSNetCommand( "ReverseMerge",
         Text = "Re&vert to Revision...", 
         Tooltip = "Revert this item to a specific revision.", 
         Bitmap = ResourceBitmaps.RevertToVersion),
         VSNetItemControl( VSNetControlAttribute.AnkhSubMenu, Position = 1 )]
    public class ReverseMergeCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            IList resources = context.Selection.GetSelectionResources(
                true, new ResourceFilterCallback(SvnItem.VersionedFilter) );
            if ( resources.Count > 0 )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            this.SaveAllDirtyDocuments( context );

            IList resources = context.Selection.GetSelectionResources(
                true, new ResourceFilterCallback(SvnItem.VersionedFilter) );
            context.StartOperation( "Merging" );
            try
            {
                using ( ReverseMergeDialog dlg = new ReverseMergeDialog() )
                {
                    dlg.GetPathInfo += new ResolvingPathInfoHandler(SvnItem.GetPathInfo);
                    dlg.Items = resources;
                    dlg.CheckedItems = resources;
                    dlg.Recursive = true;

                    if ( dlg.ShowDialog( context.HostWindow ) != DialogResult.OK )
                        return;

                    context.ProjectFileWatcher.StartWatchingForChanges();
               
                    ReverseMergeRunner runner = new ReverseMergeRunner(
                        dlg.CheckedItems, dlg.Revision, dlg.Recursive ? Recurse.Full : Recurse.None,
                        dlg.DryRun );
                    context.UIShell.RunWithProgressDialog( runner, "Merging" );

                    // we need to refresh every item, not just those selected since 
                    // the operation might be recursive
                    if ( !context.ReloadSolutionIfNecessary() )
                    {
                        foreach( SvnItem item in resources )
                            item.Refresh( context.Client);
                    }
                }
            }
            finally
            {
                context.EndOperation();
            }
        }

        #endregion

        /// <summary>
        /// A progressrunner for reverse merges.
        /// </summary>
        private class ReverseMergeRunner : IProgressWorker
        {
            public ReverseMergeRunner(  IList items, Revision revision,
                Recurse recurse, bool dryRun ) 
            {
                this.items = items;
                this.revision = revision;
                this.recurse = recurse;
                this.dryRun = dryRun;
            }

            public void Work( IContext context )
            {
                foreach( SvnItem item in this.items )
                {
                    context.Client.Merge( 
                        item.Path, Revision.Working,
                        item.Path, this.revision,
                        item.Path, this.recurse,
                        false, false, this.dryRun );
                }
            }

            private IList items;
            private Revision revision;
            private Recurse recurse;
            private bool dryRun;
        }
    }
}
