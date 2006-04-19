using System;
using System.Collections;
using NSvn.Core;

namespace Ankh.Commands
{
    /// <summary>
    /// A command class to support the svn lock command
    /// </summary>
    [VSNetCommand("Lock", Text = "Lock...", Tooltip = "Locks the selected item",
         Bitmap = ResourceBitmaps.Lock),
    VSNetProjectItemControl( "Ankh", Position = 1 ),
    VSNetProjectNodeControl( "Ankh", Position = 1 ),
    VSNetControl( "Solution.Ankh", Position = 1)]    
    public class LockCommand : CommandBase
	{
        public override void Execute(IContext context, string parameters)
        {
            IList resources = context.SolutionExplorer.GetSelectionResources(true, 
                new ResourceFilterCallback( CommandBase.NotLockedAndLockableFilter ) );

            this.info = new LockDialogInfo( resources, resources );
            
            // is Shift down?
            if ( !CommandBase.Shift )
            {
                this.info = context.UIShell.ShowLockDialog( this.info );
                if( this.info == null)
                    return;
            }

            context.UIShell.RunWithProgressDialog( new SimpleProgressWorker( 
                new SimpleProgressWorkerCallback( this.ProgressCallback ) ), "Locking files" );
            foreach( SvnItem item in info.CheckedItems )
                item.Refresh( context.Client );
        }

        private void ProgressCallback( IContext context )
        {
            string[] paths = SvnItem.GetPaths( info.CheckedItems );
            
            context.Client.Lock( paths, this.info.Message, this.info.StealLocks );
        }

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            IList resources = context.SolutionExplorer.GetSelectionResources( true,
                new ResourceFilterCallback(CommandBase.NotLockedAndLockableFilter) );
            return resources.Count > 0 ? Enabled : Disabled;
        }
        
        private LockDialogInfo info;
	}
}
