// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections;

using SharpSvn;
using Ankh.Ids;
using Ankh.ContextServices;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to resolve conflict between changes.
    /// </summary>
    //[Command(AnkhCommand.ResolveConflict)]
    class ResolveConflictCommand : CommandBase
    {
        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected virtual string GetExe(IAnkhServiceProvider context)
        {
            return null; // Internal resolve conflict doesn't have an exe
        }

        

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsConflicted)
                    return;
            }

            e.Enabled = e.Visible = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            /*SaveAllDirtyDocuments( e.Selection, context );

            using(context.StartOperation( "Resolving" ))
            {
                IList items = context.Selection.GetSelectionResources(false, 
                     new ResourceFilterCallback(SvnItem.ConflictedFilter) );

                foreach( SvnItem item in items )
                {
                    this.Resolve( context, item );
                    item.MarkDirty();
                }
            }*/
        }

        /// <summary>
        /// Resolve an item.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="item"></param>
        private void Resolve(IAnkhServiceProvider context, SvnItem item)
        {
            // TODO: Retrieve live information instead of using the cache
            // BH: We don't cache data to use it on only 1 location


            /*string mergeExe = GetExe( context );
            SvnWorkingCopyInfo entry = item.Status.WorkingCopyInfo;
            SvnWorkingCopyState state;
            bool binary = false;

            using (SvnClient client = context.ClientPool.GetNoUIClient())
            {
                if (client.GetWorkingCopyState(item.Path, out state))
                    binary = !state.IsTextFile;
                if (binary || mergeExe == null)
                {
                    string selection;

                    using (ConflictDialog dialog = new ConflictDialog())
                    {
                        entry = item.Status.WorkingCopyInfo;
                        dialog.Filenames = new string[]{
                                                       entry.ConflictWorkFile,
                                                       entry.ConflictNewFile,
                                                       entry.ConflictOldFile,
                                                       item.Path
                                                   };
                        dialog.Binary = binary;

                        if (dialog.ShowDialog(context.GetService<IAnkhDialogOwner>().DialogOwner) != DialogResult.OK)
                            return;

                        selection = dialog.Selection;
                    }

                    if (selection != item.Path)
                        this.Copy(item.Path, selection);

                    SvnResolvedArgs args = new SvnResolvedArgs();
                    args.Depth = SvnDepth.Empty;
                    client.Resolved(item.Path, args);
                    context.OutputPane.WriteLine(
                        "Resolved conflicted state of {0}", item.Path);

                    // delete the associated conflict task item
                    context.ConflictManager.RemoveTaskItem(item.Path);

                }
                else
                {
                    string itemPath = Path.GetDirectoryName(item.Path);
                    string oldPath = String.Format("\"{0}\"", Path.Combine(itemPath, item.Status.WorkingCopyInfo.ConflictOldFile));
                    string newPath = String.Format("\"{0}\"", Path.Combine(itemPath, item.Status.WorkingCopyInfo.ConflictNewFile));
                    string workingPath = String.Format("\"{0}\"", Path.Combine(itemPath, item.Status.WorkingCopyInfo.ConflictWorkFile));
                    string mergedPath = String.Format("\"{0}\"", item.Path);

                    string mergeString = mergeExe;
                    mergeString = mergeString.Replace("%merged", mergedPath);
                    mergeString = mergeString.Replace("%base", oldPath);
                    mergeString = mergeString.Replace("%theirs", newPath);
                    mergeString = mergeString.Replace("%mine", workingPath);

                    // We can't use System.Diagnostics.Process here because we want to keep the
                    // program path and arguments together, which it doesn't allow.
                    Utils.Exec exec = new Utils.Exec();
                    exec.ExecPath(mergeString);
                    exec.WaitForExit();

                    if (MessageBox.Show("Have all conflicts been resolved?",
                        "Resolve", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        SvnResolvedArgs args = new SvnResolvedArgs();
                        args.Depth = SvnDepth.Empty;
                        client.Resolved(item.Path, args);
                    }
                }
            }*/
        }

        private void Copy(string toPath, string fromFile)
        {
            string dir = Path.GetDirectoryName(toPath);
            string fromPath = Path.Combine(dir, fromFile);
            File.Copy(fromPath, toPath, true);
        }


        private readonly Regex NUMBER = new Regex(@".*\.r(\d+)");

    }
}




