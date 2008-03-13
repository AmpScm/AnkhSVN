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

namespace Ankh.Commands
{
    /// <summary>
    /// Command to commit selected items to the Subversion repository.
    /// </summary>
    [Command(AnkhCommand.CommitItem)]
    public class CommitItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(true))
            {
                if (i.IsModified)
                    return;
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            // make sure all files are saved
            SaveAllDirtyDocuments( context );

            Collection<SvnItem> resources = new Collection<SvnItem>();

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsModified)
                    resources.Add(item);
            }

            CommitOperation operation = new CommitOperation( new SimpleProgressWorker(
                new SimpleProgressWorkerCallback(this.DoCommit)), resources, context);

            operation.LogMessage = this.storedLogMessage;

            // bail out if the user cancels
            bool cancelled = !operation.ShowLogMessageDialog();
            this.storedLogMessage = operation.LogMessage;
            if ( cancelled )
                return;

            this.commitContext = operation.CommitContext;

            // we need to commit to each repository separately
            ICollection repositories = this.SortByRepository( e.Context, operation.Items );           

            this.commitInfo = null;
            
            foreach( IList items in repositories )
            {
                string startText = "Committing ";
                if ( repositories.Count > 1 && items.Count > 0 )
                    startText += "to repository " + ((SvnItem)items[0]).Status.WorkingCopyInfo.RepositoryUri;
                using (context.StartOperation(startText))
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
                        context.OutputPane.WriteLine("Commit aborted");
                        throw;
                    }
                    finally
                    {
                        if (this.commitInfo != null)
                            context.OutputPane.WriteLine("\nCommitted revision {0}.",
                                this.commitInfo.Revision);
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
            SvnCommitArgs args = new SvnCommitArgs();
            args.LogMessage = storedLogMessage;
            args.Depth = SvnDepth.Infinity;
            args.KeepLocks = commitContext.KeepLocks;
            e.Client.Commit(this.paths, args, out commitInfo);
        }

        /// <summary>
        /// Sort the items by repository.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private ICollection SortByRepository( AnkhContext context, IList items )
        {
            Hashtable repositories = new Hashtable();
            foreach( SvnItem item in items )
            {
                string uuid = this.GetUuid( context, item );

                // give up on this one
                if ( uuid == null )
                    uuid = DefaultUuid;

                if ( !repositories.ContainsKey(uuid) )
                {
                    repositories.Add( uuid, new ArrayList() ); 
                }
                ((IList)repositories[uuid]).Add( item );
            }

            return repositories.Values;
        }

        private string GetUuid( AnkhContext context, SvnItem item )
        {
            string uuid = item.Status.WorkingCopyInfo != null ? item.Status.WorkingCopyInfo.RepositoryId.ToString() : null;
            // freshly added items have no uuid
            if ( uuid == null )
            {
                string parentDir = PathUtils.GetParent( item.Path );
                if ( Directory.Exists( parentDir ) )
                {
                    IFileStatusCache statusCache = context.GetService<IFileStatusCache>();
                    SvnItem parentItem = statusCache[parentDir];
                    uuid = parentItem.Status.WorkingCopyInfo != null ? parentItem.Status.WorkingCopyInfo.RepositoryId.ToString() : null;

                    // still nothing? try the parent item
                    if ( uuid == null )
                        return this.GetUuid( context, parentItem );
                }   
                else
                {
                    return null; 
                }                    
            }
            return uuid;
        }

        private string[] paths;
        private SvnCommitResult commitInfo;
        private CommitContext commitContext;
        private string storedLogMessage = null;

        private static readonly string DefaultUuid = Guid.NewGuid().ToString();
    }
}