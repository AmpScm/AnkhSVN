// $Id$
using System;
using EnvDTE;

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
        public abstract void Execute( AnkhContext context);

        /// <summary>
        /// The EnvDTE.Command instance corresponding to this command.
        /// </summary>
        public EnvDTE.Command Command
        {
            get
            {
                return this.command; 
            }
            set
            {
                this.command = value;
            }
        }

        private EnvDTE.Command command;
	}
}
