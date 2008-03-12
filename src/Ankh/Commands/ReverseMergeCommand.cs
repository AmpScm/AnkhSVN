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
    [Command(AnkhCommand.RevertToRevision)]
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
            /*
            IContext context = e.Context.GetService<IContext>();

            SaveAllDirtyDocuments(context);

            IList resources = context.Selection.GetSelectionResources(
                true, new ResourceFilterCallback(SvnItem.VersionedFilter));
            using (e.Context.BeginOperation("Merging"))
            {
                using (ReverseMergeDialog dlg = new ReverseMergeDialog())
                {
                    dlg.GetPathInfo += new EventHandler<ResolvingPathEventArgs>(GetPathInfo);
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
            }*/
        }

        public static void GetPathInfo(object sender, ResolvingPathEventArgs args)
        {
            SvnItem item = (SvnItem)args.Item;
            args.IsDirectory = item.IsDirectory;
            args.Path = item.Path;
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
