// $Id$
using System;
using System.Collections;
using System.Windows.Forms;
using AnkhSvn.Ids;
using SharpSvn;
using System.Collections.Generic;
using Ankh.Scc;
using Ankh.VS;
using Ankh.UI;

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

            SvnDepth depth = SvnDepth.Empty;
            bool confirmed = false;

            PathSelectorResult result = null;
            PathSelectorInfo info = new PathSelectorInfo("Select items to revert",
                e.Selection.GetSelectedSvnItems(true));

            info.CheckedFilter += delegate(SvnItem item) { return item.IsModified; };
            info.VisibleFilter += delegate(SvnItem item) { return true; };

            if (!CommandBase.Shift &&
                e.Command == AnkhCommand.RevertItem)
            {
                //if(e.Command == AnkhCommand.ItemRevertSpecific)
                //    info.RevisionStart = SvnRevision.Base;

                result = context.UIShell.ShowPathSelector(info);
                if (info == null)
                    return;
                confirmed = true;
                depth = info.Depth;
            }
            else
            {
                result = info.DefaultResult;
            }
            
            string[] paths = new string[result.Selection.Count];
            for (int i = 0; i < paths.Length; i++)
                paths[i] = result.Selection[i].FullPath;

            // ask for confirmation if the Shift dialog hasn't been used
            if (!confirmed)
            {
                string msg = "Do you really want to revert these item(s)?" +
                    Environment.NewLine + Environment.NewLine;
                msg += string.Join(Environment.NewLine, paths);

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
                Dictionary<string, string> revertedItems = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                args.Notify += delegate(object sender, SvnNotifyEventArgs eNotify)
                {
                    if (eNotify.Action == SvnNotifyAction.Revert && !revertedItems.ContainsKey(eNotify.FullPath))
                        revertedItems.Add(eNotify.FullPath, eNotify.FullPath);
                };
                using (SvnClient client = e.Context.GetService<ISvnClientPool>().GetClient())
                {
                    client.Revert(paths, args);
                }
                dl.Reload(revertedItems.Keys);
            }
        }
    }
}