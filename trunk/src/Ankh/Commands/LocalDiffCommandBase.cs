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
            // get the diff itself
            IList resources = context.SolutionExplorer.GetSelectionResources(
                true, new ResourceFilterCallback(CommandBase.ModifiedFilter) );

            string diffExe = GetExe( context );
            if (diffExe == null)
            {
                // are we shifted?
                bool recurse = false;
                if ( CommandBase.Shift )
                {
                    using( PathSelector p = CommandBase.GetPathSelector( "Select items for diffing" ) )
                    {
                        p.Items = resources;
                        p.CheckedItems = resources;
                        if ( p.ShowDialog( context.HostWindow ) != DialogResult.OK )
                            return null;
                        resources = p.CheckedItems;
                        recurse = p.Recursive;
                    }
                }

                
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
                    foreach( SvnItem item in resources )
                    {
                        // try to get a relative path to the item from the solution directory
                        string path = Utils.Win32.Win32.PathRelativePathTo( slndir, 
                            Utils.Win32.FileAttribute.Directory, item.Path, 
                            Utils.Win32.FileAttribute.Normal );
                        path = path != null ? path : item.Path;

                        context.Client.Diff( new string[]{}, path, Revision.Base, 
                            path, Revision.Working, recurse, true, false, stream, Stream.Null );
                    }

                    return System.Text.Encoding.Default.GetString( stream.ToArray() );
                }
                finally
                {
                    Environment.CurrentDirectory = curdir;
                }
                
            }
            else
            {
                foreach ( SvnItem item in resources )
                {
                    string quotedLocalPath = String.Format( "\"{0}\"", item.Path );
                    string pristinePath = String.Format( "\"{0}\"", context.Client.GetPristinePath(item.Path) );
                    string diffString = diffExe;
                    diffString = diffString.Replace( "%base", pristinePath );
                    diffString = diffString.Replace( "%mine", quotedLocalPath );

                    // We can't use System.Diagnostics.Process here because we want to keep the
                    // program path and arguments together, which it doesn't allow.
                    Utils.Exec exec = new Utils.Exec();
                    exec.ExecPath( diffString );
                }

                return null;
            }
        }
    }
}
