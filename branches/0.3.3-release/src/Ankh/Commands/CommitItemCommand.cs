// $Id$
using Ankh.UI;
using System;
using System.Windows.Forms;
using NSvn;
using NSvn.Core;
using EnvDTE;

namespace Ankh.Commands
{
    /// <summary>
    /// Commits an item. 
    /// </summary>
    [VSNetCommand("CommitItem", Text = "Commit", Tooltip = "Commits an item",
         Bitmap = ResourceBitmaps.Commit),
    VSNetControl( "Item", Position = 2 ),
    VSNetControl( "Project", Position = 2 ),
    VSNetControl( "Folder", Position = 2 ),
    VSNetControl( "Solution", Position = 2)]
    internal class CommitItem : CommandBase
    {	
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            CommitCandidateVisitor v = new CommitCandidateVisitor();
            context.SolutionExplorer.VisitSelectedItems( v, true );
            if ( v.Commitable )
                return vsCommandStatus.vsCommandStatusEnabled |
                    vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusSupported;
        }
        public override void Execute(Ankh.AnkhContext context)
        {
            ResourceGathererVisitor v = new ResourceGathererVisitor();
            context.SolutionExplorer.VisitSelectedItems( v, true );

            this.context = context;
            WorkingCopyResource[] resources = (WorkingCopyResource[])
                v.WorkingCopyResources.ToArray( typeof(WorkingCopyResource) );            

            resources = context.Context.ShowLogMessageDialog( resources );

            // did the user cancel?
            if ( resources == null ) 
                return;

            CommitInfo commitInfo = null;

            try
            {
                context.StartOperation( "Committing..." );

                commitInfo = WorkingCopyResource.Commit( resources, true );
            }
            catch( NSvn.Common.SvnException )
            {
                this.context.OutputPane.WriteLine( "Commit aborted" );
                throw;
            }
            finally
            {
                if (commitInfo != null)
                    this.context.OutputPane.WriteLine("\nCommitted revision {0}.", commitInfo.Revision);
                this.context.OutputPane.EndActionText();

                context.EndOperation();
            }

            context.SolutionExplorer.UpdateSelectionStatus();
        }
        
        #endregion

        

        private class CommitCandidateVisitor : LocalResourceVisitorBase
        {
            public bool Commitable = false;

            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
            {
                if ( (resource.Status.TextStatus & commitCandidates) != 0 ||
                    (resource.Status.PropertyStatus & commitCandidates) != 0 )
                    Commitable = true;
            }

            private const StatusKind commitCandidates = StatusKind.Added | 
                StatusKind.Modified;
        }
        private AnkhContext context;

    }
}



