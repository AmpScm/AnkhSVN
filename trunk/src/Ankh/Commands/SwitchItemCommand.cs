// $Id$
using System;
using System.Collections;
using System.Windows.Forms;
using Ankh.UI;
using NSvn.Core;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that allows the user to switch an item to a different URL.
    /// </summary>
    [VSNetCommand("SwitchItem", Text="Switch...", 
         Tooltip= "Switch this item to a different URL", 
         Bitmap=ResourceBitmaps.Switch ),
    VSNetControl( "Solution.Ankh", Position=1 ),
    VSNetProjectNodeControl( "Ankh", Position = 1 ),
    VSNetFolderNodeControl( "Ankh", Position = 1),
    VSNetControl( "Item.Ankh", Position=1 )]
    public class SwitchItemCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            IList resources = context.SolutionExplorer.GetSelectionResources(
                false, new ResourceFilterCallback( CommandBase.VersionedFilter ) );
            if ( resources.Count > 0 )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(AnkhContext context, string parameters)
        {
            this.SaveAllDirtyDocuments( context );

            IList resources = context.SolutionExplorer.GetSelectionResources(
                false, new ResourceFilterCallback( CommandBase.VersionedFilter ) );

            string path = null;
            string url = null;
            Revision revision = null;
            bool recursive = false;

            // now let the user choose which item to switch
            using ( SwitchDialog dialog = new SwitchDialog() )
            {
                foreach( SvnItem item in resources )
                    dialog.AddItem( item.Path, item.Status.Entry.Url );

                if ( dialog.ShowDialog( context.HostWindow ) != DialogResult.OK )
                    return;
                path = dialog.Path;
                url = dialog.Url;
                revision = dialog.Revision;
                recursive = dialog.Recursive;
            }

            context.StartOperation( "Switching" );
            try
            {
                SwitchRunner runner = new SwitchRunner(path, url, revision,
                    recursive, context);
                runner.Start( "Switching" );
                context.SolutionExplorer.RefreshSelection();
            }
            finally
            {
                context.EndOperation();
            }
        }

        /// <summary>
        /// A progress runner that runs the switch operation.
        /// </summary>
        private class SwitchRunner : ProgressRunner
        {
            public SwitchRunner( string path, string url, Revision revision,
                bool recursive, AnkhContext context ) :
                base(context)
            {
                this.path = path;
                this.url = url;
                this.revision = revision;
                this.recursive = recursive;
            }

            protected override void DoRun()
            {
                this.Context.Client.Switch( this.path, this.url, 
                    this.revision, this.recursive );                
            }

            private string path;
            private string url;
            private Revision revision;
            private bool recursive;
        }
    }
}



