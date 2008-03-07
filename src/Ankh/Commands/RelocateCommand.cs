using System;
using Ankh.UI;
using System.Windows.Forms;

using SharpSvn;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to relocate this file.
    /// </summary>
    [VSNetCommand(AnkhCommand.Relocate,
		"Relocate",
         Text = "Relo&cate...",
         Tooltip = "Relocate this file.", 
         Bitmap = ResourceBitmaps.Relocate),
         VSNetFolderNodeControl( VSNetControlAttribute.AnkhSubMenu, Position = 7),
         VSNetControl( "Solution." + VSNetControlAttribute.AnkhSubMenu, Position = 1 ),
         VSNetControl("WorkingCopyExplorer." + VSNetControlAttribute.AnkhSubMenu, Position = 1),
         VSNetProjectNodeControl( VSNetControlAttribute.AnkhSubMenu, Position = 7 )]
    public class RelocateCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            if ( context.Selection.GetSelectionResources( false, 
                new ResourceFilterCallback(SvnItem.DirectoryFilter) ).Count == 1 )
            {
                return Enabled;
            }
            else
            {
                return Disabled;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            // We know now that there is exactly one resource
            SvnItem dir = (SvnItem)context.Selection.GetSelectionResources(
                false, new ResourceFilterCallback(SvnItem.DirectoryFilter) )[0];

            context.StartOperation( "Relocating" );
            try
            {
                using( RelocateDialog dlg = new RelocateDialog() )
                {
                    dlg.CurrentUrl = dir.Status.Uri.ToString();
                    if ( dlg.ShowDialog() != DialogResult.OK )
                        return;

                    // we need it on another thread because it actually
                    // contacts the repos to verify 
                    RelocateRunner runner = new RelocateRunner(
                        dir.Path, new Uri(dlg.FromSegment), new Uri(dlg.ToSegment), 
                        dlg.Recursive );

                    context.UIShell.RunWithProgressDialog( runner, "Relocating" );

                    dir.Refresh( context.Client );
                }
            }
            finally
            {
                context.EndOperation();
            }
        }

        #endregion

        /// <summary>
        /// Progress runner for the relocate operation.
        /// </summary>
        private class RelocateRunner : IProgressWorker
        {
            public RelocateRunner(string path, Uri from, Uri to,
                bool recursive)
            {
                this.path = path;
                this.from = from;
                this.to = to;
                this.recursive = recursive;
            }

            public void Work(IContext context)
            {
                SvnRelocateArgs args = new SvnRelocateArgs();
                args.NonRecursive = !recursive;
                context.Client.Relocate(this.path, this.from, this.to);
            }


            private string path;
            private Uri from, to;
            private bool recursive;
        }
    }
}
