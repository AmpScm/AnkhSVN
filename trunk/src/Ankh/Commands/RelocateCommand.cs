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
    [Command(AnkhCommand.Relocate)]
    public class RelocateCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsDirectory && item.IsVersioned)
                    return;
            }            
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            // We know now that there is exactly one resource
            /*SvnItem dir = (SvnItem)context.Selection.GetSelectionResources(
                false, new ResourceFilterCallback(SvnItem.DirectoryFilter) )[0];

            using (context.StartOperation("Relocating"))
            {
                using (RelocateDialog dlg = new RelocateDialog())
                {
                    dlg.CurrentUrl = dir.Status.Uri.ToString();
                    if (dlg.ShowDialog() != DialogResult.OK)
                        return;

                    // we need it on another thread because it actually
                    // contacts the repos to verify 
                    RelocateRunner runner = new RelocateRunner(
                        dir.Path, new Uri(dlg.FromSegment), new Uri(dlg.ToSegment),
                        dlg.Recursive);

                    context.UIShell.RunWithProgressDialog(runner, "Relocating");

                    dir.MarkDirty();
                }
            }*/
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

            public void Work(AnkhWorkerArgs e)
            {
                SvnRelocateArgs args = new SvnRelocateArgs();
                args.NonRecursive = !recursive;
                e.Client.Relocate(this.path, this.from, this.to);
            }


            private string path;
            private Uri from, to;
            private bool recursive;
        }
    }
}
