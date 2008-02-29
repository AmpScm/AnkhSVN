// $Id$
using System;
using System.Collections;
using System.Windows.Forms;
using Ankh.UI;


using SharpSvn;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to switch current item to a different URL.
    /// </summary>
    [VSNetCommand(AnkhCommand.SwitchItem,
		"SwitchItem",
         Text = "&Switch...", 
         Tooltip = "Switch this item to a different URL.", 
         Bitmap = ResourceBitmaps.Switch ),
         VSNetItemControl( VSNetControlAttribute.AnkhSubMenu, Position = 7 )]
    public class SwitchItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            IList resources = context.Selection.GetSelectionResources(
                false, new ResourceFilterCallback( SvnItem.VersionedFilter ) );
            if ( resources.Count > 0 )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            this.SaveAllDirtyDocuments( context );

            IList resources = context.Selection.GetSelectionResources(
                false, new ResourceFilterCallback( SvnItem.VersionedFilter ) );

            if ( resources.Count == 0 )
                return;

            SwitchDialogInfo info = new SwitchDialogInfo( resources, 
                new object[]{resources[0]} );

            info = context.UIShell.ShowSwitchDialog( info );

            if ( info == null ) 
                return;

            context.StartOperation( "Switching" );
            context.ProjectFileWatcher.StartWatchingForChanges();
            try
            {
                SwitchRunner runner = new SwitchRunner(info.Path, new Uri(info.SwitchToUrl), 
                    info.RevisionStart, info.Depth );
                context.UIShell.RunWithProgressDialog( runner, "Switching" );
                if ( !context.ReloadSolutionIfNecessary() )
                {
                    context.Selection.RefreshSelection();
                }
            }
            finally
            {
                context.EndOperation();
            }
        }

        #endregion

        /// <summary>
        /// A progress runner that runs the switch operation.
        /// </summary>
        private class SwitchRunner : IProgressWorker
        {
            public SwitchRunner(string path, Uri url, SvnRevision revision,
                SvnDepth depth)
            {
                this.path = path;
                this.url = url;
                this.revision = revision;
                this.depth = depth;
            }

            public void Work(IContext context)
            {
                SvnSwitchArgs args = new SvnSwitchArgs();
                args.Revision = revision;
                args.Depth = depth;
                context.Client.Switch(this.path, this.url, args);
            }

            private string path;
            private Uri url;
            private SvnRevision revision;
            private SvnDepth depth;
        }
    }
}