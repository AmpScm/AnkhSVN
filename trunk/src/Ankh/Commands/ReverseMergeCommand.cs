using System;
using System.Collections;
using Ankh.UI;
using System.Windows.Forms;


using System.IO;
using SharpSvn;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to revert current item to a specific revision.
    /// </summary>
    [VSNetCommand(AnkhCommand.RevertToRevision,
		"ReverseMerge",
         Text = "Re&vert to Revision...", 
         Tooltip = "Revert this item to a specific revision.", 
         Bitmap = ResourceBitmaps.RevertToVersion),
         VSNetItemControl( VSNetControlAttribute.AnkhSubMenu, Position = 4 )]
    public class ReverseMergeCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    return;
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            SaveAllDirtyDocuments(context);

            IList resources = context.Selection.GetSelectionResources(
                true, new ResourceFilterCallback(SvnItem.VersionedFilter));
            using (context.StartOperation("Merging"))
            {
                using (ReverseMergeDialog dlg = new ReverseMergeDialog())
                {
                    dlg.GetPathInfo += new ResolvingPathInfoHandler(SvnItem.GetPathInfo);
                    dlg.Items = resources;
                    dlg.CheckedItems = resources;
                    dlg.Recursive = true;

                    if (dlg.ShowDialog(context.HostWindow) != DialogResult.OK)
                        return;

                    ReverseMergeRunner runner = new ReverseMergeRunner(
                        dlg.CheckedItems, dlg.Revision, dlg.Recursive ? SvnDepth.Infinity : SvnDepth.Empty,
                        dlg.DryRun);
                    context.UIShell.RunWithProgressDialog(runner, "Merging");
                }
            }

        }

        #endregion

        /// <summary>
        /// A progressrunner for reverse merges.
        /// </summary>
        private class ReverseMergeRunner : IProgressWorker
        {
            public ReverseMergeRunner(IList items, SvnRevision revision,
                SvnDepth depth, bool dryRun)
            {
                this.items = items;
                this.revision = revision;
                this.depth = depth;
                this.dryRun = dryRun;
            }

            public void Work(IContext context)
            {
                SvnMergeArgs args = new SvnMergeArgs();
                args.Depth = depth;
                args.DryRun = dryRun;
                args.IgnoreAncestry = false;
                args.Force = false;
                foreach (SvnItem item in this.items)
                {
                    context.Client.Merge(item.Path, item.Path, new SvnRevisionRange(SvnRevision.Working, revision), args);
                }
            }

            private IList items;
            private SvnRevision revision;
            private SvnDepth depth;
            private bool dryRun;
        }
    }
}
