// $Id$
using System;
using EnvDTE;

using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections;
using NSvn.Core;

namespace Ankh.Commands
{
    /// <summary>
    /// Allows the user to resolve a conflicted file.
    /// </summary>
    [VSNetCommand( "ResolveConflict", Text="Resolve conflicted file...",  
         Bitmap = ResourceBitmaps.ResolveConflict, 
         Tooltip = "Resolve conflicted file"),
     VSNetProjectItemControl( "Ankh", Position = 1 ),
     VSNetProjectNodeControl( "Ankh", Position = 1 ),
     VSNetControl( "Solution.Ankh", Position = 1)]
    public class ResolveConflictCommand : CommandBase
    {    
        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        private string GetExe( Ankh.IContext context )
        {
            if ( !context.Config.ChooseDiffMergeManual )
                return context.Config.MergeExePath;
            else
                return null;
        }

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            int count = context.SolutionExplorer.GetSelectionResources(false, 
                new ResourceFilterCallback(ResolveConflictCommand.ConflictedFilter) ).Count;

            if ( count > 0 )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            this.SaveAllDirtyDocuments( context );

            context.StartOperation( "Resolving" );

            try
            {
                IList items = context.SolutionExplorer.GetSelectionResources(false, 
                     new ResourceFilterCallback(ResolveConflictCommand.ConflictedFilter) );

                foreach( SvnItem item in items )
                {
                    this.Resolve( context, item );
                    item.Refresh( context.Client );
                }
            }
            finally
            {
                context.EndOperation();
            }
        }

        /// <summary>
        /// Resolve an item.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="item"></param>
        private void Resolve(IContext context, SvnItem item)
        {

            string mergeExe = GetExe( context );
            Entry entry = item.Status.Entry;
            bool binary = context.Client.HasBinaryProp(item.Path);
            if ( binary || mergeExe == null )
            {
                string selection;

                using( ConflictDialog dialog = new ConflictDialog(  ) )
                {
                    entry = item.Status.Entry;
                    dialog.Filenames = new string[]{
                                                       entry.ConflictWorking,
                                                       entry.ConflictNew,
                                                       entry.ConflictOld,
                                                       item.Path
                                                   };
                    dialog.Binary = binary;

                    if ( dialog.ShowDialog( context.HostWindow ) != DialogResult.OK )
                        return;
                        
                    selection = dialog.Selection;
                }   
    
                if ( selection != item.Path )
                    this.Copy( item.Path, selection );

                context.Client.Resolved( item.Path, false );
                context.OutputPane.WriteLine( 
                    "Resolved conflicted state of {0}", item.Path );
                
                // delete the associated conflict task item
                context.ConflictManager.RemoveTaskItem(item.Path);

            }
            else
            {
                string itemPath = Path.GetDirectoryName( item.Path );
                string oldPath = String.Format("\"{0}\"", Path.Combine( itemPath, item.Status.Entry.ConflictOld ));
                string newPath = String.Format("\"{0}\"", Path.Combine( itemPath, item.Status.Entry.ConflictNew ));
                string workingPath = String.Format("\"{0}\"", Path.Combine( itemPath, item.Status.Entry.ConflictWorking ));
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
                    context.Client.Resolved( item.Path, false );
                }
            }
        }        

        private void Copy( string toPath, string fromFile )
        {
            string dir = Path.GetDirectoryName( toPath );
            string fromPath = Path.Combine( dir, fromFile );
            File.Copy( fromPath, toPath,  true );
        }

        /// <summary>
        /// Filter for conflicted items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool ConflictedFilter( SvnItem item )
        {
            return item.Status.TextStatus == StatusKind.Conflicted; 
        }
       
        private readonly Regex NUMBER = new Regex( @".*\.r(\d+)" );

    }
}




