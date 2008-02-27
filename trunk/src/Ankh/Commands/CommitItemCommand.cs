// $Id$
using Ankh.UI;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;
using Utils;



using EnvDTE;
using SharpSvn;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to commit selected items to the Subversion repository.
    /// </summary>
    [VSNetCommand("CommitItem",
         Text = "&Commit...",
         Tooltip = "Commit selected items to the Subversion repository.",
         Bitmap = ResourceBitmaps.Commit),
         VSNetItemControl( "", Position = 2 )]
    public class CommitItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.IContext context)
        {
            if ( context.Selection.GetSelectionResources( true, 
                new ResourceFilterCallback( SvnItem.ModifiedFilter) ).Count > 0 )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(Ankh.IContext context, string parameters)
        {
            // make sure all files are saved
            this.SaveAllDirtyDocuments( context );

            IList resources = context.Selection.GetSelectionResources( true, 
                new ResourceFilterCallback(SvnItem.ModifiedFilter) );

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
            ICollection repositories = this.SortByRepository( context, operation.Items );           

            this.commitInfo = null;
            
            foreach( IList items in repositories )
            {
                string startText = "Committing ";
                if ( repositories.Count > 1 && items.Count > 0 )
                    startText += "to repository " + ((SvnItem)items[0]).Status.WorkingCopyInfo.RepositoryUri;
                context.StartOperation( startText );

                try
                {
                    this.paths = SvnItem.GetPaths( items );

                    bool completed = operation.Run( "Committing" );

                    if(completed)
                    {
                        foreach( SvnItem item in items )
                            item.Refresh( context.Client );                        
                    }
                }
           
                catch( SvnException )
                {
                    context.OutputPane.WriteLine( "Commit aborted" );
                    throw;
                }
                finally
                {
                    if (this.commitInfo != null)
                        context.OutputPane.WriteLine("\nCommitted revision {0}.", 
                            this.commitInfo.Revision);

                    context.EndOperation();
                }
            }

            // not in the finally, because we want to preserve the message for a 
            // non-successful commit
            this.storedLogMessage = null;
        }

        #endregion

        private void DoCommit(IContext context)
        {
            SvnCommitArgs args = new SvnCommitArgs();
            args.LogMessage = storedLogMessage;
            args.Depth = SvnDepth.Infinity;
            args.KeepLocks = commitContext.KeepLocks;
            context.Client.Commit(this.paths, args, out commitInfo);
        }

        /// <summary>
        /// Sort the items by repository.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private ICollection SortByRepository( IContext context, IList items )
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

        private string GetUuid( IContext context, SvnItem item )
        {
            string uuid = item.Status.WorkingCopyInfo != null ? item.Status.WorkingCopyInfo.RepositoryId.ToString() : null;
            // freshly added items have no uuid
            if ( uuid == null )
            {
                string parentDir = PathUtils.GetParent( item.Path );
                if ( Directory.Exists( parentDir ) )
                {
                    SvnItem parentItem = context.StatusCache[parentDir];
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