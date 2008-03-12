// $Id$
using System;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections;

using SharpSvn;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to resolve conflict between changes.
    /// </summary>
    [Command(AnkhCommand.ResolveConflict)]
    public class ResolveConflictCommand : CommandBase
    {    
        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected virtual string GetExe( Ankh.IContext context )
        {
            if ( !context.Config.ChooseDiffMergeManual )
                return context.Config.MergeExePath;
            else
                return null;
        }

        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.Status.LocalContentStatus == SvnStatus.Conflicted)
                    return;
            }
            
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            SaveAllDirtyDocuments( context );

            using(context.StartOperation( "Resolving" ))
            {
                IList items = context.Selection.GetSelectionResources(false, 
                     new ResourceFilterCallback(SvnItem.ConflictedFilter) );

                foreach( SvnItem item in items )
                {
                    this.Resolve( context, item );
                    item.MarkDirty();
                }
            }
        }

        #endregion

        /// <summary>
        /// Resolve an item.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="item"></param>
        private void Resolve(IContext context, SvnItem item)
        {

            string mergeExe = GetExe( context );
            SvnWorkingCopyInfo entry = item.Status.WorkingCopyInfo;
            SvnWorkingCopyState state;
            bool binary = false;
            if (context.Client.GetWorkingCopyState(item.Path, out state))
                binary = !state.IsTextFile;
            if ( binary || mergeExe == null )
            {
                string selection;

                using( ConflictDialog dialog = new ConflictDialog(  ) )
                {
                    entry = item.Status.WorkingCopyInfo;
                    dialog.Filenames = new string[]{
                                                       entry.ConflictWorkFile,
                                                       entry.ConflictNewFile,
                                                       entry.ConflictOldFile,
                                                       item.Path
                                                   };
                    dialog.Binary = binary;

                    if ( dialog.ShowDialog( context.HostWindow ) != DialogResult.OK )
                        return;
                        
                    selection = dialog.Selection;
                }   
    
                if ( selection != item.Path )
                    this.Copy( item.Path, selection );

                SvnResolvedArgs args = new SvnResolvedArgs();
                args.Depth = SvnDepth.Empty;
                context.Client.Resolved( item.Path, args);
                context.OutputPane.WriteLine( 
                    "Resolved conflicted state of {0}", item.Path );
                
                // delete the associated conflict task item
                context.ConflictManager.RemoveTaskItem(item.Path);

            }
            else
            {
                string itemPath = Path.GetDirectoryName( item.Path );
                string oldPath = String.Format("\"{0}\"", Path.Combine( itemPath, item.Status.WorkingCopyInfo.ConflictOldFile ));
                string newPath = String.Format("\"{0}\"", Path.Combine( itemPath, item.Status.WorkingCopyInfo.ConflictNewFile ));
                string workingPath = String.Format("\"{0}\"", Path.Combine( itemPath, item.Status.WorkingCopyInfo.ConflictWorkFile ));
                string mergedPath = String.Format("\"{0}\"", item.Path);

                string mergeString = mergeExe;
                mergeString = mergeString.Replace( "%merged", mergedPath );
                mergeString = mergeString.Replace( "%base", oldPath );
                mergeString = mergeString.Replace( "%theirs", newPath );
                mergeString = mergeString.Replace( "%mine", workingPath );

                // We can't use System.Diagnostics.Process here because we want to keep the
                // program path and arguments together, which it doesn't allow.
                Utils.Exec exec = new Utils.Exec();
                exec.ExecPath( mergeString );
                exec.WaitForExit();

                if ( MessageBox.Show( "Have all conflicts been resolved?",
                    "Resolve", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
                {
                    SvnResolvedArgs args = new SvnResolvedArgs();
                    args.Depth = SvnDepth.Empty;
                    context.Client.Resolved( item.Path, args );
                }
            }
        }        

        private void Copy( string toPath, string fromFile )
        {
            string dir = Path.GetDirectoryName( toPath );
            string fromPath = Path.Combine( dir, fromFile );
            File.Copy( fromPath, toPath,  true );
        }

      
        private readonly Regex NUMBER = new Regex( @".*\.r(\d+)" );

    }
}




