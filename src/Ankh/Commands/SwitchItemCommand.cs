// $Id$
using System;
using System.Collections;
using System.Windows.Forms;
using Ankh.UI;
using NSvn.Core;
using NSvn.Common;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that allows the user to switch an item to a different URL.
    /// </summary>
    [VSNetCommand("SwitchItem", Text="Switch...", 
         Tooltip= "Switch this item to a different URL", 
         Bitmap=ResourceBitmaps.Switch ),
   VSNetItemControl( "Ankh", Position = 1 )]
    public class SwitchItemCommand : CommandBase
    {
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
                SwitchRunner runner = new SwitchRunner(info.Path, info.SwitchToUrl, 
                    info.RevisionStart, info.Recurse );
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

        /// <summary>
        /// A progress runner that runs the switch operation.
        /// </summary>
        private class SwitchRunner : IProgressWorker
        {
            public SwitchRunner( string path, string url, Revision revision,
                Recurse recurse )                 
            {
                this.path = path;
                this.url = url;
                this.revision = revision;
                this.recurse = recurse;
            }

            public void Work( IContext context )
            {
                context.Client.Switch( this.path, this.url, 
                    this.revision, this.recurse );
            }

            private string path;
            private string url;
            private Revision revision;
            private Recurse recurse;
        }
    }
}



