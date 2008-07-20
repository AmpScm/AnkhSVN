// $Id$
using Ankh.UI;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;
using Utils;
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
    public class CommitItemCommand : CommandBase
    {
        ICollection<string> paths;
        SvnCommitResult commitInfo;
        string storedLogMessage = null;

        SvnCommitArgs _args;

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

            if (e.Command == AnkhCommand.ProjectCommit)
            {
                IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();
                IPendingChangesManager pcm = e.GetService<IPendingChangesManager>();

                if (pfm != null && pcm != null)
                    foreach (SvnProject p in e.Selection.GetSelectedProjects(true))
                    {
                        ISvnProjectInfo pi = pfm.GetProjectInfo(p);

                        if (pi != null && !string.IsNullOrEmpty(pi.ProjectDirectory))
                        {
                            foreach (PendingChange pc in pcm.GetAllBelow(pi.ProjectDirectory))
                            {
                                if (!pfm.ContainsPath(pc.FullPath))
                                    return;
                            }
                        }
                    }
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            // make sure all files are saved
            IAnkhOpenDocumentTracker tracker = e.Context.GetService<IAnkhOpenDocumentTracker>();
            IFileStatusCache statusCache = e.Context.GetService<IFileStatusCache>();
            IProjectFileMapper projectMapper = e.Context.GetService<IProjectFileMapper>();
            IPendingChangesManager pcm = e.GetService<IPendingChangesManager>();
            IAnkhSolutionSettings sc = e.GetService<IAnkhSolutionSettings>();

            tracker.SaveDocuments(e.Selection.GetSelectedFiles(true));

            Collection<SvnItem> resources = new Collection<SvnItem>();

            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            List<SvnItem> selectedItems = new List<SvnItem>();
            HybridCollection<string> ticked;
            switch (e.Command)
            {
                case AnkhCommand.SolutionCommit:
                    ticked = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (string file in mapper.GetAllFilesOfAllProjects())
                    {
                        if (ticked.Contains(file))
                            continue;

                        ticked.Add(file);
                        SvnItem item = cache[file];

                        if (item != null)
                            selectedItems.Add(item);
                    }

                    if (!pcm.IsActive)
                        pcm.IsActive = true;

                    if (!string.IsNullOrEmpty(sc.ProjectRoot))
                    {
                        foreach (PendingChange pc in pcm.GetAllBelow(sc.ProjectRoot))
                        {
                            if (!ticked.Contains(pc.FullPath))
                            {
                                ticked.Add(pc.FullPath);

                                if (!projectMapper.ContainsPath(pc.FullPath))
                                    selectedItems.Add(pc.Item);
                            }
                        }
                    }

                    break;
                case AnkhCommand.ProjectCommit:
                    // TODO: Fetch all selecteded objects and feed them into a selection dialog with refresh option
                    ticked = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (string file in mapper.GetAllFilesOf(new List<SvnProject>(e.Selection.GetSelectedProjects(false))))
                    {
                        if (ticked.Contains(file))
                            continue;

                        ticked.Add(file);

                        SvnItem item = cache[file];

                        if (item != null)
                            selectedItems.Add(item);
                    }
                    if (!pcm.IsActive)
                        pcm.IsActive = true;

                    foreach (SvnProject p in e.Selection.GetSelectedProjects(true))
                    {
                        ISvnProjectInfo pi = projectMapper.GetProjectInfo(p);

                        if (pi != null && !string.IsNullOrEmpty(pi.ProjectDirectory))
                        {
                            foreach (PendingChange pc in pcm.GetAllBelow(pi.ProjectDirectory))
                            {
                                if (!ticked.Contains(pc.FullPath))
                                {
                                    ticked.Add(pc.FullPath);

                                    if (!projectMapper.ContainsPath(pc.FullPath))
                                        selectedItems.Add(pc.Item);
                                }
                            }
                        }
                    }
                    break;
                case AnkhCommand.CommitItem:
                    ticked = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (string file in e.Selection.GetSelectedFiles(true))
                    {
                        if (ticked.Contains(file))
                            continue;

                        ticked.Add(file);

                        SvnItem item = cache[file];

                        if (item != null)
                            selectedItems.Add(item);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }

            // TODO: Give the whole list to a refreshable dialog!
            foreach (SvnItem item in selectedItems)
            {
                if (item.IsVersioned && !item.IsModified)
                    continue; // Check for dirty files is not necessary here, because we just saved the dirty documents
                else if (item.IsIgnored)
                    continue;
                else if (!item.InSolution && !pcm.ContainsPath(item.FullPath)) // Hmm?
                    continue;

                if (!resources.Contains(item))
                    resources.Add(item);

                if (!item.IsVersioned || item.Status.LocalContentStatus == SvnStatus.Added)
                {
                    SvnItem parent = item.Parent;
                    while (parent != null && parent.IsVersioned && parent.Status.LocalContentStatus == SvnStatus.Added)
                    {
                        if (!resources.Contains(parent))
                            resources.Add(parent);

                        parent = parent.Parent;
                    }
                }
            }
            if (resources.Count == 0)
                return;

            _args = new SvnCommitArgs();

            CommitOperation operation = new CommitOperation(e.Context, resources, _args, this.DoCommit);

            operation.LogMessage = this.storedLogMessage;

            // bail out if the user cancels
            bool cancelled = !operation.ShowLogMessageDialog();
            this.storedLogMessage = operation.LogMessage;
            if (cancelled)
                return;

            // we need to commit to each repository separately
            ICollection<List<SvnItem>> repositories = SortByWorkingCopy(operation.SelectedItems);

            if (repositories == null)
            {
                throw new InvalidOperationException("One or more of the selected items are not in a working copy");
            }

            this.commitInfo = null;

            using (DocumentLock dl = tracker.LockDocuments(SvnItem.GetPaths(operation.SelectedItems), DocumentLockType.NoReload))
            {
                dl.MonitorChanges();

                foreach (List<SvnItem> items in repositories)
                {
                    string startText = "Committing ";
                    if (repositories.Count > 1 && items.Count > 0)
                        startText += "to repository " + ((SvnItem)items[0]).Status.Uri;
                    using (e.Context.BeginOperation(startText))
                    {
                        this.paths = SvnItem.GetPaths(items);

                        bool completed = operation.Run("Committing");
                    }
                }

                dl.ReloadModified();
            }

            // not in the finally, because we want to preserve the message for a 
            // non-successful commit
            this.storedLogMessage = null;
        }



        private void DoCommit(object sender, ProgressWorkerArgs e)
        {
            e.Client.Commit(this.paths, _args, out commitInfo);
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
