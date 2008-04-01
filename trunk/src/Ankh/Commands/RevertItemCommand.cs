// $Id$
using System;
using System.Collections;
using System.Windows.Forms;
using AnkhSvn.Ids;
using SharpSvn;
using System.Collections.Generic;
using Ankh.Scc;
using Ankh.VS;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to revert current item to last updated revision.
    /// </summary>
    [Command(AnkhCommand.RevertItem)]
    [Command(AnkhCommand.ItemRevertBase)]
    public class RevertItemCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsModified)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();
            IAnkhDialogOwner dialogOwner = e.Context.GetService<IAnkhDialogOwner>();
            IAnkhOpenDocumentTracker docTracker = e.Context.GetService<IAnkhOpenDocumentTracker>();
            SaveAllDirtyDocuments(e.Selection, e.Context);

            // get the modified resources
            List<SvnItem> resources = new List<SvnItem>();
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsModified)
                    resources.Add(item);
            }

            SvnDepth depth = SvnDepth.Empty;
            bool confirmed = false;

            if (!CommandBase.Shift &&
                e.Command == AnkhCommand.RevertItem)
            {
                PathSelectorInfo info = new PathSelectorInfo("Select items to revert",
                    resources, resources);

                //if(e.Command == AnkhCommand.ItemRevertSpecific)
                //    info.RevisionStart = SvnRevision.Base;

                info = context.UIShell.ShowPathSelector(info);
                if (info == null)
                    return;
                confirmed = true;
                depth = info.Depth;

                resources.Clear();
                foreach (SvnItem item in info.CheckedItems)
                {
                    resources.Add(item);
                }
            }

            List<string> paths = new List<string>();

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsModified)
                {
                    paths.Add(item.FullPath);
                    item.MarkDirty();
                }
            }

            // ask for confirmation if the Shift dialog hasn't been used
            if (!confirmed)
            {
                string msg = "Do you really want to revert these item(s)?" +
                    Environment.NewLine + Environment.NewLine;
                msg += string.Join(Environment.NewLine, paths.ToArray());

                if (dialogOwner.MessageBox.Show(msg, "Revert", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information) != DialogResult.Yes)
                {
                    return;
                }
            }

            // perform the actual revert 
            using (e.Context.BeginOperation("Reverting"))
            using (DocumentLock dl = docTracker.LockDocuments(paths))
            {
                SvnRevertArgs args = new SvnRevertArgs();
                //args.Depth = depth;
                args.ThrowOnError = false;

                using (SvnClient client = e.Context.GetService<ISvnClientPool>().GetClient())
                {
                    client.Revert(paths, args);

                    IProjectNotifier pn = e.Context.GetService<IProjectNotifier>();
                    if (pn != null)
                        pn.MarkDirty(e.Selection.GetOwnerProjects(true));
                }
                dl.Reload(paths);
            }
        }
    }
}