// $Id$
using Ankh.UI;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

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
    public class CommitItem : CommandBase
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

            this.commitInfo = null;

            try
            {

                context.StartOperation( "Committing" );
                this.paths = SvnItem.GetPaths( resources );

                ProgressRunner runner = new ProgressRunner( context, 
                    new ProgressRunnerCallback( this.DoCommit ) );
                runner.Start( "Committing" );

                if(!runner.Cancelled)
                {
                    foreach( SvnItem item in resources )
                        item.Refresh( context.Client );

                    context.Client.CommitCompleted();
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
        #endregion

        private void DoCommit( IContext context )
        {
            this.commitInfo = context.Client.Commit( this.paths, true );
        }

        private string[] paths;
        private CommitInfo commitInfo;

        
    }
}



