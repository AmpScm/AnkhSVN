using System;
using System.Collections;

namespace Ankh.Commands
{
    [VSNetCommand("Unlock", Text = "Unlock...", Tooltip = "Unlocks the selected item(s)",
         Bitmap = ResourceBitmaps.Unlock),
    VSNetProjectItemControl( "Ankh", Position = 1 ),
    VSNetProjectNodeControl( "Ankh", Position = 1 ),
    VSNetControl( "Solution.Ankh", Position = 1)] 
	public class UnlockCommand : CommandBase
	{
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            IList resources = context.SolutionExplorer.GetSelectionResources(true, 
                new ResourceFilterCallback( SvnItem.LockedFilter ) );
            return resources.Count > 0 ? CommandBase.Enabled : CommandBase.Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            IList resources = context.SolutionExplorer.GetSelectionResources(true, 
                new ResourceFilterCallback( SvnItem.LockedFilter ) );

            this.info = new PathSelectorInfo( "Unlock files", resources, resources );

            if ( !CommandBase.Shift )
            {
                this.info = context.UIShell.ShowPathSelector( this.info );

                if ( this.info == null )
                    return;
            }

            context.UIShell.RunWithProgressDialog( new SimpleProgressWorker(
                new SimpleProgressWorkerCallback( this.DoUnlock )), "Unlocking files" );
            
            foreach( SvnItem item in this.info.CheckedItems )
                item.Refresh( context.Client );
        }

        private void DoUnlock( IContext context )
        {
            string[] paths = SvnItem.GetPaths( this.info.CheckedItems );
            context.Client.Unlock( paths, false );
        }

        private PathSelectorInfo info;

	}
}
