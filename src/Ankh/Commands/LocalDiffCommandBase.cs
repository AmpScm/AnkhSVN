// $Id$
using System;
using System.Collections;
using System.IO;
using NSvn.Core;
using EnvDTE;

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
            return vsCommandStatus.vsCommandStatusEnabled |
                vsCommandStatus.vsCommandStatusSupported;
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

            MemoryStream stream = new MemoryStream();
            foreach( SvnItem item in resources )
                context.Client.Diff( new string[]{}, item.Path, Revision.Base, 
                    item.Path, Revision.Working, false, true, false, stream, Stream.Null );

            return System.Text.Encoding.Default.GetString( stream.ToArray() );
        }



    }
}
