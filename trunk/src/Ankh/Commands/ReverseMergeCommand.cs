using System;
using System.Collections;
using Ankh.UI;
using System.Windows.Forms;


using System.IO;
using SharpSvn;
using Ankh.Ids;
using System.Windows.Forms.Design;
using Ankh.Scc;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Ankh.VS;
using Ankh.Selection;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to revert current item to a specific revision.
    /// </summary>
    [Command(AnkhCommand.RevertToRevision)]
    [Command(AnkhCommand.RevertProjectToRevision)]
    [Command(AnkhCommand.RevertSolutionToRevision)]
    public class ReverseMergeCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SccProviderActive)
            {
                e.Visible = e.Enabled = false;
                return;
            }

            int count = 0;

            IFileStatusCache statusCache = e.GetService<IFileStatusCache>();
            IAnkhSolutionSettings solSettings = e.GetService<IAnkhSolutionSettings>();

            switch (e.Command)
            {
                case AnkhCommand.RevertToRevision:

                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (item.IsVersioned)
                        {
                            count++;
                            if (count > 1)
                                break;
                        }
                    }
                    break;
                case AnkhCommand.RevertSolutionToRevision:
                    SvnItem i = statusCache[solSettings.ProjectRoot];
                    if (i.IsVersioned)
                        count++;
                    break;
                case AnkhCommand.RevertProjectToRevision:
                    foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
                    {
                        if (p.IsSolution)
                            continue;

                        SvnItem ii = statusCache[p.FullPath];
                        if (ii.IsVersioned)
                        {
                            count++;
                            if (count > 1)
                                break;
                        }
                    }
                    break;
            }

            e.Enabled = count == 1;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            SvnItem item = null;
            IFileStatusCache statusCache = e.GetService<IFileStatusCache>();
            IAnkhSolutionSettings solSettings = e.GetService<IAnkhSolutionSettings>();

            switch (e.Command)
            {

                case AnkhCommand.RevertSolutionToRevision:
                    SvnItem i = statusCache[solSettings.ProjectRoot];
                    if(i.IsVersioned)
                        item = i;
                    break;
                case AnkhCommand.RevertToRevision:
                    foreach (SvnItem ii in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (ii.IsVersioned)
                        {
                            item = ii;
                            break;
                        }
                    }
                    break;
                case AnkhCommand.RevertProjectToRevision:
                    foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
                    {
                        if (p.IsSolution)
                            continue;
                        SvnItem iii = statusCache[p.FullPath];
                        if (iii.IsVersioned)
                        {
                            item = iii;
                            break;
                        }
                    }
                    break;
            }

            if (item == null)
                return;

            Execute(item, e);
        }

        void Execute(SvnItem item, CommandEventArgs e)
        {
            List<SvnRevisionRange> revisions = new List<SvnRevisionRange>();
            IUIService ui = e.GetService<IUIService>();
            using (LogViewerDialog dialog = new LogViewerDialog(item.FullPath, e.Context))
            {
                dialog.Text = "Select Revisions to revert";
                if (ui.ShowDialog(dialog) == DialogResult.OK)
                {
                    foreach(ISvnLogItem revision in dialog.SelectedItems)
                    {
                        revisions.Add(new SvnRevisionRange(SvnRevision.Working, revision.Revision));
                    }
                }
            }

            
            using (e.Context.BeginOperation("Reverse merging"))
            {
                SaveAllDirtyDocuments(e.Selection, e.Context);

                e.GetService<IProgressRunner>().Run("Merging", delegate(object sender, ProgressWorkerArgs ee)
                {
                    ee.Client.Merge(item.FullPath, new SvnPathTarget(item.FullPath), revisions);
                });
            }
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

                    if (dlg.ShowDialog(e.Context.DialogOwner) != DialogResult.OK)
                        return;

                    ReverseMergeRunner runner = new ReverseMergeRunner(
                        dlg.CheckedItems, dlg.Revision, dlg.Recursive ? SvnDepth.Infinity : SvnDepth.Empty,
                        dlg.DryRun);
                    context.UIShell.RunWithProgressDialog(runner, "Merging");
                }
            }*/
        }


        #endregion

        /// <summary>
        /// A progressrunner for reverse merges.
        /// </summary>
        private class ReverseMergeRunner
        {
            public ReverseMergeRunner(IList items, SvnRevision revision,
                SvnDepth depth, bool dryRun)
            {
                this.items = items;
                this.revision = revision;
                this.depth = depth;
                this.dryRun = dryRun;
            }

            public void Work(object sender, ProgressWorkerArgs e)
            {
                SvnMergeArgs args = new SvnMergeArgs();
                args.Depth = depth;
                args.DryRun = dryRun;
                args.IgnoreAncestry = false;
                args.Force = false;

                foreach (SvnItem item in this.items)
                {
                    e.Client.Merge(item.FullPath, item.FullPath, new SvnRevisionRange(SvnRevision.Working, revision), args);
                }

            }

            private IList items;
            private SvnRevision revision;
            private SvnDepth depth;
            private bool dryRun;
        }
    }
}
