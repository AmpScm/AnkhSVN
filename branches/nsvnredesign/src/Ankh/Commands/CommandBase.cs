// $Id$
using System;
using EnvDTE;
using NSvn.Core;

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

        protected static bool UnversionedFilter( SvnItem item )
        {
            return !item.IsVersioned;
        }

        private EnvDTE.Command command;
    }
}
