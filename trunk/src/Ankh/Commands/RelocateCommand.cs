using System;
using Ankh.UI;
using System.Windows.Forms;

namespace Ankh.Commands
{
    /// <summary>
    /// A command to do the equivalent of svn switch --relocate
    /// </summary>
    [VSNetCommand("Relocate", Text = "Relocate...", Tooltip = "Rename this file...", 
         Bitmap = ResourceBitmaps.Refresh),
    VSNetControl( "Folder.Ankh", Position = 1 ),
    VSNetControl( "Solution.Ankh", Position = 1 ),
    VSNetControl( "Project.Ankh", Position = 1)]
    internal class RelocateCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            if ( context.SolutionExplorer.GetSelectionResources( false, 
                new ResourceFilterCallback(CommandBase.DirectoryFilter) ).Count == 1 )
            {
                return Enabled;
            }
            else
            {
                return Disabled;
            }
        }

        public override void Execute(AnkhContext context, string parameters)
        {
            // We know now that there is exactly one resource
            SvnItem dir = (SvnItem)context.SolutionExplorer.GetSelectionResources(
                false, new ResourceFilterCallback(CommandBase.DirectoryFilter) )[0];

            context.StartOperation( "Relocating" );
            try
            {
                using( RelocateDialog dlg = new RelocateDialog() )
                {
                    dlg.CurrentUrl = dir.Status.Entry.Url;
                    if ( dlg.ShowDialog() != DialogResult.OK )
                        return;

                    // we need it on another thread because it actually
                    // contacts the repos to verify 
                    RelocateRunner runner = new RelocateRunner(
                        context,
                        dir.Path, dlg.FromSegment, dlg.ToSegment, 
                        dlg.Recursive );

                    runner.Start( "Relocating" );

                    dir.Refresh( context.Client );
                }
            }
            finally
            {
                context.EndOperation();
            }
        }

        /// <summary>
        /// Progress runner for the relocate operation.
        /// </summary>
        private class RelocateRunner : ProgressRunner
        {
            public RelocateRunner( AnkhContext context, 
                string path, string from, string to, 
                bool recurse ) : base( context )
            {
                this.path = path;
                this.from = from; 
                this.to = to;
                this.recurse = recurse;
            }

            protected override void DoRun()
            {
                this.Context.Client.Relocate( this.path, this.from, this.to,
                    this.recurse );
            }


            private string path, from, to;
            private bool recurse;
        }
    }
}
