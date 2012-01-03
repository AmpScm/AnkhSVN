using System;
using System.Collections;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to unlock the selected items.
    /// </summary>
    [VSNetCommand("Unlock",
         Text = "U&nlock...",
         Tooltip = "Unlock the selected items.",
         Bitmap = ResourceBitmaps.Unlock),
         VSNetItemControl( VSNetControlAttribute.AnkhSubMenu, Position = 8 )] 
	public class UnlockCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            IList resources = context.Selection.GetSelectionResources(true, 
                new ResourceFilterCallback( SvnItem.LockedFilter ) );
            return resources.Count > 0 ? CommandBase.Enabled : CommandBase.Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            IList resources = context.Selection.GetSelectionResources(true, 
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

        #endregion

        private void DoUnlock( IContext context )
        {
            string[] paths = SvnItem.GetPaths( this.info.CheckedItems );
            context.Client.Unlock( paths, false );
        }

        private PathSelectorInfo info;
	}
}