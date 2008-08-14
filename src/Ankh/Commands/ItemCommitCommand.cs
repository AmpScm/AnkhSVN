// $Id$
using Ankh.UI;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;
using SharpSvn;
using Ankh.Ids;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.VS;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to commit selected items to the Subversion repository.
    /// </summary>
    [Command(AnkhCommand.CommitItem)]
    class ItemCommitCommand : CommandBase
    {
        string storedLogMessage = null;

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(true))
            {
                if (i.IsVersioned)
                {
                    if (i.IsModified || i.IsDocumentDirty)
                        return; // There might be a new version of this file
                }
                else if (i.IsIgnored)
                    continue;
                else if (i.InSolution)
                    return; // The file is 'to be added'
            }          

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IPendingChangesManager pcm = e.GetService<IPendingChangesManager>();
            Dictionary<string, PendingChange> changes = new Dictionary<string, PendingChange>(StringComparer.OrdinalIgnoreCase);

            foreach (PendingChange pc in pcm.GetAll())
            {
                if (!changes.ContainsKey(pc.FullPath))
                    changes.Add(pc.FullPath, pc);
            }

            Dictionary<string, SvnItem> selectedChanges = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if(changes.ContainsKey(item.FullPath) &&
                    !selectedChanges.ContainsKey(item.FullPath))
                {
                    selectedChanges.Add(item.FullPath, item);
                }
            }

            // make sure all files are saved
            IAnkhOpenDocumentTracker tracker = e.Context.GetService<IAnkhOpenDocumentTracker>();
            IFileStatusCache statusCache = e.Context.GetService<IFileStatusCache>();
            
            IAnkhSolutionSettings sc = e.GetService<IAnkhSolutionSettings>();

            Collection<SvnItem> resources = new Collection<SvnItem>();

            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            List<SvnItem> selectedItems = new List<SvnItem>(selectedChanges.Values);
            
            // TODO: Give the whole list to a refreshable dialog!
            foreach (SvnItem item in selectedItems)
            {
                PendingChange pc = changes[item.FullPath];

                if (pc.IsCleanupChange())
                    continue;

                resources.Add(item);
            }
            if (resources.Count == 0)
                return;

            ICollection<List<SvnItem>> repositories;
            List<SvnItem> allItems;
            PendingChangeCommitArgs pca = new PendingChangeCommitArgs();
            using(CommitDialog dlg = new CommitDialog())
            {
                dlg.LogMessage = storedLogMessage;
                dlg.Context = e.Context;
                dlg.Items = resources;
                dlg.CommitFilter += delegate { return true; };

                DialogResult dr = dlg.ShowDialog(e.Context);

                storedLogMessage = dlg.LogMessage;

                if(DialogResult.OK != dr)
                    return;

                pca.LogMessage = dlg.LogMessage;
                pca.KeepLocks = dlg.KeepLocks;
                pca.KeepChangeLists = dlg.KeepChangeLists;

                allItems = new List<SvnItem>(dlg.CommitItems);

                repositories = SortByWorkingCopy(allItems);
            }

            // we need to commit to each repository separately
            if (repositories == null)
            {
                throw new InvalidOperationException("One or more of the selected items are not in a working copy");
            }

            using (DocumentLock dl = tracker.LockDocuments(SvnItem.GetPaths(allItems), DocumentLockType.NoReload))
            {
                dl.MonitorChanges();

                foreach (List<SvnItem> items in repositories)
                {
                    List<PendingChange> chg = new List<PendingChange>();

                    foreach (SvnItem i in items)
                    {
                        PendingChange pc;

                        if (changes.TryGetValue(i.FullPath, out pc))
                            chg.Add(pc);
                    }

                    if (!e.GetService<IPendingChangeHandler>().Commit(chg, pca))
                    {
                        // TODO: Failure!
                        return;
                    }
                }

                dl.ReloadModified();
            }

            // not in the finally, because we want to preserve the message for a 
            // non-successful commit
            this.storedLogMessage = null;
        }

        /// <summary>
        /// Sort the items by working copy.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        static ICollection<List<SvnItem>> SortByWorkingCopy(IEnumerable<SvnItem> items)
        {
            Dictionary<SvnWorkingCopy, List<SvnItem>> wcs = new Dictionary<SvnWorkingCopy, List<SvnItem>>();
            foreach (SvnItem item in items)
            {
                SvnWorkingCopy wc = item.WorkingCopy;

                if (wc == null)
                    return null;

                if (!wcs.ContainsKey(wc))
                    wcs.Add(wc, new List<SvnItem>());

                wcs[wc].Add(item);
            }

            return wcs.Values;
        }  
    }
}
