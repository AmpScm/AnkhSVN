// $Id$
using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using NSvn.Core;
using EnvDTE;
using Ankh.UI;
using System.Windows.Forms;
using Utils;

namespace Ankh.Commands
{
    /// <summary>
    /// Base class for the DiffLocalItem and CreatePatch commands
    /// </summary>
    public abstract class LocalDiffCommandBase : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // always allow diff - worst case you get an empty diff            
            return Enabled;
        }

        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        protected virtual string GetExe( Ankh.IContext context )
        {
            if ( !context.Config.ChooseDiffMergeManual )
                return context.Config.DiffExePath;
            else 
                return null;
        }

        /// <summary>
        /// Generates the diff from the current selection.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The diff as a string.</returns>
        protected virtual string GetDiff( IContext context )
        {
            bool useExternalDiff = GetExe( context ) != null;

            // We use VersionedFilter here to allow diffs between arbitrary revisions
            IList resources = context.SolutionExplorer.GetSelectionResources(
                true, new ResourceFilterCallback(CommandBase.VersionedFilter) );

            // are we shifted?
            PathSelectorInfo info = new PathSelectorInfo( "Select items for diffing", 
                resources, resources );
            info.RevisionStart = Revision.Base;
            info.RevisionEnd = Revision.Working;

            // "Recursive" doesn't make much sense if using an external diff
            info.EnableRecursive = !useExternalDiff;
            info.Recursive = !useExternalDiff;

            // default to textbase vs wc diff                
            Revision revisionStart = Revision.Base;
            Revision revisionEnd = Revision.Working;

            // should we show the path selector?
            if ( !CommandBase.Shift )
            {
                info = context.UIShell.ShowPathSelector( info );
                    
                if ( info == null )
                    return null;
            }

            if ( useExternalDiff )
            {
                return DoExternalDiff( info, context );
            }
            else
            {
                return DoInternalDiff( info, context );
            }
        }
        
        private string DoInternalDiff( PathSelectorInfo info, IContext context )
        {
            string curdir = Environment.CurrentDirectory;
                
            // we go to the solution directory so that the diff paths will be relative 
            // to that directory
            string slndir = context.SolutionDirectory;
            Debug.Assert( slndir != null, "Solution directory should not be null" );

            try
            {
                // switch to the solution dir, so we can get relative paths.
                Environment.CurrentDirectory = slndir;

                MemoryStream stream = new MemoryStream();
                foreach( SvnItem item in info.CheckedItems )
                {
                    // try to get a relative path to the item from the solution directory
                    string path = Utils.Win32.Win32.PathRelativePathTo( slndir, 
                        Utils.Win32.FileAttribute.Directory, item.Path, 
                        Utils.Win32.FileAttribute.Normal );
                    path = path != null ? path : item.Path;

                    context.Client.Diff( new string[]{}, path, info.RevisionStart, 
                        path, info.RevisionEnd, info.Recursive, true, false, 
                        stream, Stream.Null );
                }

                return System.Text.Encoding.Default.GetString( stream.ToArray() );
            }
            finally
            {
                Environment.CurrentDirectory = curdir;
            }
                
        }
        
        private string DoExternalDiff( PathSelectorInfo info, IContext context )
        {
            foreach ( SvnItem item in info.CheckedItems )
            {
                // skip unmodified for a diff against the textbase
                if ( info.RevisionStart == Revision.Base && 
                    info.RevisionEnd == Revision.Working && !item.IsModified )
                    continue;

                string quotedLeftPath = GetPath( info.RevisionStart, item, context );
                string quotedRightPath = GetPath( info.RevisionEnd, item, context );
                string diffString = this.GetExe( context );
                diffString = diffString.Replace( "%base", quotedLeftPath );
                diffString = diffString.Replace( "%mine", quotedRightPath );

                // We can't use System.Diagnostics.Process here because we want to keep the
                // program path and arguments together, which it doesn't allow.
                Utils.Exec exec = new Utils.Exec();
                exec.ExecPath( diffString );
            }

            return null;
        }

        private string GetPath( Revision revision, SvnItem item, IContext context )
        {
            // is it local?
            if ( revision == Revision.Base )
            {
                if ( item.Status.TextStatus == StatusKind.Added )
                {
                    string empty = Path.GetTempFileName();
                    File.Create(empty).Close();
                    return empty;
                }
                else
                    return context.Client.GetPristinePath( item.Path );
            }
            else if ( revision == Revision.Working )
            {
                return item.Path;
            }

            // we need to get it from the repos
            CatRunner runner = new CatRunner( revision, item.Status.Entry.Url );
            context.UIShell.RunWithProgressDialog( runner, "Retrieving file for diffing" );
            //			runner.Work( context );
            return runner.Path;
        }
    }
}
