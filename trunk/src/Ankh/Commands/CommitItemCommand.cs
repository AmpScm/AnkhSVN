// $Id$
using Ankh.UI;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;

using NSvn.Core;
using EnvDTE;

namespace Ankh.Commands
{
    /// <summary>
    /// Commits an item. 
    /// </summary>
    [VSNetCommand("CommitItem", Text = "Commit...", Tooltip = "Commits an item",
         Bitmap = ResourceBitmaps.Commit),
    VSNetControl( "Item", Position = 2 ),
    VSNetProjectNodeControl( "", Position = 2 ),
    VSNetFolderNodeControl( "", Position = 2),
    VSNetControl( "Solution", Position = 2)]
    public class CommitItemCommand : CommandBase
    {	
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.IContext context)
        {
            if ( context.SolutionExplorer.GetSelectionResources( true, 
                new ResourceFilterCallback( CommandBase.ModifiedFilter) ).Count > 0 )
                return Enabled;
            else
                return Disabled;
        }
        public override void Execute(Ankh.IContext context, string parameters)
        {
            // make sure all files are saved
            this.SaveAllDirtyDocuments( context );

            IList resources = context.SolutionExplorer.GetSelectionResources( true, 
                new ResourceFilterCallback(CommandBase.ModifiedFilter) );

            resources = context.Client.ShowLogMessageDialog( resources, false );

            // did the user cancel?
            if ( resources == null ) 
                return;

            // we need to commit to each repository separately
            ICollection repositories = this.SortByRepository( context, resources );           

            this.commitInfo = null;
            
            foreach( IList items in repositories )
            {
                string startText = "Committing ";
                if ( repositories.Count > 1 && items.Count > 0 )
                    startText += "to repository " + ((SvnItem)items[0]).Status.Entry.Uuid;
                context.StartOperation( startText );

                try
                {
                    this.paths = SvnItem.GetPaths( items );

                    bool completed = context.UIShell.RunWithProgressDialog( 
                        new SimpleProgressWorker( 
                            new SimpleProgressWorkerCallback( this.DoCommit ) ), "Committing" );

                    if(completed)
                    {
                        foreach( SvnItem item in resources )
                            item.Refresh( context.Client );                        
                    }
                }
           
                catch( NSvn.Common.SvnException )
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
            context.Client.CommitCompleted();
        }        
        #endregion

        private void DoCommit( IContext context )
        {
            this.commitInfo = context.Client.Commit( this.paths, true );
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
                string uuid = item.Status.Entry.Uuid;

                // freshly added items have no uuid
                if ( uuid == null )
                {
                    string parentDir = Path.GetDirectoryName( item.Path );
                    SvnItem parentItem = context.StatusCache[parentDir];
                    uuid = parentItem.Status.Entry.Uuid;
                }

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

        private string[] paths;
        private CommitInfo commitInfo;

        private const string DefaultUuid = Guid.NewGuid().ToString();
        
    }
}



