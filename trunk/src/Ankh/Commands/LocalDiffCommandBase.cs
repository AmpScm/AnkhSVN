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
    internal abstract class LocalDiffCommandBase : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            // always allow diff - worst case you get an empty diff            
            return Enabled;
        }

        /// <summary>
        /// Generates the diff from the current selection.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The diff as a string.</returns>
        protected virtual string GetDiff( AnkhContext context )
        {
            // get the diff itself
            IList resources = context.SolutionExplorer.GetSelectionResources(
                true, new ResourceFilterCallback(CommandBase.ModifiedFilter) );

            string diffExe = context.Config.DiffExePath;
            if (diffExe == null)
            {
                // are we shifted?
                bool recurse = false;
                if ( this.Shift )
                {
                    using( PathSelector p = this.GetPathSelector( "Select items for diffing" ) )
                    {
                        p.Items = resources;
                        p.CheckedItems = resources;
                        if ( p.ShowDialog( context.HostWindow ) != DialogResult.OK )
                            return String.Empty;
                        resources = p.CheckedItems;
                        recurse = p.Recursive;
                    }
                }

                MemoryStream stream = new MemoryStream();
                foreach( SvnItem item in resources )
                    context.Client.Diff( new string[]{}, item.Path, Revision.Base, 
                        item.Path, Revision.Working, recurse, true, false, stream, Stream.Null );

                return System.Text.Encoding.Default.GetString( stream.ToArray() );
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
