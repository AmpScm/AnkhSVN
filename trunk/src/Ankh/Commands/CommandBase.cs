// $Id$
using System;
using EnvDTE;
using NSvn.Core;
using Microsoft.Office.Core;

namespace Ankh.Commands
{
    /// <summary>
    /// Base class for ICommand instances
    /// </summary>
    internal abstract class CommandBase : ICommand
    {
        /// <summary>
        /// Get the status of the command
        /// </summary>
        public abstract vsCommandStatus QueryStatus( AnkhContext context );

        /// <summary>
        /// Execute the command
        /// </summary>
        public abstract void Execute( AnkhContext context, string parameters );

        /// <summary>
        /// The EnvDTE.Command instance corresponding to this command.
        /// </summary>
        public EnvDTE.Command Command
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return this.command; 
            }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                this.command = value;
            }
        }

        protected CommandBarControl GetControl(AnkhContext context, string barName, string name )
        {
            // TODO: either preload this or find a better way to map to 
            // the commandbarcontrols for a command
            CommandBar bar = CommandMap.GetCommandBar( barName, context );           
            CommandBarControl cntl = bar.FindControl( Type.Missing, Type.Missing, 
                barName + "." + name, Type.Missing, Type.Missing );
            return cntl;
        }


        /// <summary>
        /// A ResourceFilterCallback method that filters for modified items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected static bool ModifiedFilter( SvnItem item )
        {
            return item.IsModified;
        }

        /// <summary>
        /// A ResourceFilterCallback that filters for versioned directories.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected static bool DirectoryFilter( SvnItem item )
        {
            return item.IsVersioned && item.IsDirectory;
        }

        protected static bool VersionedFilter( SvnItem item )
        {
            return item.IsVersioned;
        }

        protected static bool UnversionedFilter( SvnItem item )
        {
            return !item.IsVersioned;
        }

        protected static bool UnmodifiedSingleFileFilter( SvnItem item )
        {
            return item.IsVersioned && !item.IsModified && item.IsFile;
        }

        private EnvDTE.Command command;
    }
}
