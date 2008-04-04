// $Id$
using Ankh.UI;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;
using Utils;
using SharpSvn;
using AnkhSvn.Ids;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ankh.Scc;
using Ankh.Selection;

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

        static readonly string DefaultUuid = Guid.NewGuid().ToString();

        SvnCommitArgs _args;

        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IAnkhOpenDocumentTracker documentTracker = e.Context.GetService<IAnkhOpenDocumentTracker>();
            IProjectFileMapper projectMapper = e.Context.GetService<IProjectFileMapper>();

            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(true))
            {
                if (i.IsVersioned)
                {
                    if (i.IsModified || documentTracker.IsDocumentDirty(i.FullPath))
                        return; // There might be a new version of this file
                }
                else if (i.IsIgnored)
                    continue;
                else if (projectMapper.ContainsPath(i.FullPath))
                    return; // The file is 'to be added'
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            // make sure all files are saved
            IAnkhOpenDocumentTracker tracker = e.Context.GetService<IAnkhOpenDocumentTracker>();
            IFileStatusCache statusCache = e.Context.GetService<IFileStatusCache>();
            IProjectFileMapper projectMapper = e.Context.GetService<IProjectFileMapper>();

            tracker.SaveDocuments(e.Selection.GetSelectedFiles(true));

            Collection<SvnItem> resources = new Collection<SvnItem>();

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned && !item.IsModified)
                    continue; // Check for dirty files is not necessary here, because we just saved the dirty documents
                else if (item.IsIgnored)
                    continue;
                else if (!projectMapper.ContainsPath(item.FullPath))
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

            CommitOperation operation = new CommitOperation(
                _args,
                new SimpleProgressWorker(new SimpleProgressWorkerCallback(this.DoCommit)),
                resources,
                e.Context);

            operation.LogMessage = this.storedLogMessage;

            // bail out if the user cancels
            bool cancelled = !operation.ShowLogMessageDialog();
            this.storedLogMessage = operation.LogMessage;
            if (cancelled)
                return;

            // we need to commit to each repository separately
            ICollection<List<SvnItem>> repositories = this.SortByRepository(e.Context, operation.Items);

            this.commitInfo = null;

			foreach (List<SvnItem> items in repositories)
            {
                string startText = "Committing ";
                if (repositories.Count > 1 && items.Count > 0)
                    startText += "to repository " + ((SvnItem)items[0]).Status.Uri;
                using (e.Context.BeginOperation(startText))
                {
                    try
                    {
                        this.paths = SvnItem.GetPaths(items);

                        bool completed = operation.Run("Committing");

                        if (completed)
                        {
                            foreach (SvnItem item in items)
                                item.MarkDirty();
                        }
                    }
                    catch (SvnException)
                    {
                        //context.OutputPane.WriteLine("Commit aborted");
                        throw;
                    }
                    finally
                    {
                        //if (this.commitInfo != null)
                        //    context.OutputPane.WriteLine("\nCommitted revision {0}.",
                        //        this.commitInfo.Revision);
                    }
                }
            }

            // not in the finally, because we want to preserve the message for a 
            // non-successful commit
            this.storedLogMessage = null;
        }

        #endregion

        private void DoCommit(AnkhWorkerArgs e)
        {
            IFileStatusCache statusCache = e.Context.GetService<IFileStatusCache>();
            IProjectFileMapper projectMap = e.Context.GetService<IProjectFileMapper>();
            LinkedList<string> paths = new LinkedList<string>();

            _args.ThrowOnError = false;
            _args.Notify += delegate(object sender, SvnNotifyEventArgs ne)
            {
                statusCache.MarkDirty(ne.FullPath);
                paths.AddLast(ne.FullPath);
            };
            e.Client.Commit(this.paths, _args, out commitInfo);

            IProjectNotifier pn = e.Context.GetService<IProjectNotifier>();
            if (pn != null)
                pn.MarkDirty(projectMap.GetAllProjectsContaining(paths));
        }

        /// <summary>
        /// Sort the items by repository.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private ICollection<List<SvnItem>> SortByRepository(AnkhContext context, ICollection<SvnItem> items)
        {
            Dictionary<string, List<SvnItem>> repositories = new Dictionary<string, List<SvnItem>>(StringComparer.OrdinalIgnoreCase);
            foreach (SvnItem item in items)
            {
                string uuid = this.GetUuid(context, item);

                // give up on this one
                if (uuid == null)
                    uuid = DefaultUuid;

                if (!repositories.ContainsKey(uuid))
                {
                    repositories.Add(uuid, new List<SvnItem>());
                }
                repositories[uuid].Add(item);
            }

            return repositories.Values;
        }

        private string GetUuid(AnkhContext context, SvnItem item)
        {
            string uuid = item.Status.RepositoryId;
            // freshly added items have no uuid
            if (uuid == null)
            {
                string parentDir = item.Parent.FullPath;
                if (Directory.Exists(parentDir))
                {
                    IFileStatusCache statusCache = context.GetService<IFileStatusCache>();
                    SvnItem parentItem = statusCache[parentDir];
                    uuid = parentItem.Status.RepositoryId;

                    // still nothing? try the parent item
                    if (uuid == null)
                        return this.GetUuid(context, parentItem);
                }
                else
                {
                    return null;
                }
            }
            return uuid;
        }
    }
}
